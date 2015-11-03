using UnityEngine;
using System.Collections;

public class DJ : MonoBehaviour {
	
	private Component[] mAudioSources;
	private AudioSource NellyMusic;
	private AudioSource StopScratching;

	// Use this for initialization
	void Start () {

		mAudioSources = GetComponents<AudioSource>();
		NellyMusic = (AudioSource)mAudioSources[0];
		StopScratching = (AudioSource)mAudioSources[1];
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetButtonUp("Music")) {
			if(!NellyMusic.isPlaying) {
				NellyMusic.Play();

			} else {
				NellyMusic.Stop();
				StopScratching.Play ();
			}

		}

	}
}
