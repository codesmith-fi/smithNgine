using System;

namespace Codesmith.SmithNgine.Smith3D.Primitives
{
    using System.Collections.Generic;    
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Codesmith.SmithNgine.Smith3D.Primitives;

    public class Mesh3D
    {
        public List<Vertex3D> Vertices { get; set; } = new List<Vertex3D>();
        public List<Vector3> Normals { get; set; } = new List<Vector3>();
        public List<int> Indices { get; set; } = new List<int>();
        public Texture2D Texture { get; set; } = null;
        public List<Vector2> TextureUVs { get; set; } = new List<Vector2>();

        public Mesh3D(Texture2D texture,
            List<Vertex3D> vertices,
            List<Vector3> normals = null,
            List<Vector2> textureUVs = null,
            List<int> indices = null)
        {
            // If no texture is provided, set emtpty UVs list
            if (texture == null || textureUVs == null)
            {
                textureUVs = new List<Vector2>();
            }

            if (vertices == null || vertices.Count == 0)
            {
                throw new ArgumentNullException(nameof(vertices), "Vertices cannot be null or empty.");
            }

            if (indices == null || indices.Count != vertices.Count)
            {
                throw new ArgumentNullException(nameof(indices), "Indices cannot be null or must match the number of vertices.");
            }

            // If normals are not provided, create default normals
            // or use existing normals if provided
            if (normals == null || normals.Count == 0)
            {
                normals = new List<Vector3>(new Vector3[vertices.Count]);
                for (int i = 0; i < vertices.Count; i++)
                {
                    normals[i] = Vector3.Up; // Default normal
                }
            }

            Vertices = vertices;
            Normals = normals;
            Indices = indices;
            Texture = texture;
            TextureUVs = textureUVs;

        }
        public static void CombineMeshes(Mesh3D fromMesh, Mesh3D toMesh) 
        {
            if (fromMesh.Texture != toMesh.Texture)
            {
                throw new ArgumentException("Cannot combine meshes with different textures.");
            }

            int vertexOffset = toMesh.Vertices.Count;

            toMesh.Vertices.AddRange(fromMesh.Vertices);
            toMesh.Normals.AddRange(fromMesh.Normals);
            toMesh.TextureUVs.AddRange(fromMesh.TextureUVs);

            foreach (var index in fromMesh.Indices)
            {
                toMesh.Indices.Add(index + vertexOffset);
            }
        }

        public static Mesh3D MergeMeshes(Mesh3D mesh1, Mesh3D mesh2)
        {
            if (mesh1.Texture != mesh2.Texture)
            {
                throw new ArgumentException("Cannot merge meshes with different textures.");
            }

            var vertices = new List<Vertex3D>(mesh1.Vertices);
            var normals = new List<Vector3>(mesh1.Normals);
            var textureUVs = new List<Vector2>(mesh1.TextureUVs);
            var indices = new List<int>(mesh1.Indices);

            int vertexOffset = vertices.Count;

            foreach (var vertex in mesh2.Vertices)
            {
                vertices.Add(vertex);
                normals.Add(vertex.Normal);
                textureUVs.Add(vertex.TextureUV);
                indices.Add(vertexOffset++);
            }

            return new Mesh3D(mesh1.Texture, vertices, normals, textureUVs, indices);
        }   
    }
}