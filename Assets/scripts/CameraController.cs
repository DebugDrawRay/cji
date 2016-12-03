using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject theCamera;

    //this is the amount of time the camera is shaking. A shake includes multiple jitters
    //public float shakingTimerBase;
    public float shakingTimer;

    //a jitter is a single movement. Multiple jitters make up one screenshake
    public float jitterTimerMin;
    public float jitterTimerMax;
    public float jitterTimer;

    public float jitterMoveMin;
    public float jitterMoveMax;

    public Vector3 homePos;
    public bool awayFromHomePos;


	// Use this for initialization
	void Start () 
    {
        homePos = transform.position;   
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (shakingTimer > 0)
        {
            DoJitter();
            shakingTimer -= Time.deltaTime;
        }
        else
        {
            //stop shaking
        }
	}

    public void DoScreenShake(float shakeTime)
    {
        shakingTimer = shakeTime;
    }

    void DoJitter()
    {
        if (jitterTimer > 0)
        {
            jitterTimer -= Time.deltaTime;
        }
        else
        {
            if (awayFromHomePos == false)//is at home, so move it away
            {
                transform.position = new Vector3(Random.Range(jitterMoveMin, jitterMoveMax), Random.Range(jitterMoveMin, jitterMoveMax), -10);
                jitterTimer = Random.Range(jitterTimerMin, jitterTimerMax);
                awayFromHomePos = true;
            }
            else//is not at home, so move it home
            {
                transform.position = homePos;
                jitterTimer = Random.Range(jitterTimerMin, jitterTimerMax);
                awayFromHomePos = false;
            }
        }
    }
}
