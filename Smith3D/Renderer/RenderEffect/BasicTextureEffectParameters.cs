namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    // <summary>
    // Parameters for basic effect having transofmrations and texture only
    // No lighting!
    // </summary>
    public class BasicTextureEffectParameters : EffectParameters
    {
        public Texture2D Texture { get; set; }

        public override void ApplyTo(Effect effect) 
        {
            effect.Parameters["Texture"]?.SetValue(Texture);
            base.ApplyTo(effect);
        }
    }
}

