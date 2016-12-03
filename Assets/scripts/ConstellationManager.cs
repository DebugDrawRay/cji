using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConstellationManager : MonoBehaviour
{
	public static ConstellationManager Instance;

	public List<GameData.Constellation> Constellations;

	public List<GameData.Star> Stars;
	public GameData.Star LastStar;

	void Awake()
	{
		Instance = this;
	}

	void Start ()
	{
		Constellations = new List<GameData.Constellation>();
	}
	
	void Update ()
	{
	
	}

	public void AddStar(GameData.Star star)
	{
		if (!Stars.Contains(star))
		{
			Stars.Add(star);
		}
		LastStar = star;
	}

	public void CompleteConstellation()
	{

	}

	public void BreakConstellation()
	{

	}

	public void BreakLine()
	{

	}
}
