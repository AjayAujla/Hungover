using UnityEngine;
using System.Collections;

public class ButtonBehavior : MonoBehaviour {

    public GameObject loadingImage;

    public void LoadOnClick(int level)
    {
        this.loadingImage.SetActive(true); //show loading image

        Application.LoadLevel(level);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.LoadLevel(0);
        }
    }
}