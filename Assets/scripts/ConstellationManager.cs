using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ConstellationManager : MonoBehaviour
{
	public static ConstellationManager Instance;
	public GameObject LinkPrefab;
	public Transform StarLinkParent;
	protected float Speed = 2;

	public GameObject[] TestStars;

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

		for (int i = 0; i < TestStars.Length; i++)
		{
			var controller = TestStars[i].GetComponent<StarController>();
			var testStar = new GameData.Star(TestStars[i].transform.position, controller);
			AddStar(testStar);
		}
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
				if (hit.transform.gameObject.name != "TestStar")
				{
					//Debug.Log(hit.transform.gameObject.name);
					//BreakLink(link);
					//destroyedLinks.Add(link);
				}
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
		if (Stars.Count >= GameData.minimumStars)
		{
			LastStar = null;

			var constellation = new GameData.Constellation();
			constellation.Stars = new Dictionary<Guid, GameData.Star>(Stars);
			constellation.Links = new List<GameData.Link>(Links);

			Stars.Clear();
			Links.Clear();

			StartCoroutine(ConstellationFlyAway(constellation));
		}
	}

	public void BreakConstellation()
	{

	}

	protected void BreakLink(GameData.Link link)
	{
		Debug.Log("Breaking Link");

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

	protected IEnumerator ConstellationFlyAway(GameData.Constellation constellation)
	{
		Debug.Log("Constellation Fly Away");

		var yDisplace = 0f;
		while (yDisplace < 5f)
		{
			//Update Stars
			var stars = new List<GameData.Star>(constellation.Stars.Values);
			for (int i = 0; i < stars.Count; i++)
			{
				Debug.Log("Star!");
				var star = stars[i];
				if (star.Controller != null)
				{
					var starGo = star.Controller.gameObject;
					var pos = starGo.transform.position;
					pos.y += Time.deltaTime * Speed;
					starGo.transform.position = pos;
				}
				else
				{
					Debug.Log("Star controller is null!");
				}
			}

			//Update Linnks
			for (int i = 0; i < constellation.Links.Count; i++)
			{
				var link = constellation.Links[i];
				link.StartPos.y += Time.deltaTime * Speed;
				link.EndPos.y += Time.deltaTime * Speed;
				link.LineComponent.SetPosition(0, link.StartPos);
				link.LineComponent.SetPosition(1, link.EndPos);
			}

			yDisplace += Time.deltaTime * Speed;
			yield return null;
		}
		DestroyConstellation(constellation);
	}

	protected void DestroyConstellation(GameData.Constellation constellation)
	{
		Debug.Log("DESTROY");
		//Destroy Stars
		var stars = new List<GameData.Star>(constellation.Stars.Values);
		for (int i = 0; i < stars.Count; i++)
		{
			Debug.Log("Star!");
			var star = stars[i];
			if (star.Controller != null)
			{
				Destroy(star.Controller.gameObject);
			}
			else
			{
				Debug.Log("Star controller is null!");
			}
		}

		//Destroy Linnks
		for (int i = 0; i < constellation.Links.Count; i++)
		{
			var link = constellation.Links[i];
			Destroy(link.LineComponent.gameObject);
		}

	}
}
