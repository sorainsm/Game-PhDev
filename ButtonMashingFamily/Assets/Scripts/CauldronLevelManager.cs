using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CauldronLevelManager : LevelManager
{
    float score;
    int goal;
    float scoreModifier;

    /*
    Alternating button inputs
    Considerations: size of buttons, location of buttons relative to each other.
    Default: Left and right arrows; could do shifts
    Note: target1 should always be the first in sequence
    */
    KeyCode target1 = KeyCode.RightArrow;
    KeyCode target2 = KeyCode.LeftArrow;

    public GameObject cauldron;
    public GameObject smoke;
    public GameObject bubbles;

    public GameObject player;
    Animator playerAnim;
    KeyCode nextKey;

    public AudioSource fail;
    public AudioSource success;
    public AudioSource superSucess;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Cauldron::Setup run");
        Setup();
        maxGameTime = 10f;
        Debug.Log("Cauldron::Game time set to " + maxGameTime);
        goal = 45;
        Debug.Log("Cauldron::Goal is set to " + goal);
        Debug.Log("Cauldron::Target keys are set to " + target1 + " " + target2);
        scoreModifier = 1f;
        Debug.Log("Cauldron::Score modifier is " + scoreModifier);
    
        nextKey = target1; 
        Debug.Log("Cauldron::nextKey is " + nextKey);


        playerAnim = player.GetComponent<Animator>();
        playerAnim.Play("Base Layer.Idle");

    }

    void EndGame()
    {
        string outro = "good job";
        //StartCoroutine(FinishingAnimations());
        FinalScoring();
        if (score < 0.5f*goal)
        {
            outro = "Not quite...";
        } else if (0.5f*goal < score && score < 1.0f*goal)
        {
            outro = "Good job!";
        } else if (1.0f*goal <= score)
        {
            outro = "A perfect cast!";
        }
        outroText.GetComponent<TMPro.TextMeshProUGUI>().text = outro;
        EndLevel(10f);
    }

    /*
    * Purpose: Call appropriate stir animation for the key pressed
    */
    void HalfStir(EventType e, KeyCode pressedKey)
    {
        if (e == EventType.KeyDown)
        {
            //Animations should only trigger when the sequence is correct
            if (pressedKey == nextKey)
            {
                Debug.Log("Correct key in sequence");
                score += scoreModifier;
                Debug.Log("New score is " + score);
                //When the 1st key is pressed, animator should move the forward stir
                if(pressedKey == target1)
                {
                    playerAnim.Play("Base Layer.StirF");
                    nextKey = target2;
                //When the 2nd key is pressed, animator should move the backwards stir
                } else if (pressedKey == target2) 
                {
                   playerAnim.Play("Base Layer.StirB");
                   nextKey = target1;
                }
             }
        }
    }

    void OnGUI()
    {
        Event e = Event.current;

        if (lvlState==LevelState.Instructions && e.type==EventType.KeyUp)
        {
            if (e.keyCode == KeyCode.Space && instructionCount < instructions.Length)
            {
                ShowInstructions(++instructionCount);
            }
        }

        if (lvlState==LevelState.Playing && e.isKey)
        {
            EventType t = e.type;
            KeyCode pressedKey = e.keyCode;
            HalfStir(t, pressedKey);
        }    
    }

    void FinalScoring()
    {
    	Debug.Log("FinalScoring called.");
    	ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
    	var scoreSmoke = ps.main;
    	int successScore = 0;

    	Debug.Log("Score is " + score + ". Goal is " + goal);
		if (score < 0.5f*goal) 
		{
			Debug.Log("Fail");
			successScore = 0;
    	} else if (0.5f*goal < score && score < 0.75f*goal)
    	{
    		Debug.Log("Success: green");
    		scoreSmoke.startColor = Color.green;
    		successScore = 1;
    	} else if (0.75f*goal < score && score < 1.0f*goal) 
    	{
    		Debug.Log("Success: yellow");
    		scoreSmoke.startColor = Color.yellow;
    		successScore = 1;
    	} else if (1.0f*goal < score && score < 1.5f*goal)
    	{
    		Debug.Log("Success: blue");    		
    		scoreSmoke.startColor = Color.blue;
    		successScore = 2;
    	} else if (1.5f*goal < score) 
    	{
    		Debug.Log("Success: cyan");    		
    		scoreSmoke.startColor = Color.cyan;
    		successScore = 2;
    	}

    	if (ps != null)
    	{
	   		smoke.SetActive(true);
    		ps.Play();
    	}

    	switch(successScore)
    	{
    		case 0:
    			fail.Play();
    			break;
    		case 1:
    			success.Play();
    			break;
    		case 2:
    			superSucess.Play();
    			break;
    		default:
    			fail.Play();
    			break;
    	}
    }

    // Update is called once per frame
    void Update()
    {
        if (lvlState==LevelState.Countdown)
        {
            playerAnim.Play("Base Layer.Idle");
        }

        if (lvlState==LevelState.Playing)
        {           
            if (Time.time-gameStartTime >= maxGameTime)
            {
                EndGame();
                return;
            }
        }        
    }
        
}
