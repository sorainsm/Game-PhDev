using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class ScoresHandler
{
	private List<GameScores> ScoresList;
	private string scoreFilePath;
	static private string sessionStart;

	public ScoresHandler(string path)
	{
		scoreFilePath = path;
		ScoresList = new List<GameScores>();
		sessionStart = System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
	}

	public void AddScoreToList(GameScores s)
	{
		ScoresList.Add(s);
	}

	public string GetScorePath()
	{
		return scoreFilePath;
	}

	public string SerializeScores()
	{
		Debug.Log("ScoresHandler::Serializing scores");
		var scoreData = new Scores();
		scoreData.Games = ScoresList;
		scoreData.StartTime = sessionStart;
		scoreData.EndTime = System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");

		var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
		string json = JsonConvert.SerializeObject(scoreData,Formatting.Indented,settings);

		return json;
	}

}
