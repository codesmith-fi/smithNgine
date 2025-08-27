namespace Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect
{
    using System;
    using Microsoft.Xna.Framework.Graphics;
    using Codesmith.SmithNgine.Smith3D.Primitives;
    // <summary>
    // Interface for managing effects in the renderer
    // Register, Get and set Effect related Parameters
    // </summary>
    public interface IEffectHandler
    {
        /// <summary>
        /// Registers an effect with its associated EffectType.
        /// </summary>
        public void RegisterEffect<TParameters>(EffectType type, Effect effect)
            where TParameters : EffectParameters, new();

        /// <summary>
        /// Attempts to retrieve the effect associated with the given effect type.
        /// </summary>
        public bool TryGetEffect(EffectType type, out Effect effect);

        /// <summary>
        /// Applies the given parameters to the effect associated with the type.
        /// </summary>
        public bool ApplyParameters(EffectType type, EffectParameters parameters);
        }
}
