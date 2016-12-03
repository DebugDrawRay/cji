using UnityEngine;
using System.Collections;
using DG.Tweening;
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

    public int currentScore;

    [Header("Player")]
    public GameObject player;
    public Transform playerSpawn;

    [Header("Comet Properties")]
    public GameObject comet;
    public Transform cometSpawn;
    private Rigidbody cometRigid;

    private float currentDistance;
    private float currentTarget;

    //Current tween
    private Tween currentTween;
    private bool hit;

    [Header("Debug")]
    public bool spawnComet;

    public delegate void CollisionEvent(float strength, float speed);
    public static event CollisionEvent CometCollisionEvent;

    public static void TriggerCometCollision(float strength, float speed)
    {
        if(CometCollisionEvent != null)
        {
            CometCollisionEvent(strength, speed);
        }
    }

    public delegate void ScoreEvent(int score);
    public static event ScoreEvent AddScoreEvent;

    public static void TriggerAddScore(int score)
    {
        if(AddScoreEvent != null)
        {
            AddScoreEvent(score);
        }

    }

    void Awake()
    {
        CometCollisionEvent += AddDistanceToComet;
        AddScoreEvent += AddScore;
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
        currentDistance = GameData.cometStartY;
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
                UpdateComet();
                UiController.TriggerScoreEvent(currentScore);
                UiController.TriggerDistanceEvent(currentDistance);
                break;
            case State.Pause:
                break;
            case State.End:
                break;
        }
    }

    void UpdateComet()
    {
        if(!hit)
        {
            currentDistance = Mathf.MoveTowards(currentDistance, GameData.cometDest, GameData.cometAcceleration);
        }
        cometRigid.transform.position = new Vector2(0, currentDistance);
    }

    [ContextMenu("Do it sweet child")]
    void AddDistanceToComet(float strength, float speed)
    {
        hit = true;
        if(currentTween != null)
        {
            currentTween.Kill();
        }
        currentTween = DOTween.To(() => currentDistance, x => currentDistance = x, currentDistance + strength, speed);
        currentTween.OnComplete(() => hit = false);
    }

    void AddScore(int score)
    {
        currentScore += score;
    }
}
