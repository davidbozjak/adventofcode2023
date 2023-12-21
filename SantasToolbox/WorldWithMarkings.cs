using System.Drawing;

namespace SantasToolbox
{
    public class WorldWithMarkings<T> : IWorld
        where T : IWorldObject
    {
        private readonly List<IWorldObject> worldObjects = new();
        public IEnumerable<IWorldObject> WorldObjects => this.worldObjects.Cast<IWorldObject>();

        public WorldWithMarkings(IWorld world, char c, IEnumerable<T> path)
            : this(world, path.Select(w => (w, c)))
        {
        }

        public WorldWithMarkings(IWorld world, IEnumerable<(T tile, char c)> path)
        {
            this.worldObjects.AddRange(world.WorldObjects);

            this.worldObjects.AddRange(path.Select(w => new MarkedTile(w.tile, w.c)));
        }

        private class MarkedTile : IWorldObject
        {
            private readonly IWorldObject pathObject;
            private readonly char c;

            public Point Position => pathObject.Position;

            public char CharRepresentation => c;

            public int Z => int.MaxValue;

            public MarkedTile(IWorldObject pathObject, char c)
            {
                this.pathObject = pathObject;
                this.c = c;
            }   
        }
    }
}
