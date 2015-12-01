using UnityEngine;
using System.Collections;

public class DoorManager : MonoBehaviour {
	
	void OnTriggerStay2D(Collider2D coll) {
		
		
		if (coll.gameObject.tag == "OuterWall")
		{
			// Destroy(coll.gameObject);
//			coll.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			GameObject player = GameObject.Find("AshFlashem(Clone)");
			Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), coll.gameObject.GetComponent<BoxCollider2D>());
		}  
	}
}