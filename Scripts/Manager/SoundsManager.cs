using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SoundsManager : Singleton<SoundsManager>
{
    private ObjectPool objectPool;

    public AudioSource musicSource;

    private Dictionary<GameObject, SoundSource> dicSound = new Dictionary<GameObject, SoundSource>();
    private Dictionary<string, AudioClip> dicAudioClip = new Dictionary<string, AudioClip>();
    private List<SoundSource> lisPlaySound = new List<SoundSource>();

    private const int nMax = 10;
    public override void Awake()
    {
        base.Awake();
        objectPool = GetComponent<ObjectPool>();
        objectPool.Init(nMax);
    }
    public void PlayBGM(string sPath)
    {
        if (!dicAudioClip.ContainsKey(sPath))
        {
            AudioClip _audioClip = Resources.Load<AudioClip>(sPath);
            dicAudioClip.Add(sPath, _audioClip);
        }
        musicSource.clip = dicAudioClip[sPath];
        musicSource.Play();
    }
    public void Play(string sPath)
    {
        if (!dicAudioClip.ContainsKey(sPath))
        {
            AudioClip _audioClip = Resources.Load<AudioClip>(sPath);
            dicAudioClip.Add(sPath, _audioClip);
        }

        if (dicAudioClip[sPath] == null)
            return;

        GameObject _obj = objectPool.GetObj();
        if (!dicSound.ContainsKey(_obj))
        {
            dicSound.Add(_obj, _obj.GetComponent<SoundSource>());
            dicSound[_obj].Init();
        }
        dicSound[_obj].Play(dicAudioClip[sPath]);
        lisPlaySound.Add(dicSound[_obj]);
    }
    public void SetSoundMute()
    {
        foreach (KeyValuePair<GameObject, SoundSource> sounds in dicSound)
        {
            sounds.Value.SetMute();
        }
    }
    public void SetMusicMute()
    {
        if (SaveManager.Instance.localGameData.nMusicMute == 0)
            musicSource.mute = false;
        else
            musicSource.mute = true;
    }
    public void SetSoundVolume()
    {
        float _fVolume = SaveManager.Instance.localGameData.fSounds;
        foreach (KeyValuePair<GameObject, SoundSource> sounds in dicSound)
        {
            sounds.Value.SetVolume(_fVolume * 0.01f);
        }
    }

    public void SetMusicVolume()
    {
        float _fVolume = SaveManager.Instance.localGameData.fMusic;
        musicSource.volume = _fVolume * 0.01f;
    }

    public void Update()
    {
        if (lisPlaySound.Count > 0)
        {
            for (int i = 0; i < lisPlaySound.Count; ++i)
            {
                if (!lisPlaySound[i].audioSource.isPlaying)
                {
                    objectPool.ReturnObj(lisPlaySound[i].gameObject);
                    lisPlaySound.RemoveAt(i);
                    break;
                }
            }
        }

    }
}
