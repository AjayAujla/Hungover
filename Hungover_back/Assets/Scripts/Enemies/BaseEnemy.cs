using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {


	Vector3 direction;
	Vector3[] directions = { Vector3.up, Vector3.right, Vector3.down, Vector3.down };

	// Use this for initialization
	void Start () {

		// Starting each enemy in a random direction
		int directionsIdx = Random.Range(0, 4);
		this.direction = directions[directionsIdx];

	}
	
	// Update is called once per frame
	void Update () {
		MoveCharacter();
		ChangeDirection ();
	}

	void MoveCharacter () {
		this.transform.Translate(this.direction * 2.0f * Time.deltaTime);
	}

	void ChangeDirection () {

		// Every 2 seconds, change direction
		// Note that newDirection could be the same as current one, on purpose
		if(Time.time % 2 <= 0.1f) {

			int directionsIdx = Random.Range(0, 4);
			Vector3 newDirection = directions[directionsIdx];
			this.direction = newDirection;

		}

	}
}
