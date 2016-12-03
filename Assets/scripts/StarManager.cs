using UnityEngine;
using System.Collections;

public class StarManager : MonoBehaviour {

    GameObject activeList;
    GameObject inactiveList;

    public GameObject StarToSpawn;

    public float SpawnTimerMin;
    public float SpawnTimerMax;
    public float SpawnTimer;

	// Use this for initialization
	void Start () 
    {
        activeList = GameObject.Find("ActiveList");
        inactiveList = GameObject.Find("InactiveList");

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
            Instantiate(StarToSpawn, transform.position, Quaternion.identity);

            //reset timer
            GetNewSpawnTime();
        }
	}

    void GetNewSpawnTime()
    {
        SpawnTimer = Random.Range(SpawnTimerMin, SpawnTimerMax);
    }
}
