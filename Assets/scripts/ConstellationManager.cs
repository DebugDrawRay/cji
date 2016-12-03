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
		Links = new List<GameData.Link>();
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
	}
	
	void Update ()
	{
		var destroyedLinks = new List<GameData.Link>();
		for (int i = 0; i < Links.Count; i++)
		{
			var link = Links[i];
			RaycastHit hit;

			if (Physics.Linecast(link.StartPos, link.EndPos, out hit))
			{
				Debug.Log(hit.transform.gameObject.name);
				BreakLink(link);
				destroyedLinks.Add(link);
			}
		}

		for (int i = 0; i < destroyedLinks.Count; i++)
		{
			Links.Remove(destroyedLinks[i]);
		}
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
				link.StartPos = LastStar.Position;
				link.EndPos = star.Position;
				line.SetPosition(0, link.StartPos);
				line.SetPosition(1, link.EndPos);
				Links.Add(link);
			}
		}
		LastStar = star;
	}

	public void CompleteConstellation()
	{

	}

	public void BreakConstellation()
	{

	}

	protected void BreakLink(GameData.Link link)
	{
		//Unlink Stars
		Stars[link.StarIds[0]].LinkedStars.Remove(link.StarIds[1]);
		Stars[link.StarIds[1]].LinkedStars.Remove(link.StarIds[0]);

		//Delete Link
		Destroy(link.LineComponent.gameObject);

		CheckStrandedStar(Stars[link.StarIds[0]]);
		CheckStrandedStar(Stars[link.StarIds[1]]);
	}

	protected void CheckStrandedStar(GameData.Star star)
	{
		if (star.LinkedStars.Count <= 0)
		{
			Debug.Log("Releasing Star");
			star.Controller.StartMovement();
			Stars.Remove(star.StarId);
		}
	}

	protected void LinkStars()
	{

	}
}
