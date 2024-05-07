using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{

    public static float scale = 32f;

    public static int octaves = 2;

    public static float persistance = 0.43f;

    public static float lacunarity = 2.66f;

    //need to change noisemap to output int value between 0 and max chunk height rather than float value between 0 and 1
    //or, offset the 0 to 1 value from a base "ground height" value, with a magnitude multiplier
    public static float[,] GenerateNoiseArea(Vector2 offset)
    {
        float[,] noiseMap = new float[WorldSettings.CHUNK_SIZE.x, WorldSettings.CHUNK_SIZE.z]; 

        System.Random prng = new System.Random(WorldSettings.SEED);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x / scale;
            float offsetZ = prng.Next(-100000, 100000) + offset.y / scale; //appropriate name would be offset.z, but Vector2 property name can't be changed
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        if (scale <= 0)
            scale = 0.0001f;

        float maxNoiseHeight = 1.5f;
        float minNoiseHeight = 0f;

        float halfWidth = WorldSettings.CHUNK_SIZE.x / 2f;
        float halfDepth = WorldSettings.CHUNK_SIZE.z / 2f;

        for (int z = 0; z < WorldSettings.CHUNK_SIZE.z; z++)
        {
            for (int x = 0; x < WorldSettings.CHUNK_SIZE.x; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x * frequency;
                    float sampleZ = (z - halfDepth) / scale * frequency + octaveOffsets[i].y * frequency;
                    
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                /*if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }*/
                noiseMap[x, z] = noiseHeight;
            }
        }

        for (int z = 0; z < WorldSettings.CHUNK_SIZE.z; z++)
        {
            for (int x = 0; x < WorldSettings.CHUNK_SIZE.x; x++)
            {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, z]);
            }
        }

        return noiseMap;
    }

    public static BlockMap GenerateBlockMap(float[,] noise)
    {
        BlockMap blockMap = new BlockMap(WorldSettings.CHUNK_BLOCK_COUNT);

        for (int x = 0; x < WorldSettings.CHUNK_SIZE.x; x++)
        {
            for (int y = 0; y < WorldSettings.CHUNK_SIZE.y; y++)
            {
                for (int z = 0; z < WorldSettings.CHUNK_SIZE.z; z++)
                {
                    if (y < WorldSettings.MIN_GROUND_HEIGHT + Mathf.Clamp((int)(noise[x, z] * WorldSettings.PERLIN_AMPLITUDE), 0, WorldSettings.CHUNK_HEIGHT) - Mathf.Clamp((int)(noise[x, z] * WorldSettings.PERLIN_AMPLITUDE / 2.5f), 0, WorldSettings.CHUNK_HEIGHT)
                        && y > 0)
                    {
                        blockMap[x, y, z] = BlockType.STONE;
                        continue;
                    }
                    else if (y == 0)
                    {
                        blockMap[x, y, z] = BlockType.BEDROCK;
                        continue;
                    }

                    if (y >= WorldSettings.MIN_GROUND_HEIGHT + Mathf.Clamp((int)(noise[x, z] * WorldSettings.PERLIN_AMPLITUDE), 0, WorldSettings.CHUNK_HEIGHT) - Mathf.Clamp((int)(noise[x, z] * WorldSettings.PERLIN_AMPLITUDE / 2.5f), 0, WorldSettings.CHUNK_HEIGHT) && y < WorldSettings.MIN_GROUND_HEIGHT + Mathf.Clamp((int)(noise[x, z] * WorldSettings.PERLIN_AMPLITUDE), 0, WorldSettings.CHUNK_HEIGHT))
                        blockMap[x, y, z] = BlockType.DIRT;
                    else if (y == WorldSettings.MIN_GROUND_HEIGHT + Mathf.Clamp((int)(noise[x, z] * WorldSettings.PERLIN_AMPLITUDE), 0, WorldSettings.CHUNK_HEIGHT))
                        blockMap[x, y, z] = BlockType.GRASS;
                    else
                        blockMap[x, y, z] = BlockType.AIR;
                }
            }
        }

        return blockMap;
    }
}