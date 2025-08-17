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
                return Matrix.CreateScale(Scale) *
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
            Quaternion delta = Quaternion.CreateFromYawPitchRoll(
                angleDeltaRadians.Y,
                angleDeltaRadians.X,
                angleDeltaRadians.Z
            );
            Rotation *= delta;
        }

        public void AddPolygon(Polygon3D polygon)
        {
            if (polygon.Texture == null)
            {
                throw new InvalidOperationException("Polygon must have a texture!");
            }
            Polygons.Add(polygon);
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

        public void BuildMeshes()
        {
            MeshesByTexture = new Dictionary<Texture2D, Mesh3D>();
            IEnumerable<Polygon3D> transformedPolygons =
                GetTransformedPolygons();

            foreach (var polygon in transformedPolygons)
            {
            }
        }
    }
}