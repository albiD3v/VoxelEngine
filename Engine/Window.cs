using Silk.NET.GLFW;
using System.Numerics;

namespace Engine
{
    public class Window
    {
        private readonly Glfw glfw;
        internal readonly unsafe WindowHandle* handle;

        private bool vsync = false;

        public delegate void OnKeyPressDelegate(Keys key, KeyModifiers modifiers);
        public delegate void OnKeyRepeatDelegate(Keys key, KeyModifiers modifiers);
        public delegate void OnKeyReleaseDelegate(Keys key, KeyModifiers modifiers);

        public event OnKeyPressDelegate? OnKeyPress;
        public event OnKeyRepeatDelegate? OnKeyRepeat;
        public event OnKeyReleaseDelegate? OnKeyRelease;

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
            glfw.SetKeyCallback(handle, (window, key, scanCode, action, mods) =>
            {
                switch (action)
                {
                    case InputAction.Press:   OnKeyPress?.Invoke(key, mods); break;
                    case InputAction.Repeat:  OnKeyRepeat?.Invoke(key, mods); break;
                    case InputAction.Release: OnKeyRelease?.Invoke(key, mods); break;
                }
            });
        }

        public Window(Glfw glfw) : this(glfw, 1280, 720, "Voxel Engine") { }

        public unsafe void MakeCurrent()
        {
            glfw.MakeContextCurrent(handle);
        }

        public unsafe void SwapBuffers()
        {
            glfw.SwapBuffers(handle);
        }

        public unsafe bool ShouldClose()
        {
            return glfw.WindowShouldClose(handle);
        }

        public unsafe bool IsKeyPressed(Keys key)
        {
            return glfw.GetKey(handle, key) == 1;
        }

        public unsafe Vector2 GetCursorPos()
        {
            glfw.GetCursorPos(handle, out double x, out double y);
            return new Vector2((float)x, (float)y);
        }
    }
}
