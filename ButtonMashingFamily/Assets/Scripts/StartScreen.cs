using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
	public Button Play;
	public Button Practice;

	private bool IsConfigLoaded;


    // Start is called before the first frame update
    void Start()
    {
    	Debug.Log("Running Atomic Challenges");

    	IsConfigLoaded = false;
    	Play.interactable = false;
        Practice.interactable = true;

        Play.onClick.AddListener(PlayGames);
        Practice.onClick.AddListener(PracticeGames);
    }

    void GetExperimentConfig(string text)
    {
    	
    	try
    	{
    		MinigameManager.Instance.LoadGames(text);    		
    	}
    	catch (Exception e)
    	{
    		if (e is EmptyConfigException)
    		{
    			Debug.Log("ERR: Config is empty.");
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
    		else
    		{
    			Debug.Log("ERR: Config raised " + e.Message);
    			return;
    		}
    	}
    	Debug.Log("Config loaded successfully.");
    	IsConfigLoaded = true;
    	Play.interactable = true;
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
