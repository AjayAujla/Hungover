using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void Print(string s) {
		print("[" + Time.time + "]" + " " + s);
	}
}
