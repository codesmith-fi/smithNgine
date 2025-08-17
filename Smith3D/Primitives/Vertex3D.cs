namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    
    // 3D vertex structure for 3D graphics
    public class Vertex3D
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 TextureUV { get; set; }
        public Color Color { get; set; }

        public Vertex3D(Vector3 position, Color color = default)
        {
            Position = position;
            Color = color == default(Color) ? Color.White : color;
            Normal = Vector3.Up; // Default normal
            TextureUV = Vector2.Zero; // Default texture coordinate
        }

        public Vertex3D(Vector3 position, Vector3 normal, Vector2 textureUV)
            : this(position, normal, textureUV, Color.White)
        {
        }

        public Vertex3D(Vector3 position, Vector2 textureUV, Color color = default)
        {
            Position = position;
            Normal = Vector3.Up;
            TextureUV = textureUV;
            Color = color == default(Color) ? Color.White : color;
        }

        public Vertex3D(Vector3 position, Vector3 normal, Vector2 textureUV, Color color = default)
        {
            Position = position;
            Normal = normal;
            TextureUV = textureUV;
            Color = color == default(Color) ? Color.White : color;
        }

        public Vertex3D Transform(Matrix matrix)
        {
            Vector3 transformedPosition = Vector3.Transform(Position, matrix);
            Matrix normalMatrix = Matrix.Transpose(Matrix.Invert(matrix));
            Vector3 transformedNormal = Vector3.TransformNormal(Normal, normalMatrix);

            return new Vertex3D(transformedPosition, transformedNormal, TextureUV, Color);
        }

        public VertexPositionColor ToVertexPositionColor()
        {
            return new VertexPositionColor(this.Position, this.Color);
        }

        public override string ToString()
        {
            return $"Position: {Position}, Normal: {Normal}, TextureCoordinate: {TextureUV}";
        }
    }

}

