using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Light3D
    {
        public enum LightType
        {
            Directional,
            Point,
            Spot
        }

        public LightType Type { get; set; } = LightType.Point;
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Direction { get; set; } = Vector3.Forward;
        public Color Color { get; set; } = Color.White;
        public float Intensity { get; set; } = 1.0f; // Default intensity for lights
        public float Range { get; set; } // For point and spot lights
        public float SpotAngle { get; set; } // For spot lights
        public float Falloff { get; set; } = 1.0f; // Default falloff for lights
        public bool Enabled { get; set; } = true; // Light is enabled by default

        public Light3D() : this(LightType.Directional, Vector3.Zero, Vector3.Forward, Color.White, 1.0f)
        {
            Range = 100f;
            SpotAngle = MathHelper.PiOver4;
            Enabled = true;
        }

        public Light3D(Light3D light) : this(light.Type, light.Position, light.Direction, light.Color, light.Intensity)
        {
            Range = light.Range;
            SpotAngle = light.SpotAngle;
            Enabled = light.Enabled;
        }

        public Light3D(LightType type, Vector3 position, Vector3 direction, Color color, float intensity)
        {
            Type = type;
            Position = position;
            Direction = direction;
            Color = color;
            Intensity = intensity;
            Range = 100f; // Default range for point lights
            SpotAngle = MathHelper.PiOver4; // Default spot angle
            Enabled = true;
        }
    }
}
