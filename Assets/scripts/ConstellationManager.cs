using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ConstellationManager : MonoBehaviour
{
	public static ConstellationManager Instance;

	[Header("Constellation Things")]
	public TextAsset ConstellationNounsText;
	public TextAsset ConstellationAdjectivesText;
	public string[] ConstellationNouns;
	public string[] ConstellationAdjectives;
	public Sprite[] Constellationbackgrounds;

	[Header("Scene Stuff")]
	public GameObject LinkPrefab;
	public Transform StarLinkParent;
	protected float Speed = 2;

	protected float InvincibilityCountdown = 0f;
	protected float InvincibiltyCountdownMax = 1f;

	protected List<GameData.Constellation> Constellations;
	protected Dictionary<Guid, GameData.Star> Stars;
	protected GameData.Star LastStar;
	protected List<GameData.Link> Links;

    GameObject player;

	void Awake()
	{
		Instance = this;
		Stars = new Dictionary<Guid, GameData.Star>();
		Links = new List<GameData.Link>();
		LastStar = null;

		ConstellationNameSetup();
	}

	void Start ()
	{
		Constellations = new List<GameData.Constellation>();
	}

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
				line.useWorldSpace = false;
				
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

	public bool CompleteConstellation()
	{
		Debug.Log("Complete Constellation");
		if (Stars.Count >= GameData.minimumStars)
		{
			LastStar = null;
			var constellation = new GameData.Constellation();
			constellation.Stars = new Dictionary<Guid, GameData.Star>(Stars);
			constellation.Links = new List<GameData.Link>(Links);

			GameObject constellationParent = new GameObject("Constellation");
			constellation.ConstellationParent = constellationParent;

			string constellationName = GenerateConstellationName(Stars.Keys.Count);
			constellation.ConstellationName = constellationName;
			Debug.Log("Constellation Name:" + constellationName);

			var keys = new List<Guid>(constellation.Stars.Keys);
			for (int i = 0; i < keys.Count; i++)
			{
				Guid key = keys[i];
				GameData.Star star = constellation.Stars[key];
				star.Controller.UpdateLayerToSendStar();
				star.Controller.starTriggered.AddListener(() => StarToCometCollision(constellation, key));
				star.Controller.gameObject.transform.SetParent(constellationParent.transform);
			}

			for (int i = 0; i < constellation.Links.Count; i++)
			{
				constellation.Links[i].LineComponent.gameObject.transform.SetParent(constellationParent.transform);
			}

			//Send data to Visualization and score
			int score = constellation.Stars.Count * constellation.Links.Count;
			UiController.TriggerScoreData(constellation.Stars.Count, constellation.Links.Count, score, constellationName);

			Stars.Clear();
			Links.Clear();

			StartCoroutine(ConstellationFlyAway(constellation));
			return true;
		}
		else
		{
			return false;
		}
	}

	protected void BreakStarLink(GameData.Constellation constellation, Guid starId)
	{
		var star = constellation.Stars[starId];

		//Remove star from linked stars
		for (int i = 0; i < star.LinkedStars.Count; i++)
		{
			if (constellation.Stars.ContainsKey(star.LinkedStars[i]))
			{
				GameData.Star linkedStar = constellation.Stars[star.LinkedStars[i]];
				linkedStar.LinkedStars.Remove(star.StarId);
			}
		}

		//Get all involved links
		List<GameData.Link> removedLinks = new List<GameData.Link>();
		for (int i = 0; i < constellation.Links.Count; i++)
		{
			if (constellation.Links[i].StarIds.Contains(star.StarId))
				removedLinks.Add(constellation.Links[i]);
		}

		//Destroy Link
		for (int i = 0; i < removedLinks.Count; i++)
		{
			GameData.Link link = removedLinks[i];
			constellation.Stars[link.StarIds[0]].LinkedStars.Remove(constellation.Stars[link.StarIds[1]].StarId);
			constellation.Stars[link.StarIds[1]].LinkedStars.Remove(constellation.Stars[link.StarIds[0]].StarId);
			Destroy(link.LineComponent.gameObject);
			constellation.Links.Remove(link);
		}

		//Remove star from constellation
		constellation.Stars.Remove(starId);
		PooledObject pool = star.Controller.gameObject.GetComponent<PooledObject>();
		pool.ReturnToPool();
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
         //Stars[keys[i]].Controller.Shrinkle();
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
		var yDisplace = 0f;
		while (yDisplace < 100f && constellation.ConstellationParent != null)
		{
			var pos = constellation.ConstellationParent.transform.position;
			pos.y += Time.deltaTime * Speed;
			constellation.ConstellationParent.transform.position = pos;

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

	protected void StarToCometCollision(GameData.Constellation constellation, Guid starId)
	{
		if (constellation.Stars.ContainsKey(starId))
		{
			Debug.Log("Calling Star Destroy for " + starId);
			GameData.Star star = constellation.Stars[starId];

			//Pushback
			float strength = GameData.strengthMultiplier * (star.LinkedStars.Count + 1);
			GameController.TriggerCometCollision(strength, GameData.cometCollisionSpeed);

			//Score
			int score = GameData.scorePerStar;
			float totalConnections = 1 + (GameData.scoreConnectionMulti * star.LinkedStars.Count);
			float totalStars = 1 + (GameData.constSizeMulti * constellation.Stars.Count);
			GameController.TriggerAddScore((int)((score * totalConnections) * totalStars));

			BreakStarLink(constellation, starId);
			if (constellation.Stars.Count <= 0)
			{
				Destroy(constellation.ConstellationParent);
			}
		}
	}


	#region Constellation Name Generation

	protected void ConstellationNameSetup()
	{
		ConstellationNouns = ConstellationNounsText.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		ConstellationAdjectives = ConstellationAdjectivesText.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
	}
	
	protected string GenerateConstellationName(int starCount)
	{
		int wordCount = Mathf.Max(1, Mathf.FloorToInt(starCount / 2));
		string finalName = "";
		for (int i = 0; i < wordCount - 1; i++)
		{
			finalName += ConstellationAdjectives[UnityEngine.Random.Range(0, ConstellationAdjectives.Length)] + " ";
		}
		finalName += ConstellationNouns[UnityEngine.Random.Range(0, ConstellationNouns.Length)];
		return finalName;
	}

	#endregion



	#region Workshop

	//protected void CreateDisplayConstellation(GameObject original)
	//{
	//	//Duplicate Constellation
	//	var duplicateConstellation = Instantiate(original);
	//	for (int i = 0; i < duplicateConstellation.transform.childCount; i++)
	//	{
	//		var child = duplicateConstellation.transform.GetChild(i);
	//		var poolComp = child.GetComponentInChildren<PooledObject>();
	//		Destroy(poolComp);

	//		var starComp = child.GetComponentInChildren<StarController>();
	//		Destroy(starComp);

	//		var lineComp = child.GetComponentInChildren<LineRenderer>();
	//		if (lineComp != null)
	//			lineComp.SetWidth(0.02f, 0.02f);
	//	}
	//	duplicateConstellation.transform.SetParent(ConstellationDisplayParent);
	//	duplicateConstellation.transform.localScale = Vector3.one;
	//	duplicateConstellation.transform.localPosition = new Vector3(0, 4 * ConstellationDisplayParent.childCount, 0);
	//}

	#endregion
}
