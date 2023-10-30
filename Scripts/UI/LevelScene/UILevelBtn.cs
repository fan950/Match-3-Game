using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelBtn : UIBtn
{
    public int nLevel;

    public Text levelTxt;

    public Image btnImg;
    public Image[] arrStar;

    private const string sStarGreyPath = "StarGrey";
    private const string sStarYellowPath = "StarYellow";
    private const string sBlueLevelButtonPath = "BlueLevelButton";
    private const string sPinkLevelButtonPath = "PinkLevelButton";
    private const string sGreyLevelButtonPath = "GreyLevelButton";

    public void Init(Action<int> action)
    {
        base.Init(delegate
        {
            action(nLevel);
        });
    }
    public void SetLevel(int nIndex)
    {
        levelTxt.text = nLevel.ToString();
        LocalGameData localGameData = SaveManager.Instance.localGameData;

        if (localGameData.nStageLevel > nLevel)
        {
            btnImg.sprite = UIManager.Instance.GetSprite(eAtlasType.Level_UI, sBlueLevelButtonPath);
            for (int i = 0; i < 3; ++i)
            {
                arrStar[i].gameObject.SetActive(true);

                if (localGameData.lisStageStar[nIndex]> i)
                    arrStar[i].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
                else
                    arrStar[i].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
            }
        }
        else if (localGameData.nStageLevel == nLevel)
        {
            btnImg.sprite = UIManager.Instance.GetSprite(eAtlasType.Level_UI, sPinkLevelButtonPath);
            for (int i = 0; i < arrStar.Length; ++i)
            {
                arrStar[i].gameObject.SetActive(false);
            }
        }
        else
        {
            btnImg.sprite = UIManager.Instance.GetSprite(eAtlasType.Level_UI, sGreyLevelButtonPath);
            for (int i = 0; i < arrStar.Length; ++i)
            {
                arrStar[i].gameObject.SetActive(false);
            }
        }
    }
}
