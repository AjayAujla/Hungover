using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
    /********************************************************************
	 *	Animation States:
	 *	1 = Walk Up			5 = Run Up			9 = Dance Move 1
	 *	2 = Walk Right		6 = Run Right		10 = Dance move 2
	 *	3 = Walk Down		7 = Run Down		11 = Dance move 3
	 *	4 = Walk Left		8 = Run Left		12 = Squat (Coming Soon)
	 ********************************************************************/

    Vector3 direction;
    Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    float directionChangeTimer;
    float minimumDirectionChangeTimer = 1.0f;
    float maximumDirectionChangeTimer = 3.0f;

    private GameObject player;
    Animator mAnimator;
    AudioSource PartyMusic;
    private Player playerScript;

    [SerializeField]
    public float speed;
    bool isDancing;

    // limiting character's movement by Camera's viewport coordinates
    private float minX, maxX, minY, maxY;

    [SerializeField]
    [Range(0.1f, 10f)]
    private float fieldOfViewRadius = 4;
    [SerializeField]
    [Range(1.0f, 360f)]
    private int fieldOfViewAngle;
    [SerializeField]
    private bool showFieldOfViewAreaFill = false;

    private Vector2 leftLineFieldOfView;
    private Vector2 rightLineFieldOfView;

    Player.DetectionRange playerDetectionRange;

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
        this.player = GameObject.Find("AshFlashem(Clone)");
        this.playerScript = this.player.GetComponent<Player>();

        // Starting each enemy in a random direction
        // Going from index 0 to 4 exclusively
        int directionsIdx = Random.Range(0, 4);
        this.direction = directions[directionsIdx];
        this.directionChangeTimer = Random.Range(this.minimumDirectionChangeTimer, this.maximumDirectionChangeTimer);

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

        if (this.transform.name == "TestEnemy") {
            if (this.PlayerInsideFieldOfView() && this.PlayerInLineOfSight()) {
                Debug.LogError("Player in " + this.playerDetectionRange);
            }
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

            this.rightLineFieldOfView = this.RotatePointAroundTransform(this.direction.normalized * this.fieldOfViewRadius, -this.fieldOfViewAngle / 2.0f);
            this.leftLineFieldOfView = this.RotatePointAroundTransform(this.direction.normalized * this.fieldOfViewRadius, this.fieldOfViewAngle / 2.0f);
        }
    }

    private bool PlayerInsideFieldOfView()
    {
        return this.InsideFieldOfView(this.player.transform.position);
    }

    /*
     *  Can be utlized to find anything within the field of view
     */
    public bool InsideFieldOfView(Vector3 position)
    {
        float squaredDistance = ((position.x - this.transform.position.x) * (position.x - this.transform.position.x)) + ((position.y - this.transform.position.y) * (position.y - this.transform.position.y)); // a^2 + b^2 = c^2

        if (this.fieldOfViewRadius * this.fieldOfViewRadius >= squaredDistance)
        {
            float signLeftLine = (this.leftLineFieldOfView.x) * (position.y - this.transform.position.y) - (this.leftLineFieldOfView.y) * (position.x - this.transform.position.x);
            float signRightLine = (rightLineFieldOfView.x) * (position.y - transform.position.y) - (rightLineFieldOfView.y) * (position.x - transform.position.x);
            if (this.fieldOfViewAngle <= 180)
            {
                if (signLeftLine <= 0.0f && signRightLine >= 0.0f)
                {
                    return true;
                }
            }
            else
            {
                if (!(signLeftLine >= 0.0f && signRightLine <= 0.0f))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool PlayerInLineOfSight()
    {
        return this.InLineOfSight(this.player.transform.position);
    }

    /* 
     *  Changed Physics2D settings in Unity -> Edit -> Project Settings -> Queries Start in Colliders -> False to prevent ray from colliding with enemy's own collider
     *  Can be utlized to check if anything is in line of sight within the field of view
     */
    private bool InLineOfSight(Vector3 position)
    {
        Vector2 rayDirection = position - this.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, rayDirection);
        if (hit.collider != null)
        {
            Debug.DrawRay(this.transform.position, rayDirection);
            Debug.LogError(this.transform.name + " --> " + hit.collider.transform.name);
            if ((hit.transform.tag == "Player"))
            {
                if(rayDirection.sqrMagnitude <= (this.fieldOfViewRadius / 2.0f * this.fieldOfViewRadius / 2.0f))
                {
                    this.playerDetectionRange = Player.DetectionRange.redZone;
                } else if(rayDirection.sqrMagnitude <= this.fieldOfViewRadius * this.fieldOfViewRadius)
                {
                    this.playerDetectionRange = Player.DetectionRange.yellowZone;
                } else
                {
                    this.playerDetectionRange = Player.DetectionRange.greenZone;
                }
                return true;
            }
        }
        this.playerDetectionRange = Player.DetectionRange.greenZone;
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
        this.leftLineFieldOfView = this.RotatePointAroundTransform(this.direction.normalized * this.fieldOfViewRadius, this.fieldOfViewAngle / 2.0f);
        this.rightLineFieldOfView = this.RotatePointAroundTransform(this.direction.normalized * this.fieldOfViewRadius, -this.fieldOfViewAngle / 2.0f);

        Vector2 p0 = this.rightLineFieldOfView;
        float divisions = 120.0f; // greater number of divisions helps smooths the curve and also adds more rays within the fill area
        float step = this.fieldOfViewAngle / divisions;

        // inner rays
        for (int i = 1; i <= divisions; ++i)
        {
            Vector2 p1 = this.RotatePointAroundTransform(this.direction.normalized * this.fieldOfViewRadius, -this.fieldOfViewAngle / 2.0f + step * i);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + p0, p1 - p0);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + p0 / 2.0f, p1 - p0);

            if (this.showFieldOfViewAreaFill)
            {
                Gizmos.color = Color.red;
                Vector2 p2 = this.RotatePointAroundTransform(this.direction.normalized * this.fieldOfViewRadius / 2.0f, -this.fieldOfViewAngle / 2.0f + step * i);
                Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + p2, p2 - p1);

                Gizmos.color = Color.yellow;
                Vector2 p3 = this.RotatePointAroundTransform(this.direction.normalized * this.fieldOfViewRadius / 2.0f, -this.fieldOfViewAngle / 2.0f + step * i);
                Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + p1, p2 - p1);
            }
            p0 = p1;
        }

        // outline rays
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + this.leftLineFieldOfView / 2.0f, this.leftLineFieldOfView / 2.0f);
        Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y) + this.rightLineFieldOfView / 2.0f, this.rightLineFieldOfView / 2.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position, this.leftLineFieldOfView / 2.0f);
        Gizmos.DrawRay(this.transform.position, this.rightLineFieldOfView / 2.0f);
    }
}