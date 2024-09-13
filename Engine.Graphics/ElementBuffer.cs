using Silk.NET.OpenGL;

namespace Engine.Graphics
{
    public class ElementBuffer : Buffer<uint>
    {
        public ElementBuffer(GL gl) : base(gl) { }

        public override void Bind(VertexArray vao)
        {
            gl.VertexArrayElementBuffer(vao.Handle, m_Handle);
        }
    }
}
