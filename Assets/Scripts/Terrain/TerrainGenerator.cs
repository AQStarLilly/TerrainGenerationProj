using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Size")]
    [Range(8, 512)] public int width = 100;
    [Range(8, 512)] public int length = 100;
    [Tooltip("Distance between sample points in world units.")]
    [Min(0.1f)] public float cellSize = 1f;

    [Header("Height Settings")]
    [Min(0f)] public float heightMultiplier = 10f;
    public AnimationCurve heightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Noise Settings")]
    public NoiseSettings noiseSettings;

    [System.Serializable]
    public struct TerrainColor
    {
        public Color color;
        [Range(0f, 1f)] public float height;
    }

    [Header("Color Settings")]
    public TerrainColor[] colorRegions;

    [Tooltip("Texture resolution used to generate the biome texture.")]
    public int textureResolution = 512;

    [Header("Generation")]
    public bool generateOnStart = true;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateTerrain();
        }
    }

    [ContextMenu("Generate Terrain")]
    public void GenerateTerrain()
    {
        if (noiseSettings == null)
        {
            Debug.LogError("TerrainGenerator: NoiseSettings is not assigned.");
            return;
        }

        float[,] heightMap = NoiseGenerator.GenerateHeightMap(
            width,
            length,
            noiseSettings);

        Mesh terrainMesh = MeshGenerator.GenerateTerrainMesh(
            heightMap,
            heightMultiplier,
            heightCurve,
            cellSize);

        meshFilter.sharedMesh = terrainMesh;
        meshCollider.sharedMesh = terrainMesh;

        Texture2D biomeTexture = GenerateColorTexture(heightMap);
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = biomeTexture;
    }

    private Texture2D GenerateColorTexture(float[,] heightMap)
    {
        Texture2D texture = new Texture2D(textureResolution, textureResolution);
        texture.wrapMode = TextureWrapMode.Clamp;

        int mapWidth = heightMap.GetLength(0);
        int mapLength = heightMap.GetLength(1);

        for (int y = 0; y < textureResolution; y++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                // Convert texture pixel - heightmap sample coordinates
                float sampleX = (float)x / (textureResolution - 1) * (mapWidth - 1);
                float sampleY = (float)y / (textureResolution - 1) * (mapLength - 1);

                float height = heightMap[(int)sampleX, (int)sampleY];

                Color pixelColor = DetermineColor(height);
                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        return texture;
    }

    // Pick the correct color region based on height
    private Color DetermineColor(float height)
    {
        foreach (TerrainColor region in colorRegions)
        {
            if (height <= region.height)
            {
                return region.color;
            }
        }

        // Safety fallback
        return colorRegions[colorRegions.Length - 1].color;
    }
}