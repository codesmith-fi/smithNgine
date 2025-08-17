namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Codesmith.SmithNgine.Smith3D.Primitives;

    /// <summary>
    /// Represents a 3D polygon defined by three vertices.
    /// </summary>
    public class Polygon3D
    {
        private Vertex3D[] vertices;
        public Vertex3D[] Vertices => vertices;
        public Texture2D Texture { get; set; }

        public Polygon3D(Vertex3D[] vertices, Texture2D texture)
        {
            if (vertices.Length != 3)
            {
                throw new ArgumentException("A polygon must have exactly three vertices.");
            }
            this.vertices = vertices;
            Texture = texture;
        }

        public Polygon3D(Vertex3D vertex1, Vertex3D vertex2, Vertex3D vertex3, Texture2D texture)
        {
            vertices = [vertex1, vertex2, vertex3];
            Texture = texture;
        }

        //
        // Methods to manipulate the polygon
        //
        public Polygon3D GetTransformedCopy(Matrix transform)
        {
            var transformedVertices = vertices.Select(v => v.Transform(transform)).ToArray();
            return new Polygon3D(transformedVertices, Texture);
        }

        public Vector3 ComputeNormal()
        {
            var edge1 = vertices[1].Position - vertices[0].Position;
            var edge2 = vertices[2].Position - vertices[0].Position;
            return Vector3.Normalize(Vector3.Cross(edge1, edge2));
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


    }
}
