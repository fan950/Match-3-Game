using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : UIPopup
{
    public Text stageTxt;
    public Text scoreTxt;

    public Image[] arrStarImg;

    public GameObject[] arrStarGrey;
    public GameObject[] arrStarYellow;

    public UIBtn nextBtn;
    private int nStage;

    private const string sStage = "Stage ";
    private const string sStarGreyPath = "StarGrey";
    private const string sStarYellowPath = "StarYellow";
    public override void Open(Action action = null)
    {
        base.Open(action);
        nextBtn.Init(NextStage);
    }
    public int SetFigure(int nStage, int nScore, int nStarScore1, int nStarScore2, int nStarScore3)
    {
        this.nStage = nStage;
        int _nStarCount = 0;
        stageTxt.text = sStage + nStage.ToString();
        scoreTxt.text = nScore.ToString();

        if (nStarScore1 > nScore)
        {
            arrStarImg[0].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
            arrStarImg[1].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
            arrStarImg[2].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);

            arrStarGrey[0].SetActive(true);
            arrStarGrey[1].SetActive(true);
            arrStarGrey[2].SetActive(true);

            arrStarYellow[0].SetActive(false);
            arrStarYellow[1].SetActive(false);
            arrStarYellow[2].SetActive(false);
        }
        else if (nStarScore1 <= nScore && nStarScore2 > nScore)
        {
            arrStarImg[0].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[1].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
            arrStarImg[2].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);

            arrStarGrey[0].SetActive(false);
            arrStarGrey[1].SetActive(true);
            arrStarGrey[2].SetActive(true);

            arrStarYellow[0].SetActive(true);
            arrStarYellow[1].SetActive(false);
            arrStarYellow[2].SetActive(false);

            _nStarCount = 1;

        }
        else if (nStarScore2 <= nScore && nStarScore3 > nScore)
        {
            arrStarImg[0].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[1].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[2].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);

            arrStarGrey[0].SetActive(false);
            arrStarGrey[1].SetActive(false);
            arrStarGrey[2].SetActive(true);

            arrStarYellow[0].SetActive(true);
            arrStarYellow[1].SetActive(true);
            arrStarYellow[2].SetActive(false);

            _nStarCount = 2;
        }
        else
        {
            arrStarImg[0].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[1].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[2].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);

            arrStarGrey[0].SetActive(false);
            arrStarGrey[1].SetActive(false);
            arrStarGrey[2].SetActive(false);

            arrStarYellow[0].SetActive(true);
            arrStarYellow[1].SetActive(true);
            arrStarYellow[2].SetActive(true);

            _nStarCount = 3;
        }

        sOpenSoundsPath = "Sounds/UI/Win";
        return _nStarCount;
    }

    public void NextStage()
    {
        action();
        Close();
    }
}
