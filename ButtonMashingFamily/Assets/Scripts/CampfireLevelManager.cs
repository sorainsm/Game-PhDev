using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CampfireLevelManager : LevelManager
{
	float score;
	int goal;
	float scoreModifier;

	KeyCode targetKey = KeyCode.Space;

	public GameObject flames;
	public GameObject magicCircle;
	public GameObject explosion;

	public GameObject player;
	Animator playerAnim;
	bool isLookingUp = false;
	bool lookUpHappened = false;

	public AudioSource fail;
	public AudioSource success;
	public AudioSource superSucess;


    // Start is called before the first frame update
    void Start()
    {
     	Debug.Log("Campfire::Setup run");
     	Setup();
     	maxGameTime = 10f; 	 
     	Debug.Log("Campfire game time set to " + maxGameTime);
     	goal = 60;
     	Debug.Log("Goal is set to " + goal);
     	Debug.Log("Target key is set to " + targetKey);
     	scoreModifier = 1f;
     	Debug.Log("Score modifier is " + scoreModifier);
     	//Make sure all the effects are off
     	flames.SetActive(false);
     	magicCircle.SetActive(false);
     	explosion.SetActive(false);

     	playerAnim = player.GetComponent<Animator>();
     	playerAnim.Play("Base Layer.Idle");
    }



    void EndGame()
    {
    	string outro = "good job";
    	StartCoroutine(FinishingAnimations());
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

    void FinalScoring()
    {
    	ParticleSystem scoreFire = null;
    	int successScore = 0;

    	Debug.Log("FinalScoring called. Score is " + score + ". Goal is " + goal);
		if (score < 0.5f*goal) 
		{
			Debug.Log("Fail");
			scoreFire = null;
			successScore = 0;
    	} else if (0.5f*goal < score && score < 0.75f*goal)
    	{
    		Debug.Log("Success: green");
    		scoreFire = flames.transform.Find("Green").gameObject.GetComponent<ParticleSystem>();
    		successScore = 1;
    	} else if (0.75f*goal < score && score < 1.0f*goal) 
    	{
    		Debug.Log("Success: yellow");
    		scoreFire = flames.transform.Find("Yellow").gameObject.GetComponent<ParticleSystem>();
    		successScore = 1;
    	} else if (1.0f*goal < score && score < 1.5f*goal)
    	{
    		Debug.Log("Success: orange");    		
    		scoreFire = flames.transform.Find("Orange").gameObject.GetComponent<ParticleSystem>();
    		successScore = 2;
    	} else if (1.5f*goal < score) 
    	{
    		Debug.Log("Success: blue");    		
    		scoreFire = flames.transform.Find("Blue").gameObject.GetComponent<ParticleSystem>();
    		successScore = 2;
    	}

    	if (scoreFire != null)
    	{
    		flames.SetActive(true);
    		scoreFire.gameObject.SetActive(true);
    		scoreFire.Play();
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

    IEnumerator FinishingAnimations()
    {
    	Debug.Log("FinishingAnimations called");
    	if (magicCircle.activeSelf == false)
    	{
    		magicCircle.SetActive(true);
    	}
    	playerAnim.Play("Base Layer.Attack");
    	yield return new WaitForSeconds(2);						//Waiting for animation length
    	explosion.SetActive(true);
    	explosion.GetComponent<ParticleSystem>().Play();
    	Debug.Log("Explosion plays");
    	yield return new WaitForSeconds(2);						//Waiting for particle system to run to make explosion
    	explosion.GetComponent<ParticleSystem>().Stop();
    	Debug.Log("Explosion stops");
    	explosion.SetActive(false);
    	FinalScoring();
    	yield return new WaitForSeconds(3);   					//Waiting for particle system to run to make fire
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

    	if (lvlState==LevelState.Playing && e.type==EventType.KeyUp)
    	{
    		if (e.keyCode == targetKey)
    		{
    			Debug.Log("target key is pressed");
    			score += scoreModifier;
    			Debug.Log("New score is " + score);
    			isLookingUp = true;
    			LookUp();
    		}
    	}
    }

    void LookUp()
    {
       	if (playerAnim != null && isLookingUp == true)
        {
        	if (lookUpHappened == false)
        	{
	       		playerAnim.Play("Base Layer.LookUp");
	       		lookUpHappened = true;
	       		magicCircle.SetActive(true);        			
        	}
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
