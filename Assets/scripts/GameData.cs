using UnityEngine;
using System;
using System.Collections.Generic;

public static class GameData
{
	public enum StarType { Star, Circle, Triangle, Square, None };

	[Serializable]
	public class Star
	{
		//Data Objects
		public Guid StarId;
		public StarType Type;
		public Vector3 Position;
		public List<Guid> LinkedStars;

		//Game Objects
		[NonSerialized]
		public StarController Controller;

		public Star(StarController controller = null)
		{
			StarId = Guid.NewGuid();
			LinkedStars = new List<Guid>();
			Controller = controller;
		}
	}

	[Serializable]
	public class Link
	{
		//Data Objects
		public List<Guid> StarIds;
		public Vector3 StartPos;
		public Vector3 EndPos;

		//Game Objects
		public LineRenderer LineComponent;

		public Link()
		{
			StarIds = new List<Guid>(); 
		}
	}

	public class Constellation
	{
		public string ConstellationName;
		public GameObject ConstellationParent;
		public Dictionary<Guid, Star> Stars;
		public List<Link> Links;
		public Sprite Background;
	}

    //Constants 
    public const float cometStartY = 7;
    public const float cometDest = -5f;
    public const int minimumStars = 3;

    //Star Speed
    public const float minStarSpeed = 1;
    public const float maxStarSpeed = 3;

    //Star timing
    public const float minTimeToSpawn = 0.25f;
    public const float maxTimeToSpawn = .5f;

    //Star spawn
    public const float fieldSize = 4;
    public const float starSize = .5f;
    public const float starSpawnY = 10;

    //Comet parameters
    public const float cometAcceleration = .005f;
    public const float accelerationIncreaseRate = 100;
    public static readonly float[] cometAcelerationLevels = { .0025f, .005f};
    public const float dangerLimit = 0f;
    //Star power
    public const float strengthMultiplier = .25f;
    public const float cometCollisionSpeed = 1f;

    //Scoring
    public const int scorePerStar = 10;
    public const float scoreConnectionMulti = .1f;
    public const float constSizeMulti = .25f;

    public const float distanceScalar = 1000;

    //Start Sequence
    public const float cometStartAccel = 2f;
    public const float playerStartAccel = 1f;

    //player
    public const float playerStartY = 0;

    //Constellation parameters
    public const float sendSpeed = 1.5f;
}
