using UnityEngine;
using System;
using System.Collections.Generic;

public class ConstellationManager : MonoBehaviour
{
	public static ConstellationManager Instance;
	public GameObject LinkPrefab;
	public Transform StarLinkParent;

	protected List<GameData.Constellation> Constellations;
	protected Dictionary<Guid, GameData.Star> Stars;
	protected GameData.Star LastStar;
	protected List<GameData.Link> Links;

	void Awake()
	{
		Instance = this;
		Stars = new Dictionary<Guid, GameData.Star>();
		LastStar = null;
	}

	void Start ()
	{
		Constellations = new List<GameData.Constellation>();

		var testStar1 = new GameData.Star(Vector3.zero);
		var testStar2 = new GameData.Star(new Vector3(0, 0, 1));
		var testStar3 = new GameData.Star(new Vector3(0, 1, 0));
		var testStar4 = new GameData.Star(new Vector3(1, 0, 0));

		AddStar(testStar1);
		AddStar(testStar2);
		AddStar(testStar3);
		AddStar(testStar4);
		AddStar(testStar1);
	}
	
	void Update ()
	{
	
	}

	public void AddStar(GameData.Star star)
	{
		//If star not already stored
		if (!Stars.ContainsKey(star.StarId))
		{
			Stars.Add(star.StarId, star);
		}

		//There is a last star
		if (LastStar != null)
		{
			//Is not currently linked to star
			if (!LastStar.LinkedStars.Contains(star.StarId))
			{
				//Link Stars to Eachother
				LastStar.LinkedStars.Add(star.StarId);
				star.LinkedStars.Add(star.StarId);

				//Create Link
				var link = new GameData.Link();
				link.StarIds.Add(LastStar.StarId);
				link.StarIds.Add(star.StarId);

				//Create Link Object
				var linkObject = Instantiate(LinkPrefab);
				var line = linkObject.GetComponent<LineRenderer>();

				

				if (StarLinkParent != null)
				{
					linkObject.transform.SetParent(StarLinkParent);
					linkObject.transform.localScale = Vector3.one;
				}

				link.LineComponent = line;
				line.SetPosition(0, LastStar.Position);
				line.SetPosition(1, star.Position);
			}
		}
		LastStar = star;
	}

	//private void AddColliderToLine(LineRenderer Line)
	//{
	//	var col = new GameObject("Collider").AddComponent<BoxCollider>();
	//	col.transform.parent = Line.; 
	//	float lineLength = Vector3.Distance(, endPos); // length of line
	//	col.size = new Vector3(lineLength, 0.1f, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
	//	Vector3 midPoint = (startPos + endPos) / 2;
	//	col.transform.position = midPoint; // setting position of collider object
	//												  // Following lines calculate the angle between startPos and endPos
	//	float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
	//	if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
	//	{
	//		angle *= -1;
	//	}
	//	angle = Mathf.Rad2Deg * Mathf.Atan(angle);
	//	col.transform.Rotate(0, 0, angle);
	//}

	public void CompleteConstellation()
	{

	}

	public void BreakConstellation()
	{

	}

	public void BreakLine()
	{

	}

	protected void LinkStars()
	{

	}
}
