using System.Collections;
using System.Collections.Generic;


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
		Index++;
	}

	public int Current()
	{
		return Index;
	}

	public string Name()
	{
		if (Index < 0)
		{
			return Start;
		}

		if (Index >= Max)
		{
			return End;
		}

		if (Index < Games.Count)
		{
			return Games[Index];
		}

		return End;
	}
}
