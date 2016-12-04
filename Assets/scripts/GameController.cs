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
                AudioController.Instance.StartMusic();
                UpdateUi();
                StartCoroutine(StartSequence());
                currentState = State.Transition;
                break;
            case State.InGame:
                UpdateComet();
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
		if (!frozen)
		{
			if (!hit)
			{
				currentDistance = Mathf.MoveTowards(currentDistance, GameData.cometDest, GameData.cometAcelerationLevels[currentAccelerationLevel]);
			}
			cometRigid.transform.position = new Vector2(0, currentDistance);

			if (timeToSpeedIncrease > 0)
			{
				timeToSpeedIncrease -= Time.deltaTime;
			}
			else
			{
				currentAccelerationLevel++;
				if (currentAccelerationLevel >= GameData.cometAcelerationLevels.Length)
				{
					currentAccelerationLevel = GameData.cometAcelerationLevels.Length - 1;
				}
			}
		}
    }

    void UpdateUi()
    {
        UiController.TriggerScoreEvent(currentScore);
        UiController.TriggerDistanceEvent(currentDistance);
    }

    [ContextMenu("Do it sweet child")]
    void AddDistanceToComet(float strength, float speed)
    {
        hit = true;
        if(currentTween != null)
        {
            currentTween.Kill();
        }

			StopAllCoroutines();
			cometRotate.enabled = false;
			frozen = true;
			StartCoroutine(DelayedJump(strength, speed));

        currentTween = DOTween.To(() => currentDistance, x => currentDistance = x, currentDistance + strength, speed);
        currentTween.OnComplete(() => hit = false);
    }

	protected IEnumerator DelayedJump(float strength, float speed)
	{
		yield return new WaitForSeconds(0.05f);
		cometRotate.enabled = true;
		frozen = false;
	}

    void AddScore(int score)
    {
        currentScore += score;
    }

    void EndGame()
    {
        currentState = State.End;
    }

    IEnumerator StartSequence()
    {
        cometRigid.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel);
        yield return new WaitUntil(() => cometRigid.transform.position.y >= GameData.cometStartY);
        currentPlayer.transform.DOMoveY(GameData.playerStartY, GameData.playerStartAccel);
        yield return new WaitUntil(() => currentPlayer.transform.position.y >= GameData.playerStartY);
        currentPlayer.GetComponent<PlayerController>().canMove = true;
        starMan.enabled = true;
        currentState = State.InGame;
    }
}
