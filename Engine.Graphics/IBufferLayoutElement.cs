using Silk.NET.OpenGL;

namespace Engine.Graphics
{
    public interface IBufferLayoutElement
    {
        uint Location { get; }

        int Size { get; }

        bool Normalized { get; }

        int SizeInBytes { get; }

        GLEnum Type { get; }
    }
}
