using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour{

	public static void Print(string s) {
		Debug.LogError("[" + Time.time + "]" + " " + s);
	}
}