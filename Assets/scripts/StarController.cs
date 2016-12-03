using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {

    //public enum starType;

    public GameData.StarType theStarType;

    private float starSpeed;

    public float destroyStarWhenBelowThisYValue;

    [HideInInspector]
    public GameData.Star starData;

    void Awake()
    {
        starData = new GameData.Star(this);
    }
	// Use this for initialization
	void Start () 
    {
        //GetComponent<Rigidbody>().velocity = new Vector3(0, starSpeed, 0);   
        //parent = GameObject.Find("InactiveStars").transform;

        starSpeed = Random.Range(GameData.minStarSpeed, GameData.maxStarSpeed);

        StartMovement();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (transform.position.y < destroyStarWhenBelowThisYValue)
        {
            StopMovement();
            transform.position = new Vector3(0, 0, 0);
            gameObject.SetActive(false);
        }
	}

	public void DeactivateCollider()
	{
		GetComponent<Collider>().enabled = false;
	}

    public void StopMovement()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void StartMovement()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().velocity = new Vector3(0, -1, 0) * starSpeed;
    }
}
