using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UiController : MonoBehaviour
{
    public delegate void Message<T>(T data);
    public static Message<int> ScoreEvent;

    public static void TriggerScoreEvent(int score)
    {
        if(ScoreEvent != null)
        {
            ScoreEvent(score);
        }
    }

    public static Message<float> DistanceEvent;

    public static void TriggerDistanceEvent(float distance)
    {
        if (DistanceEvent != null)
        {
            DistanceEvent(distance);
        }
    }

    public static Message<string> KillScreenEvent;
    public static void TriggerKillScreen(string message)
    {
        if(KillScreenEvent != null)
        {
            KillScreenEvent(message);
        }
    }

    public static Message<float> VelocityEvent;

    public static void TriggerVelocityEvent(float velocity)
    {
        if(VelocityEvent != null)
        {
            VelocityEvent(velocity);
        }
    }

    public static Message<GameObject> ConstellationEvent;
    public static void TriggerConstellationEvent(GameObject cons)
    {
        if(ConstellationEvent != null)
        {
            ConstellationEvent(cons);
        }
    }

    public delegate void ScoreData(int starCount, int linkCount, int score, string name);
    public static event ScoreData ScoreDataEvent;

    public static void TriggerScoreData(int starCount, int linkCount, int score, string name)
    {
        if(ScoreDataEvent != null)
        {
            ScoreDataEvent(starCount, linkCount, score, name);
        }
    }

	public static Message<bool> ConstellationFadeEvent;
	public static void TriggerConstellationFadeEvent(bool stuff)
	{
		if (ConstellationFadeEvent != null)
		{
			ConstellationFadeEvent(stuff);
		}
	}

	public Text scoreDisplay;
    public Text distanceDisplay;
    public DistanceMeter distanceMeter;
    public Text velocityDisplay;

	 public RawImage ConstellationDisplay;
	 protected IEnumerator ConstellationFadeIn;
	 protected IEnumerator ConstellationFadeOut;

    public GameObject killScreen;
    private Tween currentTween;

    void Awake()
    {
        AssignEvents();
    }

    void AssignEvents()
    {
        ScoreEvent += DisplayScore;
        DistanceEvent += DisplayDistance;
        DistanceEvent += distanceMeter.ChangeDistance;
        ScoreDataEvent += AddToScoreFeed;
        KillScreenEvent += DisplayKillScreen;
        VelocityEvent += DisplayVelocity;
        ConstellationEvent += distanceMeter.DisplayConstellation;
        ConstellationFadeEvent += FadeInConstellation;
	 }

    void OnDestroy()
    {
        ScoreEvent -= DisplayScore;
        DistanceEvent -= DisplayDistance;
        DistanceEvent -= distanceMeter.ChangeDistance;
        ScoreDataEvent -= AddToScoreFeed;
        KillScreenEvent -= DisplayKillScreen;
        VelocityEvent -= DisplayVelocity;
        ConstellationEvent -= distanceMeter.DisplayConstellation;
        ConstellationFadeEvent -= FadeInConstellation;

    }

    void DisplayScore(int score)
    {
        scoreDisplay.text = score.ToString();
    }

    void DisplayVelocity(float velocity)
    {
        velocityDisplay.text = (velocity * 2000).ToString();
    }

    void AddToScoreFeed(int starCount, int linkCount, int score, string name)
    {
        if(currentTween != null)
        {
            currentTween.Kill();
        }
        Color clear = Color.white;
        clear.a = 0;
    }

    void DisplayDistance(float distance)
    {
        float total = distance + Mathf.Abs(GameData.cometDest);
        distanceDisplay.text = Mathf.RoundToInt(total * GameData.distanceScalar).ToString();
    }

    void DisplayKillScreen(string message)
    {
        killScreen.SetActive(true);
    }

	void FadeInConstellation(bool stuff)
	{
		if (ConstellationFadeIn != null)
			StopCoroutine(ConstellationFadeIn);

		ConstellationFadeIn = FadeInConstellationWork();
		StartCoroutine(ConstellationFadeIn);
	}

	IEnumerator FadeInConstellationWork()
	{
		ConstellationFadeOut = FadeOutConstellationWork();
		StopCoroutine(ConstellationFadeOut);
		Debug.Log("Starting fade in");

		float t = 0;
		Color color = ConstellationDisplay.color;

		while (color.a < 1)
		{
			color.a = Mathf.Lerp(0, 1, t);
			ConstellationDisplay.color = color;
			t += (Time.deltaTime * 2);
			yield return null;
		}

		yield return new WaitForSeconds(2);

		StartCoroutine(ConstellationFadeOut);
	}

	IEnumerator FadeOutConstellationWork()
	{
		float t = 0;
		Color color = Color.white;

		while (color.a > 0)
		{
			color.a = Mathf.Lerp(1, 0, t);
			ConstellationDisplay.color = color;
			t += Time.deltaTime;
			yield return null;
		}

		yield return null;
	}
}
