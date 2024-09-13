using Silk.NET.OpenGL;
using Engine.Meshing;

namespace Engine.Graphics
{
    public class Renderer
    {
        private readonly Dictionary<IMesh, VertexArray> meshes;

        //entity
            //- mesh
            //- transform
        // save entity

        public Renderer()
        {
            meshes = new Dictionary<IMesh, VertexArray>();
        }

        public void LoadMesh(GL gl, IMesh mesh)
        {
            if (meshes.ContainsKey(mesh)) { return; };

            VertexArray vao = new VertexArray(gl);
            ArrayBuffer<int> vbo = new ArrayBuffer<int>(gl, 0);
            vbo.SetData(mesh.Data, VertexBufferObjectUsage.StaticDraw);
            vbo.Bind(vao);
            
            ElementBuffer ebo = new ElementBuffer(gl);
            ebo.SetData(mesh.Indices, VertexBufferObjectUsage.StaticDraw);
            ebo.Bind(vao);

            //BufferLayout layout = new BufferLayout(new BufferElement<int>(0, 1, false));
            //vbo.ApplyLayout(vao, layout);
            gl.EnableVertexArrayAttrib(vao.Handle, 0);
            gl.VertexArrayAttribIFormat(vao.Handle, 0, 1,VertexAttribIType.Int, 0);
            gl.VertexArrayAttribBinding(vao.Handle, 0, vbo.BindingIndex);

            meshes.Add(mesh, vao);
        }

        public unsafe void DrawMesh(GL gl, IMesh mesh)
        {
            VertexArray vao = meshes[mesh];
            vao.Bind();

            gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt , null);
            
            vao.UnBind();
        }

    }
}
