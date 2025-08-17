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
        public Color Color { get; set; }
        public Texture2D Texture { get; set; }

        public Polygon3D(Vertex3D[] vertices)
        {
            if (vertices.Length != 3)
            {
                throw new ArgumentException("A polygon must have exactly three vertices.");
            }
            this.vertices = vertices;
        }

        public Polygon3D(Vertex3D vertex1, Vertex3D vertex2, Vertex3D vertex3)
        {
            vertices = [vertex1, vertex2, vertex3];
        }

        //
        // Methods to manipulate the polygon
        //
        public Polygon3D GetTransformedCopy(Matrix transform)
        {
            var transformedVertices = vertices.Select(v => v.Transform(transform)).ToArray();
            return new Polygon3D(transformedVertices);
        } 
/*
        public void ApplyTransform()
        {
            Matrix transform =
                Matrix.CreateScale(Scale) *
                Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                Matrix.CreateTranslation(Position);


            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i].Transform(transform);
            }

            // Reset local transform after applying
            Position = Vector3.Zero;
            Rotation = new Quaternion();
            Scale = Vector3.One;
        }
*/
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

        public VertexPositionColor[] ToVertexPositionColorArray()
        {
            return vertices.Select(v => new VertexPositionColor(v.Position, v.Color)).ToArray();
        }

    }
}
