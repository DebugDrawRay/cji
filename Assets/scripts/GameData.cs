using UnityEngine;
using System;
using System.Collections;
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

		public Star(Vector3 position)
		{
			StarId = Guid.NewGuid();
			Position = position;
			LinkedStars = new List<Guid>();
		}
	}

	public class Link
	{
		public List<Guid> StarIds;
		public LineRenderer LineComponent;

		public Link()
		{
			StarIds = new List<Guid>(); 
		}
	}

	public class Constellation
	{
		public Dictionary<Guid, Star> Stars;
		
		public Constellation()
		{
			Stars = new Dictionary<Guid, Star>();
		}
	}
}
