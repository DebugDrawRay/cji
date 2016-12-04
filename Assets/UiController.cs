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
    public Text scoreFeed;
    public DistanceMeter distaceMeter;

    private Tween currentTween;

    void Awake()
    {
        ScoreEvent += DisplayScore;
        DistanceEvent += DisplayDistance;
        DistanceEvent += distaceMeter.ChangeDistance;
        ScoreDataEvent += AddToScoreFeed;
    }

    void DisplayScore(int score)
    {
        scoreDisplay.text = score.ToString();
    }


    void AddToScoreFeed(int starCount, int linkCount, int score, string name)
    {
        if(currentTween != null)
        {
            currentTween.Kill();
        }
        scoreFeed.color = Color.white;
        scoreFeed.text += name + ": " + starCount.ToString() + " Stars, " + linkCount.ToString() + " Links: " + score.ToString() + " Points \n";
        Color clear = Color.white;
        clear.a = 0;
        currentTween = scoreFeed.DOColor(clear, 2f);
    }

    void DisplayDistance(float distance)
    {
        float total = distance + Mathf.Abs(GameData.cometDest);
        distanceDisplay.text = (total * GameData.distanceScalar).ToString();
    }
}
