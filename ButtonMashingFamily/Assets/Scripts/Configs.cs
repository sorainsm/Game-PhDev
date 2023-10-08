using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SpoiledCat.Json;

public class ExperimentConfig
{
	public string StartTime {get; set;}
	public string EndTime {get; set;}

	public IList<GameConfig> Games {get; set;}
}

public class GameConfig
{
	public string minigame {get; set;}
	public string scene {get; set;}
	public string targetKey {get; set;}
	public float maxGameTime {get; set;}

}

public class SIBMConfig : GameConfig
{
	//Elements inherited from GameConfig
	public string minigame {get; set;}
	public string scene {get; set;}
	public string targetKey {get; set;}
	public float maxGameTime {get; set;}
	//Elements specific to SIBM
	public int goal {get; set;}
	public float scoreModifier {get; set;}
}

public class AIBMConfig : GameConfig
{
	//Elements inherited from GameConfig
	public string minigame {get; set;}
	public string scene {get; set;}
	public string targetKey {get; set;}
	public float maxGameTime {get; set;}
	//Elements specific to SIBM
	public int goal {get; set;}
	public float scoreModifier {get; set;}
}