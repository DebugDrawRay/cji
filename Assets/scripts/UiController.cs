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

    public delegate void ScoreData(int starCount, int linkCount, int score, string name);
    public static event ScoreData ScoreDataEvent;

    public static void TriggerScoreData(int starCount, int linkCount, int score, string name)
    {
        if(ScoreDataEvent != null)
        {
            ScoreDataEvent(starCount, linkCount, score, name);
        }
    }

    public Text scoreDisplay;
    public Text distanceDisplay;
    public DistanceMeter distaceMeter;
    public Text velocityDisplay;

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
        DistanceEvent += distaceMeter.ChangeDistance;
        ScoreDataEvent += AddToScoreFeed;
        KillScreenEvent += DisplayKillScreen;
        VelocityEvent += DisplayVelocity;

    }

    void OnDestroy()
    {
        ScoreEvent -= DisplayScore;
        DistanceEvent -= DisplayDistance;
        DistanceEvent -= distaceMeter.ChangeDistance;
        ScoreDataEvent -= AddToScoreFeed;
        KillScreenEvent -= DisplayKillScreen;
        VelocityEvent -= DisplayVelocity;
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
}
