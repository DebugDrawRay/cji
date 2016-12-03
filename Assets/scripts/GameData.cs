using UnityEngine;
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

		public Star(Vector3 position, StarController controller = null)
		{
			StarId = Guid.NewGuid();
			Position = position;
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
}
