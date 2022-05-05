using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldGeneration
{




    public static float RidgidNoise2D(float x, float z, int layers, float scale, int seed)
    {
 

        float v = 1-Mathf.Abs(1-2f*PerlinNoise2D(x, z, scale, seed));

        for (int i = 0; i < layers; i++)
            v *= v;
  
        

    
        return v;
    }

    public static float PerlinNoise2D(float x, float z, float scale,int seed)
    {


        return Mathf.PerlinNoise(x * scale + seed, z * scale + seed);
    }

}
