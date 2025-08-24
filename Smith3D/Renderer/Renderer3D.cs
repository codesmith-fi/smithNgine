
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

        public void RenderScene(Scene3D scene)
        {
            if (scene == null) throw new ArgumentNullException(nameof(scene), "Scene cannot be null.");
            // Render objects
            foreach (var obj in scene.Objects)
            {
                RenderObjectWithMesh(obj, obj.WorldMatrix, scene.Camera.ViewMatrix, scene.Camera.ProjectionMatrix);
            }
            // Render lights if needed (not implemented in this example)
        }

        public void RenderObject(Object3D obj, Matrix world, Matrix view, Matrix projection)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "Object cannot be null.");
            IEnumerable<Polygon3D> transformedVertices = obj.GetTransformedPolygons();
            foreach (var polygon in transformedVertices)
            {
                RenderPolygon(polygon, world, view, projection);
            }

        }

        public void RenderObjectFlat(Object3D obj, Matrix world, Matrix view, Matrix projection)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "Object cannot be null.");

            IEnumerable<Polygon3D> transformedVertices = obj.GetTransformedPolygons();
            foreach (var polygon in transformedVertices)
            {
                RenderPolygonFlat(polygon, world, view, projection);
            }
        }

        public void RenderObjectWithMesh(Object3D obj, Matrix world, Matrix view, Matrix projection)
        {
            // Ensure the object has meshes built from polygons
            // Does transformations and builds meshes if not already done
            obj.BuildMeshesFromPolygons();
            foreach (var mesh in obj.MeshesByTexture.Values)
            {
                RenderMesh(mesh, world, view, projection);
            }
        }

        private void RenderMesh(Mesh3D mesh, Matrix world, Matrix view, Matrix projection)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
            effect.TextureEnabled = true;
            effect.Texture = mesh.Texture;
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;

            if (mesh.Texture == null)
            {
                effect.TextureEnabled = false;
            }

            // Validate mesh data and indices
            foreach (int index in mesh.Indices)
            {
                if (index < 0 || index >= mesh.Vertices.Count)
                    throw new InvalidOperationException($"Invalid index: {index}");
            }

            VertexPositionColorTexture[] vertexArray = new VertexPositionColorTexture[mesh.Vertices.Count];
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var vertex = mesh.Vertices[i];
                vertexArray[i] = new VertexPositionColorTexture(
                    vertex.Position,
                    vertex.Color,
                    vertex.TextureUV);
            }

            VertexBuffer vertexBuffer = new VertexBuffer(
                graphicsDevice,
                VertexPositionColorTexture.VertexDeclaration,
                vertexArray.Length,
                BufferUsage.WriteOnly);

            vertexBuffer.SetData(vertexArray);
            // Bind vertex buffer to the graphics device
            graphicsDevice.SetVertexBuffers(new VertexBufferBinding(vertexBuffer));

            // Create index buffer from mesh indices
            IndexBuffer indexBuffer = new IndexBuffer(
                graphicsDevice,
                IndexElementSize.ThirtyTwoBits,
                mesh.Indices.Count,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(mesh.Indices.ToArray());
            graphicsDevice.Indices = indexBuffer;
            // Apply effect and draw
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    mesh.Indices.Count / 3);
            }
        }

        public void RenderSceneWithEffect(Scene3D scene, Effect e)
        {
            if (scene == null) throw new ArgumentNullException(nameof(scene), "Scene cannot be null.");
            if (e == null) throw new ArgumentNullException(nameof(e), "Effect cannot be null.");

            // Set effect parameters
            e.Parameters["View"].SetValue(scene.Camera.ViewMatrix);
            e.Parameters["Projection"].SetValue(scene.Camera.ProjectionMatrix);
            // Render objects generating meshes from polygons
            foreach (var obj in scene.Objects)
            {
                e.Parameters["World"].SetValue(obj.WorldMatrix);
                obj.BuildMeshesFromPolygons();
                foreach (var mesh in obj.MeshesByTexture.Values)
                {
                    RenderMeshWithEffect(mesh, e);
                }
            }
        }

        private void RenderMeshWithEffect(Mesh3D mesh, Effect e)
        {
            if (mesh.Texture == null)
            {
                throw new InvalidOperationException("Mesh texture cannot be null.");
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e), "Effect cannot be null.");
            }

            // Validate mesh data and indices
            foreach (int index in mesh.Indices)
            {
                if (index < 0 || index >= mesh.Vertices.Count)
                    throw new InvalidOperationException($"Invalid vertex index: {index}");
            }

            if (mesh.Vertices.Count == 0 || mesh.Indices.Count == 0)
            {
                throw new InvalidOperationException("Mesh has no vertices or indices to render.");
            }

            // Create vertex buffer from mesh vertices
            // Convert mesh vertices to VertexPositionNormalColorTexture format
            VertexPositionNormalColorTexture[] vertexArray = new VertexPositionNormalColorTexture[mesh.Vertices.Count];
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var vertex = mesh.Vertices[i];
                vertexArray[i] = new VertexPositionNormalColorTexture(
                    vertex.Position,
                    mesh.Normals[i],
                    vertex.Color,
                    vertex.TextureUV);
            }

            VertexBuffer vertexBuffer = new VertexBuffer(
                graphicsDevice,
                VertexPositionNormalColorTexture.VertexDeclaration,
                vertexArray.Length,
                BufferUsage.WriteOnly);

            vertexBuffer.SetData(vertexArray);
            // Bind vertex buffer to the graphics device
            graphicsDevice.SetVertexBuffers(new VertexBufferBinding(vertexBuffer));

            // Create index buffer from mesh indices
            IndexBuffer indexBuffer = new IndexBuffer(
                graphicsDevice,
                IndexElementSize.ThirtyTwoBits,
                mesh.Indices.Count,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(mesh.Indices.ToArray());
            graphicsDevice.Indices = indexBuffer;
//            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            // Set effect parameters
            e.Parameters["DiffuseTexture"].SetValue(mesh.Texture);
            // Apply effect and draw
            foreach (EffectPass pass in e.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    mesh.Indices.Count / 3);
            }
        }

        public void RenderPolygon(Polygon3D polygon, Matrix world, Matrix view, Matrix projection)
        {
            // Set transformation matrices
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            effect.Texture = polygon.Texture;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = true;

            // Convert polygon to renderable vertices
            VertexPositionColorTexture[] vertexData = polygon.ToVertexPositionColorTextureArray();
            VertexBuffer vertexBuffer = new VertexBuffer(
                graphicsDevice,
                //                VertexPositionColorTexture.VertexDeclaration,
                VertexPositionColorTexture.VertexDeclaration,
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

        public void RenderPolygonFlat(Polygon3D polygon, Matrix world, Matrix view, Matrix projection)
        {
            // Set transformation matrices
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
            effect.VertexColorEnabled = true;

            VertexPositionColor[] vertexData = polygon.ToVertexPositionColorArray();
            // Create and bind vertex buffer
            VertexBuffer vertexBuffer = new VertexBuffer(
                graphicsDevice,
                //                VertexPositionColorTexture.VertexDeclaration,
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

        public void DebugRenderAxes(Camera3D camera)
        {
            // Render axes for debugging

            BasicEffect debugEffect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = camera.ViewMatrix,
                Projection = camera.ProjectionMatrix
            };


            float axisLength = 100f; // Length of the axes
            VertexPositionColor[] axisVertices = new VertexPositionColor[]
            {
                // X axis - Red
                new VertexPositionColor(Vector3.Zero, Color.Red),
                new VertexPositionColor(Vector3.UnitX * axisLength, Color.Red),

                // Y axis - Green
                new VertexPositionColor(Vector3.Zero, Color.Green),
                new VertexPositionColor(Vector3.UnitY * axisLength, Color.Green),

                // Z axis - Blue
                new VertexPositionColor(Vector3.Zero, Color.Blue),
                new VertexPositionColor(Vector3.UnitZ * axisLength, Color.Blue),
            };
            
            foreach (EffectPass pass in debugEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    axisVertices,
                    0,
                    axisVertices.Length / 2
                );
            }            
        }
    }
}
