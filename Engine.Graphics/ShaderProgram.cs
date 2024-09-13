using Silk.NET.OpenGL;
using System.Numerics;

namespace Engine.Graphics
{
    public class ShaderProgram : IDisposable
    {
        private readonly GL gl;
        private readonly uint m_Handle;

        public uint Handle => m_Handle;
        public ShaderProgram(GL gl)
        {
            this.gl = gl;
            this.m_Handle = gl.CreateProgram();
        }

        public void Attach(Shader shader)
        {
            gl.AttachShader(this.m_Handle, shader.Handle);
        }

        public void Detach(Shader shader)
        {
            gl.DetachShader(this.m_Handle, shader.Handle);
        }

        public void Link()
        {
            gl.LinkProgram(m_Handle);
        }

        public void Use()
        {
            gl.UseProgram(m_Handle);
        }

        public unsafe void SetUniform(string name, Vector3 vector3)
        {
            int location = GetInformLocation(name);
            gl.Uniform3(location, vector3);
        }

        public unsafe void SetUniform(string name, Matrix4x4 matrix)
        {
            int location = GetInformLocation(name);
            gl.UniformMatrix4(location, 1, false, (float*)&matrix);
        }

        private int GetInformLocation(string name)
        {
            int location = gl.GetUniformLocation(m_Handle, name);

            if (location < 0)
            {
                throw new Exception($"can't find uniform with name:{name}!");
            }

            return location;
        }

        public void Dispose()
        {
            gl.DeleteProgram(m_Handle);
            GC.SuppressFinalize(this);
        }
    }
}
