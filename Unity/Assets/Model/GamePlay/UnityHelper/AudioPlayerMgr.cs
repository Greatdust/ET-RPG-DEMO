using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine.Audio;

public class AudioPlayerMgr : MonoBehaviour
{

    public AudioSource bg;

    public AudioMixerGroup master;
    public AudioMixerGroup bgm;
    public AudioMixerGroup soundEffect;
    public AudioMixerGroup characterSound;


    public void PlayBg(AudioClip audioClip)
    {
        bg.clip = audioClip;
        bg.loop = true;
        bg.Play();
    }

    public void PauseBg()
    {
        bg.Pause();
    }
}
