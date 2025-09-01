using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    // Enumeration for different types of rendering effects
    // These are kept separate from the LightType defined by Light3D
    // This can be expanded as needed for additional effects
    public enum EffectType
    {
        Undefined,
        Basic,
        BasicTexture,
        LitTextureAmbient,
        LitTextureAmbientDiffuse
    }
}
