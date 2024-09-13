namespace Engine.Meshing
{
    public interface IMesh
    {
        public int[] Data { get; }
        public uint[] Indices { get; }
    }
}
