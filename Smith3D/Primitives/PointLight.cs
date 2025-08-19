using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    // Represents a point light source in a 3D scene.
    // This class extends Light3D to include properties specific to point lights, such as attenuation factors.
    // It provides constructors for creating point lights with default or custom parameters.
    // The class can be used to simulate realistic lighting effects in 3D environments.
    // Point lights emit light in all directions from a single point in space, with intensity decreasing


    public class PointLight : Light3D
    {
        // Default attenuation values for point lights
        // These values can be adjusted based on the desired light behavior in the scene.
        // Constant attenuation is the base light intensity.
        // Linear attenuation decreases the light intensity linearly with distance.
        // Quadratic attenuation decreases the light intensity quadratically with distance.
        public const float DefaultConstantAttenuation = 1.0f;
        public const float DefaultLinearAttenuation = 0.1f;
        public const float DefaultQuadraticAttenuation = 0.01f;

        public float ConstantAttenuation { get; set; } = DefaultConstantAttenuation;
        public float LinearAttenuation { get; set; } = DefaultLinearAttenuation;
        public float QuadraticAttenuation { get; set; } = DefaultQuadraticAttenuation;

        public PointLight() : base(LightType.Point, DefaultPosition, DefaultColor, DefaultIntensity)
        {
            ConstantAttenuation = DefaultConstantAttenuation;
            LinearAttenuation = DefaultLinearAttenuation;
            QuadraticAttenuation = DefaultQuadraticAttenuation;
        }

        public PointLight(PointLight light) : base(light)
        {
            ConstantAttenuation = light.ConstantAttenuation;
            LinearAttenuation = light.LinearAttenuation;
            QuadraticAttenuation = light.QuadraticAttenuation;
        }

        public PointLight(Vector3 position, Color color, float intensity)
            : this(position, color, intensity, DefaultConstantAttenuation, DefaultLinearAttenuation, DefaultQuadraticAttenuation)
        {
        }

        public PointLight(Vector3 position)
            : this(position, DefaultColor, DefaultIntensity, DefaultConstantAttenuation, DefaultLinearAttenuation, DefaultQuadraticAttenuation)
        {
        }
        
        public PointLight(Vector3 position, Color color, float intensity,
            float constantAttenuation, float linearAttenuation, float quadraticAttenuation)
            : base(LightType.Point, position, color, intensity)
        {
            ConstantAttenuation = constantAttenuation;
            LinearAttenuation = linearAttenuation;
            QuadraticAttenuation = quadraticAttenuation;
        }

    }
}