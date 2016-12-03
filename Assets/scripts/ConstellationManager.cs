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

	protected float InvincibilityCountdown = 0f;
	protected float InvincibiltyCountdownMax = 1f;

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
	}
	
	//void Update ()
	//{
	//	var destroyedLinks = new List<GameData.Link>();
	//	for (int i = 0; i < Links.Count - 1; i++)
	//	{
	//		var link = Links[i];
	//		CheckLink(link);
	//	}
		
	//	//Invincibility for last line
	//	if (InvincibilityCountdown >= 0)
	//	{
	//		InvincibilityCountdown -= Time.deltaTime;
	//	}
	//	else if (Links.Count > 0)
	//	{
	//		CheckLink(Links[Links.Count - 1]);
	//	}

	//	for (int i = 0; i < destroyedLinks.Count; i++)
	//	{
	//		Links.Remove(destroyedLinks[i]);
	//	}
	//}

	protected void CheckLink(GameData.Link link)
	{
		RaycastHit hit;

		if (Physics.Linecast(link.StartPos, link.EndPos, out hit))
		{
			if (hit.transform.gameObject.name.Contains("Player"))
			{
				BreakConstellation();
			}
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
				InvincibilityCountdown = InvincibiltyCountdownMax;
			}
		}

		LastStar = star;
	}

	public void CompleteConstellation()
	{
		var constellationParent = new GameObject("Constellation");

		Debug.Log("Complete Constellation");
		if (Stars.Count >= GameData.minimumStars)
		{
			LastStar = null;
			var constellation = new GameData.Constellation();
			constellation.Stars = new Dictionary<Guid, GameData.Star>(Stars);
			constellation.Links = new List<GameData.Link>(Links);

			var keys = new List<Guid>(constellation.Stars.Keys);
			for (int i = 0; i < constellation.Stars.Count; i++)
			{
				var star = constellation.Stars[keys[i]];
				star.Controller.UpdateLayerToSendStar();
				star.Controller.starTriggered.AddListener(() => StarToCometCollision(constellation, star));
				star.Controller.gameObject.transform.SetParent(constellationParent.transform);
			}

			for (int i = 0; i < constellation.Links.Count; i++)
			{
				var link = constellation.Links[i];
				link.LineComponent.gameObject.transform.SetParent(constellationParent.transform);
			}

			Stars.Clear();
			Links.Clear();

			StartCoroutine(ConstellationFlyAway(constellation));
		}
	}

	protected void BreakStarLink(GameData.Constellation constellation, Guid starId)
	{
		var star = constellation.Stars[starId];

		//Remove star from linked stars
		for (int i = 0; i < star.LinkedStars.Count; i++)
		{
			constellation.Stars[star.LinkedStars[i]].LinkedStars.Remove(star.StarId);
		}

		//Get all involved links
		var removedLinks = new List<GameData.Link>();
		for (int i = 0; i < constellation.Links.Count; i++)
		{
			if (constellation.Links[i].StarIds.Contains(star.StarId))
				removedLinks.Add(constellation.Links[i]);
		}

		//Destroy Link
		for (int i = 0; i < removedLinks.Count; i++)
		{
			var link = removedLinks[i];
			Destroy(link.LineComponent.gameObject);
			constellation.Links.Remove(link);
		}
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

	public void BreakConstellation()
	{
		Debug.Log("Resetting Constellation");

		LastStar = null;

		//Release Stars
		var keys = new List<Guid>(Stars.Keys);
		for (int i = 0; i < Stars.Count; i++)
		{
			Stars[keys[i]].Controller.StartMovement();
			Stars[keys[i]].Controller.DeactivateCollider();
		}

		//Destroy Links
		for (int i = 0; i < Links.Count; i++)
		{
			var link = Links[i];
			Destroy(link.LineComponent.gameObject);
		}

		Stars = new Dictionary<Guid, GameData.Star>();
		Links = new List<GameData.Link>();
	}

	protected void CheckStrandedStar(GameData.Star star)
	{
		if (star.LinkedStars.Count <= 0)
		{
			star.Controller.StartMovement();
			Stars.Remove(star.StarId);
		}
	}

	protected IEnumerator ConstellationFlyAway(GameData.Constellation constellation)
	{
		Debug.Log("Constellation Fly Away");

		var yDisplace = 0f;
		while (yDisplace < 100f)
		{
			//Update Stars
			var stars = new List<GameData.Star>(constellation.Stars.Values);
			for (int i = 0; i < stars.Count; i++)
			{
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
		//Destroy Stars
		var stars = new List<GameData.Star>(constellation.Stars.Values);
		for (int i = 0; i < stars.Count; i++)
		{
			var star = stars[i];
			if (star.Controller != null)
			{
				Destroy(star.Controller.gameObject);
			}
		}

		//Destroy Links
		for (int i = 0; i < constellation.Links.Count; i++)
		{
			var link = constellation.Links[i];
			Destroy(link.LineComponent.gameObject);
		}
	}
	
	protected void StarToCometCollision(GameData.Constellation constellation, GameData.Star star)
	{
		star.Controller.gameObject.SetActive(false);
		BreakStarLink(constellation, star.StarId);

        float strength = GameData.strengthMultiplier * (star.LinkedStars.Count + 1);

        GameController.TriggerCometCollision(strength, GameData.cometCollisionSpeed);
	}
}
