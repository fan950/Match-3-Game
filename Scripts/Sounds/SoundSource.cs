using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    [HideInInspector] public AudioSource audioSource;

    public void Init()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void SetVolume(float fVolume)
    {
        audioSource.volume = fVolume;
    }
    public void SetMute()
    {
        if (SaveManager.Instance.localGameData.nSoundsMute == 0)
            audioSource.mute = false;
        else
            audioSource.mute = true;
    }
    public void Play(AudioClip audioClip)
    {
        if (audioSource == null)
            return;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void Stop()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
    }
}
