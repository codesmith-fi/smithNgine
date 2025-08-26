namespace Codesmith.SmithNgine.Smith3D.Renderer
{
    using System;
    using Codesmith.SmithNgine.Smith3D.Primitives;
    using Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect;
    // <summary>
    // Wrapper class for mapping meshes, objects and effects in use
    // </summary>
    public class RenderableMesh
    {
        public Mesh3D Mesh { get; }
        public EffectType EffectType { get; }
        public Object3D SourceObject { get; }
        public EffectParameters Parameters { get; }

        public RenderableMesh(Mesh3D mesh, EffectType effectType, Object3D sourceObject, EffectParameters parameters)
        {
            // Sanity check
            if (mesh == null) throw new ArgumentNullException("Mesh can't be null");
            if (sourceObject == null) throw new ArgumentNullException("Object can't be null");
            if (parameters == null) throw new ArgumentNullException("Effect parameters can't be null");

            Mesh = mesh;
            EffectType = effectType;
            SourceObject = sourceObject;
            Parameters = parameters;
        }

    }

}

