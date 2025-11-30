using UnityEngine;

public class NoiseGenerator
{
    /// <summary>
    /// Generates a 2D height map using multi-octave Perlin noise.
    /// </summary>
    public static float[,] GenerateHeightMap(
        int width,
        int length,
        NoiseSettings settings)
    {
        float[,] heightMap = new float[width, length];

        // Seed PRNG
        int seedToUse = settings.useRandomSeed
            ? Random.Range(int.MinValue, int.MaxValue)
            : settings.seed;
        System.Random prng = new System.Random(seedToUse);

        // Precompute octave offsets
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x;
            float offsetY = prng.Next(-100000, 100000) + settings.offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (settings.baseScale <= 0f)
            settings.baseScale = 0.001f;

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        // Loop over grid
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x + octaveOffsets[i].x) / settings.baseScale * frequency;
                    float sampleZ = (z + octaveOffsets[i].y) / settings.baseScale * frequency;

                    // Unity's Perlin is [0,1], shift to [-1,1]
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2f - 1f;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistence;
                    frequency *= settings.frequency;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;

                heightMap[x, z] = noiseHeight;
            }
        }

        // Normalize to [0,1] if requested
        if (settings.normalize)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    heightMap[x, z] = Mathf.InverseLerp(minLocalNoiseHeight,
                                                        maxLocalNoiseHeight,
                                                        heightMap[x, z]);
                }
            }
        }

        return heightMap;
    }
}
