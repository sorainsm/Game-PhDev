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

	KeyCode targetKey;

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

    SIBMScore scoreOut = new SIBMScore();


    // Start is called before the first frame update
    void Start()
    {
     	Debug.Log("Campfire::Setup run");
     	Setup();
        Initialize();     	
        //Make sure all the effects are off
     	flames.SetActive(false);
     	magicCircle.SetActive(false);
     	explosion.SetActive(false);

     	playerAnim = player.GetComponent<Animator>();
     	playerAnim.Play("Base Layer.Idle");
    }

    void Initialize()
    {
        SIBMConfig config = new SIBMConfig();

        SIBMConfig tmpConfig = (SIBMConfig)MinigameManager.Instance.GetCurrentConfig();
        if (tmpConfig != null)
        {
            config = tmpConfig;
        } else 
        {
            Debug.Log("No config; defaults are being used");
        }

        maxGameTime = config.maxGameTime > 0 ? config.maxGameTime : Default(10f,"maxGameTime");
        goal = config.goal > 0 ? config.goal : Default(60, "goal");
        targetKey = config.targetKey != null ? (KeyCode) System.Enum.Parse(typeof(KeyCode), config.targetKey, true) : Default(KeyCode.Space, "targetKey");
        scoreModifier = config.scoreModifier > 0 ? config.scoreModifier : Default(1f,"scoreModifier");

        scoreOut.minigame = config.minigame;
        scoreOut.scene = config.scene;
        scoreOut.maxGameTime = maxGameTime;
        scoreOut.goal = goal;
        scoreOut.targetKey = targetKey.ToString();
        scoreOut.scoreModifier = scoreModifier;
    }


    void EndGame()
    {
    	scoreOut.score = score;
        string outro = "good job";
    	StartCoroutine(FinishingAnimations());
        if (score < 0.5f*goal)
        {
            outro = "Not quite...\n";
        } else if (0.5f*goal <= score && score < 0.75f*goal)
        {
            outro = "Green! \n So close!\n";
        } else if (0.75f*goal <= score && score < 1.0f*goal)
        {
            outro = "Yellow! \n Good job!\n";
        } else if (1.0f*goal <= score && score < 1.25f*goal)
        {
            outro = "Orange! \n A great cast!\n";
        } else if (1.25f*goal <= score && score < 1.5*goal)
        {
            outro = "Blue! \n Amazing cast!\n";
        } else if (1.5f*goal <= score)
        {
            outro = "White! \n A perfect cast!\n";            
        }
        outro += "[ENTER to continue]";
    	outroText.GetComponent<TMPro.TextMeshProUGUI>().text = outro;
        ResizeOutroSize(GetRect(outroParent));
    	EndLevel(10f);
    }

    void FinalScoring()
    {
    	ParticleSystem scoreFire = null;
    	int successScore = 0;

    	Debug.Log("Campfire::FinalScoring called. Score is " + score + ". Goal is " + goal);
		if (score < 0.5f*goal) 
		{
			Debug.Log("Campfire::Fail");
			scoreFire = null;
			successScore = 0;
    	} else if (0.5f*goal <= score && score < 0.75f*goal)
    	{
    		Debug.Log("Campfire::Success: green");
    		scoreFire = flames.transform.Find("Green").gameObject.GetComponent<ParticleSystem>();
    		successScore = 1;
    	} else if (0.75f*goal <= score && score < 1.0f*goal) 
    	{
    		Debug.Log("Campfire::Success: yellow");
    		scoreFire = flames.transform.Find("Yellow").gameObject.GetComponent<ParticleSystem>();
    		successScore = 1;
    	} else if (1.0f*goal <= score && score < 1.25f*goal)
    	{
    		Debug.Log("Campfire::Success: orange");    		
    		scoreFire = flames.transform.Find("Orange").gameObject.GetComponent<ParticleSystem>();
    		successScore = 1;
    	} else if (1.25f*goal <= score && score < 1.5f*goal) 
    	{
    		Debug.Log("Campfire::Success: blue");    		
    		scoreFire = flames.transform.Find("Blue").gameObject.GetComponent<ParticleSystem>();
    		successScore = 2;
    	} else if (1.5f*goal <= score)
        {
            Debug.Log("Campfire::Success: white");         
            scoreFire = flames.transform.Find("White").gameObject.GetComponent<ParticleSystem>();
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
    	playerAnim.SetTrigger("attack");
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
    		if ((e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) && instructionCount < instructions.Length)
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

        if (lvlState == LevelState.End && e.type==EventType.KeyUp)
        {
            if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
            {
                MinigameManager.Instance.LoadNextScene();                
            }
        }
    }

    void LookUp()
    {
       	if (playerAnim != null && isLookingUp == true)
        {
        	if (lookUpHappened == false)
        	{
	       		playerAnim.SetBool("isLookUp",true);
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
                MinigameManager.Instance.AddScore(scoreOut); 
        		return;
        	}
        }
    }


}
