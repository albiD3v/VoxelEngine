using Silk.NET.OpenGL;

namespace Engine.Graphics
{
    public class Shader : IDisposable
    {
        private readonly GL gl;
        private readonly uint m_handle;

        public uint Handle => m_handle;

        public Shader(GL gl, ShaderType type)
        {
            this.gl = gl;
            this.m_handle = gl.CreateShader(type);
        }

        public void SetSource(string source)
        {
            gl.ShaderSource(m_handle, source);
        }

        public void Compile()
        {
            gl.CompileShader(m_handle);
        }

        public void Dispose()
        {
            gl.DeleteShader(m_handle);
            GC.SuppressFinalize(this);
        }
    }
}
