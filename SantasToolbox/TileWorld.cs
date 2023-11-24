using System.Drawing;

namespace SantasToolbox
{
    [System.Diagnostics.DebuggerDisplay("({Position.X}, {Position.Y})")]
    public class Tile : IWorldObject, INode, IEquatable<Tile>
    {
        public Point Position { get; }
        
        public virtual char CharRepresentation => this.IsTraversable ? '.' : '#';

        public virtual int Z => 0;

        public bool IsTraversable { get; }

        private readonly Cached<IEnumerable<Tile>> cachedNeighbours;

        public IEnumerable<Tile> TraversibleNeighbours => this.cachedNeighbours.Value;

        public int Cost => 1;

        public Tile(int x, int y, bool isTraversable, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc)
        {
            Position = new Point(x, y);
            this.IsTraversable = isTraversable;
            this.cachedNeighbours = new Cached<IEnumerable<Tile>>(() => fillTraversibleNeighboursFunc(this).ToList());
        }

        public bool Equals(Tile? other)
        {
            if (other == null) return false;

            return this.Position.Equals(other.Position);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Tile);
        }

        public override int GetHashCode()
        {
            return this.Position.GetHashCode();
        }
    }

    public class TileWorld : IWorld
    {
        private readonly Dictionary<Point, Tile> allTiles = new();
        private readonly bool allowDiagnoalNeighbours;
        private readonly Func<int, int, char, Func<Tile, IEnumerable<Tile>>, Tile> tileCreatingFunc;

        private readonly Cached<int> cachedMaxX;
        private readonly Cached<int> cachedMaxY;
        private readonly Cached<int> cachedMinX;
        private readonly Cached<int> cachedMinY;

        public int MaxX => this.cachedMaxX.Value;
        public int MaxY => this.cachedMaxY.Value;
        public int MinX => this.cachedMinX.Value;
        public int MinY => this.cachedMinY.Value;

        public IEnumerable<IWorldObject> WorldObjects => this.allTiles.Values;

        public char UnknownTileChar { get; set; } = ' ';

        public TileWorld(IEnumerable<string> map, bool allowDiagnoalNeighbours, Func<int, int, char, Func<Tile, IEnumerable<Tile>>, Tile> tileCreatingFunc)
        {
            this.allowDiagnoalNeighbours = allowDiagnoalNeighbours;
            this.tileCreatingFunc = tileCreatingFunc;

            int y = 0;
            foreach (var line in map)
            {
                for (int x = 0; x < line.Length; x++)
                {
                    char c = line[x];

                    var point = new Point(x, y);

                    allTiles[point] = tileCreatingFunc(x, y, c, GetTraversibleNeighboursOfTile);
                }
                y++;
            }

            this.cachedMaxX = new Cached<int>(() => this.allTiles.Keys.Select(w => w.X).Max());
            this.cachedMaxY = new Cached<int>(() => this.allTiles.Keys.Select(w => w.Y).Max());
            this.cachedMinX = new Cached<int>(() => this.allTiles.Keys.Select(w => w.X).Min());
            this.cachedMinY = new Cached<int>(() => this.allTiles.Keys.Select(w => w.Y).Min());
        }

        public Tile GetOrCreateTileAt(int x, int y) =>
            GetOrCreateTileAt(new Point(x, y));

        public Tile GetOrCreateTileAt(Point point)
        {
            if (!allTiles.ContainsKey(point))
            {
                allTiles[point] = tileCreatingFunc(point.X, point.Y, this.UnknownTileChar, GetTraversibleNeighboursOfTile);
            }

            this.cachedMaxX.Reset();
            this.cachedMaxY.Reset();
            this.cachedMinX.Reset();
            this.cachedMinY.Reset();

            return allTiles[point];
        }

        public Tile GetTileAt(int x, int y)
            => GetTileAt(new Point(x, y));

        public Tile GetTileAt(Point point) =>
            this.allTiles[point];

        public Tile? GetTileAtOrNull(int x, int y)
            => GetTileAtOrNull(new Point(x, y));

        public Tile? GetTileAtOrNull(Point point) =>
            this.allTiles.ContainsKey(point) ? this.allTiles[point] : null;

        private IEnumerable<Tile> GetTraversibleNeighboursOfTile(Tile tile)
        {
            Func<Point, Point, bool> neighbourFunc = this.allowDiagnoalNeighbours ?
                (p1, p2) => p1.IsNeighbourWithDiagnoals(p2) :
                (p1, p2) => p1.IsNeighbour(p2);

            return this.allTiles.Values.Where(w => w.IsTraversable &&
                neighbourFunc(w.Position, tile.Position));
        }
    }
}
