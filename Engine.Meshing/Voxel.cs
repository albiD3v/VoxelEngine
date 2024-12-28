namespace Engine.Meshing
{
    public class Voxel
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public ushort Id { get; }

        public bool IsSolid => Id != 0;

        public Voxel(int x, int y, int z, ushort id)
        {
            X = x;
            Y = y;
            Z = z;
            Id = id;
        }
    }
}
