using Silk.NET.OpenGL;

namespace Engine.Graphics
{
    public class BufferLayoutElement<T> : IBufferLayoutElement where T : unmanaged
    {
        private uint m_Location;
        private int m_Size;
        private bool m_Normalized;


        public BufferLayoutElement(uint location, int size, bool normalized)
        {
            m_Location = location;
            m_Size = size;
            m_Normalized = normalized;
        }

        public uint Location => m_Location;

        public int Size => m_Size;

        public bool Normalized => m_Normalized;

        public unsafe int SizeInBytes => m_Size * sizeof(T);

        public GLEnum Type => ToGLType();

        private static GLEnum ToGLType()
        {
            if (typeof(T) == typeof(int))
            {
                return GLEnum.Int;
            }
            else if (typeof(T) == typeof(uint))
            {
                return GLEnum.UnsignedInt;
            }
            else if (typeof(T) == typeof(float))
            {
                return GLEnum.Float;
            }
            else if (typeof(T) == typeof(bool))
            {
                return GLEnum.Bool;
            }
            else
            {
                return GLEnum.None;
            }
        }
    }
}
