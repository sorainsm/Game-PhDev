using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SpoiledCat.Json;

public class EmptyConfigException : Exception{}
public class BadConfigException : Exception{}
public class InvalidScenesException : Exception{}

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
		var config = new ExperimentConfig();
		config.Games = new List<GameConfig>();

		foreach (var game in GameList)
		{
			var g = game.Value;
			g.scene = game.Key;
			config.Games.Add(game.Value);
		}

		string json = SimpleJson.SerializeObject(config);

		return json;
	}

	public string Serialize()
	{
		string json = SimpleJson.SerializeObject(Config);
		return json;
	}


	public void Load(string json)
	{
		ExperimentConfig result = null;

		result = SimpleJson.DeserializeObject<ExperimentConfig>(json);

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
