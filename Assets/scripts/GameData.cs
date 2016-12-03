﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameData
{
	public enum StarType { Star, Circle, Triangle, Square };

	public class Star
	{
		public StarType Type;
		public Vector2 Position;
	}

	public class Constellation
	{
		public List<Star> Stars;
		public List<int[]> StarLinks;

		public Constellation()
		{
			Stars = new List<Star>();
			StarLinks = new List<int[]>();
		}
	}
}
