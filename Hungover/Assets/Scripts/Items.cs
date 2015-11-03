using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Items : MonoBehaviour {

    /**
     *  Makes item sprite opaque if it has been found
     *  TODO: Invoke this function within collision detection code of player against the item hidden within the scene
     */
    public void foundItem()
    {
        this.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
	}
}