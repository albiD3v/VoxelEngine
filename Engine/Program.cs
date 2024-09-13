using Engine.Graphics;
using Engine.Meshing;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace Engine
{
    public class Program
    {
        private static PerspectiveCamera m_Camera;
        private static Vector2 lastMousePos = Vector2.Zero;
        private static bool m_MouseMoved = false;

        private static long frames = 0;

        public static unsafe void Main(string[] args)
        {
            Glfw glfw = Glfw.GetApi();
            glfw.Init();

            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGL);
            glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            glfw.WindowHint(WindowHintInt.ContextVersionMajor, 4);
            glfw.WindowHint(WindowHintInt.ContextVersionMinor, 5);

            Window window = new Window(glfw, 1280, 720, "Engine");
            glfw.SetInputMode(window.handle, CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);
            window.MakeCurrent();

            m_Camera = new PerspectiveCamera(1280.0f / 720.0f);
            m_Camera.Position = Vector3.UnitZ * 0.0f;

            GL gl = GL.GetApi(glfw.GetProcAddress);
            gl.Enable(EnableCap.DepthTest);
            gl.Disable(EnableCap.Blend);
            gl.Disable(EnableCap.StencilTest);

            gl.Enable(EnableCap.CullFace);
            gl.CullFace(TriangleFace.Back);
            gl.FrontFace(FrontFaceDirection.CW);

            Graphics.Shader vertShader = new Graphics.Shader(gl, ShaderType.VertexShader);
            vertShader.SetSource(File.ReadAllText(@"C:\Users\albir\Desktop\Dev\Progetti C#\VoxelEngine\Engine.Graphics\resources\Shader.vert"));
            vertShader.Compile();

            Graphics.Shader fragShader = new Graphics.Shader(gl, ShaderType.FragmentShader);
            fragShader.SetSource(File.ReadAllText(@"C:\Users\albir\Desktop\Dev\Progetti C#\VoxelEngine\Engine.Graphics\resources\Shader.frag"));
            fragShader.Compile();

            ShaderProgram program = new ShaderProgram(gl);
            program.Attach(vertShader);
            program.Attach(fragShader);
            program.Link();

            vertShader.Dispose();
            fragShader.Dispose();

            gl.ClearColor(Color.CadetBlue);

            glfw.SetFramebufferSizeCallback(window.handle, (WindowHandle* window, int width, int height) =>
            {
                gl.Viewport(0, 0, (uint)width, (uint)height);
                m_Camera.AspectRatio = (float)width / height;
                program.SetUniform("mvp", m_Camera.ProjectionViewMatrix);
            });

            List<Chunk> chunks = new List<Chunk>();

            for (int x = 0; x < 8; x++)
            {
                for(int z = 0; z < 8; z++)
                {
                    for(int y = 0; y < 8; y++)
                    {
                        chunks.Add(new Chunk(new Vector3(x, y, z)));
                    }
                }
            }

            List<IMesh> meshes = new List<IMesh>(chunks.Count);
            Renderer renderer = new Renderer();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < chunks.Count; i++)
            {
                int octaves = (i % 10) + 1;
                chunks[i].GenerateChunkPerlin(octaves, 0.5f, 1.0f);
            }
            sw.Stop();
            Console.WriteLine($"Generated {chunks.Count} chunks in {sw.Elapsed.TotalMilliseconds}ms");

            sw.Restart();
            foreach(Chunk chunk in chunks)
            {
                meshes.Add(chunk.GenerateMesh());
            }
            sw.Stop();
            Console.WriteLine($"Generated {chunks.Count} chunk mesh in {sw.Elapsed.TotalMilliseconds}ms");

            sw.Restart();
            foreach(IMesh mesh in meshes)
            {
                renderer.LoadMesh(gl, mesh);
            }
            sw.Stop();
            Console.WriteLine($"Loaded {chunks.Count} chunk mesh in {sw.Elapsed.TotalMilliseconds}ms");

            PolygonMode mode = PolygonMode.Fill;
            glfw.SetKeyCallback(window.handle, (WindowHandle* window, Keys key, int scanCode, InputAction action, KeyModifiers mods) =>
            {
                 if (action != InputAction.Press) { return; }

                if (key == Keys.Z)
                {
                    if (mode == PolygonMode.Fill)
                    {
                        gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                        mode = PolygonMode.Line;
                    }
                    else
                    {
                        gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                        mode = PolygonMode.Fill;
                    }
                }
                return;
            });


            float lastTime = 0;

            program.Use();
            while (!glfw.WindowShouldClose(window.handle))
            {
                float time = (float)glfw.GetTime();
                float dt = time - lastTime;
                lastTime = time;

                float fps = frames / time;

                HandleMouseMovement(glfw, window, dt);
                HandleCameraMovement(glfw, window, dt);

                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                program.SetUniform("mvp", m_Camera.ProjectionViewMatrix);

                for (int i = 0; i < meshes.Count; i++)
                {
                    program.SetUniform("worldPos", chunks[i].WorldPos);
                    renderer.DrawMesh(gl, meshes[i]);
                }

                glfw.SwapBuffers(window.handle);
                glfw.PollEvents();
                frames++;
            }
        }

        private static unsafe void HandleMouseMovement(Glfw glfw, Window window, float dt)
        {
            glfw.GetCursorPos(window.handle, out double x, out double y);
            if (!m_MouseMoved)
            {
                lastMousePos.X = (float)x;
                lastMousePos.Y = (float)y;
                m_MouseMoved = true;
            }

            float xoffset = lastMousePos.X - (float)x;
            float yoffset = lastMousePos.Y - (float)y;

            float sensitivity = 10.0f;

            m_Camera.Yaw -= xoffset * sensitivity * dt;
            m_Camera.Pitch += yoffset * sensitivity * dt;

            if (m_Camera.Pitch > 89.0f)
                m_Camera.Pitch = 89.0f;
            if (m_Camera.Pitch < -89.0f)
                m_Camera.Pitch = -89.0f;

            lastMousePos.X = (float)x;
            lastMousePos.Y = (float)y;
        }

        private static unsafe void HandleCameraMovement(Glfw glfw, Window window, float dt)
        {
            bool w = glfw.GetKey(window.handle, Keys.W) == 1;
            bool s = glfw.GetKey(window.handle, Keys.S) == 1;
            bool a = glfw.GetKey(window.handle, Keys.A) == 1;
            bool d = glfw.GetKey(window.handle, Keys.D) == 1;

            bool up = glfw.GetKey(window.handle, Keys.Space) == 1;
            bool down = glfw.GetKey(window.handle, Keys.ShiftLeft) == 1;

            float speed = 15.0f * dt;

            if (w)
            {
                m_Camera.Position += speed * new Vector3(m_Camera.Direction.X, 0.0f, m_Camera.Direction.Z);
            }
            if (s)
            {
                m_Camera.Position -= speed * new Vector3(m_Camera.Direction.X, 0.0f, m_Camera.Direction.Z);
            }
            if (a)
            {
                m_Camera.Position -= Vector3.Normalize(Vector3.Cross(m_Camera.Direction, m_Camera.Up)) * speed;
            }
            if (d)
            {
                m_Camera.Position += Vector3.Normalize(Vector3.Cross(m_Camera.Direction, m_Camera.Up)) * speed;
            }
            if (up)
            {
                m_Camera.Position += Vector3.UnitY * speed;
            }
            if (down)
            {
                m_Camera.Position -= Vector3.UnitY * speed;
            }
        }
    }
}
