using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private BoardManager boardManagerScript;
    private GameObject player;

    private float smoothness = 5.0f;

    void Start()
    {
        this.boardManagerScript = GameObject.Find("GameManager").GetComponent<BoardManager>();
        this.player = GameObject.Find("AshFlashem(Clone)");
    }

    void Update()
    {
        BoardManager.Room currentRoom = this.boardManagerScript.getCurrentRoom();
        if (currentRoom != null) // player in a room
        {
            float widthMultiplier;
            if (currentRoom.widthRoom >= currentRoom.heightRoom)
            {
                widthMultiplier = 1.5f;
            }
            else
            {
                widthMultiplier = 1.0f;
            }

            // resizes based on current room width
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, currentRoom.widthRoom / widthMultiplier, Time.deltaTime * this.smoothness);
            // centered at room position
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(currentRoom.xCenterPos, currentRoom.yCenterPos, this.transform.position.z), Time.deltaTime);
        }
        else // player on a path between rooms
        {
            // centered at player if on a path
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.player.transform.position.x, this.player.transform.position.y, this.transform.position.z), Time.deltaTime);
        }
    }
}