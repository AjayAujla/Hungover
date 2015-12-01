using UnityEngine;
using System;
using System.Collections;

public class ReSkin : MonoBehaviour {

	public string spriteSheetName;

	private enum SpriteSheetsEnum {
		Character_Brown,
		Character_Green,
		Character_Red,
		Character_Yellow
	}



	void Start() {
		int SpriteSheetsEnumIdx = UnityEngine.Random.Range(0, 4);
		spriteSheetName = ((SpriteSheetsEnum)SpriteSheetsEnumIdx).ToString();
	}

	// Update is called once per frame
	void LateUpdate () {


		Sprite[] subSprites = Resources.LoadAll<Sprite>("SpriteSheets/" + spriteSheetName + "/Character_Naked");

		foreach(SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {

			if(renderer.sprite) {
				string spriteName = renderer.sprite.name;
				Sprite newSprite = Array.Find (subSprites, item => item.name == spriteName);

				if(newSprite)
					renderer.sprite = newSprite;

				if(GameObject.Find ("AlarmSound").GetComponent<AudioSource>().isPlaying)
					renderer.sprite = Array.Find (subSprites, item => item.name == "Character_Naked_88");
			}

		}
	
	
	
	}
}
