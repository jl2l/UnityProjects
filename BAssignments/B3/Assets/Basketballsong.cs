using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Basketballsong : MonoBehaviour {

	public int PlaySong;
	private AudioSource audioS;
	private AudioClip Song;

	// Use this for initialization
	void Start () {
		audioS = GetComponent<AudioSource>();
		Song = audioS.clip;
		PlaySong = 0;

	}
	
	// Update is called once per frame
	void Update () {
	

		if (PlaySong == 1 && audioS.isPlaying != true)
		{
			audioS.PlayOneShot(Song, 0.7F);
			//StartCoroutine(ExecuteAfterTime(10));

		}
		if(PlaySong == 0 && audioS.isPlaying == true)
		{
			audioS.Stop();
		}
	}

	IEnumerator ExecuteAfterTime(float time)
	{
		yield return new WaitForSeconds(time);

		Debug.Log ("hi");
		
		// Code to execute after the delay
	}

}
