using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    public Text scoreDisplay;
    public Text distanceDisplay;
    public DistanceMeter distaceMeter;


    void Awake()
    {
        ScoreEvent += DisplayScore;
        DistanceEvent += DisplayDistance;
        DistanceEvent += distaceMeter.ChangeDistance;
    }

    void DisplayScore(int score)
    {
        scoreDisplay.text = score.ToString();
    }

    void DisplayDistance(float distance)
    {
        float total = distance + Mathf.Abs(GameData.cometDest);
        distanceDisplay.text = (total * GameData.distanceScalar).ToString();
    }
}
