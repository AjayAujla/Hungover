using UnityEngine;
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

    private int embarrassment = 0;
    private EmbarrassmentMeter embarrassmentMeter;
    private bool insideEnemyFieldOfView;

	private PlayerStats playerStats;

    private float allowCooldownTime = 1.0f;
    private float cooldownTimer;

    private GameObject[] enemyList;
    private BaseEnemy enemyScript;

    public bool isInsideEnemyFieldOfView()
    {
        return this.insideEnemyFieldOfView;
    }

    public void setInsideEnemyFieldOfView(bool insideEnemyFieldOfView)
    {
        this.insideEnemyFieldOfView = insideEnemyFieldOfView;
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
    }

    void Update()
    {
        MoveCharacter();

        this.CooldownEmbarrassment();
        this.embarrassmentMeter.setEmbarrassmentMeterValue(this.embarrassment);
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

    void OnTriggerEnter2D(Collider2D other)
    {
		if (other.gameObject.layer == 9) // layer 9 is "Item"
		{   
		
		
			// if it's a clothe (Boxers, Pants, Shirt, or Shoes), reskin player
			if(other.gameObject.tag != "Phone" && other.gameObject.tag != "Wallet" && other.gameObject.tag != "Can") {
				PlayerReskin.ChangeSprite(other.gameObject.tag);
			}

	        // find the corresponding item UI (top-right corner)
	        GameObject clothe = GameObject.Find(other.gameObject.tag);
	        if (clothe)
	        {
	            clothe.GetComponent<Image>().color = Color.white;   // set the item UI opacity to 1 (opaque)
	            Destroy(other.gameObject);  // remove the item object from the map
	        }
			
			// else, check if it is a beer can
			if(other.gameObject.tag == "Can") {
				playerStats.incrementBeerCans();
				playerStats.incrementCash(0.05f);
				Destroy(other.gameObject);
			}
		
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
        if (detectionRange == DetectionRange.redZone)
        {
            this.insideEnemyFieldOfView = true;
            if (this.embarrassment < this.embarrassmentMeter.getMaximumEmbarrassmentValue())
            {
                this.embarrassment += 4;
            }
            else
            {
                this.embarrassment = (int)this.embarrassmentMeter.getMaximumEmbarrassmentValue();
            }
        }
        else if (detectionRange == DetectionRange.yellowZone)
        {
            this.insideEnemyFieldOfView = true;
            if (this.embarrassment < this.embarrassmentMeter.getMaximumEmbarrassmentValue())
            {
                this.embarrassment += 2;
            }
            else
            {
                this.embarrassment = (int)this.embarrassmentMeter.getMaximumEmbarrassmentValue();
            }
        }
        else if (detectionRange == DetectionRange.greenZone)
        {
            if (this.embarrassment > this.embarrassmentMeter.getMinimumEmbarrassmentValue())
            {
                this.embarrassment -= 1;
            }
            else
            {
                this.embarrassment = (int)this.embarrassmentMeter.getMinimumEmbarrassmentValue();
            }
        }
    }
}