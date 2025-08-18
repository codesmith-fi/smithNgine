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
    }
}