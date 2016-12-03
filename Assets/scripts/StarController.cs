﻿using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {

    //public enum starType;

    public GameData.StarType theStarType;

    public float starSpeedMin;
    public float starSpeedMax;
    public float starSpeed;

    public float destroyStarWhenBelowThisYValue;

	// Use this for initialization
	void Start () 
    {
        //GetComponent<Rigidbody>().velocity = new Vector3(0, starSpeed, 0);   
        //parent = GameObject.Find("InactiveStars").transform;

        starSpeed = Random.Range(starSpeedMin, starSpeedMax);

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

    public void StopMovement()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void StartMovement()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, -1, 0) * starSpeed;
    }
}
