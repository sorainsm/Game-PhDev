using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EndScreen : MonoBehaviour
{
	public Button Exit;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("EndScreen::Starting");

        Exit.onClick.AddListener(QuitGames);
    }

    void QuitGames()
    {
        Debug.Log("EndScreen::QuitGames");
        MinigameManager.Instance.WriteScores();
        
    	if (Application.isEditor)
    	{
    		Debug.Log("EndScreen:: Running in Editor. Application is 'closed'");
    		return;
    	} else 
    	{
    		Application.Quit();
    		return;
    	}
    }
}
