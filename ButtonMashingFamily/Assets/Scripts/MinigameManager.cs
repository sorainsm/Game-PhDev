using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SpoiledCat.Json;


public class MinigameManager
{
   // Configuration
    //private const string ServerURL = "http://127.0.0.1:8000/connect";
    private const string StartScene = "Battery Start";
    private const string EndScene = "Battery End";
    private const string ConfigFileName = "ExperimentConfig.json";

    // Where Generated Configs are Placed
    private const string ConfigPath = "./Assets/Configs/";
    private const string GeneratedFileName = "GeneratedTemplate.json";

    // Name the same as generated file name for clean up purposes.
    private const string GeneratedMetaFileName = "GeneratedTemplate.meta";


	static private Dictionary<string,GameConfig> GameList = new Dictionary<string,GameConfig>
	{
		{"SIBM-Campfire", new SIBMConfig()},
        {"AIBM-Cauldron", new AIBMConfig()}
	};

	private ConfigsHandler Config;
	private SceneController Scene;
	private FileHandler File;

	private bool IsLoaded;
	private bool IsPractice = false;

	public static readonly MinigameManager Instance = new MinigameManager();

	private MinigameManager()
	{
		Reset();
	}

	public void Reset()
	{
		Config = new ConfigsHandler(GameList);
        File = new FileHandler(ConfigPath, GeneratedFileName, GeneratedMetaFileName);
        IsLoaded = false;
        //Guid token = Guid.NewGuid();
        //Token = token.ToString();
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
    public void LoadGames(string json)
    {
        Config.Load(json);
        Scene = new SceneController(StartScene, EndScene, Config.GameScenes());
        IsLoaded = true;
    }

    // Scenes are loaded by name
    public void LoadScene(string Scene)
    {
        // LoadSceneMode.Single means that all other scenes are unloaded before new scene is loaded.
        SceneManager.LoadScene(Scene, LoadSceneMode.Single); 
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
            Debug.Log(Scene.Name());
            LoadScene(Scene.Name());
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

    // As the configurable variables are added, deleted or renamed during development in order not have to constantly sync these names with the configuration files this function can be used to generate a blank configuration file based off those variables. 
    public void WriteExampleConfig()
    {
        File.WriteGenerated(Config.Generate());
    }    


}
