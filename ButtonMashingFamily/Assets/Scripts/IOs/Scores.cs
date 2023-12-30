using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scores
{
	public string StartTime {get; set;}
	public string EndTime {get; set;}

	public IList<GameScores> Games {get; set;}
}

public interface GameScores
{
	public string minigame {get; set;}
	public string scene {get; set;}
	public string targetKey {get; set;}
	public float maxGameTime {get; set;}
	public int goal {get;set;}
	public float scoreModifier {get;set;}
	public float score {get;set;}
}

public class SIBMScore : GameScores
{
	public string minigame {get; set;}
	public string scene {get; set;}
	public string targetKey {get; set;}
	public float maxGameTime {get; set;}
	public int goal {get;set;}
	public float scoreModifier {get;set;}
	public float score {get;set;}
}

public class AIBMScore : GameScores
{
	public string minigame {get; set;}
	public string scene {get; set;}
	public string targetKey {get; set;}
	public string targetKey2 {get;set;}
	public float maxGameTime {get; set;}
	public int goal {get;set;}
	public float scoreModifier {get;set;}
	public float score {get;set;}
}

public class MIBMScore : GameScores
{
	public string minigame {get; set;}
	public string scene {get; set;}
	public string targetKey {get; set;}
	public string targetKey2 {get;set;}
	public float maxGameTime {get; set;}
	public int goal {get;set;}
	public float scoreModifier {get;set;}
	public float score {get;set;}
}