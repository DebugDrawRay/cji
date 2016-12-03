using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour {

    public GameObject MyParent;



	// Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    void OnDisable()
    {
        //transform.SetParent(MyParent.transform);
    }
}
