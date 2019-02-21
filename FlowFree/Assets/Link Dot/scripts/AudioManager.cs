using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    [SerializeField]
    public AudioSource bgAudioSource, effectAudioSource;
    [Header("____________List Clip Audio_____________")]
    [SerializeField]
    private AudioClip buttonClip;
    [SerializeField]
    private AudioClip winClip;
    [SerializeField]
    private AudioClip wrongClip;
    [SerializeField]
    private List<AudioClip> effectClip;

    public void PlayButtonClip()
    {
        effectAudioSource.PlayOneShot(buttonClip);
    }

    public void PlayEffectClip()
    {
        effectAudioSource.PlayOneShot(effectClip[UnityEngine.Random.Range(0, effectClip.Count)]);
    }

    public void PlayWinClip()
    {
        effectAudioSource.PlayOneShot(winClip);
    }

    public void PlayWrongClip()
    {
        effectAudioSource.PlayOneShot(wrongClip);
    }


    public void MuteBackgroundMusic(bool isMute)
    {
        bgAudioSource.mute = isMute;
    }

    public void MuteEffectMusic(bool isMute)
    {
        effectAudioSource.mute = isMute;
    }

}
