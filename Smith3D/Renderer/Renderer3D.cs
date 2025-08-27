
namespace Codesmith.SmithNgine.Smith3D.Renderer
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using Codesmith.SmithNgine.Smith3D.Primitives;
    using Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect;
    using System.Net.Http;

    /// <summary>
    /// Renderer class for rendering 3D objects in the game.
    /// </summary>


    public class Renderer3D : IEffectHandler
    {
        private GraphicsDevice graphicsDevice;
        // Map to hold all different supported effects for rendering
        private readonly Dictionary<EffectType, Effect> _effectMap = new();

        public Renderer3D(GraphicsDevice device)
        {
            graphicsDevice = device;


            /* TODO: BasicEffect could be used as recovery if certain type of custom effect
                    is not registered in this implementation of IEffectHandler

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
                    */
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
            if (!_effectMap.TryGetValue(type, out effect))
            {
                return false;
            }
            return true;
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
            return true;
        }

        // Future optimization: Render entire scene with a single Mesh 
        // But then it needs somehow handle how WorldMatrix
        // since the effect.World needs to be set per object and meshes are built for the whole scene 
        // and then they are all combined into one big mesh per texture (loosing WorldMatrix per object).
        public void RenderScene(Scene3D scene)
        {
            if (scene == null) throw new ArgumentNullException(nameof(scene), "Scene cannot be null.");
            // Render objects
            foreach (var obj in scene.Objects)
            {
                renderMesh(scene, obj);
            }
        }
        //  RenderObjectWithMesh(obj.WorldMatrix, scene.Camera.ViewMatrix, scene.Camera.ProjectionMatrix) // OLD OLD OLD
        private void renderMesh(Scene3D scene, Object3D obj)
        {
            // Ensure the object has meshes built from polygons
            // Does transformations and builds meshes if not already done
            obj.UpdateObject();
            foreach (var mesh in obj.MeshesByTexture.Values)
            {
                doRenderMesh(scene, mesh, obj.WorldMatrix);
            }
        }

        private void doRenderMesh(Scene3D scene, RenderableMesh mesh, Matrix world)
        {
            if (mesh == null) throw new ArgumentNullException(nameof(mesh), "Mesh cannot be null.");

            // Get the effect for this mesh if the requested type is registered
            // TODO: Fallback to some basic effect if no specific effect 
            if (!TryGetEffect(mesh.EffectType, out var meshEffect))
            {
                throw new InvalidOperationException("Requested effect not registered");
            }

            // Set specific effect related properties, if any.
            switch (mesh.EffectType)
            {
                case EffectType.Basic:
                    break;
                case EffectType.BasicTexture:
                    meshEffect.Parameters["Texture"].SetValue(mesh.Texture);
                    break;
                case EffectType.LitTextureAmbientDiffuse:
                    meshEffect.Parameters["Texture"].SetValue(mesh.Texture);
                    break;
                case EffectType.Undefined:
                    // TODO improvement: For recovery, XNA BasicEffect could perhaps be used?
                    throw new InvalidOperationException(
                        "Mesh does not define any custom effect");
            }

            meshEffect.Parameters["World"].SetValue(world);
            meshEffect.Parameters["View"].SetValue(scene.Camera.ViewMatrix);
            meshEffect.Parameters["Projection"].SetValue(scene.Camera.ProjectionMatrix);
            doRenderMesh(mesh, meshEffect);
        }

        /// <summary>
        ///    Builds vertex and other buffers
        ///    Renders mesh with the requested effect using DrawIndexedPrimitives
        ///    
        ///     NOTE: Assumes that the effect has all parameters set 
        ///     for the shader bound to it (using Effect.Parameters("name")) 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="effect"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void doRenderMesh(Mesh3D mesh, Effect effect)
        {
            if (mesh == null) throw new ArgumentNullException("Mesh cannot not be null!");
            if (effect == null) throw new ArgumentException("Render effect cannot be null");

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
