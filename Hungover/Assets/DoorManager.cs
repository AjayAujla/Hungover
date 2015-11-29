using UnityEngine;
using System.Collections;

public class DoorManager : MonoBehaviour {
	
	void OnTriggerStay2D(Collider2D coll) {
		
		
		if (coll.gameObject.tag == "OuterWall")
		{
			Destroy(coll.gameObject);
		}  
	}
}