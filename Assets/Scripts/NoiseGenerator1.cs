using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator1
{


    private int octaves; //can also be called layers. Increasing this increases the level of detail in the terrain.
    private float lacunarity; //determines how fast the frequency changes for each octave.
    private float gain; //determines how fast the amplitude changes for each octave. Also called persistence.
    private float perlinScale;
    NoiseGenerator2 pNoise = ScriptableObject.CreateInstance<NoiseGenerator2>();

    public NoiseGenerator1() { }

    public NoiseGenerator1(int octaves, float lacunarity, float gain, float perlinScale)
    {
        this.octaves = octaves;
        this.lacunarity = lacunarity;
        this.gain = gain;
        this.perlinScale = perlinScale;
       
    } 

    public float SimpleNoise()
    {
        return Random.value;
    }

    public float PerlinNoise(float x, float z)
    {
    
        return (float)(2 * pNoise.GetPerlinNoise(x,0,z,octaves,lacunarity) - 1);
    }

    public float FractalNoise(float x, float z) //implementing FBM with the help of perlin noise because it is better at terrains looking more realistic
    {
        float fractalNoise = 0;

        float frequency = 1;
        float amplitude = 1;

        for (int i = 0; i < octaves; i++)
        {
            float xVal = x * frequency * perlinScale;
         
            float zVal = z * frequency * perlinScale;

            fractalNoise += amplitude * PerlinNoise(xVal, zVal);

            frequency *= lacunarity;
            amplitude *= gain;
        }

        return fractalNoise;
    }
}

