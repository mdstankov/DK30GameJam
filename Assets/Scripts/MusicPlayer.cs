using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer> {

    AudioSource audioSource;

	// Use this for initialization
	void Start ()
	{
	//	audioSource = GetComponent<AudioSource>();
	//	audioSource.volume = PlayerPrefsController.GetMasterVolume();
	}
	
   // public void SetVolume(float volume)
   // {
   //    audioSource.volume = volume;
   // }

}
