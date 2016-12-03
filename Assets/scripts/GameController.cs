using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public enum State
    {
        Setup,
        Start,
        InGame,
        Pause,
        End
    }
    public State currentState;

    public int score;

    [Header("Player")]
    public GameObject player;
    public Transform playerSpawn;

    [Header("Comet Properties")]
    public GameObject comet;
    public Transform cometSpawn;
    private Rigidbody cometRigid;

    private float currentDistance;

    [Header("Debug")]
    public bool spawnComet;

    void Awake()
    {

    }

    void Update()
    {
        RunStates();
    }

    void SpawnObjects()
    {
        Instantiate(player, playerSpawn.position, playerSpawn.rotation);
        if (spawnComet)
        {
            GameObject newComet = (GameObject)Instantiate(comet, cometSpawn.position, cometSpawn.rotation);
            cometRigid = newComet.GetComponent<Rigidbody>();
        }
    }

    void SetupLevel()
    {
        currentDistance = GameData.levelLength;
    }

    void RunStates()
    {
        switch(currentState)
        {
            case State.Setup:
                SpawnObjects();
                SetupLevel();
                currentState = State.Start;
                break;
            case State.Start:
                currentState = State.InGame;
                break;
            case State.InGame:
                UpdateDistance();
                UpdateComet();
                break;
            case State.Pause:
                break;
            case State.End:
                break;
        }
    }

    void UpdateDistance()
    {
        currentDistance = Mathf.Lerp(currentDistance, 0, GameData.levelSpeed);
    }
    void UpdateComet()
    {
        if(cometRigid && currentDistance <= 5)
        {
            cometRigid.transform.position = new Vector2(0, currentDistance);
        }
    }
}
