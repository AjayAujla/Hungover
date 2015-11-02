using UnityEngine;
using System.Collections;

public class CrushEnemy : BaseEnemy {

	// Use this for initialization
	void Start () {

	}
	
	void MoveCharacter () {
		Utils.Print ("Overriden and not moving!");
	}
}
