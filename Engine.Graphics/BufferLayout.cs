
namespace Engine.Graphics
{
    public class BufferLayout
    {
        private readonly List<IBufferLayoutElement> elements;

        //stride
        //offset?

        public List<IBufferLayoutElement> Elements => elements;

        public uint Stride => (uint)elements.Select(e => e.SizeInBytes).Sum();

        public BufferLayout(params IBufferLayoutElement[] elements)
        {
            this.elements = elements.ToList();
        }
    }
}
