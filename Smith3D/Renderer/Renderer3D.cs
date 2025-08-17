
namespace Codesmith.SmithNgine.Smith3D.Renderer
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using Codesmith.SmithNgine.Smith3D.Primitives;

    /// <summary>
    /// Renderer class for rendering 3D objects in the game.
    /// </summary>
    public class Renderer3D
    {
        private GraphicsDevice graphicsDevice;
        private BasicEffect effect;

        public Renderer3D(GraphicsDevice device)
        {
            graphicsDevice = device;

            effect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false,
                PreferPerPixelLighting = true
            };

            // Optional: Set default lighting
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = new Vector3(0, -1, -1);
            effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
        }

        public void RenderObject(Object3D obj, Matrix world, Matrix view, Matrix projection)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            IEnumerable<Polygon3D> transformedVertices = obj.GetTransformedPolygons();
            foreach (var polygon in transformedVertices)
            {
                RenderPolygon(polygon, world, view, projection);
            }

        }

        public void RenderPolygon(Polygon3D polygon, Matrix world, Matrix view, Matrix projection)
        {
            // Set transformation matrices
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            // Convert polygon to renderable vertices
            VertexPositionColor[] vertexData = polygon.ToVertexPositionColorArray();

            // Create and bind vertex buffer
            VertexBuffer vertexBuffer = new VertexBuffer(
                graphicsDevice,
                VertexPositionColor.VertexDeclaration,
                vertexData.Length,
                BufferUsage.WriteOnly);

            vertexBuffer.SetData(vertexData);
            graphicsDevice.SetVertexBuffer(vertexBuffer);

            // Apply effect and draw
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }
        }
    }
}
