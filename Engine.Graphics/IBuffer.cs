using Silk.NET.OpenGL;

namespace Engine.Graphics
{
    public interface IBuffer<T> where T : unmanaged
    {
        void SetData(T[] data, VertexBufferObjectUsage usage);
        void Bind(VertexArray vao);
    }
}