using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarTop : MonoBehaviour
{
    private Level level;
    public Transform uiSignBox;
    public Text moveCount;
    public Text scoreTxt;

    public Dictionary<Goal, GoalSignSlot> dicGoalSignSlot = new Dictionary<Goal, GoalSignSlot>();

    [HideInInspector] public GoalSignSlot chocolateSlot;
    public Image starGageImg;
    public Image[] arrStarImg;
    private Animator[] arrStarAni;

    private int nStarCount = 0;

    private const string sGoalPath = "UI/InGame/GoalSignSlot";
    private const string sStarProgressBarSoundsPath = "Sounds/Game/StarProgressBar";
    private const string sStarGreyPath = "StarGrey";
    private const string sStarYellowPath = "StarYellow";

    public void Init(Level level)
    {
        this.level = level;

        SetMoveCount(level.nMove);
        SetScore(0);

        for (int i = 0; i < level.lisGoal.Count; ++i)
        {
            string spriteName = string.Empty;
            GameObject obj = Instantiate(Resources.Load(sGoalPath) as GameObject, uiSignBox.transform);
            GoalSignSlot goalSignSlot = obj.GetComponent<GoalSignSlot>();

            if (level.lisGoal[i].isTile)
            {
                spriteName = level.lisGoal[i].tileType.ToString();
                if (level.lisGoal[i].tileType == eTileType.Chocolate)
                {
                    chocolateSlot = goalSignSlot;
                }
            }
            else
                spriteName = level.lisGoal[i].elementType.ToString();

            goalSignSlot.Init(UIManager.Instance.GetSprite(eAtlasType.Tile, spriteName), level.lisGoal[i].nCount, new Vector2(90, 90));
            dicGoalSignSlot.Add(level.lisGoal[i], goalSignSlot);
        }

        arrStarAni = new Animator[arrStarImg.Length];
        for (int i = 0; i < arrStarImg.Length; ++i)
        {
            arrStarAni[i] = arrStarImg[i].GetComponent<Animator>();
        }

        nStarCount = 0;

    }

    public void SetMoveCount(int nCount)
    {
        moveCount.text = nCount.ToString();
    }

    public void SetScore(int nScore)
    {
        scoreTxt.text = nScore.ToString();

        starGageImg.fillAmount = nScore / (level.nScore3 * 1.0f);

        if (level.nScore1 > nScore)
        {
            arrStarImg[0].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
            arrStarImg[1].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
            arrStarImg[2].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
        }
        else if (level.nScore1 <= nScore && level.nScore2 > nScore && nStarCount < 1)
        {
            arrStarImg[0].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[1].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
            arrStarImg[2].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);

            arrStarAni[0].SetTrigger("Active");

            ++nStarCount;
            SoundsManager.Instance.Play(sStarProgressBarSoundsPath);
        }
        else if (level.nScore2 <= nScore && level.nScore3 > nScore && nStarCount < 2)
        {
            if (nStarCount != 1)
                arrStarAni[0].SetTrigger("Active");

            arrStarAni[1].SetTrigger("Active");

            arrStarImg[0].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[1].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[2].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);

            ++nStarCount;
            SoundsManager.Instance.Play(sStarProgressBarSoundsPath);
        }
        else if (level.nScore3 <= nScore && nStarCount < 3)
        {
            if (nStarCount == 0)
            {
                arrStarAni[0].SetTrigger("Active");
                arrStarAni[1].SetTrigger("Active");
            }
            if (nStarCount != 2)
                arrStarAni[1].SetTrigger("Active");

            arrStarAni[2].SetTrigger("Active");


            arrStarImg[0].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[1].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            arrStarImg[2].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);

            ++nStarCount;
            SoundsManager.Instance.Play(sStarProgressBarSoundsPath);

        }

    }
}
