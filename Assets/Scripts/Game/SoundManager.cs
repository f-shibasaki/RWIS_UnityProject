using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private string bgmName = "bgm01";

    public AudioSource audioSourceSE;
    Dictionary<string, AudioClip> audioClipsSE = new Dictionary<string, AudioClip>();

    public AudioSource audioSourceBGM;
    public AudioClip audioClipsBGM;

    private void Awake()
    {
        //audioSource = GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        audioSourceBGM.clip = (AudioClip)Resources.Load(bgmName);
    }

    public void SetAudioClipsSE(String seName)
    {
        if (audioClipsSE.ContainsKey(seName) == true)
            return; // not register
        
        audioClipsSE.Add(seName, (AudioClip)Resources.Load( seName ));
    }

    public void PlaySE(String seName)
    {
        SetAudioClipsSE(seName);
        audioSourceSE.PlayOneShot(audioClipsSE[seName]);
    }

    public void PlayBGM()
    {
        audioSourceBGM.Play();
    }
    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    public void PauseBGM()
    {
        audioSourceBGM.Pause();
    }
}
