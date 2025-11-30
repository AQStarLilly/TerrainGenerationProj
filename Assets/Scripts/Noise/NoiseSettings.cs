using UnityEngine;

[CreateAssetMenu(
    fileName = "NoiseSettings",
    menuName = "Terrain/Noise Settings")]
public class NoiseSettings : ScriptableObject
{
    [Header("Seed & Offset")]
    public int seed = 12345;
    public Vector2 offset = Vector2.zero;
    public bool useRandomSeed = false;

    [Header("Scale")]
    [Min(0.001f)] public float baseScale = 20f;

    [Header("Octaves (Spectral Synthesis)")]
    [Range(1, 8)] public int octaves = 4;
    [Min(1f)] public float frequency = 2f;      // frequency multiplier per octave
    [Range(0f, 1f)] public float persistence = 0.5f; // amplitude multiplier per octave

    [Header("Height Normalization")]
    public bool normalize = true;
}
