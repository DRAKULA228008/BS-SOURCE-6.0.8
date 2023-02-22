using System;
using System.Collections.Generic;
using System.IO;
using FreeJSON;
using UnityEngine;

public class AnimRecord : MonoBehaviour
{
	[Serializable]
	public class Point
	{
		public Transform target;

		public bool pos;

		public bool rot;

		public List<Vector3> posList = new List<Vector3>();

		public List<Vector3> rotList = new List<Vector3>();
	}

	public bool isRecord;

	public List<Point> points = new List<Point>();

	private int recordClip;

	public int maxClip;

	public bool play;

	public int t;

	[ContextMenu("Save")]
	private void Save()
	{
		JsonObject jsonObject = new JsonObject();
		for (int i = 0; i < points.Count; i++)
		{
			JsonObject jsonObject2 = new JsonObject();
			if (points[i].pos)
			{
				jsonObject2.Add("pos", points[i].posList);
			}
			if (points[i].rot)
			{
				jsonObject2.Add("rot", points[i].rotList);
			}
			jsonObject.Add(points[i].target.name, jsonObject2);
		}
		File.WriteAllText(Path.GetDirectoryName(Application.dataPath) + "/Record.txt", jsonObject.ToString());
	}

	[ContextMenu("Snapshot")]
	private void Snapshot()
	{
		JsonObject jsonObject = new JsonObject();
		for (int i = 0; i < points.Count; i++)
		{
			JsonObject jsonObject2 = new JsonObject();
			jsonObject2.Add("pos", points[i].target.localPosition);
			jsonObject2.Add("rot", points[i].target.localEulerAngles);
			jsonObject.Add(points[i].target.name, jsonObject2);
		}
		File.WriteAllText(Path.GetDirectoryName(Application.dataPath) + "/Record.txt", jsonObject.ToString());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			isRecord = !isRecord;
			if (!isRecord)
			{
			}
		}
		if (isRecord)
		{
			recordClip++;
			if (recordClip == maxClip)
			{
				isRecord = false;
			}
			for (int i = 0; i < points.Count; i++)
			{
				if (points[i].pos)
				{
					points[i].posList.Add(points[i].target.localPosition);
				}
				if (points[i].rot)
				{
					points[i].rotList.Add(points[i].target.localEulerAngles);
				}
			}
		}
		if (!play)
		{
			return;
		}
		t++;
		if (t >= recordClip)
		{
			t = 0;
		}
		for (int j = 0; j < points.Count; j++)
		{
			if (points[j].pos)
			{
				points[j].target.localPosition = points[j].posList[t];
			}
			if (points[j].rot)
			{
				points[j].target.localEulerAngles = points[j].rotList[t];
			}
		}
	}

	private void OnGUI()
	{
		if (!GUI.Button(NewRect(5f, 5f, 40f, 10f), "Next"))
		{
			return;
		}
		t++;
		if (t >= recordClip)
		{
			t = 0;
		}
		for (int i = 0; i < points.Count; i++)
		{
			if (points[i].pos)
			{
				points[i].target.localPosition = points[i].posList[t];
			}
			if (points[i].rot)
			{
				points[i].target.localEulerAngles = points[i].rotList[t];
			}
		}
	}

	private Rect NewRect(float x, float y, float width, float height)
	{
		x = (float)Screen.width * x / 100f;
		y = (float)Screen.height * y / 100f;
		width = (float)Screen.width * width / 100f;
		height = (float)Screen.height * height / 100f;
		return new Rect(x, y, width, height);
	}
}
