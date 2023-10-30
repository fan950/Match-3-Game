using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SettingsPopup : UIPopup
{
    public UIBtn closeBtn;

    public Slider soundSlider;
    public Slider musicSlider;

    public Slider soundMuteSlider;
    public Slider musicMuteSlider;
    public Image soundsMuteImg;
    public Image musicMuteImg;

    private const string sPath = "Sounds/UI/Button";

    public override void Init(Transform parent)
    {
        base.Init(parent);

        soundSlider.onValueChanged.AddListener(delegate
        {
            SaveManager.Instance.localGameData.fSounds = soundSlider.value;

            SoundsManager.Instance.SetSoundVolume();
            SoundsManager.Instance.Play(sPath);

        });

        musicSlider.onValueChanged.AddListener(delegate
        {
            SaveManager.Instance.localGameData.fMusic = musicSlider.value;

            SoundsManager.Instance.SetMusicVolume();
        });

        soundMuteSlider.onValueChanged.AddListener(delegate
        {
            if (soundMuteSlider.value == 0)
                soundsMuteImg.sprite = UIManager.Instance.GetSprite(eAtlasType.Level_UI, "SliderPink");
            else
                soundsMuteImg.sprite = UIManager.Instance.GetSprite(eAtlasType.Level_UI, "SliderGreen");

            SaveManager.Instance.localGameData.nSoundsMute = (int)soundMuteSlider.value;
            SoundsManager.Instance.SetSoundMute();
        });

        musicMuteSlider.onValueChanged.AddListener(delegate
        {
            if (musicMuteSlider.value == 0)
                musicMuteImg.sprite = UIManager.Instance.GetSprite(eAtlasType.Level_UI, "SliderPink");
            else
                musicMuteImg.sprite = UIManager.Instance.GetSprite(eAtlasType.Level_UI, "SliderGreen");

            SaveManager.Instance.localGameData.nMusicMute = (int)musicMuteSlider.value;
            SoundsManager.Instance.SetMusicMute();
        });

        closeBtn.Init(delegate
        {
            Close();
        });
    }

    public override void Open(Action action = null)
    {
        soundSlider.value = SaveManager.Instance.localGameData.fSounds;
        musicSlider.value = SaveManager.Instance.localGameData.fMusic;

        soundMuteSlider.value = SaveManager.Instance.localGameData.nSoundsMute;
        musicMuteSlider.value = SaveManager.Instance.localGameData.nMusicMute;

        base.Open(action);
    }

    public override void Close()
    {
        SaveManager.Instance.Save();

        base.Close();
    }
}
