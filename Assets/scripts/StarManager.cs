using UnityEngine;
using System.Collections;

public class StarManager : MonoBehaviour
{
    private GameObject ActiveStars;
    private GameObject InactiveStars;

    public GameObject[] StarsToSpawn;

    private float SpawnTimer;

    public float starSize;


    void Awake()
    {
        ActiveStars = new GameObject("ActiveStars");
        InactiveStars = new GameObject("InactiveStars");
    }

	// Use this for initialization
	void Start () 
    {
        GetNewSpawnTime();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (SpawnTimer > 0)
        {
            SpawnTimer -= Time.deltaTime;
        }
        else
        {
            //spawn
            //Instantiate(StarToSpawn, transform.position, Quaternion.identity);
            Vector3 aSpawnPos = GetSpawnPos();

            DetermineIfSpawnNewObjOrUsePooledObj(aSpawnPos, new Vector3(0, 0, 0), new Vector3(starSize, starSize, starSize));

            //reset timer
            GetNewSpawnTime();
        }
	}

    void GetNewSpawnTime()
    {
        SpawnTimer = Random.Range(GameData.minTimeToSpawn, GameData.maxTimeToSpawn);
    }

    Vector3 GetSpawnPos()
    {
        Vector3 theSpawnPos = new Vector3(Random.Range(GameData.starSpawnXMin, GameData.starSpawnXMax), GameData.starSpawnY, 0);
        return theSpawnPos;
    }


    void DetermineIfSpawnNewObjOrUsePooledObj(Vector3 thePos, Vector3 theRotation, Vector3 theScale)
    {
        if (InactiveStars.transform.childCount > 0)
        {
            //use a pooled object
            MovePooledObj(thePos, new Vector3(theRotation.x, theRotation.y, theRotation.z), theScale);
        }
        else
        {
            //spawn a new one and add it to the active list
            SpawnNewPooledObj(thePos, new Vector3(theRotation.x, theRotation.y, theRotation.z), theScale);
        }

    }

    //spawns a new object when there's not any inactive pooled objects to use
    void SpawnNewPooledObj(Vector3 thePos, Vector3 theRotation, Vector3 theScale)
    {
        GameObject a = Instantiate(StarsToSpawn[Random.Range(0, StarsToSpawn.Length)], thePos, Quaternion.Euler(theRotation.x, theRotation.y, theRotation.z)) as GameObject;
        a.name = "Star";
        a.transform.localScale = theScale;
        a.transform.parent = ActiveStars.transform;
        a.GetComponent<PooledObject>().MyParent = InactiveStars;
    }

    //moves an inactive pooled object to the appropriate place and then makes it set active
    void MovePooledObj(Vector3 thePos, Vector3 theRotation, Vector3 theScale)
    {

        GameObject thePooledObj = InactiveStars.transform.GetChild(0).gameObject;

        thePooledObj.transform.parent = ActiveStars.transform;
        thePooledObj.transform.position = thePos;
        thePooledObj.transform.rotation = Quaternion.Euler(theRotation.x, theRotation.y, theRotation.z);
        thePooledObj.transform.localScale = theScale;
        //thePooledObj.GetComponent<StarController>().StopMovement();
        thePooledObj.GetComponent<StarController>().StartMovement();
        thePooledObj.SetActive(true);

    }
}
