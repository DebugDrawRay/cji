using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class StarController : MonoBehaviour
{
    //public enum starType;

    public GameData.StarType theStarType;

    private float starSpeed;

    public float destroyStarWhenBelowThisYValue;

	public UnityEvent starTriggered;

    [HideInInspector]
    public GameData.Star starData;

    public float shrinkSpeed;
    public bool doShrink;

    public GameObject starBoing;
    public GameObject starBoing02;
    public float delayBeforeSecondBoingTimerBase;
    public float delayBeforeSecondBoingTimer;

    //public float starSize;

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

        //transform.localScale = new Vector3(starSize, starSize, starSize);

        StartMovement();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (transform.position.y < destroyStarWhenBelowThisYValue)
        {
            StopMovement();
            transform.position = new Vector3(0, 0, 0);
            GetComponent<PooledObject>().ReturnToPool();
        }

        if (doShrink == true)
        {
            if (transform.localScale.x > 0.1f)
            {
                transform.localScale = new Vector3(transform.localScale.x - (shrinkSpeed * Time.deltaTime), transform.localScale.y - (shrinkSpeed * Time.deltaTime), transform.localScale.z - (shrinkSpeed * Time.deltaTime));
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (delayBeforeSecondBoingTimer > 0 && delayBeforeSecondBoingTimer < 100)
        {
            delayBeforeSecondBoingTimer -= Time.deltaTime;
        }
        else if(delayBeforeSecondBoingTimer <= 0)
        {
            starBoing02.gameObject.SetActive(true);
            delayBeforeSecondBoingTimer = 120;
        }
	}

	public void UpdateLayerToSendStar()
	{
		gameObject.layer = LayerMask.NameToLayer("SentStars");
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

	void OnTriggerEnter(Collider coll)
	{
		starTriggered.Invoke();
	}

    //this function shrinkles the failed constellation stars
    public void Shrinkle()
    {
        
        doShrink = true;

    }

    //this function twinkles the failed constellation stars
    public void Twinkle()
    {
        
    }
}
