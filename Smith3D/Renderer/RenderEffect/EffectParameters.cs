namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework.Graphics;

    // <summary>
    // Abstract base class for handling effect specific parameters
    // This acts also as a comtainer for shader effect parameters
    //
    // Future improvement:
    // When setting parameters in the concrete parameter class with:
    //    effect.Parameters["ParameterName"]?.SetValue(ParameterValue);
    // we could check if the named attribut actually exists in the shader
    // </summary>
    public abstract class EffectParameters
    {
        public abstract void ApplyTo(Effect effect);
    }
}

