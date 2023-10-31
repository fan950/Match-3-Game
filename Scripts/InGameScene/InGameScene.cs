using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
    private static InGameScene Instance;
    public static InGameScene instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject obj;
                obj = GameObject.Find("InGameScene");
                Instance = obj.GetComponent<InGameScene>();
            }

            return Instance;
        }
    }
    [HideInInspector] public bool isPuase = false;
    [HideInInspector] public bool isEnd = false;

    [Header("UI")]
    [SerializeField] private BarTop barTop;
    [SerializeField] private ItemList itemList;
    [SerializeField] private GameObject uiApplyItemBg;
    [SerializeField] private UIComboGroup uiComboGroup;
    [SerializeField] private UIBtn pauseBtn;

    [Header("Obj")]
    public Transform uiPopupPos;
    [SerializeField] private GameBoard gameBoard;

    [Header("Pool")]
    public TilePool tilePool;
    public FxPool fxPool;
    public ElementPool elementPool;
    public UITilePool uiTilePool;

    private Level level;
    private List<Goal> lisTempGoal = new List<Goal>();

    private Coroutine endGameCoro;
    [HideInInspector] public bool isApplyItem;

    private const string sClearPopupPath = "Popups/ClearPopup";
    private const string sPausePupupPath = "Popups/PausePupup";
    private const string sReadyPopupPath = "Popups/ReadyPopup";

    private const string sBGMPath = "Sounds/InGameMusic";

    public void Start()
    {
        SoundsManager.Instance.PlayBGM(sBGMPath);
        GlobalCanvas.Instance.Fade(false, null);

        ReadyPopup _uIPopup = UIManager.Instance.OpenPopup(sReadyPopupPath, uiPopupPos).GetComponent<ReadyPopup>();
        _uIPopup.Open();

        isEnd = false;
        isPuase = true;
        uiApplyItemBg.SetActive(false);

        var jsonText = Resources.Load<TextAsset>("Level/" + GameManager.Instance.nSelectLevel);
        level = JsonUtility.FromJson<Level>(jsonText.ToString());

        for (int i = 0; i < level.lisGoal.Count; ++i)
        {
            if (level.lisGoal[i].tileType != eTileType.None)
                level.lisGoal[i].isTile = true;
            else
                level.lisGoal[i].isTile = false;
        }
        //Pool
        fxPool.Init();
        tilePool.Init();
        elementPool.Init();
        uiTilePool.Init();

        //UI
        barTop.Init(level);
        itemList.Init(SaveManager.Instance.localGameData);
        uiComboGroup.Init();
        pauseBtn.Init(delegate
        {
            isPuase = true;
            PausePupup _uIPopup = UIManager.Instance.OpenPopup(sPausePupupPath, uiPopupPos).GetComponent<PausePupup>();
            _uIPopup.Open();
        });

        //obj
        gameBoard.Init(level);
    }

    public void ChangeChocolateText(int nCount)
    {
        barTop.chocolateSlot.countTxt.text = nCount.ToString();
    }
    public void CheckGoalTile(Tile tile, Element element)
    {
        lisTempGoal.Clear();
        for (int i = 0; i < level.lisGoal.Count; ++i)
        {
            if (level.lisGoal[i].tileType == tile.tileType || (element != null && level.lisGoal[i].elementType == element.elementType))
            {
                uiTilePool.CallUITile(level.lisGoal[i].isTile, level.lisGoal[i], tile, element, RemoveGoal);
            }
        }
    }
    public GoalSignSlot GetGoalSlot(Goal goal) 
    {
        return barTop.dicGoalSignSlot[goal];
    }
    public void RemoveGoal(Tile tile, Element element)
    {
        for (int i = 0; i < level.lisGoal.Count; ++i)
        {
            if (level.lisGoal[i].tileType == tile.tileType || (element != null && level.lisGoal[i].elementType == element.elementType))
            {
                --level.lisGoal[i].nCount;

                if (barTop.dicGoalSignSlot.ContainsKey(level.lisGoal[i]))
                {
                    barTop.dicGoalSignSlot[level.lisGoal[i]].countTxt.text = level.lisGoal[i].nCount.ToString();
                }

                if (level.lisGoal[i].nCount <= 0)
                {
                    lisTempGoal.Add(level.lisGoal[i]);
                }
            }
        }

        if (lisTempGoal.Count > 0)
        {
            for (int i = 0; i < lisTempGoal.Count; ++i)
            {
                level.lisGoal.Remove(lisTempGoal[i]);
            }
        }

        if (level.lisGoal.Count <= 0)
        {
            isPuase = true;
            if (endGameCoro == null)
                endGameCoro = StartCoroutine(EndGame(true));
        }
    }
    public IEnumerator EndGame(bool isClear)
    {
        EndItemMode(false, false);

        yield return new WaitForSeconds(2.0f);

        while (true)
        {
            yield return null;
            if (!gameBoard.isPlayAni && !gameBoard.isTileMove)
            {
                break;
            }
        }

        if (isClear)
        {
            ClearPopup _uIPopup = UIManager.Instance.OpenPopup(sClearPopupPath, uiPopupPos).GetComponent<ClearPopup>();
            _uIPopup.SetPopupAction(delegate
            {
                StartCoroutine(gameBoard.ChangeBonusTile());
            });
            _uIPopup.Open();
        }
    }

    public void SetMoveCount(int nCount)
    {
        SetMoveCountTxt(nCount);
    }
    public void SetMoveCountTxt(int nCount)
    {
        barTop.SetMoveCount(nCount);

    }
    public void SetScore(int nCount, int nCombo)
    {
        barTop.SetScore(nCount);
        if (nCombo > 0)
            uiComboGroup.SetCombo(nCombo);
    }

    #region Item
    public void ApplyItemMode()
    {
        if ((gameBoard.isPlayAni || gameBoard.isTileMove) || level.lisGoal.Count <= 0)
        {
            return;
        }
        isApplyItem = true;
        uiApplyItemBg.SetActive(true);

        if (GetItemType() == eItemType.Lollipop)
        {
            foreach (KeyValuePair<GameObject, Tile> tile in gameBoard.dicTile)
            {
                if (tile.Value.tileType == eTileType.Chocolate || tile.Value.tileType == eTileType.Marshmallow)
                {
                    tile.Value.boxCollider.enabled = true;
                }
            }
        }
    }
    public void EndItemMode(bool isUse, bool isActive)
    {
        if (isUse)
        {
            UseItem();
        }

        if (GetItemType() == eItemType.Lollipop)
        {
            foreach (KeyValuePair<GameObject, Tile> tile in gameBoard.dicTile)
            {
                if (tile.Value.tileType == eTileType.Chocolate || tile.Value.tileType == eTileType.Marshmallow)
                {
                    tile.Value.boxCollider.enabled = false;
                }
            }
        }

        isApplyItem = isActive;
        uiApplyItemBg.SetActive(isActive);
    }

    public eItemType GetItemType()
    {
        return itemList.itemType;
    }

    public void UseItem()
    {
        switch (itemList.itemType)
        {
            case eItemType.Lollipop:
                --SaveManager.Instance.localGameData.nLollipop;
                break;
            case eItemType.All:
                --SaveManager.Instance.localGameData.nAll;
                break;
            case eItemType.Switch:
                --SaveManager.Instance.localGameData.nSwitch;
                break;
            case eItemType.ColorBomb:
                --SaveManager.Instance.localGameData.nColorBomb;
                break;
        }
        itemList.SetCount();
        SaveManager.Instance.Save();
    }
    #endregion
}
