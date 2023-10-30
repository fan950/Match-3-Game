using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private bool isOneSet;
    [HideInInspector] public int nSelectLevel;
    [HideInInspector] public bool isStageMove;
    public void GameSet()
    {
        if (isOneSet)
            return;
        isOneSet = true;

        nSelectLevel = SaveManager.Instance.localGameData.nStageLevel;

        SoundsManager.Instance.SetMusicMute();
        SoundsManager.Instance.SetSoundMute();

        SoundsManager.Instance.SetSoundVolume();
        SoundsManager.Instance.SetMusicVolume();
    }
    public void NextScene(eScene scene)
    {
        GlobalCanvas.Instance.Fade(true, delegate
        {
            SceneManager.LoadScene(scene.ToString());
            UIManager.Instance.UIPopupClear();
        });
    }
}
