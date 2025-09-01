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
        public EffectType EffectType { get; } = EffectType.Undefined;
        public Object3D SourceObject { get; } = null;

        public RenderableMesh(
            Texture2D texture,
            List<Vertex3D> vertices,
            List<Vector3> normals,
            List<Color> colours,
            List<Vector2> textureUVs,
            List<int> indices) : base(texture, vertices, normals, colours, textureUVs, indices)
        {
            EffectType = EffectType.Undefined;
            SourceObject = null;
        }

        public RenderableMesh(
            EffectType effectType, Object3D sourceObject,
            Texture2D texture,
            List<Vertex3D> vertices,
            List<Vector3> normals,
            List<Color> colours,
            List<Vector2> textureUVs,
            List<int> indices) : base(texture, vertices, normals, colours, textureUVs, indices)
        {
            // Sanity check
            if (sourceObject == null) throw new ArgumentNullException("Object can't be null");

            EffectType = effectType;
            SourceObject = sourceObject;
        }
    }
}

