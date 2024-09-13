using Silk.NET.OpenGL;

namespace Engine.Graphics
{
    public abstract class Buffer<T> : IBuffer<T> where T : unmanaged
    {
        protected readonly GL gl;
        protected readonly uint m_Handle;

        public Buffer(GL gl)
        {
            this.gl = gl;
            m_Handle = gl.CreateBuffer();
        }

        public unsafe void SetData(T[] data, VertexBufferObjectUsage usage)
        {
            fixed (T* ptr = &data[0])
            {
                gl.NamedBufferData(m_Handle, (nuint)(sizeof(T) * data.Length), ptr, usage);
            }
        }

        public abstract void Bind(VertexArray vao);

        public void Dispose()
        {
            gl.DeleteBuffer(m_Handle);
        }
    }
}
