namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    // <summary>
    // Parameters for basic effect having transofmrations and texture only
    // No lighting!
    // </summary>
    public class BasicTextureParameters : EffectParameters
    {
        public Matrix World { get; set; } = Matrix.Identity;
        public Matrix View { get; set; } = new Matrix();
        public Matrix Projection { get; set; } = new Matrix();
        public Texture2D Texture { get; set; }

        public override void ApplyTo(Effect effect)
        {
            effect.Parameters["World"]?.SetValue(World);
            effect.Parameters["View"]?.SetValue(View);
            effect.Parameters["Projection"]?.SetValue(Projection);
            effect.Parameters["Texture"]?.SetValue(Texture);
        }
    }
}

