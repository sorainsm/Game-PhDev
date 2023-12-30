using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController
{
	private int Index = -1;
	private string Start;
	private string End;
	private List<string> Games;

	private int Max = 100;

	public SceneController(string start, string end, List<string> games)
	{
		Start = start;
		End = end;
		Games = games;
	}

	public void SetIndex(int n)
	{
		Index = n;
	}

	public void SetMax(int n)
	{
		Max = n;
	}

	public void Next()
	{
		Debug.Log("SceneController::Incrementing index to " + Index);
		Index++;
	}

	public int Current()
	{
		return Index;
	}

	public string Name()
	{
		Debug.Log("SceneController:: Getting name at index " + Index);
		if (Index < 0)
		{
			Debug.Log("SceneController::Returning " + Start);
			return Start;
		} else if (Index >= Max)
		{
			Debug.Log("SceneController::Returning " + End);			
			return End;
		} else if (Index < Games.Count)
		{
			Debug.Log("SceneController::Returning " + Games[Index]);			
			return Games[Index];
		} else 
		{
			return End;			
		}
	}
}
