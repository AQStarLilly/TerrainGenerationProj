using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    public TerrainGenerator terrainGenerator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (terrainGenerator != null && !terrainGenerator.enabled)
        {
            terrainGenerator.enabled = true;
            terrainGenerator.GenerateTerrain();
        }
    }
}
