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
        End,
        Transition
    }
    public State currentState;

    public int currentScore;

    [Header("Player")]
    public GameObject player;
    private GameObject currentPlayer;
    public Transform playerSpawn;

    [Header("Comet Properties")]
    public GameObject comet;
    public Transform cometSpawn;
    private Rigidbody cometRigid;
	protected Rotate cometRotate;
	public bool frozen = false;

    private float currentDistance;
    private float currentTarget;

    //Current tween
    private Tween currentTween;
    private bool hit;

    //Timing
    private float timeToSpeedIncrease;
    private int currentAccelerationLevel;
    private bool inDanger;

	protected float levelTimer;

    [Header("Star Properties")]
    public StarManager starMan;

    [Header("Debug")]
    public bool spawnComet;


    //Events
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

    public delegate void Trigger();
    public static event Trigger EndGameTrigger;

    public static void TriggerEndGame()
    {
        if(EndGameTrigger != null)
        {
            EndGameTrigger();
        }
    }

    void Awake()
    {
        AssignEvents();
        starMan = GetComponent<StarManager>();
    }

    void AssignEvents()
    {
        CometCollisionEvent += AddDistanceToComet;
        AddScoreEvent += AddScore;
        EndGameTrigger += EndGame;
    }

    void OnDestroy()
    {
        CometCollisionEvent -= AddDistanceToComet;
        AddScoreEvent -= AddScore;
        EndGameTrigger -= EndGame;
    }
    void Update()
    {
        RunStates();
    }

    void SpawnObjects()
    {
        currentPlayer = (GameObject)Instantiate(player, playerSpawn.position, playerSpawn.rotation);
        if (spawnComet)
        {
            GameObject newComet = (GameObject)Instantiate(comet, cometSpawn.position, cometSpawn.rotation);
            cometRigid = newComet.GetComponent<Rigidbody>();
            cometRotate = newComet.GetComponentInChildren<Rotate>();
        }
    }

    void SetupLevel()
    {
        currentDistance = GameData.cometStartY;
        timeToSpeedIncrease = GameData.accelerationIncreaseRate;
        currentAccelerationLevel = 0;
        levelTimer = GameData.levelTime;
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
                UpdateUi();
                InvokeRepeating("UpdateMeters", 0, .15f);
                AudioController.Instance.StartMusic();
                StartCoroutine(StartSequence());
                currentState = State.Transition;
                break;
            case State.InGame:
                UpdateComet();
                UpdateLevel();
                UpdateUi();
                break;
            case State.Pause:
                break;
            case State.End:
                UiController.TriggerKillScreen("Nothing");
                starMan.enabled = false;
                break;
            case State.Transition:
                break;
        }
    }

    void UpdateComet()
    {
		if (!hit)
		{
			currentDistance = Mathf.MoveTowards(currentDistance, GameData.cometDest, GameData.cometAcelerationLevels[currentAccelerationLevel]);
		}
		cometRigid.transform.position = new Vector2(0, currentDistance);

        if (currentDistance <= GameData.dangerLimit)
        {
            if (!inDanger)
            {
                AudioController.Instance.FadeToDanger(true);
                inDanger = true;
            }
        }
        else
        {
            if (inDanger)
            {
                AudioController.Instance.FadeToDanger(false);
                inDanger = false;
            }
        }
    }

    float lastY;

	void UpdateLevel()
	{
		if (currentAccelerationLevel < GameData.cometAcelerationLevels.Length - 1)
		{
			if (levelTimer <= 0)
			{
				levelTimer = GameData.levelTime;
				currentAccelerationLevel++;
				starMan.spawnLevel++;
				Debug.Log("NEW LEVEL: " + currentAccelerationLevel);
			}
			else
			{
				levelTimer -= Time.deltaTime;
			}
		}
	}

    void UpdateUi()
    {
        UiController.TriggerScoreEvent(currentScore);
        UiController.TriggerDistanceEvent(currentDistance);
    }

    void UpdateMeters()
    {
        if (Time.deltaTime != 0)
        {
            float speed = (lastY - cometRigid.transform.position.y) / Time.deltaTime;
            lastY = cometRigid.transform.position.y;

            UiController.TriggerVelocityEvent(speed);
        }
    }

    void AddDistanceToComet(float strength, float speed)
    {
        hit = true;
        if(currentTween != null)
        {
            currentTween.Kill();
        }
        float newDistance = currentDistance + strength;
        currentTween = DOTween.To(() => currentDistance, x => currentDistance = x, currentDistance + strength, speed).SetEase(Ease.OutExpo);
        currentTween.OnComplete(() => hit = false);
    }

    void AddScore(int score)
    {
        currentScore += score;
    }

    void EndGame()
    {
		currentState = State.Transition;
		StartCoroutine(EndSequence());
        //currentState = State.End;
    }

    IEnumerator StartSequence()
    {
        cometRigid.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel);
        yield return new WaitUntil(() => cometRigid.transform.position.y >= GameData.cometStartY);
        currentPlayer.transform.DOMoveY(GameData.playerStartY, GameData.cometStartAccel);
        yield return new WaitUntil(() => currentPlayer.transform.position.y >= GameData.playerStartY);
        currentPlayer.GetComponent<PlayerController>().canMove = true;
        starMan.enabled = true;
        currentState = State.InGame;
    }

	IEnumerator EndSequence()
	{
		cometRigid.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel * 1.2f);
		currentPlayer.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel * 1.2f).SetEase(Ease.InOutBack);
		yield return new WaitUntil(() => cometRigid.transform.position.y >= GameData.cometStartY);
		currentPlayer.GetComponent<PlayerController>().canMove = false;
		starMan.enabled = false;
		currentState = State.End;
	}
}
