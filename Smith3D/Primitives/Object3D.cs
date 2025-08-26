namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Codesmith.SmithNgine.Smith3D.Primitives;
    public class Object3D
    {
        private EffectType effectType = EffectType.LitVertexColor;
        public List<Polygon3D> Polygons { get; private set; } = new List<Polygon3D>();
        public Dictionary<Texture2D, List<Polygon3D>> PolygonsByTexture { get; private set; } = new Dictionary<Texture2D, List<Polygon3D>>();
        public Dictionary<Texture2D, Mesh3D> MeshesByTexture { get; private set; } = new Dictionary<Texture2D, Mesh3D>();
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; } = Vector3.One;
        // Effect type to use when rendering the object
        // Setting this will propagate the effect type to all polygons in the object
        public EffectType EffectType
        {
            get => effectType;
            set => SetEffect(value);
        }

        public Matrix WorldMatrix
        {
            get
            {
                return
                    Matrix.CreateScale(Scale) *
                    Matrix.CreateFromQuaternion(Rotation) *
                    Matrix.CreateTranslation(Position);
            }
        }

        public Object3D()
        {
        }

        public Object3D(Object3D obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "Object cannot be null.");
            Position = obj.Position;
            Rotation = obj.Rotation;
            Scale = obj.Scale;
            Polygons = new List<Polygon3D>(obj.Polygons);
            PolygonsByTexture = new Dictionary<Texture2D, List<Polygon3D>>(obj.PolygonsByTexture);
            MeshesByTexture = new Dictionary<Texture2D, Mesh3D>(obj.MeshesByTexture);
        }

        public Object3D(Vector3 position, Quaternion eulerRotationRadians, Vector3 scale)
        {
            Position = position;
            Rotation = eulerRotationRadians;
            Scale = scale;
        }

        public Object3D(Vector3 position, Vector3 eulerRotationRadians, Vector3 scale)
        {
            Position = position;
            Rotation = Quaternion.CreateFromYawPitchRoll(
                eulerRotationRadians.Y,
                eulerRotationRadians.X,
                eulerRotationRadians.Z
            );
            Scale = scale;
        }

        public void SetEffect(EffectType effect)
        {
            effectType = effect; 
            foreach (var polygon in Polygons)
            {
                polygon.EffectType = effect;
            }
        }

        public void RotateY(float angleRadians)
        {
            Quaternion rotationY = Quaternion.CreateFromAxisAngle(Vector3.Up, angleRadians);
            Rotation *= rotationY;
        }

        public void RotateX(float angleRadians)
        {
            Quaternion rotationX = Quaternion.CreateFromAxisAngle(Vector3.Right, angleRadians);
            Rotation *= rotationX;
        }

        public void RotateZ(float angleRadians)
        {
            Quaternion rotationZ = Quaternion.CreateFromAxisAngle(Vector3.Forward, angleRadians);
            Rotation *= rotationZ;
        }

        public void Rotate(Vector3 angleDeltaRadians)
        {
            RotateX(angleDeltaRadians.X);
            RotateY(angleDeltaRadians.Y);
            RotateZ(angleDeltaRadians.Z);
        }

        // Adds a polygon to the object and groups it by its texture.
        // This grouping is used when building meshes during rendering.
        // Also computes the polygon normal
        //
        // Future note for Optimization:
        // Consider merging polygons that share the same plane and texture into larger polygons.
        // This can reduce the number of polygons and improve rendering performance.
        // One solution could be to keep a separate "unique vertex" list here and utilize those 
        // vertices later when building meshes.
        public void AddPolygon(Polygon3D polygon)
        {
            if (polygon.Texture == null)
            {
                throw new InvalidOperationException("Polygon must have a texture!");
            }
            polygon.ComputeNormal();
            Polygons.Add(polygon);

            // Group polygons by texture. If there is no entry for the texture, 
            // create a new list for it
            if (!PolygonsByTexture.ContainsKey(polygon.Texture))
            {
                PolygonsByTexture[polygon.Texture] = new List<Polygon3D>();
            }
            PolygonsByTexture[polygon.Texture].Add(polygon);
        }

        public IEnumerable<Polygon3D> GetTransformedPolygons()
        {
            Matrix transform = WorldMatrix;
            foreach (var polygon in Polygons)
            {
                yield return polygon.GetTransformedCopy(transform);
            }
        }

        public void ClearPolygons()
        {
            Polygons.Clear();
            PolygonsByTexture.Clear();
            MeshesByTexture.Clear();
        }

        // Builds meshes from polygons, applying the object's world transformation
        public void BuildMeshes()
        {
            if (PolygonsByTexture.Count == 0)
            {
                throw new InvalidOperationException("No polygons to build meshes from.");
            }
            MeshesByTexture.Clear();
            Matrix transform = WorldMatrix;
            foreach (var pbt in PolygonsByTexture)
            {
                var texture = pbt.Key;
                List<Polygon3D> transformedPolygons = new List<Polygon3D>();
                foreach (var polygon in pbt.Value)
                {
                    transformedPolygons.Add(polygon.GetTransformedCopy(transform));
                }

                if (MeshesByTexture.ContainsKey(texture))
                {
                    throw new InvalidOperationException($"Mesh for texture {texture.Name} already exists.");
                }

                Mesh3D mesh = BuildMeshForTexturePolygons(transformedPolygons, texture);
                MeshesByTexture[texture] = mesh;
            }
        }

        // Updates existing meshes from current polygons and transformation
        // This can be called if polygons or transformation have changed
        public void UpdateObject()
        {
            if (PolygonsByTexture.Count == 0)
            {
                throw new InvalidOperationException("No polygons to build meshes from.");
            }
            MeshesByTexture.Clear();
            foreach (var pbt in PolygonsByTexture)
            {
                var texture = pbt.Key;
                List<Polygon3D> polygons = pbt.Value;
                if (MeshesByTexture.ContainsKey(texture))
                {
                    throw new InvalidOperationException($"Mesh for texture {texture.Name} already exists.");
                }

                Mesh3D mesh = BuildMeshForTexturePolygons(polygons, texture);
                MeshesByTexture[texture] = mesh;
            }
        }

        private Mesh3D BuildMeshForTexturePolygons(List<Polygon3D> polygons, Texture2D texture)
        {
            var vertices = new List<Vertex3D>();
            var normals = new List<Vector3>();
            var textureUVs = new List<Vector2>();
            var indices = new List<int>();

            if (polygons == null || polygons.Count == 0)
            {
                throw new ArgumentException("Polygons cannot be null or empty.");
            }

            if (polygons.Any(p => p.Vertices.Length != 3))
            {
                throw new ArgumentException("All polygons must have exactly three vertices.");
            }
            
            // Iterate through each polygon and extract vertices, normals, and texture coordinates
            
            int vertexOffset = 0;
            foreach (var polygon in polygons)
            {
                foreach(var vertex in polygon.Vertices)
                {
                    vertices.Add(vertex);
                    normals.Add(polygon.Normal);
                    textureUVs.Add(vertex.TextureUV);
                    indices.Add(vertexOffset++);
                }
            }
            return new Mesh3D(texture, vertices, normals, textureUVs, indices);
        }
    }
}