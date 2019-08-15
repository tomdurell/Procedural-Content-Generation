using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour {
    public GameObject[] allianceWings; //aWing1, aWing2, aWing3, aWing4
   
    public GameObject[] allianceBodies;
  
    public GameObject[] allianceEngines; //aEngine1, aEngine2, aEngine3, aEngine4
  

    public static List<GameObject> allianceShips = new List<GameObject>();
  

    int shipRows = 5;
    int shipCollumns = 5;
    public static int boundingBox = 30;
    public static Vector3 boundingBoxOrigin;
    Vector3 AllianceEngineOffset = new Vector3(0, 0, -1.7f);
    public GameObject emptyA;
   

    // Use this for initialization
    void Start () {
        
        float boundingBoxAngle = Random.Range(0f, 360f);
        float boundingBoxDistance = Random.Range(80f, 140f);
        boundingBoxOrigin.x = ((Mathf.Cos(boundingBoxAngle)) * boundingBoxDistance);
        boundingBoxOrigin.z= ((Mathf.Sin(boundingBoxAngle)) * boundingBoxDistance);

        transform.position = new Vector3(0, 0, 0);
        Random.InitState(seedGenerator.seed);
       
            for (int j = 0; j <= shipRows; j++)
            {
                for (int i = 0; i <= shipCollumns; i++)
                {
                    
                       
                    GameObject shipMaster = Instantiate(emptyA, boundingBoxOrigin, Quaternion.Euler(0, -90, 0));
                    GameObject shipBody = Instantiate(allianceBodies[Random.Range(0, 4)], shipMaster.transform.position, Quaternion.Euler(0, 90, 0));
                    GameObject shipWings = Instantiate(allianceWings[Random.Range(0, 4)], shipMaster.transform.position, Quaternion.Euler(0, 180, 0));
                    GameObject shipEngines = Instantiate(allianceEngines[Random.Range(0, 4)], shipMaster.transform.position, Quaternion.Euler(0, 0, 0));
                    shipBody.transform.parent = shipMaster.transform;
                    shipEngines.transform.localPosition += AllianceEngineOffset;
                    shipEngines.transform.parent = shipMaster.transform;
                    shipWings.transform.parent = shipMaster.transform;
                        
                    allianceShips.Add(shipMaster);
                    boundingBoxOrigin.z += 5;
                }
                boundingBoxOrigin.z -= 30;
                boundingBoxOrigin.x += 10;
            }
          
        
    }
	
	
}
