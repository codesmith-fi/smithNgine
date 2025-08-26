namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework.Graphics;

    // Abstract base class for handling effect specific parameters
    public abstract class EffectParameters
    {
        public abstract void ApplyTo(Effect effect);
    }
}

