
namespace Codesmith.SmithNgine.Smith3D.Renderer
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using Codesmith.SmithNgine.Smith3D.Primitives;
    using Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect;

    /// <summary>
    /// Renderer class for rendering 3D objects in the game.
    /// </summary>

    public class Renderer3D : IEffectHandler
    {
        private GraphicsDevice graphicsDevice;
        private BasicEffect basicEffect;
        // Map to hold all different supported effects for rendering
        private readonly Dictionary<EffectType, Effect> _effectMap = new();
        private readonly Dictionary<EffectType, EffectParameters> _parameterMap = new();

        public Renderer3D(GraphicsDevice device)
        {
            graphicsDevice = device;

            basicEffect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false,
                PreferPerPixelLighting = true
            };

            // Optional: Set default lighting
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(0, -1, -1);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
        }

        public void RegisterEffect<TParameters>(EffectType type, Effect effect)
            where TParameters : EffectParameters, new()
        {
            _effectMap[type] = effect;
        }

        /// <summary>
        /// Attempts to retrieve the effect associated with the given type.
        /// </summary>
        public bool TryGetEffect(EffectType type, out Effect effect)
        {
            if (!_effectMap.TryGetValue(type, out var e))
            {
                effect = null;
                return false;
            }
            else
            {
                effect = e;
                return true;
            }
        }

        /// <summary>
        /// Applies the given parameters to the effect associated with the type.
        /// </summary>
        public bool ApplyParameters(EffectType type, EffectParameters parameters)
        {
            if (!_effectMap.TryGetValue(type, out var effect))
            {
                return false;
            }
            parameters.ApplyTo(effect);
            return false;
        }

        public void RenderScene(Scene3D scene)
        {
            if (scene == null) throw new ArgumentNullException(nameof(scene), "Scene cannot be null.");
            // Render objects
            foreach (var obj in scene.Objects)
            {
                RenderObjectWithMesh(obj, obj.WorldMatrix,
                    scene.Camera.ViewMatrix, scene.Camera.ProjectionMatrix);
            }
            // Render lights if needed (not implemented in this example)
        }

        public void RenderScene(Scene3D scene, Effect customEffect)
        {
            if (scene == null) throw new ArgumentNullException(nameof(scene), "Scene cannot be null.");
            if (customEffect == null) throw new ArgumentNullException(nameof(customEffect), "Effect cannot be null.");

            // Render objects
            foreach (var obj in scene.Objects)
            {
                RenderObjectWithMesh(customEffect, obj, obj.WorldMatrix,
                    scene.Camera.ViewMatrix, scene.Camera.ProjectionMatrix);
            }
            // Render lights if needed (not implemented in this example)
        }

        /* Future optimization: Render entire scene with single effect, but how to handle WorldMatrix in this case
           since the effect.World needs to be set per object and meshes are built for the whole scene 
           and then they are all combined into one big mesh per texture (loosing WorldMatrix per object).
                                public void RenderScene(Scene3D scene, Effect e)
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
                                        obj.UpdateObject();
                                        foreach (var mesh in obj.MeshesByTexture.Values)
                                        {
                                            RenderMesh(mesh, e);
                                        }
                                    }
                                }
                        */

        public void RenderObjectWithMesh(Effect customEffect, Object3D obj, Matrix world, Matrix view, Matrix projection)
        {
            // Ensure the object has meshes built from polygons
            // Does transformations and builds meshes if not already done
            // Future optimization note:
            //    This can be optimized to only build meshes if polygons or transformation have changed
            obj.UpdateObject();
            foreach (var mesh in obj.MeshesByTexture.Values)
            {
                RenderMesh(customEffect, mesh, world, view, projection);
            }
        }

        public void RenderObjectWithMesh(Object3D obj, Matrix world, Matrix view, Matrix projection)
        {
            // Ensure the object has meshes built from polygons
            // Does transformations and builds meshes if not already done
            obj.UpdateObject();
            foreach (var mesh in obj.MeshesByTexture.Values)
            {
                RenderMesh(mesh, world, view, projection);
            }
        }

        private void RenderMesh(Effect customEffect, Mesh3D mesh, Matrix world, Matrix view, Matrix projection)
        {
            if (customEffect == null) throw new ArgumentNullException(nameof(customEffect), "Effect cannot be null.");
            if (mesh == null) throw new ArgumentNullException(nameof(mesh), "Mesh cannot be null.");

            customEffect.Parameters["View"].SetValue(view);
            customEffect.Parameters["Projection"].SetValue(projection);
            customEffect.Parameters["World"].SetValue(world);

            if (mesh.Texture != null)
            {
                customEffect.Parameters["Texture"].SetValue(mesh.Texture);
                customEffect.CurrentTechnique = customEffect.Techniques["Textured"];
                //                customEffect.CurrentTechnique = customEffect.Techniques["TexturedPointLight"];            

            }
            else
            {
                customEffect.CurrentTechnique = customEffect.Techniques["Colored"];
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
            foreach (EffectPass pass in customEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    mesh.Indices.Count / 3);
            }
        }

        private void RenderMesh(Mesh3D mesh, Matrix world, Matrix view, Matrix projection)
        {
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = mesh.Texture;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;

            if (mesh.Texture == null)
            {
                basicEffect.TextureEnabled = false;
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
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    mesh.Indices.Count / 3);
            }
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

        public void RenderPolygon(Polygon3D polygon, Matrix world, Matrix view, Matrix projection)
        {
            // Set transformation matrices
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;

            basicEffect.Texture = polygon.Texture;
            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = true;

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
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }
        }

        public void RenderPolygonFlat(Polygon3D polygon, Matrix world, Matrix view, Matrix projection)
        {
            // Set transformation matrices
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;

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
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
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
