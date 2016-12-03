using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {

    //public enum starType;

    public GameData.StarType theStarType;

    public float starSpeed;

	// Use this for initialization
	void Start () 
    {
        //GetComponent<Rigidbody>().velocity = new Vector3(0, starSpeed, 0);   

        StartMovement();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    public void StopMovement()
    {
        GetComponent<Rigidbody>().velocity *= 0;
    }

    public void StartMovement()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, -1, 0);
        GetComponent<Rigidbody>().velocity *= starSpeed;
    }
}
