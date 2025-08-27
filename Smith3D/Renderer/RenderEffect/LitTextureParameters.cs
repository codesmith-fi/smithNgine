using System;

namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class LitTextureParameters : EffectParameters
    {
        public Matrix World { get; set; } = Matrix.Identity;
        public Matrix View { get; set; } = new Matrix();
        public Matrix Projection { get; set; } = new Matrix();
        public Texture2D Texture { get; set; } = null;
        public Vector3 LightPosition { get; set; } = new Vector3(0, 0, 0);
        public Color LightColor { get; set; } = Color.White;
        public float LightIntensity { get; set; } = 1.0f;
        public float ConstantAttenuation { get; set; } = 0.0f;
        public float LinearAttenuation { get; set; } = 0.0f;
        public float QuadraticAttenuation { get; set; } = 0.0f;
        public float GameTime { get; set; } = 0.0f;

        public override void ApplyTo(Effect effect)
        {
            effect.Parameters["World"]?.SetValue(World);
            effect.Parameters["View"]?.SetValue(View);
            effect.Parameters["Projection"]?.SetValue(Projection);
            effect.Parameters["Texture"]?.SetValue(Texture);
            effect.Parameters["lightPosition"]?.SetValue(LightPosition);
            effect.Parameters["lightColor"]?.SetValue(LightColor.ToVector3());
            effect.Parameters["lightIntensity"]?.SetValue(LightIntensity);
            effect.Parameters["constantAttenuation"]?.SetValue(ConstantAttenuation);
            effect.Parameters["linearAttenuation"]?.SetValue(LinearAttenuation);
            effect.Parameters["quadraticAttenuation"]?.SetValue(QuadraticAttenuation);
            effect.Parameters["gameTime"]?.SetValue(GameTime);
        }
    }
}