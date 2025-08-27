namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    // <summary>
    // Abstract base class for handling effect specific parameters
    // This acts also as a comtainer for shader effect parameters
    //
    // Generic design rule: 
    //    All Effects have transformation matrices
    //
    // Future improvement:
    // When setting parameters in the concrete parameter class with:
    //    effect.Parameters["ParameterName"]?.SetValue(ParameterValue);
    // we could check if the named attribut actually exists in the shader
    // </summary>
    public class EffectParameters
    {
        public Matrix World { get; set; } = Matrix.Identity;
        public Matrix View { get; set; } = new Matrix();
        public Matrix Projection { get; set; } = new Matrix();

        public virtual void ApplyTo(Effect effect)
        {
            effect.Parameters["World"]?.SetValue(World);
            effect.Parameters["View"]?.SetValue(View);
            effect.Parameters["Projection"]?.SetValue(Projection);
        }
    }
}

