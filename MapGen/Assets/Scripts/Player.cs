using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public float speed;

    private const string kIdle = "Idle";
    private const string kUp = "Up";
    private const string kDown = "Down";
    private const string kRight = "Right";
    private const string kLeft = "Left";

    private Animator mAnimator;

	void Start ()
    {
        mAnimator = GetComponent<Animator>();
	}

	void Update ()
    {
        Movement();
	}

    void Movement()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            SetWalking(MovementDirection.Right);
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 0);
        }

        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            SetWalking(MovementDirection.Left);
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            SetWalking(MovementDirection.Up);
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }

        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            SetWalking(MovementDirection.Down);
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }

        else
        {
            SetIdle();
        }
    }

    void SetWalking(MovementDirection direction)
    {
        mAnimator.SetBool(direction.ToString(), true);
        mAnimator.SetBool(kIdle, false);
    }

    void SetIdle()
    {
        mAnimator.SetBool(kIdle, true);
        ResetMovementTriggers();
    }

    void ResetMovementTriggers()
    {
        mAnimator.SetBool(kUp, false);
        mAnimator.SetBool(kDown, false);
        mAnimator.SetBool(kRight, false);
        mAnimator.SetBool(kLeft, false);
    }
}
