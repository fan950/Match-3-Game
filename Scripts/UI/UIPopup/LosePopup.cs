using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class LosePopup : UIPopup
{
    public UIBtn retryBtn;
    public UIBtn closeBtn;

    public Text stageTxt;
    public Text ScoreTxt;

    public Transform goalGroup;

    private const string sGoalPath = "UI/InGame/GoalSignSlot";

    public override void Init(Transform parent)
    {
        base.Init(parent);

        retryBtn.Init(delegate
        {
            GameManager.Instance.NextScene(eScene.InGameScene);
        });

        closeBtn.Init(delegate
        {
            GameManager.Instance.NextScene(eScene.LevelScene);
        });

        sOpenSoundsPath = "Sounds/UI/Lose";
    }
    public void SetFigure(int nScore, Level level)
    {
        ScoreTxt.text = nScore.ToString();
        stageTxt.text = "Stage " + GameManager.Instance.nSelectLevel.ToString();

        for (int i = 0; i < level.lisGoal.Count; ++i)
        {
            GameObject _obj = Instantiate(Resources.Load(sGoalPath) as GameObject, goalGroup);
            GoalSignSlot goalSignSlot = _obj.GetComponent<GoalSignSlot>();

            string spriteName = string.Empty;
            if (level.lisGoal[i].isTile)
                spriteName = level.lisGoal[i].tileType.ToString();
            else
                spriteName = level.lisGoal[i].elementType.ToString();

            goalSignSlot.Init(UIManager.Instance.GetSprite(eAtlasType.Tile, spriteName), level.lisGoal[i].nCount, new Vector2(130, 130));
            goalSignSlot.gameObject.SetActive(true);
        }
    }
}
