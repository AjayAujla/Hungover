﻿using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{

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
    float directionChangeTimer;
    float minimumDirectionChangeTimer = 1.0f;
    float maximumDirectionChangeTimer = 3.0f;

    Animator mAnimator;
    AudioSource PartyMusic;

    [SerializeField]
    public float speed;
    bool isDancing;

    // limiting character's movement by Camera's viewport coordinates
    private float minX, maxX, minY, maxY;

    [Range(0.1f, 10f)]
    [SerializeField]
    private float radius = 4;
    [SerializeField]
    private bool showFieldOfViewFill = false;

    [Range(1.0f, 360f)]
    public int fieldOfViewAngle;
    private Vector2 leftLineFOV;
    private Vector2 rightLineFOV;

    private Transform player;

    void Awake()
    {
        // If you want the min max values to update if the resolution changes 
        // set them in update else set them in Start
        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));
        minX = bottomCorner.x;
        maxX = topCorner.x;
        minY = bottomCorner.y;
        maxY = topCorner.y;

        mAnimator = GetComponent<Animator>();
        PartyMusic = (AudioSource)GameObject.Find("PartyMusic").GetComponents<AudioSource>()[0];
        this.player = GameObject.Find("AshFlashem(Clone)").transform;

        // Starting each enemy in a random direction
        // Going from index 0 to 4 exclusively
        int directionsIdx = Random.Range(0, 4);
        this.direction = directions[directionsIdx];
        this.directionChangeTimer = Random.Range(this.minimumDirectionChangeTimer, this.maximumDirectionChangeTimer);

        speed = 2.0f;
        isDancing = false;
    }

    void Update()
    {

        if (PartyMusic.isPlaying)
        {
            if (!isDancing)
            {
                isDancing = true;
                DanceCharacter();
            }
        }
        else
        {
            isDancing = false;
            MoveCharacter();
        }

        mAnimator.SetBool("is_dancing", isDancing);

        ChangeDirection();
        LimitPosition();

        if (player != null)
        {
            this.rightLineFOV = this.RotatePointAroundTransform(this.direction.normalized * this.radius, -this.fieldOfViewAngle / 2.0f);
            this.leftLineFOV = this.RotatePointAroundTransform(this.direction.normalized * this.radius, this.fieldOfViewAngle / 2.0f);
            Debug.Log(this.InsideFieldOfView(new Vector2(this.player.position.x, this.player.position.y)));
        }
    }

    void MoveCharacter()
    {
        this.transform.Translate(this.direction * speed * Time.deltaTime);
    }

    void DanceCharacter()
    {
        int dance_move = Random.Range(9, 12);   // will generate 9, 10, or 11
        mAnimator.SetInteger("move_direction", dance_move);
    }

    // Prevents character from walking outside of the viewport
    void LimitPosition()
    {
        // Get current position
        Vector3 pos = transform.position;

        // Horizontal contraint
        if (pos.x < minX) pos.x = minX;
        if (pos.x > maxX) pos.x = maxX;

        // vertical contraint
        if (pos.y < minY) pos.y = minY;
        if (pos.y > maxY) pos.y = maxY;

        // Update position
        transform.position = pos;
    }

    void ChangeDirection()
    {

        // Every few seconds, change direction
        // Note that newDirection could be the same as current one, on purpose
        this.directionChangeTimer -= Time.deltaTime;

        if (this.directionChangeTimer <= 0.0f)
        {
            int directionsIdx = Random.Range(0, 4);
            Vector3 newDirection = directions[directionsIdx];
            this.direction = newDirection;
            mAnimator.SetInteger("move_direction", directionsIdx + 1);
            this.directionChangeTimer = Random.Range(this.minimumDirectionChangeTimer, this.maximumDirectionChangeTimer);
        }
    }

    public bool InsideFieldOfView(Vector2 playerPosition)
    {
        float squaredDistance = ((playerPosition.x - this.transform.position.x) * (playerPosition.x - this.transform.position.x)) + ((playerPosition.y - this.transform.position.y) * (playerPosition.y - this.transform.position.y)); // a^2 + b^2 = c^2

        if (this.radius * this.radius >= squaredDistance)
        {
            float signLeftLine = (this.leftLineFOV.x) * (playerPosition.y - this.transform.position.y) - (this.leftLineFOV.y) * (playerPosition.x - this.transform.position.x);
            float signRightLine = (rightLineFOV.x) * (playerPosition.y - transform.position.y) - (rightLineFOV.y) * (playerPosition.x - transform.position.x);
            if (fieldOfViewAngle <= 180)
            {
                //Debug.Log(signLeftLine + " " + signRightLine);
                if (signLeftLine <= 0.0f && signRightLine >= 0.0f)
                {
                    return true;
                }
            }
            else
            {
                if (!(signLeftLine >= 0.0f && signRightLine <= 0.0f))
                {
                    //return true;
                }
            }
        }
        return false;
    }

    //Rotate point (px, py) around point (ox, oy) by angle theta you'll get:
    //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
    //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
    private Vector2 RotatePointAroundTransform(Vector2 p, float angles)
    {
        return new Vector2(Mathf.Cos((angles) * Mathf.Deg2Rad) * (p.x) - Mathf.Sin((angles) * Mathf.Deg2Rad) * (p.y),
                           Mathf.Sin((angles) * Mathf.Deg2Rad) * (p.x) + Mathf.Cos((angles) * Mathf.Deg2Rad) * (p.y));
    }

    void OnDrawGizmos()
    {
        this.leftLineFOV = this.RotatePointAroundTransform(this.direction.normalized * this.radius, this.fieldOfViewAngle / 2.0f);
        this.rightLineFOV = this.RotatePointAroundTransform(this.direction.normalized * this.radius, -this.fieldOfViewAngle / 2.0f);

        Vector2 p0 = this.rightLineFOV;
        float divisions = 20.0f;
        float step = this.fieldOfViewAngle / divisions;

        // inner rays
        for (int i = 1; i <= divisions; ++i)
        {
            Vector2 p1 = this.RotatePointAroundTransform(this.direction.normalized * this.radius, -this.fieldOfViewAngle / 2.0f + step * i);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + p0, p1 - p0);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + p0 / 2.0f, p1 - p0);

            if (this.showFieldOfViewFill)
            {
                Gizmos.color = Color.red;
                Vector2 p2 = this.RotatePointAroundTransform(this.direction.normalized * this.radius / 2.0f, -this.fieldOfViewAngle / 2.0f + step * i);
                Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + p2, p2 - p1);

                Gizmos.color = Color.yellow;
                Vector2 p3 = this.RotatePointAroundTransform(this.direction.normalized * this.radius / 2.0f, -this.fieldOfViewAngle / 2.0f + step * i);
                Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + p1, p2 - p1);
            }
            p0 = p1;
        }

        // outline rays
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + this.leftLineFOV / 2.0f, this.leftLineFOV / 2.0f);
        Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + this.rightLineFOV / 2.0f, this.rightLineFOV / 2.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position, this.leftLineFOV / 2.0f);
        Gizmos.DrawRay(this.transform.position, this.rightLineFOV / 2.0f);
    }
}