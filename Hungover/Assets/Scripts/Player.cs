﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
	public enum DetectionRange { greenZone, yellowZone, redZone };
	
	/********************************************************************
	 * 
	 *	Animation States:
	 *	1 = Walk Up			5 = Run Up			9 = Dance Move 1
	 *	2 = Walk Right		6 = Run Right		10 = Dance move 2
	 *	3 = Walk Down		7 = Run Down		11 = Dance move 3
	 *	4 = Walk Left		8 = Run Left		12 = Squat (Coming Soon)
	 *
	 ********************************************************************/
	
	[SerializeField]
	public float speed;
	
	private Animator mAnimator;
	private AudioSource mAudioSource;
	private float footStepsPitch = 1.0f;
	
	private float embarrassment = 0.0f;
	private EmbarrassmentMeter embarrassmentMeter;
	private bool insideEnemyFieldOfView;
    [SerializeField]
    [Range(1, 5)]
    private int redZoneEmbarrassmentIncrement = 4;
    [SerializeField]
    [Range(1, 5)]
    private int yellowZoneEmbarrassmentIncrement = 2;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float cooldownEmbarrassmentDecrement = 0.5f;

	private float allowCooldownTime = 1.0f;
	private float cooldownTimer;
	
	private GameObject[] enemyList;
	private BaseEnemy enemyScript;
	AudioSource AlarmSound;

    private PlayerStats playerStats;

    [SerializeField]
    private GameObject actionButtonE;
    private GameObject actionButtonInstance;

	private bool foundBoxers = false;
	private bool foundPants = false;
    private bool foundShirt = false;
    private bool foundShoes = false;
	private bool foundPhone = false;
    private bool hidden = false;
    private bool isFullyClothed = false;

    private GameObject objecthiddenInEnvironmentIn;

	// Ewwwww sound when player dies
	EwwManager ewwManager;
	
    private PlayerReskin playerReskinScript;
    private HeadsUpDisplay headsUpDisplayScript;

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public bool isInsideEnemyFieldOfView()
	{
		return this.insideEnemyFieldOfView;
	}
	
	public void setInsideEnemyFieldOfView(bool insideEnemyFieldOfView)
	{
		this.insideEnemyFieldOfView = insideEnemyFieldOfView;
	}

    public bool ishiddenInEnvironment()
    {
        return this.hidden;
    }
	
    void Awake()
    {
        if (GameObject.FindObjectsOfType(GetType()).Length > 1)
        {
            //Destroy(this.gameObject);
        }
    }

	void Start()
	{
		mAnimator = GetComponent<Animator>();
		mAudioSource = GetComponent<AudioSource>();
		this.embarrassmentMeter = GameObject.Find("EmbarassmentMeter").GetComponent<EmbarrassmentMeter>();
		playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
		this.cooldownTimer = this.allowCooldownTime;
		
		this.enemyList = GameObject.FindGameObjectsWithTag("Enemy");
		this.enemyScript = this.enemyList[0].GetComponent<BaseEnemy>();
		AlarmSound = (AudioSource)GameObject.Find("AlarmSound").GetComponent<AudioSource>();

        this.playerReskinScript = this.GetComponent<PlayerReskin>();
		
		ewwManager = GameObject.Find ("EwwManager").GetComponent<EwwManager>();
	
        this.headsUpDisplayScript = GameObject.Find("HeadsUpDisplay").GetComponent<HeadsUpDisplay>();


        //this.actionButtonE = GameObject.Find("ActionButtonE");
    }

    void Update()
    {
        MoveCharacter();

        this.CooldownEmbarrassment();
        this.embarrassmentMeter.setEmbarrassmentMeterValue(this.embarrassment);

        if (this.embarrassment >= this.embarrassmentMeter.getMaximumEmbarrassmentValue())
        {
            this.PlayerDieOfEmbarrassment();
        }
    }

    public void PlayerDieOfEmbarrassment()
    {
		Utils.Print("YOU DIED OF EMBARRASSMENT");
		ewwManager.Ewwwwwwww();
		
		this.headsUpDisplayScript.PlayerDieOfEmbarrassment();
		
		//Application.LoadLevel(Application.loadedLevel);
		//GameObject.Find("GameManager").GetComponent<BoardManager>().SetupScene(1);

    }

    private void CooldownEmbarrassment()
	{
		bool canCooldown = false;
		
		foreach (GameObject enemy in this.enemyList)
		{
			if (!(enemyScript.PlayerInsideFieldOfView() && enemyScript.PlayerInLineOfSight()))
			{
				canCooldown = true;
			}
			else
			{
				break;
			}
		}
		
		this.cooldownTimer -= Time.deltaTime;
		//if (this.cooldownTimer <= 0.0f)
		if (canCooldown)
		{
			this.updateEmbarrassment(DetectionRange.greenZone);
			this.cooldownTimer = this.allowCooldownTime;
		}
	}

    private float absoluteReduction = 0.0f;

    void OnTriggerEnter2D(Collider2D other)
	{
        if (other.gameObject.layer == 9) // layer 9 is "Item"
		{   
			// if it's a clothe (Boxers, Pants, Shirt, or Shoes), reskin player
			if(other.gameObject.tag != "Phone" && other.gameObject.tag != "Wallet" && other.gameObject.tag != "Can") {
				PlayerReskin.ChangeSprite(other.gameObject.tag);
			}

            if (other.gameObject.tag == "Wallet")
            {
                float cashInWallet = Random.Range(0.0f, 5.0f);
                if(cashInWallet > 1.0f)
                {
                    //play cha-ching sound when you get a lot of money
                }
                this.playerStats.incrementCash(cashInWallet);
            }

            // find the corresponding item UI (top-right corner)
            GameObject clothe = GameObject.Find(other.gameObject.tag);
			if (clothe)
			{
				clothe.GetComponent<Image>().color = Color.white;   // set the item UI opacity to 1 (opaque)
                if (other.gameObject.tag == "Boxers")
                {
                    this.foundBoxers = true;
                    if (!this.foundPants)
                    {
                        //this.absoluteReduction += this.yellowZoneEmbarrassmentIncrement * 4.0f / 10.0f; //0.4
                    }
                    playerStats.incrementExperience(15);
                }
                else if (other.gameObject.tag == "Pants")
                {
                    this.foundPants = true;
                    //this.absoluteReduction += this.yellowZoneEmbarrassmentIncrement * 9.0f / 10.0f / 2.0f; //0.9
                    playerStats.incrementExperience(30);
                }
                else if (other.gameObject.tag == "Shirt")
				{
					this.foundShirt = true;
                    //this.absoluteReduction += this.yellowZoneEmbarrassmentIncrement * 7.0f / 10.0f / 2.0f; //0.7
                    playerStats.incrementExperience(30);
				}
				else if (other.gameObject.tag == "Shoes")
				{
					this.foundShoes = true;
                    //this.absoluteReduction += this.yellowZoneEmbarrassmentIncrement * 4.0f / 10.0f / 2.0f; //0.4
                    playerStats.incrementExperience(30);
				}
				else if (other.gameObject.tag == "Phone")
				{
					this.foundPhone = true;
					//this.absoluteReduction += this.yellowZoneEmbarrassmentIncrement * 4.0f / 10.0f / 2.0f; //0.4
					playerStats.incrementExperience(15);
				}
				else
				{
					playerStats.incrementExperience(15);
				}
				Destroy(other.gameObject);  // remove the item object from the map

                if (this.foundShirt && this.foundPants && this.foundShoes)
                {
                    this.isFullyClothed = true;
                }
            }
			
			// else, check if it is a beer can
			if(other.gameObject.tag == "Can") {
				playerStats.incrementBeerCans();
				playerStats.incrementCash(0.05f);
				Destroy(other.gameObject);
			}
		}

		if (other.gameObject.tag == "Alarm")
		{
			if (this.actionButtonInstance == null && !AlarmSound.isPlaying)
			{
				this.actionButtonInstance = (GameObject)Instantiate(this.actionButtonE, new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 1.0f, other.gameObject.transform.position.z), Quaternion.identity);
			}
		}

        if (other.gameObject.tag == "Exit")
        {
            if (this.foundShirt && this.foundPants && this.foundShoes)
            {
				// if already last level... reload it, else load next level
				int nextLevel = Application.loadedLevel + 1 == Application.levelCount ? Application.loadedLevel : Application.loadedLevel + 1;
                this.playerStats.checkForUpgrades();
                if (this.foundPhone) {
					// set next level in PlayerPrefs, and load the slideshow
					PlayerPrefs.SetInt("NextLevel", nextLevel);
					Application.LoadLevel("Slideshow");
				} else {
                	Utils.Print("VICTORY");
					Application.LoadLevel(nextLevel);
				}
            }
            else
            {
                Utils.Print("Still missing some clothes.");
            }
        }
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Alarm")
		{
			if (Input.GetKeyDown(KeyCode.E) && !AlarmSound.isPlaying) {
				AlarmSound.Play();
				Physics2D.IgnoreLayerCollision(11, 12);	// ignores collision between enemies and tables
			}
		}

	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Alarm")
		{
				Destroy(this.actionButtonInstance);
		}
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Table")
        {
            if (this.actionButtonInstance == null)
            {
                this.actionButtonInstance = (GameObject)Instantiate(this.actionButtonE, new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 1.0f, other.gameObject.transform.position.z), Quaternion.identity);
            }
        }

    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Table")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                this.objecthiddenInEnvironmentIn = other.gameObject;
                this.transform.position = new Vector3 (other.transform.position.x, other.transform.position.y - 0.25f, other.transform.position.z);
                this.GetComponent<Animator>().enabled = false;
                this.playerReskinScript.UnderTable();
                this.hidden = true;
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), other.gameObject.GetComponent<Collider2D>(), true);
                Destroy(this.actionButtonInstance);
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Table")
        {
            Destroy(this.actionButtonInstance);
        }
    }

    void MoveCharacter()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		
		Vector2 direction = new Vector2(h, v);
		transform.Translate(direction * speed * Time.deltaTime);
		
		if (direction != Vector2.zero)
		{
			if (!mAudioSource.isPlaying)
			{
				if (Input.GetButton("Run"))
				{
					footStepsPitch = 0.8f;
				}
				else
				{
					footStepsPitch = 0.4f;
				}
				mAudioSource.pitch = footStepsPitch;
				mAudioSource.Play();
			}

            if (this.hidden)
            {
                this.GetComponent<Animator>().enabled = true;
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), this.objecthiddenInEnvironmentIn.GetComponent<Collider2D>(), false);
                Destroy(this.actionButtonInstance);
                this.hidden = false;
            }
        }
		else
		{
			mAudioSource.Stop();
		}

        SetAnimation(direction);
    }

    void SetAnimation(Vector2 direction)
	{
		int animationIdx = 0;
		
		if (direction.y > 0.0f) animationIdx = 1;       // Up
		else if (direction.x > 0.0f) animationIdx = 2;  // Right
		else if (direction.y < 0.0f) animationIdx = 3;  // Down
		else if (direction.x < 0.0f) animationIdx = 4;  // Left
		else { mAnimator.Play("Character_Idle"); }
		
		if (direction != Vector2.zero && Input.GetButton("Run"))
		{
			animationIdx += 4;  // Will be in Run range
			speed = 3;
		}
		else
		{
			speed = 2;
		}
		
		mAnimator.SetInteger("move_direction", animationIdx);
	}
	
	public void updateEmbarrassment(DetectionRange detectionRange)
	{
        if (detectionRange == DetectionRange.greenZone/* || (this.foundShirt && this.foundShirt && this.foundShoes)*/)
        {
            if (this.embarrassment > this.embarrassmentMeter.getMinimumEmbarrassmentValue())
            {
                this.embarrassment -= this.cooldownEmbarrassmentDecrement;
            }
            else
            {
                this.embarrassment = (int)this.embarrassmentMeter.getMinimumEmbarrassmentValue();
            }
        }
        else if (detectionRange == DetectionRange.redZone && !this.hidden && !this.isFullyClothed)
		{
			this.insideEnemyFieldOfView = true;
			if (this.embarrassment < this.embarrassmentMeter.getMaximumEmbarrassmentValue())
			{
                this.embarrassment += this.redZoneEmbarrassmentIncrement - this.absoluteReduction;
				ewwManager.Play();
            }
			else
			{
				this.embarrassment = (int)this.embarrassmentMeter.getMaximumEmbarrassmentValue();
			}
		}
		else if (detectionRange == DetectionRange.yellowZone && !this.hidden && !this.isFullyClothed)
		{
			this.insideEnemyFieldOfView = true;
			if (this.embarrassment < this.embarrassmentMeter.getMaximumEmbarrassmentValue())
			{
				this.embarrassment += this.yellowZoneEmbarrassmentIncrement - this.absoluteReduction;
				ewwManager.Play();
			}
			else
			{
				this.embarrassment = (int)this.embarrassmentMeter.getMaximumEmbarrassmentValue();
			}
		}
	}
}