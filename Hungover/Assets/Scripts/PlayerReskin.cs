using UnityEngine;
using System;
using System.Collections;

public class PlayerReskin : MonoBehaviour {

	public static string spriteSheetName;
	private static string[] clothes = new string[4];
	
	private static int clothesNbr = 0;
    private Player playerScript;

	private enum SpriteSheetsEnum {
		Character_Brown,
		Character_Green,
		Character_Red,
		Character_Yellow
	}

    void Start() {
		int SpriteSheetsEnumIdx = UnityEngine.Random.Range(0, 5);
		spriteSheetName = ((SpriteSheetsEnum)SpriteSheetsEnumIdx).ToString();
        this.playerScript = GameObject.Find("AshFlashem(Clone)").GetComponent<Player>();
    }

	public static void ChangeSprite(string clothe) {

		bool hasClothe = false;
		foreach(string c in clothes) {
			if(clothe == c)
				hasClothe = true;
		}

		if(!hasClothe) {
			clothes.SetValue(clothe, clothesNbr);
			Array.Sort(clothes, StringComparer.InvariantCultureIgnoreCase);
			string s = string.Join("", clothes);
			spriteSheetName = s;
			Utils.Print(s);
		}
	}
	
	void LateUpdate () {
		
		Sprite[] subSprites = Resources.LoadAll<Sprite>("SpriteSheets/Player/" + spriteSheetName + "/Character_Naked");
		
		foreach(SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
			
			if(renderer.sprite) {
				string spriteName = renderer.sprite.name;
				Sprite newSprite = Array.Find (subSprites, item => item.name == spriteName);
				
				if(newSprite)
					renderer.sprite = newSprite;
            }
		}
	}

    public void UnderTable()
    {
        this.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/hidden");
    }
}