using UnityEngine;
using System.Collections;

public class ConstellationMainMenuSpawner : MonoBehaviour {

    public float spawnTimerMin;
    public float spawnTimerMax;

    public float spawnTimer;

    public float constellationScaleMin;
    public float constellationScaleMax;

    public GameObject constellationObject;

    public float xSpawnMin;
    public float xSpawnMax;

	// Use this for initialization
	void Start () 
    {
        GetNewSpawnTimer();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            //instantiate
            GameObject a = Instantiate(constellationObject, new Vector3(Random.Range(xSpawnMin, xSpawnMax), transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
            float theScale = Random.Range(constellationScaleMin, constellationScaleMax);
            a.transform.localScale = new Vector3(theScale, theScale, theScale);

            GetNewSpawnTimer();
        }
	}

    void GetNewSpawnTimer()
    {
        spawnTimer = Random.Range(spawnTimerMin, spawnTimerMax);
    }
}
