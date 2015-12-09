using UnityEngine;
using System.Collections;

public class HeadsUpDisplay : MonoBehaviour
{
    private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.

    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        if (GameObject.FindObjectsOfType(GetType()).Length > 1)
        {
            //Destroy(this.gameObject);
        }
        this.boardScript = GameObject.Find("GameManager").GetComponent<BoardManager>();
    }

    void Start()
    {
        //this.boardScript.SetupScene();
        //Utils.Print("from HUD " + this.boardScript.name);
    }

    public void PlayerDieOfEmbarrassment()
    {
        Utils.Print("YOU DIED OF EMBARRASSMENT");
        Application.LoadLevel(Application.loadedLevel);
        //GameObject.Find("GameManager").GetComponent<BoardManager>().SetupScene(1);
    }
}