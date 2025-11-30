using UnityEngine;

public class MeshGenerator
{
    /// <summary>
    /// Builds a Mesh from a height map.
    /// </summary>
    public static Mesh GenerateTerrainMesh(
        float[,] heightMap,
        float heightMultiplier,
        AnimationCurve heightCurve,
        float cellSize)
    {
        int width = heightMap.GetLength(0);
        int length = heightMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * length];
        int[] triangles = new int[(width - 1) * (length - 1) * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        int vertIndex = 0;
        int triIndex = 0;

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float height = heightCurve.Evaluate(heightMap[x, z]) * heightMultiplier;

                vertices[vertIndex] = new Vector3(
                    x * cellSize,
                    height,
                    z * cellSize);

                // Simple UV mapping 0-1 across terrain
                uvs[vertIndex] = new Vector2(
                    (float)x / (width - 1),
                    (float)z / (length - 1));

                // Build triangles except on last row/column
                if (x < width - 1 && z < length - 1)
                {
                    int a = vertIndex;
                    int b = vertIndex + width;
                    int c = vertIndex + width + 1;
                    int d = vertIndex + 1;

                    // First triangle
                    triangles[triIndex + 0] = a;
                    triangles[triIndex + 1] = b;
                    triangles[triIndex + 2] = c;

                    // Second triangle
                    triangles[triIndex + 3] = a;
                    triangles[triIndex + 4] = c;
                    triangles[triIndex + 5] = d;

                    triIndex += 6;
                }

                vertIndex++;
            }
        }

        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
