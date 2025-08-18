using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Codesmith.SmithNgine.Smith3D.Primitives;

    /// <summary>
    /// Represents a camera in a 3D scene.
    /// </summary>
    public class Camera3D
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        public Camera3D(Vector3 position, Vector3 target, Vector3 up, float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            Position = position;
            Target = target;
            Up = up;

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public void UpdateViewMatrix()
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }        
    }

}