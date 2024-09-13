using Silk.NET.GLFW;

namespace Engine
{
    public class Window
    {
        private readonly Glfw glfw;
        internal readonly unsafe WindowHandle* handle;

        private bool vsync = false;

        public bool Vsync
        {
            get => vsync;
            set
            {
                vsync = value;
                glfw.SwapInterval(vsync ? 1 : 0);
            }
        }

        public unsafe Window(Glfw glfw, int width, int height, string title)
        {
            this.glfw = glfw;

            handle = glfw.CreateWindow(width, height, title, null, null);

            glfw.SwapInterval(0);
        }

        public Window(Glfw glfw) : this(glfw, 1280, 720, "Voxel Engine") { }

        public void SwapInterval(int value)
        {
            glfw.SwapInterval(value);
        }

        public unsafe void MakeCurrent()
        {
            glfw.MakeContextCurrent(handle);
        }

        public unsafe void SwapBuffers()
        {
            glfw.SwapBuffers(handle);
        }
    }
}
