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
    KeyCode target1;
    KeyCode target2;

    public GameObject cauldron;
    public GameObject smoke;
    public GameObject bubbles;

    public GameObject player;
    Animator playerAnim;
    KeyCode nextKey;

    public AudioSource fail;
    public AudioSource success;
    public AudioSource superSucess;

    AIBMScore scoreOut = new AIBMScore();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Cauldron::Setup run");
        Setup();
        Initialize();

        nextKey = target1; 
        Debug.Log("Cauldron::nextKey is " + nextKey);


        playerAnim = player.GetComponent<Animator>();
        playerAnim.Play("Base Layer.Idle");

    }

    void Initialize()
    {
        AIBMConfig config = new AIBMConfig();

        AIBMConfig tmpConfig = (AIBMConfig)MinigameManager.Instance.GetCurrentConfig();
        if (tmpConfig != null)
        {
            config = tmpConfig;
        } else 
        {
            Debug.Log("No config; defaults are being used");
        }

        maxGameTime = config.maxGameTime > 0 ? config.maxGameTime : Default(10f,"maxGameTime");
        goal = config.goal > 0 ? config.goal : Default(45, "goal");
        target1 = config.targetKey != null ? (KeyCode) System.Enum.Parse(typeof(KeyCode), config.targetKey, true)  : Default(KeyCode.RightArrow, "targetKey");
        target2 = config.targetKey2 != null ? (KeyCode) System.Enum.Parse(typeof(KeyCode), config.targetKey2, true)  : Default(KeyCode.LeftArrow, "targetKey");
        scoreModifier = config.scoreModifier > 0 ? config.scoreModifier : Default(1f,"scoreModifier");

        scoreOut.minigame = config.minigame;
        scoreOut.scene = config.scene;
        scoreOut.maxGameTime = maxGameTime;
        scoreOut.goal = goal;
        scoreOut.targetKey = target1.ToString();
        scoreOut.targetKey2 = target2.ToString();
        scoreOut.scoreModifier = scoreModifier;
    }

    void EndGame()
    {
        scoreOut.score = score;
        string outro = "good job";
        StartCoroutine(FinalAnimations());
        FinalScoring();
        if (score < 0.5f*goal)
        {
            outro = "Are those ghosts?! \n Not quite what we wanted...\n Rank: F\n";
        } else if (0.5f*goal <= score && score < 0.75f*goal)
        {
            outro = "A poison cloud! \n So close, but so far!\n Rank: D\n";
        } else if (0.75f*goal <= score && score < 1.0f*goal)
        {
            outro = "Purple smoke! \n Good progress!\n Rank: C\n";
        } else if (1.0f*goal <= score && score < 1.25f*goal)
        {
            outro = "A soft glow! \n A great cast!\n Rank: B\n";
        } else if (1.25f*goal <= score && score < 1.5*goal)
        {
            outro = "Crackling with power! \n Amazing cast!\n Rank: A\n";
        } else if (1.5f*goal <= score)
        {
            outro = "Overflowing with power!! \n A perfect cast!\n Rank: S\n";            
        }
        outro += "[ENTER to continue]";        
        outroText.GetComponent<TMPro.TextMeshProUGUI>().text = outro;
        ResizeOutroSize(GetRect(outroParent));
        EndLevel(5f);
    }

    IEnumerator FinalAnimations()
    {
        playerAnim.SetTrigger("stirIdle");
        yield return new WaitForSeconds(5f);
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
                Debug.Log("Cauldron::Correct key in sequence");
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
                   //Score increases at the end of successful sequence
                   score += scoreModifier;
                   Debug.Log("New score is " + score);
                }
             }
        }
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

        if (lvlState==LevelState.Playing && e.isKey)
        {
            EventType t = e.type;
            KeyCode pressedKey = e.keyCode;
            HalfStir(t, pressedKey);
        }

        if (lvlState == LevelState.End && e.type==EventType.KeyUp && !MinigameManager.Instance.GetPractice())
        {
            if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
            {
                MinigameManager.Instance.LoadNextScene();                
            }
        }    
    }

    void FinalScoring()
    {
    	Debug.Log("Cauldron::FinalScoring called.");
    	int successScore = 0;
        int effectScore = 0;

    	Debug.Log("Cauldron::Score is " + score + ". Goal is " + goal);
		if (score < 0.5f*goal) 
		{
			Debug.Log("Cauldron::Fail: Abyssmal");
			successScore = 0;
            effectScore = 0;
    	} else if (0.5f*goal <= score && score < 0.75f*goal)
    	{
    		Debug.Log("Cauldron::Fail: Bad");
            effectScore = 1;
    		successScore = 1;
    	} else if (0.75f*goal <= score && score < 1.0f*goal) 
    	{
    		Debug.Log("Cauldron::Success: Average-");
            effectScore = 2;
    		successScore = 1;
    	} else if (1.0f*goal <= score && score < 1.25f*goal)
    	{
    		Debug.Log("Cauldron::Success: Average+");    		
            effectScore = 3;
    		successScore = 2;
    	} else if (1.25f*goal <= score && score < 1.5f*goal) 
    	{
    		Debug.Log("Cauldron::Success: Good");    		
            effectScore = 4;
    		successScore = 2;
    	} else if (1.5f*goal <= score)
        {
            Debug.Log("Cauldron::Success: Excellent");         
            effectScore = 5;
            successScore = 2;
        }

        var finalEffect = FinalEffect(smoke, effectScore);
        Debug.Log("Cauldron::Final effect is " + finalEffect.name);
        var ps = finalEffect.GetComponent<ParticleSystem>();
 
    	if (ps != null)
    	{
	   		finalEffect.SetActive(true);
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

    GameObject FinalEffect(GameObject effectParent, int effect)
    {
        Debug.Log("Cauldron::FinalEffect Start");
        Transform final;
        switch(effect)
        {
            case 0:
                final = effectParent.transform.Find("Souls");
                break;
            case 1:
                final = effectParent.transform.Find("PoisonCloud");
                break;
            case 2:
                final = effectParent.transform.Find("PurpleSmoke");
                break;
            case 3:
                final = effectParent.transform.Find("Glow");
                break;
            case 4:
                final = effectParent.transform.Find("Electric");
                break;
            case 5:
                final = effectParent.transform.Find("Firewall");
                break;
            default:
                final = effectParent.transform.Find("Souls");
                break;
        }

        return final.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (lvlState==LevelState.Countdown)
        {
            playerAnim.SetTrigger("idle");
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
