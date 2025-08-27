namespace Codesmith.SmithNgine.Smith3D.Renderer
{
    using System;
    using System.Collections.Generic;    
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Codesmith.SmithNgine.Smith3D.Primitives;
    using Codesmith.SmithNgine.Smith3D.Renderer.RenderEffect;
    // <summary>
    // Wrapper class for mapping meshes, objects and effects in use
    // </summary>
    public class RenderableMesh : Mesh3D
    {
        public EffectType EffectType { get; } = EffectType.BasicColor;
        public Object3D SourceObject { get; } = null;
        public EffectParameters Parameters { get; } = null;

        public RenderableMesh(
            Texture2D texture,
            List<Vertex3D> vertices,
            List<Vector3> normals,
            List<Vector2> textureUVs,
            List<int> indices) : base(texture, vertices, normals, textureUVs, indices)
        {
            EffectType = EffectType.BasicColor;
            SourceObject = null;
            Parameters = null;
        }

        public RenderableMesh(
            EffectType effectType, Object3D sourceObject, EffectParameters parameters,
            Texture2D texture,
            List<Vertex3D> vertices,
            List<Vector3> normals,
            List<Vector2> textureUVs,
            List<int> indices) : base(texture, vertices, normals, textureUVs, indices)
        {
            // Sanity check
            if (sourceObject == null) throw new ArgumentNullException("Object can't be null");
            if (parameters == null) throw new ArgumentNullException("Effect parameters can't be null");

            EffectType = effectType;
            SourceObject = sourceObject;
            Parameters = parameters;
        }
    }
}

