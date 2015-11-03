using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {
	
	/********************************************************************
	 * 
	 *	Animation States:
	 *	1 = Walk Up			5 = Run Up			9 = Dance Move 1
	 *	2 = Walk Right		6 = Run Right		10 = Dance move 2
	 *	3 = Walk Down		7 = Run Down		11 = Dance move 3
	 *	4 = Walk Left		8 = Run Left		12 = Squat (Coming Soon)
	 *
	 ********************************************************************/

	Vector3 direction;
	Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

	Animator mAnimator;
	AudioSource PartyMusic;

	[SerializeField]
	public float speed;
	bool isDancing;

	// limiting character's movement by Camera's viewport coordinates
	private float minX, maxX, minY, maxY;

	void Awake () {
			
		// If you want the min max values to update if the resolution changes 
		// set them in update else set them in Start
		float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
		Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0,0, camDistance));
		Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1,1, camDistance));
		minX = bottomCorner.x;
		maxX = topCorner.x;
		minY = bottomCorner.y;
		maxY = topCorner.y;
		
		mAnimator = GetComponent<Animator>();
		PartyMusic = (AudioSource)GameObject.Find ("PartyMusic").GetComponents<AudioSource>()[0];

	}

	// Use this for initialization
	void Start () {

		// Starting each enemy in a random direction
		// Going from index 0 to 4 exclusively
		int directionsIdx = Random.Range(0, 4);
		this.direction = directions[directionsIdx];

		speed = 2.0f;
		isDancing = false;

	}
	
	// Update is called once per frame
	void Update () {

		if(PartyMusic.isPlaying) {
			if(!isDancing) {
				isDancing = true;
				DanceCharacter();
			}
		} else {
			isDancing = false;
			MoveCharacter();
		}

		mAnimator.SetBool("is_dancing", isDancing);

		ChangeDirection ();
		LimitPosition ();
	}

	void MoveCharacter () {
		this.transform.Translate(this.direction * speed * Time.deltaTime);
	}

	void DanceCharacter () {
		int dance_move = Random.Range(9, 12);	// will generate 9, 10, or 11
		mAnimator.SetInteger("move_direction", dance_move);
	}

	// Prevents character from walking outside of the viewport
	void LimitPosition () {
		// Get current position
		Vector3 pos = transform.position;
		
		// Horizontal contraint
		if(pos.x < minX) pos.x = minX;
		if(pos.x > maxX) pos.x = maxX;
		
		// vertical contraint
		if(pos.y < minY) pos.y = minY;
		if(pos.y > maxY) pos.y = maxY;
		
		// Update position
		transform.position = pos;
	}

	void ChangeDirection () {

		// Every 2 seconds, change direction
		// Note that newDirection could be the same as current one, on purpose
		if(Time.time % 1.5f <= 0.1f) {

			int directionsIdx = Random.Range(0, 4);
			Vector3 newDirection = directions[directionsIdx];
			this.direction = newDirection;
			mAnimator.SetInteger("move_direction", directionsIdx + 1);
		}

	}
}
