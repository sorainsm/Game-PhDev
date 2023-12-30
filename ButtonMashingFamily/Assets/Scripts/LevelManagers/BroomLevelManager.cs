using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BroomLevelManager : LevelManager
{
	//Variables for the game
	int goal;
	float score;
	float scoreModifier;

	KeyCode target1;
	KeyCode target2;
    HashSet<KeyCode> currentKeys = new HashSet<KeyCode>();

	public GameObject playerParent;
    public GameObject player;
	Animator playerAnim;

	public GameObject magicCircle;
	public GameObject effectParent;
	public GameObject targetParent;
	Transform targetObject;

    bool isCharging = false;
    bool chargingHappened = false;
    bool isMoving = false;

	public AudioSource fail;
    public AudioSource success;
    public AudioSource superSuccess;

    int successScore;
    int effectScore;

    MIBMScore scoreOut = new MIBMScore();


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Broom::Setup run");
        Setup();
        Initialize();

        playerAnim = player.GetComponent<Animator>();
    }

    void Initialize()
    {
        MIBMConfig config = new MIBMConfig();

        MIBMConfig tmpConfig = (MIBMConfig)MinigameManager.Instance.GetCurrentConfig();
        if (tmpConfig != null)
        {
            config = tmpConfig;
        } else 
        {
            Debug.Log("No config; defaults are being used");
        }

        maxGameTime = config.maxGameTime > 0 ? config.maxGameTime : Default(10f,"maxGameTime");
        goal = config.goal > 0 ? config.goal : Default(60, "goal");
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
        StartCoroutine(FinishingAnimations());
        if (score < 0.5f*goal)
        {
            outro = "Not quite...\n";
        } else if (0.5f*goal < score && score < 0.75f*goal)
        {
            outro = "We have lift off! \n So close!\n";
        } else if (0.75f*goal <= score && score < 1.0f*goal)
        {
            outro = "In the clouds! \n Good job!\n";
        } else if (1.0f*goal <= score && score < 1.25f*goal)
        {
            outro = "Above the clouds! \n A great cast!\n";
        } else if (1.25f*goal <= score && score < 1.5*goal)
        {
            outro = "Almost in orbit! \n Amazing cast!\n";
        } else if (1.5f*goal <= score)
        {
            outro = "Out of this world! \n A perfect cast!\n";            
        }
        outro += "[ENTER to continue]";
        outroText.GetComponent<TMPro.TextMeshProUGUI>().text = outro;
        ResizeOutroSize(GetRect(outroParent));        
        EndLevel(8f);
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
            if (e.keyCode == target1 || e.keyCode == target2)
            {
                if (e.type == EventType.KeyDown)
                {
                    currentKeys.Add(e.keyCode);
                } else if (e.type == EventType.KeyUp)
                {
                    currentKeys.Remove(e.keyCode);
                }

                if (currentKeys.Count == 2 && currentKeys.Contains(target1) && currentKeys.Contains(target2))
                {
                    Debug.Log("Broom::Both keys pressed");
                    score += scoreModifier;
                    Debug.Log("Broom::New score is " + score);

                    if (isCharging == false)
                    {
                         isCharging = true;
                         Charging();
                    }
                }               
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

    void Charging()
    {
        if (playerAnim != null && isCharging == true)
        {
            if (chargingHappened == false)
            {
                   Debug.Log("Broom::First Press, starting animations.");
                   playerAnim.SetBool("isPressing", true);
                   chargingHappened = true;
                   magicCircle.SetActive(true);
            }
        }
    }

    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(playerParent.transform.position,targetObject.position) > 0.001f)
        {
            Debug.Log("Broom::isMoving is true. Moving from " + playerParent.transform.position + " to " + targetObject.position);
            playerParent.transform.position = Vector3.MoveTowards(playerParent.transform.position,targetObject.position,1*Time.deltaTime);            
            yield return null;
        }

       	Debug.Log("Broom::Distance is approximately equal. Setting isMoving to false");
       	isMoving = false;
    }

    public void Moving()
    {
        if (isMoving)
        {
            StartCoroutine(MoveToTarget());
        } else 
        {
            return;
        }
    }

    IEnumerator FinishingAnimations()
    {
    	Debug.Log("Broom::FinishingAnimations called");
    	if (magicCircle.activeSelf == true)
    	{
    		magicCircle.SetActive(false);
    	}
    	Debug.Log("Broom::Check if player succeeds");
        Scoring();
        Debug.Log("Broom::Success score is " + successScore);
        var finalEffect = FinalEffect(effectParent,effectScore);
        var ps = finalEffect.GetComponent<ParticleSystem>();
        Debug.Log("Broom::Final effect is " + finalEffect.name);

        switch(successScore)
        {
        	case 0:
        		targetObject = null;
        		break;
        	case 1:
        		targetObject = targetParent.transform.Find("MediocreTarget");
        		break;
        	case 2:
        		targetObject = targetParent.transform.Find("NeutralTarget");        	
        		break;
        	case 3:
        		targetObject = targetParent.transform.Find("GoodTarget");        	
        		break;
        	case 4:
        		targetObject = targetParent.transform.Find("BetterTarget");        	
        		break;
        	case 5:
        		targetObject = targetParent.transform.Find("ExcellentTarget");        	
        		break;
        }


        if (ps != null)
        {
            switch(successScore)
            {
                //Fail
                case 0:
                    Debug.Log("Broom::Player Fails");
                    Debug.Log("Broom::Player takes off");
                    playerAnim.SetBool("timesUp",true);
                    yield return new WaitForSeconds(0.25f);                
                    Debug.Log("Broom::Player Falls");
                    playerAnim.SetBool("isFail", true);
                    isMoving = false;
                    finalEffect.SetActive(true);
                    ps.Play();
                    fail.Play();
                    break;
                //Mediocre
                case 1:
                    Debug.Log("Broom::Player Mediocre");
                    finalEffect.SetActive(true);
                    ps.Play();
                    yield return new WaitForSeconds(1f);
                    Debug.Log("Broom::Player takes off");
                    playerAnim.SetBool("timesUp",true);
                    yield return new WaitForSeconds(0.25f);                    
                    Debug.Log("Broom::Player Zooms");
                    playerAnim.SetBool("isSuccess",true);
                    isMoving = true;
                    yield return new WaitForSeconds(3);
                    fail.Play();
                    break;
                //Neutral
                case 2:
                    Debug.Log("Broom::Player Neutral");                    
                    finalEffect.SetActive(true);
                    ps.Play();
                    yield return new WaitForSeconds(1f);
                    Debug.Log("Broom::Player takes off");
                    playerAnim.SetBool("timesUp",true);
                    yield return new WaitForSeconds(0.25f);                       
                    Debug.Log("Broom::Player Zooms");
                    playerAnim.SetBool("isSuccess",true);
                    isMoving = true;
                    yield return new WaitForSeconds(3);                               
                    success.Play();                    
                    break;          
                //Good
                case 3:
                    Debug.Log("Broom::Player Good");
                    finalEffect.SetActive(true);
                    ps.Play();
                    yield return new WaitForSeconds(1f);
                    Debug.Log("Broom::Player takes off");
                    playerAnim.SetBool("timesUp",true);
                    yield return new WaitForSeconds(0.25f);                       
                    Debug.Log("Broom::Player Zooms");
                    playerAnim.SetBool("isSuccess",true);
                    isMoving = true;
                    yield return new WaitForSeconds(3);                                  
                    success.Play();                       
                    break;          
                //Better
                case 4:
                    Debug.Log("Broom::Player Better");
                    finalEffect.SetActive(true);
                    ps.Play();
                    yield return new WaitForSeconds(1f);
                    Debug.Log("Broom::Player takes off");
                    playerAnim.SetBool("timesUp",true);
                    yield return new WaitForSeconds(0.25f);                       
                    Debug.Log("Broom::Player Zooms");
                    playerAnim.SetBool("isSuccess",true);
                    isMoving = true;
                    yield return new WaitForSeconds(3);                    
                    superSuccess.Play();                       
                    break;          
                //Excellent
                case 5:
                    Debug.Log("Broom::Player Excellent");
                    finalEffect.SetActive(true);
                    ps.Play();
                    yield return new WaitForSeconds(1f);
                    Debug.Log("Broom::Player takes off");
                    playerAnim.SetBool("timesUp",true);
                    yield return new WaitForSeconds(0.25f);                       
                    Debug.Log("Broom::Player Zooms");
                    playerAnim.SetBool("isSuccess",true);
                    isMoving = true;          
                    yield return new WaitForSeconds(3);              
                    superSuccess.Play();                            
                    break;          
            }                  
        }
    }

    void Scoring()
    {
    	Debug.Log("Broom::Scoring called. Score is " + score + ". Goal is " + goal);
    	if (score < 0.5*goal)
    	{
    		Debug.Log("Broom::Fail");
    		successScore = 0;
    		effectScore = 0;
    	} else if (0.5f*goal <= score && score < 0.75f*goal)
    	{
    		Debug.Log("Broom::Mediocre");
    		successScore = 1;
    		effectScore = 1;
    	} else if (0.75f*goal <= score && score < 1.0f*goal)
    	{
    		Debug.Log("Broom::Neutral");
    		successScore = 2;
    		effectScore = 2;    	
    	} else if (1f*goal <= score && score < 1.25f*goal)
    	{
    		Debug.Log("Broom::Good");
    		successScore = 3;
    		effectScore = 3;    	
    	} else if (1.25f*goal <= score && score <1.5f*goal)
    	{
    		Debug.Log("Broom::Better");
    		successScore = 4;
    		effectScore = 4;    	
    	} else if (1.5f*goal <= score)
    	{
    		Debug.Log("Broom::Excellent");
    		successScore = 5;
    		effectScore = 5;    	
    	}
    }


    GameObject FinalEffect(GameObject effectParent, int effect)
    {
    	Debug.Log("Broom::FinalEffect called with effectParent " + effectParent + ", and effectScore " + effect);
    	Transform final;
    	switch(effect)
    	{
    		case 0:
    			final = effectParent.transform.Find("Fail-GroundHit");
    			break;
    		case 1:
    			final = effectParent.transform.Find("Mediocre-SpiralThin");
    			break;
    		case 2:
    			final = effectParent.transform.Find("Neutral-Spiral");
    			break;    		
    		case 3:
    			final = effectParent.transform.Find("Good-SpiralFire");
    			break;    		
    		case 4:
    			final = effectParent.transform.Find("Better-SprialIce");
    			break;    		
    		case 5:
    			final = effectParent.transform.Find("Excellent-Flash");
    			break;
            default:
                final = effectParent.transform.Find("Fail-GroundHit");
                break;    		
    	}

    	return final.gameObject;
    }


    IEnumerator StartAnimations()
    {
        Debug.Log("Starting Animations called");
        playerAnim.SetBool("flyStart",true);
 		yield return new WaitForSeconds(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (lvlState==LevelState.Countdown)
        {
            StartCoroutine(StartAnimations());
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

        Moving();
         
    }
}
