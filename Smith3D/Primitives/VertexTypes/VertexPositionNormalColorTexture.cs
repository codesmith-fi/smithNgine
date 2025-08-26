using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives.VertexTypes
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public struct VertexPositionNormalColorTexture : IVertexType
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Color Color { get; set; }
        public Vector2 TextureUV { get; set; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 6 + sizeof(int), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        public VertexPositionNormalColorTexture(Vector3 position, Vector3 normal, Color color, Vector2 textureUV)
        {
            Position = position;
            Normal = normal;
            Color = color;
            TextureUV = textureUV;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}