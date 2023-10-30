using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGamePopup : UIPopup
{
    public UIBtn playBtn;
    public UIBtn closeBtn;

    public Text stageTxt;
    public Image[] arrStarImg;

    public Transform goalGroup;

    private List<GoalSignSlot> lisUnSlot = new List<GoalSignSlot>();
    private Dictionary<GameObject, GoalSignSlot> dicGoalSignSlot = new Dictionary<GameObject, GoalSignSlot>();

    private const int nMax = 5;

    private const string sGoalPath = "UI/InGame/GoalSignSlot";
    private const string sStarGreyPath = "StarGrey";
    private const string sStarYellowPath = "StarYellow";
    public override void Init(Transform parent)
    {
        base.Init(parent);

        playBtn.Init(delegate
        {
            GameManager.Instance.NextScene(eScene.InGameScene);
        });
        closeBtn.Init(Close);

        CreateSlot();
    }

    public void CreateSlot()
    {
        for (int i = 0; i < nMax; ++i)
        {
            GameObject _obj = Instantiate(Resources.Load(sGoalPath) as GameObject, goalGroup);
            GoalSignSlot goalSignSlot = _obj.GetComponent<GoalSignSlot>();
            _obj.SetActive(false);
            lisUnSlot.Add(goalSignSlot);
        }
    }
    public override void Open(Action action = null)
    {
        stageTxt.text = "Stage " + GameManager.Instance.nSelectLevel.ToString();
        LocalGameData localGameData = SaveManager.Instance.localGameData;
        if (localGameData.lisStageStar.Count <= GameManager.Instance.nSelectLevel - 1)
        {
            for (int i = 0; i < arrStarImg.Length; ++i)
            {
                arrStarImg[i].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
            }
        }
        else
        {
            for (int i = 0; i < arrStarImg.Length; ++i)
            {
                if (localGameData.lisStageStar[GameManager.Instance.nSelectLevel - 1] <= i)
                    arrStarImg[i].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarGreyPath);
                else
                    arrStarImg[i].sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, sStarYellowPath);
            }
        }
        var jsonText = Resources.Load<TextAsset>("Level/" + GameManager.Instance.nSelectLevel);
        Level level = JsonUtility.FromJson<Level>(jsonText.ToString());

        for (int i = 0; i < level.lisGoal.Count; ++i)
        {
            if (lisUnSlot.Count <= 0)
                CreateSlot();

            GoalSignSlot goalSignSlot = lisUnSlot[0];
            lisUnSlot.RemoveAt(0);

            string spriteName = string.Empty;
            if (level.lisGoal[i].tileType != eTileType.None)
                spriteName = level.lisGoal[i].tileType.ToString();
            else
                spriteName = level.lisGoal[i].elementType.ToString();

            goalSignSlot.Init(UIManager.Instance.GetSprite(eAtlasType.Tile, spriteName), level.lisGoal[i].nCount, new Vector2(130, 130));
            goalSignSlot.gameObject.SetActive(true);
            dicGoalSignSlot.Add(goalSignSlot.gameObject, goalSignSlot);
        }
        base.Open(action);
    }
    public override void Close()
    {
        foreach (KeyValuePair<GameObject, GoalSignSlot> slot in dicGoalSignSlot)
        {
            slot.Key.gameObject.SetActive(false);
            lisUnSlot.Add(slot.Value);
        }
        dicGoalSignSlot.Clear();

        base.Close();
    }
}
