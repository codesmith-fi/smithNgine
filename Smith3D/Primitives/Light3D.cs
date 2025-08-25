using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    // Represents a 3D light source in the scene.
    // This class can be extended to create different types of lights (e.g., PointLight, DirectionalLight, SpotLight).
    // It includes properties for position, direction, color, and intensity.
    // The Enabled property allows toggling the light on or off.
    // The constructor initializes the light with default values or from another Light3D instance.
    // The class can be used to manage lighting in 3D scenes, affecting how objects are rendered based on light sources.
    // This class is designed to be flexible and can be extended for specific light types.
    public class Light3D
    {
        public const float DefaultIntensity = 1.0f;
        public static readonly Color DefaultColor = Color.White;
        public const LightType DefaultLightType = LightType.Point;
        public static readonly Vector3 DefaultPosition = Vector3.Zero;

        public enum LightType
        {
            None,
            Ambient,
            Directional,
            Point,
            Spot
        }

        public LightType Type { get; set; } = DefaultLightType;
        public Vector3 Position { get; set; } = DefaultPosition;
        public Color Color { get; set; } = DefaultColor;
        public float Intensity { get; set; } = DefaultIntensity; // Default intensity for lights
        public bool Enabled { get; set; } = true; // Light is enabled by default

        public Light3D() : this(LightType.Ambient, DefaultPosition, DefaultColor, DefaultIntensity)
        {
        }

        public Light3D(Light3D light) : this(light.Type, light.Position, light.Color, light.Intensity)
        {
            Enabled = light.Enabled;
        }

        public Light3D(LightType type, Vector3 position, Color color)
            : this(type, position, color, DefaultIntensity)
        {
        }

        public Light3D(LightType type, Vector3 position, Color color, float intensity)
        {
            Type = type;
            Position = position;
            Color = color;
            Intensity = intensity;
            Enabled = true;
        }        
    }
}
