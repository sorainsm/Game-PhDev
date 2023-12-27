using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PracticeScreen : MonoBehaviour
{
	public Button Campfire;
    public Button Cauldron;
	public Button Back;

    // Start is called before the first frame update
    void Start()
    {
        Campfire.onClick.AddListener(LoadCampfire);
        Cauldron.onClick.AddListener(LoadCauldron);
        Back.onClick.AddListener(ReturnMain);
    }

    void LoadCampfire()
    {
		SceneManager.LoadScene("SIBM-Campfire", LoadSceneMode.Single);
    }

    void ReturnMain()
    {
    	MinigameManager.Instance.SetPractice(false);
    	SceneManager.LoadScene("StartScreen", LoadSceneMode.Single);
    }

    void LoadCauldron()
    {
        SceneManager.LoadScene("AIBM-Cauldron", LoadSceneMode.Single);
    }

}
