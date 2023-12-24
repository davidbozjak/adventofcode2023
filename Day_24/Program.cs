using Microsoft.Z3;
using System.Text.RegularExpressions;

var hailParticles = new InputProvider<HailParticle?>("Input.txt", GetHailParticle).Where(w => w != null).Cast<HailParticle>().ToList();

// test data boundries:
//long testAreaMinX = 7;
//long testAreaMaxX = 27;

//long testAreaMinY = 7;
//long testAreaMaxY = 27;

// real data boundries:
double testAreaMinX = 200000000000000;
double testAreaMaxX = 400000000000000;

double testAreaMinY = 200000000000000;
double testAreaMaxY = 400000000000000;

long countCollisionsWithinTestArea = 0;

for (int i = 0; i < hailParticles.Count; i++)
{
    var first = hailParticles[i];

    for (int j = i + 1; j < hailParticles.Count; j++)
    {
        var other = hailParticles[j];

        var collision = first.Get2DCollision(other);

        if (collision != null && 
            (collision.Value.X >= testAreaMinX && collision.Value.X <= testAreaMaxX) &&
            (collision.Value.Y >= testAreaMinY && collision.Value.Y <= testAreaMaxY))
        {
            if (first.HitPointIn(collision.Value.X, collision.Value.Y) >= 0 &&
                other.HitPointIn(collision.Value.X, collision.Value.Y) >= 0)
            {
                countCollisionsWithinTestArea++;
            }
        }
    }
}

Console.WriteLine($"Part 1: {countCollisionsWithinTestArea}");

// using part 2 to learn about Z3 - new to me - https://github.com/Z3Prover/z3

var context = new Context();

var solver = context.MkSolver();

var x = context.MkRealConst("x");
var vx = context.MkRealConst("vx");
var y = context.MkRealConst("y");
var vy = context.MkRealConst("vy");
var z = context.MkRealConst("z");
var vz = context.MkRealConst("vz");

for (int i = 0; i < 3; i++)
{
    var hailParticle = hailParticles[i];
    var t = context.MkRealConst($"t_{i+1}");

    solver.Add(t >= 0);
    
    solver.Add(context.MkEq(x + vx * t, hailParticle.X + hailParticle.Vx * t));
    solver.Add(context.MkEq(y + vy * t, hailParticle.Y + hailParticle.Vy * t));
    solver.Add(context.MkEq(z + vz * t, hailParticle.Z + hailParticle.Vz * t));
}

var status = solver.Check();

if (status != Status.SATISFIABLE)
    throw new Exception();

var model = solver.Model;

var solution_x = ((RatNum)model.Evaluate(x)).Double;
var solution_y = ((RatNum)model.Evaluate(y)).Double;
var solution_z = ((RatNum)model.Evaluate(z)).Double;


var final = solution_x + solution_y + solution_z;

Console.WriteLine($"Part 2: x:{solution_x}, y:{solution_y}, z: {solution_z}");
Console.WriteLine($"SUM : {final}");

static bool GetHailParticle(string? input, out HailParticle? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => double.Parse(w.Value)).ToArray();

    if (numbers.Length != 6)
        throw new Exception();

    value = new HailParticle(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5]);

    return true;
}

record HailParticle (double X, double Y, double Z, double Vx, double Vy, double Vz)
{
    public (double X, double Y)? Get2DCollision(HailParticle other)
    {
        // formula from https://stackoverflow.com/questions/4543506/algorithm-for-intersection-of-2-lines

        (var A1, var B1, var C1) = GetLineFormula(this);
        (var A2, var B2, var C2) = GetLineFormula(other);

        double delta = A1 * B2 - A2 * B1;
 
        if (delta == 0)
            return null;

        var x = (B2 * C1 - B1 * C2) / delta;
        var y = (A1 * C2 - A2 * C1) / delta;

        return (x, y);
    }

    public double? HitPointIn(double x, double y)
    {
        var deltaX = x - this.X;
        var nX = deltaX / this.Vx;

        var deltaY = y - this.Y;
        var nY = deltaY / this.Vy;

        if (Math.Abs(nX - nY) > 1e-1)
            throw new Exception();

        if (nX < 0)
            return null;

        return Math.Ceiling(nX);
    }

    static (double A, double B, double C) GetLineFormula(HailParticle particle)
    {
        // formula from https://stackoverflow.com/questions/4543506/algorithm-for-intersection-of-2-lines
        //A = y2 - y1; B = x1 - x2; C = Ax1 + By1

        var factor = 10;

        var x1 = particle.X;
        var x2 = particle.X + factor * particle.Vx;

        var y1 = particle.Y;
        var y2 = particle.Y + factor * particle.Vy;

        var A = y2 - y1;
        var B = x1 - x2;

        var C = A * x1 + B * y1;

        return (A, B, C);
    }
}