using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSrc;

    [SerializeField] private AudioSource sfxSrc;

    public void SetBackgroundMusic(AudioClip clip)
    {
        if (musicSrc.clip == clip)
        {
            return;
        }
        
        musicSrc.Stop();
        musicSrc.clip = clip;
        musicSrc.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        sfxSrc.PlayOneShot(clip);
    }
}
