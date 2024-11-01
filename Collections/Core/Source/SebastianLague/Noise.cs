using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    // Credit for this wonderfull~ class to Sebastian Lague (I know everyone in 2024 in the gamedev field arleady)
    // (knows them, they're absolutely one chad of a person), as of 2024 this class was made like, 8 years ago lol
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float lerpMin = 0.5f, float lerpMax = 1f)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            UnityEngine.Random.InitState(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = UnityEngine.Random.Range(-100000, 100000);
                float offsetY = UnityEngine.Random.Range(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;


            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {

                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x + offset.x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y + offset.y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(lerpMin, lerpMax, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }

    }
}
