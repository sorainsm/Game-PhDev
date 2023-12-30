using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class MinigameManager
{
   // Configuration
    private const string StartScene = "StartScreen";
    private const string EndScene = "EndScreen";
    private const string ConfigFileName = "ExperimentConfig.json";

    // Where Generated Configs are Placed
    private string m_Path = Application.persistentDataPath;
    private string ConfigPath;
    private string ScorePath;
    private string GeneratedFileName;

    // Name the same as generated file name for clean up purposes.
    private const string GeneratedMetaFileName = "GeneratedTemplate.meta";


	static private Dictionary<string,GameConfig> GameList = new Dictionary<string,GameConfig>
	{
		{"SIBM-Campfire", new SIBMConfig()},
        {"AIBM-Cauldron", new AIBMConfig()},
        {"MIBM-Broom", new MIBMConfig()}
	};

	private ConfigsHandler Config;
    private ScoresHandler Scores;
	private SceneController Scene;
	private FileHandler FileH;

	private bool IsLoaded;
	private bool IsPractice = false;

	public static readonly MinigameManager Instance = new MinigameManager();

	private MinigameManager()
	{
        ConfigPath = m_Path + "/Configs/";
        ScorePath = m_Path + "/Scores/";
        GeneratedFileName  = "GeneratedTemplate.json";
		Reset();
	}

	public void Reset()
	{
		Config = new ConfigsHandler(GameList);
        Scores = new ScoresHandler(ScorePath);
        FileH = new FileHandler(ConfigPath, GeneratedFileName, GeneratedMetaFileName);
        IsLoaded = false;
        IsPractice = false;
	}

	public string GetConfigFileName()
	{
		return ConfigFileName;
	}

    public string GetGameName()
    {
        // Game name is part of the GameConfig interface so does not require casting to the specific game config. Useful to generating log files by name. Name is not the name of the game but that specific test of a game.
        if (IsLoaded)
        {
            return Config.GetGameName(Scene.Current());
        }
        return "";
    }

    // Returns the GameConfig interface type. Specific games will have to cast the GameConfig to their respective Config class in order to child parameters. 
    public GameConfig GetCurrentConfig()
    {
        if (IsLoaded)
        {
            return Config.Get(Scene.Current());
        }
        return null;
    }

    public bool GetPractice()
    {
    	return IsPractice;
    }

    public void SetPractice(bool p)
    {
    	IsPractice = p;
    }

    // Load the BatteryConfig JSON file and deserialize it while maintaining type information. Currently uses TextAsset which is a Unity Resource type. This allows easier file reading but it may not be wise to clutter resource folder. 
    public void LoadGames(string jsonFile)
    {
        string json = FileH.ReadFile(jsonFile);
        Config.Load(json);
        Scene = new SceneController(StartScene, EndScene, Config.GameScenes());
        IsLoaded = true;
    }

    // Scenes are loaded by name
    public void LoadScene(string thisScene)
    {
        // LoadSceneMode.Single means that all other scenes are unloaded before new scene is loaded.
        SceneManager.LoadScene(thisScene, LoadSceneMode.Single); 
    }

    public string SerializedConfig()
    {
        return Config.Serialize();
    }

    public void StartGames()
    {
        if (IsLoaded)
        {
            Config.Start();
        }
    }

    public void EndGames()
    {
        if (IsLoaded)
        {
            Config.End();
        }
    }
   
    public string GetStartTime()
    {
        return Config.StartTime();
    }
    
    public string GetEndTime()
    {
        return Config.EndTime();
    }

    public void LoadNextScene()
    {
        // Scenes are indexed according to the order they appear in the experiment config games list. The earlier in the list the earlier they will be loaded.
        if (IsLoaded)
        {
            Scene.Next();
            Debug.Log("MinigameManager:: Loading " + Scene.Name());
            LoadScene(Scene.Name());
        } else 
        {
            LoadScene(EndScene);
        }
    }

    private string GetCurrentScene()
    {  
        return Scene.Name();
    }

    // Lists the games that player will play during the battery session. Useful for testings.
    public List<string> GetBatteryGameList()
    {
        if (IsLoaded)
        {
            return Config.GameScenes();
        }
        return null;
    }

    public void AddScore(GameScores s)
    {
        Scores.AddScoreToList(s);
    }

    // As the configurable variables are added, deleted or renamed during development in order not have to constantly sync these names with the configuration files this function can be used to generate a blank configuration file based off those variables. 
    public void WriteExampleConfig()
    {
        FileH.WriteGenerated(Config.Generate());
    }

    public void WriteScores()
    {
        string data = Scores.SerializeScores();
        string path = Scores.GetScorePath();
        if (File.Exists(path))
        {
            FileH.WriteFile(path,data);    
        } else 
        {
            FileH.WriteGenerated(data);
        }      
    }

}
