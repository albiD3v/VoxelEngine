using Silk.NET.OpenGL;

namespace Engine.Graphics
{
    public class ArrayBuffer<T> : Buffer<T> where T : unmanaged
    {
        private readonly uint m_BindingIndex;
        private readonly nint m_Offset;

        public uint BindingIndex => m_BindingIndex;
        public nint Offset => m_Offset;
        public unsafe uint Stride => (uint)sizeof(T);

        public ArrayBuffer(GL gl, uint bindingIndex, nint offset = 0) : base(gl)
        {
            m_BindingIndex = bindingIndex;
            m_Offset = offset;
        }

        public unsafe override void Bind(VertexArray vao)
        {
            gl.VertexArrayVertexBuffer(vao.Handle, m_BindingIndex, m_Handle, m_Offset, Stride);
        }

        public unsafe void ApplyLayout(VertexArray vao, BufferLayout layout)
        {
            uint offset = 0;
            foreach (IBufferLayoutElement element in layout.Elements)
            {
                gl.EnableVertexArrayAttrib(vao.Handle, element.Location);
                gl.VertexArrayAttribIFormat(vao.Handle, element.Location, element.Size, element.Type, offset);
                gl.VertexArrayAttribBinding(vao.Handle, element.Location, m_BindingIndex);

                offset += (uint)element.SizeInBytes;
            }
        }
    }
}
