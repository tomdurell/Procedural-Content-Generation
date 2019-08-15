using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;




public class PerlinNoiseScript : MonoBehaviour
{
    public int mapX = 256; // for heightmaps, this would be 2^n +1
    public int mapY = 256; // for heightmaps, this would be 2^n +1

    public float sampleSizeX = 4.0f; // perlin sample size
    public float sampleSizeY = 4.0f; // perlin sample size

    public float sampleOffsetX = 2.0f; // to tile, add size to the offset. eg, next tile across would be 6.0f
    public float sampleOffsetY = 1.0f; // to tile, add size to the offset. eg, next tile up would be 5.0f


  

    private Texture2D texture; // texture created for testing


    //  Persistant Functions
    //    ----------------------------------------------------------------------------


    void Start()
    {
        Generate();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Generate();
    }


    void Generate()
    {
        Perlin myPerlin = new Perlin();

        ModuleBase myModule = myPerlin;



        // generates a heightmap to a texture, 
        // and sets the renderer material texture of a cube to the generated texture

        Noise2D heightMap;

        heightMap = new Noise2D(mapX, mapY, myModule);
        heightMap.GeneratePlanar(
            sampleOffsetX,
            sampleOffsetX + sampleSizeX,
            sampleOffsetY,
            sampleOffsetY + sampleSizeY
            );

        texture = heightMap.GetTexture(GradientPresets.Grayscale);

        GetComponent<Renderer>().material.mainTexture = texture;
    }
}