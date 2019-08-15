using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

public class GalaxySpawner : MonoBehaviour {
 
    public int numberOfPlanets = 15;
    public GameObject spawnModel, asteroidBelt;
    public int planetMaxDistance = 5;
    public int planetMinDistance = 2; 
    List<GameObject> PlanetOrbits = new List<GameObject>();
    List<SphereCollider> planetColliders = new List<SphereCollider>();
    SphereCollider thisCollider;
    private GameObject[] moonRotations;
    private GameObject[] planetRotations;
    Vector3 origin = Vector3.zero;
    public float spinRate = 0.01f;
    float celestialSpinRate = 0.01f;
    float planetSpinRate = 0.1f;
    public Material defaultMaterial;
    float planetScaleDecider;
    Vector3 planetScale;
    Vector3 planetPosition = new Vector3(0, 0, 0);
    public GameObject moon;
    


    // Use this for initialization
    void Awake () {
        Random.InitState(seedGenerator.seed);
        spawnPlanets();
        
    }

    void spawnPlanets()
    {

        for (int counter = 0; counter < numberOfPlanets; counter++) 
        {
            //planet location calculations
            float planetAngle = Random.Range(0f, 360f);
            float planetDistance = Random.Range(planetMinDistance, planetMaxDistance);
            //Debug.Log("Planet Min Distance is: " + planetMinDistance + "Planet Max Distance is: " + planetMaxDistance);
           // Debug.Log("Planet Distance is: " + planetDistance);
            planetPosition.x = ((Mathf.Cos(planetAngle)) * planetDistance);
            planetPosition.z = ((Mathf.Sin(planetAngle)) * planetDistance);
           


            if (collisionChecker(planetPosition)) //if the check for collisions returns true (no collision)
            {

                //planet spawning
                GameObject currentPlanet = Instantiate(spawnModel, origin + planetPosition, Random.rotation); //spawn in the object at calculated position offset from the origin, apply random rotation
                planetScaleDecider = Random.Range(0.2f, 1.0f); // randomly inflate the object

                thisCollider = currentPlanet.AddComponent(typeof(SphereCollider)) as SphereCollider; // create a collider
                planetColliders.Add(thisCollider); // add the collider
                planetColliders[counter].radius *= 2f; //inflate che collider to twice the size

               

                //applying scale
                currentPlanet.transform.localScale = new Vector3(planetScaleDecider, planetScaleDecider, planetScaleDecider);

                //celestal object decider
                int celestialOrbiter = Random.Range(1, 4);

               // different spawn patterns based on moon or asteroid belt
                if (celestialOrbiter == 1)
                {
                    GameObject moonObject = Instantiate(moon, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                    moonObject.transform.parent = currentPlanet.transform;
                    float celestialAngle = Random.Range(0f, 360f);
                    float celestialDistance = Random.Range(planetScaleDecider * 1.8f, 5.5f);
                    float celestialX = ((Mathf.Cos(celestialAngle)) * celestialDistance);
                    float celestialZ = ((Mathf.Cos(celestialAngle)) * celestialDistance);
                    moonObject.transform.position = currentPlanet.transform.position + new Vector3(celestialX, 0f, celestialZ); //planet scale decider * distance away from planet 
                    moonObject.transform.localScale = new Vector3((planetScaleDecider / 100), (planetScaleDecider / 100), (planetScaleDecider / 100));
                    applyPerlin(moonObject);
                    
                }
                else if (celestialOrbiter == 2)
                {
                    float asteroidDistance = Random.Range(planetScaleDecider, planetScaleDecider * 2);
                    for (int i = 0; i < 25; i++)
                    {
                        GameObject asteroidObject;

                        Vector3 asteroidLocation = Vector3.zero;
                        float asteroidAngle = Random.Range(0f, 360f);
                       
                        asteroidLocation.x = ((Mathf.Cos(asteroidAngle)) * asteroidDistance);
                        asteroidLocation.z = ((Mathf.Sin(asteroidAngle)) * asteroidDistance);
                        asteroidObject = Instantiate(asteroidBelt, currentPlanet.transform.position + asteroidLocation, Random.rotation);
                        asteroidObject.transform.parent = currentPlanet.transform;



                    }
                }

                //add planet to the list
                PlanetOrbits.Add(currentPlanet);
                currentPlanet.transform.parent = GameObject.Find("Galaxy Spawn Node").transform;


                applyPerlin(currentPlanet);

            }
            else
            {
                counter--;
                Debug.Log("Overlapping - Fuck it off!");
            }


        }
    }

    void FixedUpdate()
    {
        Transform galaxySpin = GameObject.Find("Galaxy Spawn Node").transform;
        moonRotations = GameObject.FindGameObjectsWithTag("Moon");
        planetRotations = GameObject.FindGameObjectsWithTag("Planet");
        foreach(GameObject moon in moonRotations)
        {
            Transform moonSpin = moon.transform;
            celestialSpinRate = Random.Range(0.01f, 0.1f);
            moonSpin.Rotate(0, celestialSpinRate, 0);


        }
        foreach (GameObject planet in planetRotations)
        {
            Transform planetSpin = planet.transform;
            planetSpin.Rotate(0, planetSpinRate, 0);


        }

        galaxySpin.Rotate(0, spinRate, 0);
    }

    Texture GeneratePerlin()
    {
        int mapX = 256; // for heightmaps, this would be 2^n +1
        int mapY = 256; // for heightmaps, this would be 2^n +1

        float sampleSizeX = 4.0f; // perlin sample size
        float sampleSizeY = 4.0f; // perlin sample size

        float sampleOffsetX = 2.0f; // to tile, add size to the offset. eg, next tile across would be 6.0f
        float sampleOffsetY = 1.0f; // to tile, add size to the offset. eg, next tile up would be 5.0f
        Texture2D texture;

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
        return texture;
    }

    void applyPerlin(GameObject needsMaterial)
    {
        Material planetMat = new Material(defaultMaterial);
        Texture perlinTexture = GeneratePerlin();
        planetMat.SetTexture("_MainTex", perlinTexture);
        float H = Random.Range(0f, 1f);
        float S = Random.Range(0f, 1f);
        Color color = Color.HSVToRGB(H, S, 1);
        planetMat.SetColor("_Color", color);
        needsMaterial.GetComponent<MeshRenderer>().sharedMaterial = planetMat;
        Texture2D planetBumpMap = GenerateBumpMap((Texture2D)perlinTexture);
        planetMat.SetTexture("_BumpMap", planetBumpMap);
    }

    Texture2D GenerateBumpMap(Texture2D bumpSource)
    {
        float xLeft;
        float xRight;
        float yUp;
        float yDown;
        float xDelta;
        float yDelta;
        Texture2D bumpTexture = new Texture2D(bumpSource.width,bumpSource.height);
        for (int y = 0; y < bumpTexture.height; y++)
        {
            for (int x = 0; x < bumpTexture.width; x++)
            {

                xLeft = bumpSource.GetPixel(x - 1, y).grayscale;
                xRight = bumpSource.GetPixel(x + 1, y).grayscale;
                yUp = bumpSource.GetPixel(x, y - 1).grayscale;
                yDown = bumpSource.GetPixel(x, y + 1).grayscale;
                xDelta = ((xLeft - xRight) + 1) * 2.5f;
                yDelta = ((yUp - yDown) + 1) * 2.5f;

                bumpTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, 1.0f));

            }
        }
        bumpTexture.Apply();
        return bumpTexture;
    }

    bool collisionChecker(Vector3 position)
    {

        foreach (Collider collider in planetColliders)
        {
            if (collider.bounds.Contains(position))
            {
                Debug.Log("Overlapping");
                return false;
            }
           
        }
        return true;
    }
        
        
    


}
