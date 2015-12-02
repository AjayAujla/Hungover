using UnityEngine;
using System.Collections;

public class ButtonBehavior : MonoBehaviour {

    public void LoadOnClick(int level)
    {
        Application.LoadLevel(level);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.LoadLevel("MainMenu");
        }

        if (Input.GetButtonDown("Submit") && Application.loadedLevel <= 1)
        {
            Application.LoadLevel(2);
        }
    }
}