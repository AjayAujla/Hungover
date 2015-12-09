using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SlideshowController : MonoBehaviour {
	
	private Sprite[] images;
	private int currentImageIndex;

	private float TOGGLE_TIMEOUT = 1f;	// change image every 3 seconds
	private float currentTimeout = 0f;

	private string nextLevel;

	private GameObject slideshowImage;
	private GameObject cameraFlash;
	private bool hasFlashed = false;

	public Camera mainCamera;
	private AudioSource audioSource;	// to play shutter sound

	// Use this for initialization
	void Start () {


		slideshowImage = GameObject.Find ("SlideshowImage");
		audioSource = GetComponent<AudioSource>();

		cameraFlash = GameObject.Find ("CameraFlash");
		ResizeSpriteToScreen(cameraFlash, mainCamera);

		// get level slideshow from PlayerPrefs
		PlayerPrefs.SetString("SlideshowLevel", "PoolParty");
		string slideshowLevel = PlayerPrefs.GetString("SlideshowLevel");
		
		images = Resources.LoadAll<Sprite>("Slideshows/" + slideshowLevel);
		Utils.Print("Loaded " + images.Length + " " + slideshowLevel + " images");
		
		currentImageIndex = 0;
		ChangeSlideshowImage(images[currentImageIndex]);
		CameraFlash();

		// scene to load at end of slideshow
		// nextLevel = Enum.Parse(BoardManager.Level, PlayerPrefs.GetString("NextLevel"));
	}
	
	// Update is called once per frame
	void Update () {

		currentTimeout += Time.deltaTime;

		if(currentTimeout >= TOGGLE_TIMEOUT) {
			++currentImageIndex;
			currentTimeout = 0f;	// reset timer
			
			CameraFlash();
			ChangeSlideshowImage(images[currentImageIndex]);
		}

		if(hasFlashed) {
			// decrement its alpha to 0
			float newAlpha = cameraFlash.GetComponent<SpriteRenderer>().color.a - Time.deltaTime;
			cameraFlash.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, newAlpha);
		}

		// load next level when last image is shown
		if(currentImageIndex == images.Length - 1)
			--currentImageIndex;

	}


	void CameraFlash() {

		audioSource.Play();
		cameraFlash.GetComponent<SpriteRenderer>().color = Color.white;
		hasFlashed = true;

	}

	void ChangeSlideshowImage(Sprite image) {
		slideshowImage.GetComponent<SpriteRenderer>().sprite = image;
		ResizeSpriteToScreen(slideshowImage, mainCamera);
	}

	// Borrowed and modified from
	// http://answers.unity3d.com/questions/701188/sprite-resized-to-whole-screen.html
	void ResizeSpriteToScreen(GameObject spriteObject, Camera camera)
	{
		SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
		if (spriteRenderer == null) return;
		
		double width = spriteRenderer.sprite.bounds.size.x;
		double height = spriteRenderer.sprite.bounds.size.y;
		
		double worldScreenHeight = camera.orthographicSize * 2.0;
		double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
		

//		spriteObject.transform.localScale = new Vector3((float)(worldScreenWidth / width), (float)height);    
//		spriteObject.transform.localScale = new Vector3((float)width, (float)(worldScreenHeight / height));

		spriteObject.transform.localScale = new Vector3((float)(worldScreenWidth / width), (float)(worldScreenHeight / height));    

	}

}
