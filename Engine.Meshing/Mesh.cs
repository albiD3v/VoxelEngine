namespace Engine.Meshing
{
    public class Mesh : IMesh
    {
        public Mesh(int[] data, uint[] indices)
        {
            this.Data = data;
            this.Indices = indices;
        }

        public int[] Data { get; }

        public uint[] Indices { get; }
    }
}
