using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ExperimentConfig
{
	public string StartTime {get; set;}
	public string EndTime {get; set;}

	public IList<GameConfig> Games {get; set;}
}

public interface GameConfig
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
	//Elements specific to AIBM
	public int goal {get; set;}
	public float scoreModifier {get; set;}
	public string targetKey2 {get;set;}
}

public class MIBMConfig : GameConfig
{
	//Elements inherited from GameConfig
	public string minigame {get; set;}
	public string scene {get; set;}
	public string targetKey {get; set;}
	public float maxGameTime {get; set;}
	//Elements specific to MIBM
	public int goal {get; set;}
	public float scoreModifier {get; set;}
	public string targetKey2 {get;set;}
}