namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using System;
//    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Codesmith.SmithNgine.Smith3D.Primitives;
    using Codesmith.SmithNgine.Smith3D.Primitives.VertexTypes;
    using System.Linq;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a 3D polygon defined by three vertices.
    /// </summary>
    public class Polygon3D
    {
        // Represents a vertex with position, normal, color, and texture coordinates
        private Vertex3D[] vertices;
        public Vertex3D[] Vertices => vertices;
        public Texture2D Texture { get; set; }
        public Vector3 Normal { get; private set; }
        public Light3D.LightType LightType { get; set; }

        public Color Color
        {
            get => vertices.Length > 0 ? vertices[0].Color : Color.White;
            set => SetColor(value);
        }
        
        public Polygon3D(Vertex3D[] vertices, Texture2D texture)
        {
            if (vertices.Length != 3)
            {
                throw new ArgumentException("A polygon must have exactly three vertices.");
            }
            this.vertices = vertices;
            Texture = texture;
            ComputeNormal();
        }

        public Polygon3D(Vertex3D vertex1, Vertex3D vertex2, Vertex3D vertex3, Texture2D texture)
        {
            vertices = [vertex1, vertex2, vertex3];
            Texture = texture;
            ComputeNormal();
        }

        //
        // Methods to manipulate the polygon
        //
        public Polygon3D GetTransformedCopy(Matrix transform)
        {
            var transformedVertices = vertices.Select(v => v.Transform(transform)).ToArray();
            return new Polygon3D(transformedVertices, Texture);
        }

        public BoundingBox GetBoundingBox()
        {
            var positions = vertices.Select(v => v.Position).ToArray();
            return BoundingBox.CreateFromPoints(positions);
        }

        public void SetColor(Color color)
        {
            foreach (var vertex in vertices)
            {
                vertex.Color = color;
            }
        }

        public VertexPositionColor[] ToVertexPositionColorArray()
        {
            return vertices.Select(v => new VertexPositionColor(v.Position, v.Color)).ToArray();
        }

        public VertexPositionColorTexture[] ToVertexPositionColorTextureArray()
        {
            return Vertices.Select(v => new VertexPositionColorTexture(v.Position, v.Color, v.TextureUV)).ToArray();
        }

        public VertexPositionNormalColorTexture[] ToVertexPositionNormalColorTextureArray()
        {
            return Vertices.Select(v => new VertexPositionNormalColorTexture(v.Position, Normal, v.Color, v.TextureUV)).ToArray();
        }

        public void ComputeNormal()
        {
            if (vertices.Length >= 3)
            {
                Vector3 v0 = vertices[0].Position;
                Vector3 v1 = vertices[1].Position;
                Vector3 v2 = vertices[2].Position;

                Vector3 edge1 = v1 - v0;
                Vector3 edge2 = v2 - v0;

                Normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));
            }
            else
            {
                throw new InvalidOperationException("Polygon must have at least three vertices to compute normal.");
            }
        }
        public override string ToString()
        {
            return $"Polygon3D: {Vertices.Length} vertices, Texture: {Texture?.Name ?? "None"}";
        }
    }
}
