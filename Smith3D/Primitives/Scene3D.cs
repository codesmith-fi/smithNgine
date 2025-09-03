using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Codesmith.SmithNgine.Smith3D.Primitives;

    /// <summary>
    /// Represents a 3D scene containing objects, lights, and cameras.
    /// </summary>}
    public class Scene3D
    {
        public List<Object3D> Objects { get; set; } = new List<Object3D>();
        public List<Light3D> Lights { get; set; } = new List<Light3D>();
        public Camera3D Camera { get; set; }
        public Dictionary<Texture2D, Mesh3D> MeshesByTexture { get; private set; } = new Dictionary<Texture2D, Mesh3D>();

        public Scene3D(Camera3D camera)
        {
            Camera = camera ?? throw new ArgumentNullException(nameof(camera), "Camera cannot be null.");
        }

        public void AddObject(Object3D obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object cannot be null.");
            }
            Objects.Add(obj);
        }

        public void AddLight(Light3D light)
        {
            if (light == null)
            {
                throw new ArgumentNullException(nameof(light), "Light cannot be null.");
            }
            Lights.Add(light);
        }

        public void Clear()
        {
            Objects.Clear();
            Lights.Clear();
            MeshesByTexture.Clear();
        }

        public void UpdateScene()
        {
            MeshesByTexture.Clear();

            // Update meshes for all objects in the scene and combine them into scene meshes
            foreach (var obj in Objects)
            {
                // Create or update the object's meshes from its polygons
                IEnumerable<Polygon3D> transformedVertices = obj.GetTransformedPolygons();
                obj.UpdateObject();

                // Merge the object's meshes into the scene's meshes
                foreach (var mesh in obj.MeshesByTexture)
                {
                    Mesh3D existingMesh = MeshesByTexture.GetValueOrDefault(mesh.Key);
                    if (existingMesh == null)
                    {
                        MeshesByTexture[mesh.Key] = mesh.Value;
                    }
                    else
                    {
                        // Merge meshes with the same texture
                        // This can be optimized further if needed
                        //   -> instead of creating a new mesh each time we could modify existing mesh
                        //                        MeshesByTexture[mesh.Key] = MergeMeshes(existingMesh, mesh.Value);
                        Mesh3D.CombineMeshes(mesh.Value, existingMesh);
                    }
                }
            }
        }        
    }
}