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
    }
}