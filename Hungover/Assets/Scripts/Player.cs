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

    private int embarrassment;
    private EmbarrassmentMeter embarrassmentMeter;

	private PlayerStats playerStats;

	void Start ()
    {
        mAnimator = GetComponent<Animator>();
		mAudioSource = GetComponent<AudioSource>();
        this.embarrassmentMeter = GameObject.Find("EmbarassmentMeter").GetComponent<EmbarrassmentMeter>();
		playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
    }

    void Update ()
    {
        MoveCharacter();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.layer == 9) {	// layer 9 is "Item"

			// if it's a clothe (Boxers, Pants, Shirt, or Shoes), reskin player
			if(other.gameObject.tag != "Phone" && other.gameObject.tag != "Wallet" && other.gameObject.tag != "Can") {
				PlayerReskin.ChangeSprite(other.gameObject.tag);
			}

			// find the corresponding item UI (top-right corner)
			GameObject clothe = GameObject.Find(other.gameObject.tag);
			if(clothe) {
				clothe.GetComponent<Image>().color = Color.white;	// set the item UI opacity to 1 (opaque)
				Destroy(other.gameObject);	// remove the item object from the map
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

		if(direction != Vector2.zero) {
			if(!mAudioSource.isPlaying) {
				if(Input.GetButton("Run")) {
					footStepsPitch = 0.8f;
				} else {
					footStepsPitch = 0.4f;
				}
				mAudioSource.pitch = footStepsPitch;
				mAudioSource.Play();
			}
		} else {
			mAudioSource.Stop();
		}

		SetAnimation(direction);
			   
    }

    void SetAnimation(Vector2 direction)
    {
		int animationIdx = 0;

		if(direction.y > 0.0f) animationIdx = 1;		// Up
		else if(direction.x > 0.0f) animationIdx = 2;	// Right
		else if(direction.y < 0.0f) animationIdx = 3;	// Down
		else if(direction.x < 0.0f) animationIdx = 4;	// Left
		else { mAnimator.Play("Character_Idle"); }

		if(direction != Vector2.zero && Input.GetButton("Run")) {
			animationIdx += 4;	// Will be in Run range
			speed = 3;
		} else {
			speed = 2;
		}

		mAnimator.SetInteger("move_direction", animationIdx);
    }

    private void getEmbarrassed(DetectionRange detectionRange)
    {
        switch (detectionRange)
        {
            case DetectionRange.greenZone:
                break;
            case DetectionRange.yellowZone:
                break;
        }
    }

    private void coolDownEmbarrassment()
    {

    }
}