using Silk.NET.OpenGL;

namespace Engine.Graphics
{
    public class VertexArray : IDisposable
    {
        private readonly GL gl;
        private readonly uint m_handle;

        public uint Handle => m_handle;
        public VertexArray(GL gl)
        {
            this.gl = gl;
            m_handle = gl.CreateVertexArray();
        }

        public void Bind()
        {
            gl.BindVertexArray(m_handle);
        }

        public void UnBind()
        {
            gl.BindVertexArray(0);
        }

        public void Dispose()
        {
            gl.DeleteVertexArray(m_handle);
            GC.SuppressFinalize(this);
        }
    }
}
