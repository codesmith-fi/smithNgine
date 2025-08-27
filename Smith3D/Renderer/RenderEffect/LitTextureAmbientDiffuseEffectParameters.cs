using System;

namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class LitTextureAmbientDiffuseEffectParameters : EffectParameters
    {
        public Texture2D Texture { get; set; }
        public Color AmbientColor { get; set; }
        public float AmbientIntensity { get; set; }
        public Color DiffuseColor { get; set; }
        public float DiffuseIntensity { get; set; }
        public Vector3 LightDirection { get; set; }


        public override void ApplyTo(Effect effect)
        {
            effect.Parameters["AmbientColor"]?.SetValue(AmbientColor.ToVector3());
            effect.Parameters["AmbientIntensity"]?.SetValue(AmbientIntensity);
            effect.Parameters["DiffuseColor"]?.SetValue(DiffuseColor.ToVector3());
            effect.Parameters["DiffuseIntensity"]?.SetValue(DiffuseIntensity);
            Vector3 normalizedDirection = Vector3.Normalize(LightDirection);
            effect.Parameters["LightDirection"]?.SetValue(normalizedDirection);
            effect.Parameters["Texture"]?.SetValue(Texture);
            base.ApplyTo(effect);
        }

    }
}

