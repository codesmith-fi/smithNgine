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
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }
        public Vector3 Up { get; private set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        public Camera3D(Vector3 position, Vector3 target, Vector3 up, float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            Position = position;
            Target = target;
            Up = up;

            UpdateViewMatrix();
            UpdateProjectionMatrix(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
            UpdateViewMatrix();
        }   

        public void SetTarget(Vector3 target)
        {
            Target = target;
            UpdateViewMatrix();
        }
        
        public void SetUp(Vector3 up)
        {
            Up = up;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }

        private void UpdateProjectionMatrix(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);

            // Flip the projection matrix to match XNA's coordinate system
            // This is necessary because XNA uses a right-handed coordinate system
            // while many 3D engines use a left-handed coordinate system.
            // This line flips the Y-axis in the projection matrix.
            ProjectionMatrix *= Matrix.CreateScale(-1f, 1.0f, 1.0f); 
        }
    }

}