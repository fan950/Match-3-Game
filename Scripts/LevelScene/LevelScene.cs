using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScene : MonoBehaviour
{
    [Header("UI")]
    public UILevelBtn[] arrLevelBtn;
    public UIBtn soundsBtn;

    [Header("Obj")]
    public Transform uiPopupPos;
    public RectTransform uiDirectionPos;

    private Coroutine moveCoro;

    private WaitForSeconds delayTime = new WaitForSeconds(1.5f);
    //path
    private string sClearPopupPath = "Popups/StartGamePopup";
    private string sSettingsPopupPath = "Popups/SettingsPopup";
    private string sBGMPath = "Sounds/BackGroundMusic";
    public void Awake()
    {
        GameManager.Instance.GameSet();
    }

    public void Start()
    {
        SoundsManager.Instance.PlayBGM(sBGMPath);

        GlobalCanvas.Instance.Fade(false, null);

        for (int i = 0; i < arrLevelBtn.Length; ++i)
        {
            arrLevelBtn[i].SetLevel(i);
            arrLevelBtn[i].Init(OpenStartGame);
        }

        soundsBtn.Init(delegate
        {
            SettingsPopup _uIPopup = UIManager.Instance.OpenPopup(sSettingsPopupPath, uiPopupPos).GetComponent<SettingsPopup>();
            _uIPopup.Open();
        });

        uiDirectionPos.transform.position = arrLevelBtn[GameManager.Instance.nSelectLevel - 1].transform.position;

        if (GameManager.Instance.isStageMove)
        {
            GameManager.Instance.isStageMove = false;
            moveCoro = StartCoroutine(MoveDirection());
        }
    }

    public void OpenStartGame(int nLevel)
    {
        if (SaveManager.Instance.localGameData.nStageLevel < nLevel)
            return;

        GameManager.Instance.nSelectLevel = nLevel;

        StartGamePopup _uIPopup = UIManager.Instance.OpenPopup(sClearPopupPath, uiPopupPos).GetComponent<StartGamePopup>();
        _uIPopup.Open(delegate
        {
            if (moveCoro != null)
                StopCoroutine(moveCoro);
        });
    }

    public IEnumerator MoveDirection()
    {
        yield return delayTime;

        while (true)
        {
            yield return null;

            uiDirectionPos.transform.position = Vector3.MoveTowards(uiDirectionPos.transform.position,
                arrLevelBtn[SaveManager.Instance.localGameData.nStageLevel - 1].transform.position,
                Time.deltaTime);

            if (uiDirectionPos.transform.position == arrLevelBtn[SaveManager.Instance.localGameData.nStageLevel - 1].transform.position)
            {
                break;
            }
        }
    }
}
