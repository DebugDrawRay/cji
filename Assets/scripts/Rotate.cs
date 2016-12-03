using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public float xRotateSpeed;
    public float yRotateSpeed;
    public float zRotateSpeed;

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {   
        //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x + (xRotateSpeed * Time.deltaTime), transform.rotation.y + (yRotateSpeed * Time.deltaTime), transform.rotation.z + (zRotateSpeed * Time.deltaTime)));
        transform.Rotate(new Vector3(1, 1, 1) * (Time.deltaTime * xRotateSpeed));
    }
}
