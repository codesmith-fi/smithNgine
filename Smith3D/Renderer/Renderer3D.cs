
namespace Codesmith.SmithNgine.Smith3D.Renderer
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using Codesmith.SmithNgine.Smith3D.Primitives;
    using Codesmith.SmithNgine.Smith3D.Primitives.VertexTypes;
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
        private Scene3D _cachedScene = null;
        private int _cachedSceneGeometrySignature = 0;
        private List<BatchedMesh> _cachedBatchedMeshes = new();

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
            int sceneGeometrySignature = calculateSceneGeometrySignature(scene);
            if (_cachedScene != scene || _cachedSceneGeometrySignature != sceneGeometrySignature)
            {
                _cachedBatchedMeshes = buildSceneBatches(scene);
                _cachedScene = scene;
                _cachedSceneGeometrySignature = sceneGeometrySignature;
            }

            foreach (BatchedMesh batch in _cachedBatchedMeshes)
            {
                renderMesh(scene, batch.Mesh, batch.EffectType, Matrix.Identity);
            }
        }
        //  RenderObjectWithMesh(obj.WorldMatrix, scene.Camera.ViewMatrix, scene.Camera.ProjectionMatrix) // OLD OLD OLD
        private void renderObject(Scene3D scene, Object3D obj)
        {
            // Ensure the object has meshes built from polygons
            // Does transformations and builds meshes if not already done
            obj.UpdateObject();
            foreach (var mesh in obj.MeshesByTexture.Values)
            {
                renderMesh(scene, mesh, obj.WorldMatrix);
            }
        }

        private List<BatchedMesh> buildSceneBatches(Scene3D scene)
        {
            Dictionary<(Texture2D texture, EffectType effectType), MeshBuildData> groupedData = new();

            foreach (Object3D obj in scene.Objects)
            {
                Matrix world = obj.WorldMatrix;
                foreach (Polygon3D polygon in obj.Polygons)
                {
                    EffectType effectType = polygon.EffectType != EffectType.Undefined
                        ? polygon.EffectType
                        : obj.EffectType;

                    if (effectType == EffectType.Undefined)
                    {
                        throw new InvalidOperationException("Object polygon does not define a render effect.");
                    }

                    var key = (polygon.Texture, effectType);
                    if (!groupedData.TryGetValue(key, out MeshBuildData data))
                    {
                        data = new MeshBuildData();
                        groupedData[key] = data;
                    }

                    int vertexOffset = data.Vertices.Count;
                    foreach (Vertex3D vertex in polygon.Vertices)
                    {
                        Vertex3D transformedVertex = vertex.Transform(world);
                        data.Vertices.Add(transformedVertex);
                        data.Normals.Add(transformedVertex.Normal);
                        data.Colours.Add(transformedVertex.Color);
                        data.TextureUVs.Add(transformedVertex.TextureUV);
                    }

                    data.Indices.Add(vertexOffset);
                    data.Indices.Add(vertexOffset + 1);
                    data.Indices.Add(vertexOffset + 2);
                }
            }

            List<BatchedMesh> result = new(groupedData.Count);
            foreach (var groupedMesh in groupedData)
            {
                Mesh3D mesh = new Mesh3D(
                    groupedMesh.Key.texture,
                    groupedMesh.Value.Vertices,
                    groupedMesh.Value.Normals,
                    groupedMesh.Value.Colours,
                    groupedMesh.Value.TextureUVs,
                    groupedMesh.Value.Indices);

                result.Add(new BatchedMesh(groupedMesh.Key.effectType, mesh));
            }

            return result;
        }

        private static int calculateSceneGeometrySignature(Scene3D scene)
        {
            HashCode hash = new HashCode();
            hash.Add(scene.Objects.Count);

            foreach (Object3D obj in scene.Objects)
            {
                hash.Add((int)obj.EffectType);
                addMatrixToHash(ref hash, obj.WorldMatrix);
                hash.Add(obj.Polygons.Count);

                foreach (Polygon3D polygon in obj.Polygons)
                {
                    hash.Add((int)polygon.EffectType);
                    hash.Add(RuntimeHelpers.GetHashCode(polygon.Texture));
                    hash.Add(polygon.Vertices.Length);

                    foreach (Vertex3D vertex in polygon.Vertices)
                    {
                        hash.Add(vertex.Position);
                        hash.Add(vertex.Normal);
                        hash.Add(vertex.TextureUV);
                        hash.Add(vertex.Color.PackedValue);
                    }
                }
            }

            return hash.ToHashCode();
        }

        private static void addMatrixToHash(ref HashCode hash, Matrix matrix)
        {
            hash.Add(matrix.M11); hash.Add(matrix.M12); hash.Add(matrix.M13); hash.Add(matrix.M14);
            hash.Add(matrix.M21); hash.Add(matrix.M22); hash.Add(matrix.M23); hash.Add(matrix.M24);
            hash.Add(matrix.M31); hash.Add(matrix.M32); hash.Add(matrix.M33); hash.Add(matrix.M34);
            hash.Add(matrix.M41); hash.Add(matrix.M42); hash.Add(matrix.M43); hash.Add(matrix.M44);
        }

        private void renderMesh(Scene3D scene, RenderableMesh mesh, Matrix world)
        {
            if (mesh == null) throw new ArgumentNullException(nameof(mesh), "Mesh cannot be null.");
            renderMesh(scene, (Mesh3D)mesh, mesh.EffectType, world);
        }

        private void renderMesh(Scene3D scene, Mesh3D mesh, EffectType effectType, Matrix world)
        {
            if (mesh == null) throw new ArgumentNullException(nameof(mesh), "Mesh cannot be null.");

            // Get the effect for this mesh if the requested type is registered
            // TODO: Fallback to some basic effect if no specific effect 
            if (!TryGetEffect(effectType, out var meshEffect))
            {
                throw new InvalidOperationException("Requested effect not registered");
            }

            // Set specific effect related properties, if any.
            switch (effectType)
            {
                case EffectType.Basic:
                    break;
                case EffectType.BasicTexture:
                    meshEffect.Parameters["Texture"].SetValue(mesh.Texture);
                    break;
                case EffectType.LitTextureAmbientDiffuse:
                    meshEffect.Parameters["Texture"].SetValue(mesh.Texture);
                    break;
                case EffectType.PointLight:
                    meshEffect.Parameters["Texture"]?.SetValue(mesh.Texture);
                    applyPointLightParameters(scene, meshEffect);
                    break;
                case EffectType.Undefined:
                    // TODO improvement: For recovery, XNA BasicEffect could perhaps be used?
                    throw new InvalidOperationException(
                        "Mesh does not define any custom effect");
            }

            meshEffect.Parameters["World"].SetValue(world);
            meshEffect.Parameters["View"].SetValue(scene.Camera.ViewMatrix);
            meshEffect.Parameters["Projection"].SetValue(scene.Camera.ProjectionMatrix);
            try
            {
                doRenderMeshWithNormals(mesh, meshEffect);
            }
            catch (InvalidOperationException ex)
            {
                string techniqueName = meshEffect.CurrentTechnique?.Name ?? "<null>";
                throw new InvalidOperationException(
                    $"Render failed for EffectType '{effectType}' (technique '{techniqueName}').", ex);
            }
        }

        private class MeshBuildData
        {
            public List<Vertex3D> Vertices { get; } = new();
            public List<Vector3> Normals { get; } = new();
            public List<Color> Colours { get; } = new();
            public List<Vector2> TextureUVs { get; } = new();
            public List<int> Indices { get; } = new();
        }

        private class BatchedMesh
        {
            public EffectType EffectType { get; }
            public Mesh3D Mesh { get; }

            public BatchedMesh(EffectType effectType, Mesh3D mesh)
            {
                EffectType = effectType;
                Mesh = mesh;
            }
        }

        private static void applyPointLightParameters(Scene3D scene, Effect effect)
        {
            PointLight pointLight = scene.Lights
                .OfType<PointLight>()
                .FirstOrDefault(light => light.Enabled);

            if (pointLight == null)
            {
                Light3D genericPointLight = scene.Lights
                    .FirstOrDefault(light => light.Enabled && light.Type == Light3D.LightType.Point);

                if (genericPointLight != null)
                {
                    pointLight = new PointLight(genericPointLight.Position, genericPointLight.Color, genericPointLight.Intensity);
                }
            }

            Light3D ambientLight = scene.Lights
                .FirstOrDefault(light => light.Enabled && light.Type == Light3D.LightType.Ambient);

            if (pointLight == null)
            {
                pointLight = new PointLight(Vector3.Zero, Color.White, 0.0f);
            }

            Color ambientColor = ambientLight?.Color ?? Color.White;
            float ambientIntensity = ambientLight?.Intensity ?? 0.0f;

            effect.Parameters["lightPosition"]?.SetValue(pointLight.Position);
            effect.Parameters["lightColor"]?.SetValue(pointLight.Color.ToVector3());
            effect.Parameters["lightIntensity"]?.SetValue(pointLight.Intensity);
            effect.Parameters["constantAttenuation"]?.SetValue(pointLight.ConstantAttenuation);
            effect.Parameters["linearAttenuation"]?.SetValue(pointLight.LinearAttenuation);
            effect.Parameters["quadraticAttenuation"]?.SetValue(pointLight.QuadraticAttenuation);
            effect.Parameters["ambientColor"]?.SetValue(ambientColor.ToVector3());
            effect.Parameters["ambientIntensity"]?.SetValue(ambientIntensity);
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

        private void doRenderMeshWithNormals(Mesh3D mesh, Effect effect)
        {
            if (mesh == null) throw new ArgumentNullException("Mesh cannot not be null!");
            if (effect == null) throw new ArgumentException("Render effect cannot be null");

            // Validate mesh data and indices
            foreach (int index in mesh.Indices)
            {
                if (index < 0 || index >= mesh.Vertices.Count)
                    throw new InvalidOperationException($"Invalid index: {index}");
            }

            VertexPositionNormalColorTexture[] vertexArray = new VertexPositionNormalColorTexture[mesh.Vertices.Count];
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var vertex = mesh.Vertices[i];
                vertexArray[i] = new VertexPositionNormalColorTexture(
                    vertex.Position,
                    vertex.Normal,
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
