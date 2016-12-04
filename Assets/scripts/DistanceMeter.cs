using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class DistanceMeter : MonoBehaviour
{
    public GameObject constellationIcon;
    public RectTransform constellationStart;

    public RectTransform comet;
    public float cometStart;

    public void ChangeDistance(float distance)
    {
        float currentDistance = (distance + Mathf.Abs(GameData.cometDest)) / (GameData.cometStartY + Mathf.Abs(GameData.cometDest));
        currentDistance = Mathf.Clamp(currentDistance, 0, 1) * cometStart;
        
        comet.anchoredPosition = new Vector2(0, currentDistance);
    }

    public void DisplayConstellation(GameObject cons)
    {
        GameObject newCon = (GameObject)Instantiate(constellationIcon, new Vector2(0, 5000), Quaternion.identity, transform);
        newCon.GetComponent<RectTransform>().anchoredPosition = constellationStart.anchoredPosition;
        newCon.GetComponent<ConstellationIcon>().parent = cons.transform;
        newCon.GetComponent<ConstellationIcon>().destinationY = cometStart;
    }
}
