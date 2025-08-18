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
        public List<Polygon3D> Polygons { get; private set; } = new List<Polygon3D>();
        public Dictionary<Texture2D, List<Polygon3D>> PolygonsByTexture { get; private set; } = new Dictionary<Texture2D, List<Polygon3D>>();
        public Dictionary<Texture2D, Mesh3D> MeshesByTexture { get; private set; } = new Dictionary<Texture2D, Mesh3D>();
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; } = Vector3.One;
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

        public void AddPolygon(Polygon3D polygon)
        {
            if (polygon.Texture == null)
            {
                throw new InvalidOperationException("Polygon must have a texture!");
            }
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

        public void BuildMeshesFromPolygons()
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
            var indices = new List<ushort>();

            if (polygons == null || polygons.Count == 0)
            {
                throw new ArgumentException("Polygons cannot be null or empty.");
            }

            if (polygons.Any(p => p.Vertices.Length != 3))
            {
                throw new ArgumentException("All polygons must have exactly three vertices.");
            }
            
            // Iterate through each polygon and extract vertices, normals, and texture coordinates
            
            ushort vertexOffset = 0;
            foreach (var polygon in polygons)
            {
                // Transform the polygon vertices using the object's world matrix
                Polygon3D transformedPolygon = polygon.GetTransformedCopy(WorldMatrix);
                if (transformedPolygon == null)
                {
                    throw new InvalidOperationException("Transformed polygon is null.");
                }
                vertices.AddRange(transformedPolygon.Vertices);
                normals.AddRange(transformedPolygon.Vertices.Select(v => v.Normal));
                textureUVs.AddRange(transformedPolygon.Vertices.Select(v => v.TextureUV));
                foreach (var vertex in transformedPolygon.Vertices)
                {
                    indices.Add(vertexOffset++);
                }
            }
            return new Mesh3D(texture, vertices, normals, textureUVs, indices);
        }
    }
}