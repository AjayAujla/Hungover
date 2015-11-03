using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {


	Vector3 direction;
	Vector3[] directions = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };

	Animator mAnimator;

	[SerializeField]
	float speed;
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

	}

	// Use this for initialization
	void Start () {

		// Starting each enemy in a random direction
		// Going from index 0 to 4 exclusively
		int directionsIdx = Random.Range(0, 4);
		this.direction = directions[directionsIdx];

		speed = 2.0f;

	}
	
	// Update is called once per frame
	void Update () {
		MoveCharacter();
		ChangeDirection ();
		LimitPosition ();
	}

	void MoveCharacter () {
		this.transform.Translate(this.direction * speed * Time.deltaTime);
	}

	// Prevents character from walking outside of the viewport
	void LimitPosition() {
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
