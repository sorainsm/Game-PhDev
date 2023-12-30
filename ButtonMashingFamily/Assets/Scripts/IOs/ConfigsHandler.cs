using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;


public class EmptyConfigException : Exception{}
public class BadConfigException : Exception{}
public class InvalidScenesException : Exception{}
public class NoConfigException : Exception{}

public class ConfigsHandler
{

	static private Dictionary<string,GameConfig> GameList;

	private ExperimentConfig Config;
	private bool ConfigLoaded;

	public ConfigsHandler(Dictionary<string,GameConfig> Games)
	{
		GameList = Games;
	}

	public Dictionary<string,GameConfig> GetGameList()
	{
		return GameList;
	}

	public List<string> GameScenes()
	{
		var list = new List<string>();
		foreach (GameConfig game in Config.Games)
		{
			list.Add(game.scene);
		}
		return list;
	}

	public string StartTime()
	{
		return Config.StartTime;
	}

	public string EndTime()
	{
		return Config.EndTime;
	}

	public string TimeStamp()
	{
		return System.DateTime.Now.ToString("yyy-MM-dd-hh-mm-ss");
	}


	public void Start()
	{
		Config.StartTime = TimeStamp();
	}

	public void End()
	{
		Config.EndTime = TimeStamp();
	}

	public GameConfig Get(int index)
	{
		return Config.Games[index];
	}

	public string GetGameName(int index)
	{
		return Get(index).minigame;
	}

	public string Generate()
	{
		Debug.Log("ConfigsHandler::Generating file");
		var config = new ExperimentConfig();
		config.Games = new List<GameConfig>();

		foreach (var game in GameList)
		{
			Debug.Log("ConfigsHandler::Generating config for " + game);
			var g = game.Value;
			Debug.Log("ConfigsHandler::g is " + g);
			g.scene = game.Key;
			string[] tmpStr = game.Key.Split('-', 2);
			g.minigame = tmpStr[1];
			Debug.Log("ConfigsHandler::g.scene is " + g.scene);
			config.Games.Add(g);
		}
		Debug.Log("ConfigsHandler::config.Games is " + config.Games);
		var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
		string json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
		Debug.Log("ConfigsHandler:: json is " + json);
		return json;
	}

	public string Serialize()
	{
		var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
		string json = JsonConvert.SerializeObject(Config,Formatting.Indented,settings);
		return json;
	}


	public void Load(string json)
	{
		Debug.Log("ConfigsHandler::Loading " + json);
		if (json == null || json == "")
		{
			throw new NoConfigException();
		}

		var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto};
		ExperimentConfig result = null;

		result = JsonConvert.DeserializeObject<ExperimentConfig>(json,settings);
		Debug.Log("ConfigsHandler::Converted json to result: " + result);

		if (result==null)
		{
			throw new EmptyConfigException();
		}

		int numScenes = result.Games.Count;
		int valid = 0;
		List<string> validScenes = new List<string>(GameList.Keys);

		foreach (GameConfig game in result.Games)
		{
			if (validScenes.Contains(game.scene))
			{
				var a = GameList[game.scene].GetType();
				var b = game.GetType();
				if (a==b)
				{
					valid++;
				}
			}
		}

		if (valid != numScenes)
		{
			throw new InvalidScenesException();
		}

		Config = result;

	}

}
