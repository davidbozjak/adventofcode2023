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

bool found = false;

double? collisionX = null, collisionY = null, collisionZ = null;

for (int maxRecordedSpeed = 200; !found; maxRecordedSpeed += 50)
{
    Console.WriteLine($"{DateTime.Now.TimeOfDay}: New cycle, now (-{maxRecordedSpeed}, {maxRecordedSpeed})");

    for (int rock_Vx = -maxRecordedSpeed; rock_Vx < maxRecordedSpeed && !found; rock_Vx++)
    {
        for (int rock_Vy = -maxRecordedSpeed; rock_Vy < maxRecordedSpeed && !found; rock_Vy++)
        {
            for (int rock_Vz = -maxRecordedSpeed; rock_Vz < maxRecordedSpeed && !found; rock_Vz++)
            {
                var hailParticle1 = hailParticles[0];
                var newHailParticle1 = new HailParticle(hailParticle1.X, hailParticle1.Y, hailParticle1.Z, hailParticle1.Vx - rock_Vx, hailParticle1.Vy - rock_Vy, hailParticle1.Vz - rock_Vz);

                collisionX = collisionY = collisionZ = null;

                found = true;

                for (int i = 1; i < hailParticles.Count; i++)
                {
                    var hailParticle2 = hailParticles[i];
                    var newHailParticle2 = new HailParticle(hailParticle2.X, hailParticle2.Y, hailParticle2.Z, hailParticle2.Vx - rock_Vx, hailParticle2.Vy - rock_Vy, hailParticle2.Vz - rock_Vz);

                    var collisionPoint = newHailParticle1.Get2DCollision(newHailParticle2);

                    if (collisionPoint == null)
                    {
                        found = false;
                        break;
                    }

                    var t1 = newHailParticle1.HitPointIn(collisionPoint.Value.X, collisionPoint.Value.Y);
                    var t2 = newHailParticle2.HitPointIn(collisionPoint.Value.X, collisionPoint.Value.Y);

                    if (t1 == null || t2 == null)
                    {
                        found = false;
                        break;
                    }

                    var z2_low = newHailParticle2.Z + ((newHailParticle2.Vz > 0 ? t2 - 1 : t2 + 1) * newHailParticle2.Vz);
                    var z2_mid = newHailParticle2.Z + (t2 * newHailParticle2.Vz);
                    var z2_high = newHailParticle2.Z + ((newHailParticle2.Vz > 0 ? t2 + 1 : t2 - 1) * newHailParticle2.Vz);

                    if (z2_low > z2_mid)
                        throw new Exception();

                    if (z2_mid > z2_high)
                        throw new Exception();

                    if (collisionX == null)
                    {
                        collisionX = collisionPoint.Value.X;
                        collisionY = collisionPoint.Value.Y;
                        collisionZ = z2_mid;
                    }
                    else
                    {
                        if (Math.Abs(collisionX.Value - collisionPoint.Value.X) > 1 ||
                            Math.Abs(collisionY.Value - collisionPoint.Value.Y) > 1 ||
                            collisionZ.Value < z2_low || collisionZ.Value > z2_high)
                        {
                            found = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}

if (collisionX == null || collisionY == null || collisionZ == null)
    throw new Exception();

Console.WriteLine($"Part 2 - brute force: x:{collisionX}, y:{collisionY}, z: {collisionZ}");
Console.WriteLine($"SUM : {collisionX + collisionY + collisionZ}");

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
var solution_vx = ((RatNum)model.Evaluate(vx)).Double;
var solution_vy = ((RatNum)model.Evaluate(vy)).Double;
var solution_vz = ((RatNum)model.Evaluate(vz)).Double;

var final = solution_x + solution_y + solution_z;

Console.WriteLine($"Part 2 - Z3 appraoch: x:{solution_x}, y:{solution_y}, z: {solution_z}");
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
        if (this.Vx != 0)
        {
            var deltaY = y - this.Y;
            var nY = deltaY / this.Vy;

            return Math.Ceiling(nY);
        }
        else if (this.Vy != 0)
        {
            var deltaX = x - this.X;
            var nX = deltaX / this.Vx;

            return Math.Ceiling(nX);
        }

        return null;

        //var deltaX = x - this.X;
        //var nX = deltaX / this.Vx;

        //if (nX < 0)
        //    return null;

        //var deltaY = y - this.Y;
        //var nY = deltaY / this.Vy;

        ////if (Math.Abs(nX - nY) > 1)
        ////    throw new Exception();

        //return Math.Ceiling(nX);
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