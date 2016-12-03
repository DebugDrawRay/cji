﻿using UnityEngine;
using System;
using System.Collections.Generic;

public static class GameData
{
	public enum StarType { Star, Circle, Triangle, Square };

	public class Star
	{
		public Guid StarId;
		public StarType Type;
		public Vector3 Position;
		public List<Guid> LinkedStars;
		public StarController Controller;

		public Star(StarController controller = null)
		{
			StarId = Guid.NewGuid();
			LinkedStars = new List<Guid>();
			Controller = controller;
		}
	}

	public class Link
	{
		public List<Guid> StarIds;
		public LineRenderer LineComponent;
		public Vector3 StartPos;
		public Vector3 EndPos;

		public Link()
		{
			StarIds = new List<Guid>(); 
		}
	}

	public class Constellation
	{
		public Dictionary<Guid, Star> Stars;
		public List<Link> Links;
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
    public const float starSpawnXMin = -4;
    public const float starSpawnXMax = 4;
    public const float starSpawnY = 5;

    //Comet parameters
    public const float cometAcceleration = .01f;

    //Star power
    public const float strengthMultiplier = .25f;
    public const float cometCollisionSpeed = .25f;

}
