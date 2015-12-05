using UnityEngine;
using System.Collections;

public class EwwManager : MonoBehaviour {

	public AudioClip[] ewws;
	AudioSource audioSource;
	AudioSource backgroundMusicSource;

	void Start() {
		audioSource = GetComponent<AudioSource>();
		backgroundMusicSource = GameObject.Find("GameManager").GetComponent<AudioSource>();
	}

	public void Play() {
		if(!audioSource.isPlaying) {
			AudioClip eww = ewws[Random.Range (0, ewws.Length)];
			audioSource.clip = eww;
			audioSource.Play();
		}
	}

	public void Ewwwwwwww() {
		AudioClip ewwwww = Resources.Load<AudioClip>("Sounds/Ewws/EwFemaleLong");
		backgroundMusicSource.Stop();

		audioSource.clip = ewwwww;

		if(!audioSource.isPlaying) {
			audioSource.Play();
		}
	}

}
