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

        public LightType Type { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Color Color { get; set; }
        public float Intensity { get; set; }
        public float Range { get; set; } // For point and spot lights
        public float SpotAngle { get; set; } // For spot lights
        public bool Enabled { get; set; }

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
