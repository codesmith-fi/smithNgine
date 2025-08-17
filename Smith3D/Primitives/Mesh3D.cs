using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using System.Collections.Generic;    
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Codesmith.SmithNgine.Smith3D.Primitives;

    public class Mesh3D
    {
        public List<Vertex3D> Vertices { get; set; }
        public List<int> Indices { get; set; }
        public Texture2D Texture { get; set; }

        // Optional: GPU buffers
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }

        public Mesh3D(List<Vertex3D> vertices, Texture2D texture)
        {
            Vertices = vertices;
            Texture = Texture;
        }

        public void BuildBuffers(GraphicsDevice device)
        {
            // Create and fill VertexBuffer and IndexBuffer here
        }

    }
}