namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    public class BasicTextureParameters : EffectParameters
    {
        public Texture2D Texture { get; set; }

        public override void ApplyTo(Effect effect)
        {
            effect.Parameters["Texture"]?.SetValue(Texture);
        }
    }

}

