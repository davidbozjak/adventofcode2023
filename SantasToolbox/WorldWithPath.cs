using System.Drawing;

namespace SantasToolbox
{
    public class WorldWithPath<T> : IWorld
        where T : IWorldObject
    {
        private readonly List<IWorldObject> worldObjects = new();
        public IEnumerable<IWorldObject> WorldObjects => this.worldObjects.Cast<IWorldObject>();

        public WorldWithPath(IWorld world, IEnumerable<T> path)
        {
            this.worldObjects.AddRange(world.WorldObjects);

            this.worldObjects.AddRange(path.Select(w => new PathTile(w)));
        }

        private class PathTile : IWorldObject
        {
            private readonly IWorldObject pathObject;
            public Point Position => pathObject.Position;

            public char CharRepresentation => '*';

            public int Z => int.MaxValue;

            public PathTile(IWorldObject pathObject)
            {
                this.pathObject = pathObject;
            }   
        }
    }
}
