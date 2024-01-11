/* 
*	Module Name: Level Manager 
*	Created by: Sasha Soraine
*	Purpose: This module is the abstract class for the game managers for the various button mashing mini-games. 
*	It provides the basic functions necessary for all the mini-games: instructions, score keeping, game setup and teardown, and defaults.
*	Organizational layout and ideas borrowed from the Mactivision mini-game modules.
*/
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

/*
*	Partitions the level into three states: hold, instructions, countdown, playing, and end.
*	These states are used to understand how to interpret UI interactions at different points in time.
*/
public enum LevelState
{
	Hold,
	Instructions,
	Countdown,
	Playing,
	End
}

public abstract class LevelManager : MonoBehaviour
{
	//Level flow related variables
	public LevelState lvlState;			//Tells us what state the level is in

	//Gameplay related variables
	public float gameStartTime;			//Time stamp from system clock when the game starts
	public float maxGameTime;			//Maximum length of the game
	public AudioSource bgmGameplay;


	public PostProcessVolume postProcess;	//graphical effect used for blurring the scene

	//Instruction and other text-based overlay information
	public GameObject instructionParent;
	public GameObject[] instructions;
	public int instructionCount;			//Keeps track of the current instruction page

	public GameObject countdownParent;
	public GameObject countdownText;
	public RectTransform countdownSize;
	public string startText = "Start!";

	public GameObject outroParent;
	public GameObject outroText;
	public RectTransform outroSize;

	public GameObject timerText;

	public GameObject textBG;
	public RectTransform textBG_Main;

	public AudioSource countdownChime;
	public AudioSource bgmInstructions;
	bool isInstructionPlaying;



	/*
	*	
	*/
	public void Setup()
	{
		Debug.Log("Setup started.");
		lvlState = LevelState.Hold;
		Debug.Log("lvlState= " + lvlState);

		textBG.SetActive(true);
		countdownText.SetActive(false);
		outroText.SetActive(false);
		timerText.SetActive(false);

		bgmInstructions.time = 3;
		isInstructionPlaying = false;
		ShowInstructions(0);
		instructionCount = 0;
		ChangeBlur(2f);

	}

	IEnumerator CountdownGame()
	{
		Debug.Log("Gameplay countdown begins.");
		int timeCountdown = (int)maxGameTime;
		timerText.SetActive(true);					//Make sure timer is being displayed
		while (timeCountdown >= 0)
		{
			timerText.GetComponent<TMP_Text>().text = timeCountdown.ToString();
			timeCountdown--;
			yield return new WaitForSeconds(1);
		}
		timerText.SetActive(false);					//Hide timer for outro animations
	}



	/*	Method: ShowInstructions
		Inputs: int idx; this is the instruction we will be displaying
		This method displays the instructions for the mini-game. if the number passed in is higher than instruction.Length then all the instructions have been displayed and the game can begin.
	*/
	public void ShowInstructions(int idx)
	{
		Debug.Log("Starting ShowInstructions()");
		lvlState = LevelState.Instructions;
		Debug.Log("ShowInstructions::lvlState= " + lvlState);
		PlayInstructions();
		for (int i=0; i<instructions.Length; i++)
		{
			instructions[i].SetActive(i==idx);
		}

		if (idx<instructions.Length)
		{
			ResizeTextBG(GetRect(instructionParent));
		} else {
			instructionParent.SetActive(false);
			StartCoroutine(Countdown());
		}
	}

	void PlayInstructions()
	{
		if (lvlState == LevelState.Instructions)
		{
			if (isInstructionPlaying)
			{
				return;
			} else 
			{
				bgmInstructions.Play();
				isInstructionPlaying = true;
			}
		} else 
		{
			bgmInstructions.Stop();
			isInstructionPlaying = false;
		}
	}

	/*
	*	Co-routine: Countdown
	*	Runs the countdown before game start and then starts level
	*/
	IEnumerator Countdown()
	{
		Debug.Log("Starting Countdown()");
		lvlState = LevelState.Countdown;
		Debug.Log("Countdown::lvlState= " + lvlState);
		countdownText.SetActive(true);
		ResizeTextBG(GetRect(countdownParent));
		ResizeCountdownSize(GetRect(countdownParent));
		countdownChime.PlayDelayed(0.0f);

		countdownText.GetComponent<TMP_Text>().text = "3";
		yield return new WaitForSeconds(1);
		countdownText.GetComponent<TMP_Text>().text = "2";
		yield return new WaitForSeconds(1);
		countdownText.GetComponent<TMP_Text>().text = "1";
		yield return new WaitForSeconds(1);
		countdownText.GetComponent<TMP_Text>().text = startText;
		yield return new WaitForSeconds(1);

		countdownText.SetActive(false);
		textBG.SetActive(false);
		ChangeBlur(10f);

		StartLevel();
	}



	public void StartLevel()
	{
		Debug.Log("StartLevel called. Original lvlState: " + lvlState);
		lvlState = LevelState.Playing;
		Debug.Log("StartLevel::lvlState= " + lvlState);
		gameStartTime = Time.time;
		Debug.Log("gameStartTime: " + gameStartTime);
		bgmGameplay.Play();
		Debug.Log("Started bgmGameplay");
		StartCoroutine(CountdownGame());
	}


	public void EndLevel(float delay)
	{
		lvlState = LevelState.End;
		bgmGameplay.Stop();					
		if (MinigameManager.Instance.GetPractice())
		{
			StartCoroutine(PracticeReturn(delay));
		} else {
			StartCoroutine(WrapUp(delay));
		}
	}

	IEnumerator WrapUp(float delay)
	{
		Debug.Log("Starting WrapUp()");
		yield return new WaitForSeconds(delay);
		ChangeBlur(2f);
		textBG.SetActive(true);
		outroText.SetActive(true);	
		ResizeTextBG(GetRect(outroParent));
	}


	IEnumerator PracticeReturn(float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene("PracticeList", LoadSceneMode.Single);
	}

    // Returns a text object's bounding box
    public Rect GetRect(GameObject obj) 
    {
        return obj.GetComponent<RectTransform>().rect;
    }

	// Resizes the red background according to text's bounding box
    public void ResizeTextBG(Rect box) 
    {
        float w = box.width+40;
        float h = box.height+40;
        textBG.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
        textBG_Main.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, w);
        textBG_Main.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, h);
    }

    public void ResizeOutroSize(Rect box) 
    {
        float w = box.width+40;
        float h = box.height+40;
        outroText.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
        outroSize.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, w);
        outroSize.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, h);
    }

    public void ResizeCountdownSize(Rect box) 
    {
        float w = box.width+40;
        float h = box.height+40;
        countdownText.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
        countdownSize.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, w);
        countdownSize.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, h);
    }



    // Blurs the scene by changing the scene camera's depth of field
    void ChangeBlur(float dist)
    {
    	Debug.Log("Starting ChangeBlur()");
        if (postProcess) {
            DepthOfField pr;
            
            if (postProcess.profile.TryGetSettings<DepthOfField>(out pr)){
                pr.focusDistance.value = dist;
            }
         }
    }

    //From Mactivision
    public int Default(int val, string log)
    {
        Debug.LogFormat("Missing or invalid value for {0}, using {1}", log, val.ToString());
        return val;
    }

    public float Default(float val, string log)
    {
        Debug.LogFormat("Missing or invalid value for {0}, using {1}", log, val.ToString());
        return val;
    }

    public bool Default(bool val, string log)
    {
        Debug.LogFormat("Missing or invalid value for {0}, using {1}", log, val.ToString());
        return val;
    }

    public KeyCode Default(KeyCode val, string log)
    {
        Debug.LogFormat("Missing or invalid value for {0}, using {1}", log, val.ToString());
        return val;
    }       

}
