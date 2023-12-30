using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SFB;


public class StartScreen : MonoBehaviour
{
	public Button Play;
	public Button Practice;
    public Button LoadConfig;

	private bool IsConfigLoaded;

    // Start is called before the first frame update
    void Start()
    {
    	Debug.Log("StartScreen::Running Atomic Challenges");

    	IsConfigLoaded = false;
    	Play.interactable = false;
        Practice.interactable = true;
        LoadConfig.interactable = true;

        Play.onClick.AddListener(PlayGames);
        Practice.onClick.AddListener(PracticeGames);
        LoadConfig.onClick.AddListener(GetExperimentConfig);
    }

    string GetFile()
    {
        string file;
        var paths = StandaloneFileBrowser.OpenFilePanel("Load Config File", "","",false);
        if (paths.Length == 0)
        {
            file = null;
        } else 
        {
            file = paths[0];
        }
        Debug.Log("StartScreen::File is: " + file);
        return file;
    }


    void GetExperimentConfig()
    {
        string configFile = GetFile();
           
       	try
    	{
    		MinigameManager.Instance.LoadGames(configFile);    		
    	}
    	catch (Exception e)
    	{
    		if (e is EmptyConfigException)
    		{
    			Debug.Log("ERR: Config is empty.");
                MinigameManager.Instance.WriteExampleConfig();
    			return;
    		}
    		else if (e is InvalidScenesException)
    		{
    			Debug.Log("ERR: Config has invalid scenes.");
    			return;
    		}
    		else if (e is BadConfigException)
    		{
    			Debug.Log("ERR: Config could not be parsed.");
    			return;
    		}
            else if (e is NoConfigException)
            {
                Debug.Log("ERR: No config is chosen.");
                MinigameManager.Instance.WriteExampleConfig();
                return;
            }
    		else
    		{
    			Debug.Log("ERR: Config raised " + e.Message);
    			return;
    		}
    	}
    	Debug.Log("StartScreen::Config loaded successfully.");
    	IsConfigLoaded = true;
    	Play.interactable = true;
        LoadConfig.interactable = false;
    }

    void PlayGames()
    {
    	MinigameManager.Instance.StartGames();

    	Debug.Log("Play button clicked.");
    	if (IsConfigLoaded)
    	{
    		MinigameManager.Instance.LoadNextScene();
    	}
    }

    void PracticeGames()
    {
    	MinigameManager.Instance.SetPractice(true);
    	SceneManager.LoadScene("PracticeList", LoadSceneMode.Single);
    }


}
