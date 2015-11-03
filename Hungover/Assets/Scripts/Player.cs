using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	[SerializeField]
    public float speed;

    private Animator mAnimator;

	void Start ()
    {
        mAnimator = GetComponent<Animator>();
	}

	void Update ()
    {
        MoveCharacter();
	}

	void MoveCharacter()
    {
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		
		Vector2 direction = new Vector2(h, v);
		transform.Translate(direction * speed * Time.deltaTime);

		SetWalking(direction);	   
    }

    void SetWalking(Vector2 direction)
    {
		int directionIdx = 0;

		if(direction.y > 0.0f) directionIdx = 1;		// Up
		else if(direction.x > 0.0f) directionIdx = 2;	// Right
		else if(direction.y < 0.0f) directionIdx = 3;	// Down
		else if(direction.x < 0.0f) directionIdx = 4;	// Left

		mAnimator.SetInteger("move_direction", directionIdx);

    }

}
