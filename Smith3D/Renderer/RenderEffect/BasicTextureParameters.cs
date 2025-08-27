namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    // <summary>
    // Parameters for basic effect having a texture only
    // </summary>
    public class BasicTextureParameters : EffectParameters
    {
        public Texture2D Texture { get; set; }

        public override void ApplyTo(Effect effect)
        {
            effect.Parameters["Texture"]?.SetValue(Texture);
        }
    }
}

