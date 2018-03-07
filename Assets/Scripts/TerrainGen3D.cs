using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen3D : MeshGenerator
{

    [SerializeField] private int resolutionX = 20;
    [SerializeField] private int resolutionZ = 20;

    [SerializeField, Range(1, 8)] private int octaves = 1; //value beyond 8 does not make a difference
    [SerializeField] private float lacunarity = 2;
    [SerializeField, Range(0, 1)] private float gain = 0.5f; //best value for gain

    [SerializeField] private float perlinScale = 1;
    [SerializeField] private float UVScale = 1;
    [SerializeField] private float meshScale = 1;
    [SerializeField] private float YScale = 1;

    [SerializeField] private float height;
    [SerializeField] private float color;
    [SerializeField] private string regionName;

    [SerializeField] private FallOffEnum fall;
    [SerializeField] private float fallOffsize;
    [SerializeField] private float Sealevel;

    [SerializeField] private Gradient grad;
    [SerializeField] private float gradMin = -3;
    [SerializeField] private float gradMax = 5;


    protected override void SetMeshNumber()
    {
        numVertices = (resolutionX + 1) * (resolutionZ + 1);  //making a grid from bottom left to right and forward
        numTriangles = 6 * resolutionX * resolutionZ;
    }

    protected override void SetVertices()
    {
        float v, y, w = 0;
        NoiseGenerator1 noise = new NoiseGenerator1(octaves, lacunarity, gain, perlinScale);

        for (int z = 0; z <= resolutionZ; z++)
        {
            for (int x = 0; x <= resolutionX; x++)
            {
                v = ((float)x / resolutionX) * meshScale;
                w = ((float)z / resolutionZ) * meshScale;

                y = YScale * noise.FractalNoise(v, w);
                y = FallOffType((float)x, y, (float)z);

                vertices.Add(new Vector3(v, y, w));
            }
        }
    }

    protected override void SetTriangles()
    {
        int numTriangle = 0;
        for (int z = 0; z < resolutionZ; z++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                triangles.Add(numTriangle);
                triangles.Add(numTriangle + resolutionX + 1);
                triangles.Add(numTriangle + 1);

                triangles.Add(numTriangle + 1);
                triangles.Add(numTriangle + resolutionX + 1);
                triangles.Add(numTriangle + resolutionX + 2);

                numTriangle++;
            }
            numTriangle++;
        }
    }


    protected override void SetUVs()
    {
        for (int z = 0; z <= resolutionZ; z++)
        {
            for (int x = 0; x <= resolutionX; x++)
            {
                UVs.Add(new Vector2(x / (UVScale * resolutionX), z / (UVScale * resolutionZ)));
            }
        }
    }


    protected override void SetVertexColors()
    {

        float diff = gradMax - gradMin;
        for (int i = 0; i < numVertices; i++)
        {
            vertexColors.Add(grad.Evaluate((vertices[i].y - gradMin) / diff));
        }

    }
    private float FallOffType(float x, float height, float z)
    {
        x = x - resolutionX / 2f;
        z = z - resolutionZ / 2f;

        float fallOff = 0;

        switch (fall)
        {
            case FallOffEnum.None:
                return height;

            case FallOffEnum.Circle:
                fallOff = Mathf.Sqrt(x * x + z * z) / fallOffsize; //increasing size will reduce the height
                return getHeight(fallOff, height);

            case FallOffEnum.Square:
                fallOff = Mathf.Sqrt(x * x * x * x + z * z * z * z) / fallOffsize;
                return getHeight(fallOff, height);
            default:
                print("unknown falloff " + fall);
                return height;

        }


    }


    private float getHeight(float fallOff, float height)
    {
        if (fallOff < 1)
        {
            fallOff = fallOff * fallOff * (3 - 2 * fallOff);

            height = height - fallOff * (height - Sealevel);


        }
        else
        {
            height = Sealevel;
        }
        return height;
    }

}
