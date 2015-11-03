using UnityEngine;
using System.Collections;

public class DJ : MonoBehaviour {
	
	private AudioSource mAudioSource;
	private AudioClip currentMusic;
	private AudioClip musicStopScratching;

	// Use this for initialization
	void Start () {
		mAudioSource = GetComponent<AudioSource>();
		currentMusic = mAudioSource.clip;
		musicStopScratching = Resources.Load<AudioClip>("Sounds/Music/MusicStopScratching");
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetButtonUp("Music")) {
			if(!mAudioSource.isPlaying) {
				mAudioSource.Play();

			} else {
				mAudioSource.Stop();
				mAudioSource.clip = musicStopScratching;
				mAudioSource.PlayOneShot(musicStopScratching);
				mAudioSource.Play ();
				mAudioSource.clip = currentMusic;
			}

		}

	}
}
