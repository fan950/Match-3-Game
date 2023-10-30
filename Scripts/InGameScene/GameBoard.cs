using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameBoard : MonoBehaviour
{
    private Level level;

    //Type
    public eTileType[,] arrTileType = new eTileType[,] { };
    public eElementType[,] arrElementType = new eElementType[,] { };

    //Element
    public Element[,] arrElement = new Element[,] { };
    [HideInInspector] public List<Element> lisElement_Ice = new List<Element>();
    [HideInInspector] private List<Element> lisElement_Honey = new List<Element>();
    [HideInInspector] private List<Element> lisElement_Syrup1 = new List<Element>();
    [HideInInspector] private List<Element> lisElement_Syrup2 = new List<Element>();

    //Tile
    private List<Tile> lisRespawnTile = new List<Tile>();
    private List<Tile> lisTileMove = new List<Tile>();
    private List<Tile> lisTempMove = new List<Tile>();

    public Tile[,] arrTile = new Tile[,] { };
    private Tile selectTile = null;

    public Dictionary<GameObject, Tile> dicTile = new Dictionary<GameObject, Tile>();
    private List<Tile> lisExplodeTile = new List<Tile>();
    private List<Tile> lisSupportTile = new List<Tile>();

    private List<Vector2Int> lisEmptyPos = new List<Vector2Int>();
    private Dictionary<Vector2Int, Vector3> dicFixPos = new Dictionary<Vector2Int, Vector3>();

    //3개 이상의 맞는 타일 찾는 클래스
    private SearchMatch searchMatch = new SearchMatch();

    private Dictionary<eTileType, SearchMatch> dicSpecialList = new Dictionary<eTileType, SearchMatch>();

    [HideInInspector] public bool isPlayAni = false;
    [HideInInspector] public bool isTileMove = false;

    private int nEmptyCount;
    private int nTileMax;
    private List<int> lisRespawnX = new List<int>();

    private float fSupportTime = 0;

    public int nScore;
    private int nCombo = 0;
    private int nStarCount;

    private bool isWin = false;
    private bool isLose = false;

    //상수
    private const float fSupportTimeMax = 10;
    private const float fSpacePos = 1.31f;
    private const float fMoveSpeed = 7f;
    public const int nBaseScore = 30;
    public const int nComboScore = 10;

    //팝업 경로
    private const string sWinPopupPath = "Popups/WinPopup";
    private const string sLosePopupPath = "Popups/LosePopup";

    //사운드 경로
    private const string sFallingSoundsPath = "Sounds/Game/CandyFalling";
    private const string sMoveSoundsPath = "Sounds/Game/CandyMove";
    private const string sExplodeSoundsPath = "Sounds/Game/CandyExplode";
    private const string sColorBombSoundsPath = "Sounds/Game/ColorBomb";
    private const string sChocolateSoundsPath = "Sounds/Game/Chocolate";
    private const string sMarshmallowSoundsPath = "Sounds/Game/Marshmallow";
    private const string sIceSoundsPath = "Sounds/Game/Ice";
    private const string sHoneySoundsPath = "Sounds/Game/Honey";
    private const string sSyrupSoundsPath = "Sounds/Game/Syrup";
    private const string sChangeSoundsPath = "Sounds/Game/CandyChange";
    private const string sRespawnSoundsPath = "Sounds/Game/Respawn";
    private const string sChocolateRespawnSoundsPath = "Sounds/Game/sChocolateRespawn";

    //Coroutine
    private Coroutine moveAniCoro;
    private Coroutine GravityCoro;
    private Coroutine supportCoro;

    //CoroutineTime
    private WaitForSeconds respawnWait = new WaitForSeconds(0.15f);
    private WaitForSeconds gravityDownWait = new WaitForSeconds(0.1f);
    private WaitForSeconds applyGravityWait = new WaitForSeconds(0.6f);
    private WaitForSeconds changeTileWait = new WaitForSeconds(0.25f);
    private WaitForSeconds endTileWait = new WaitForSeconds(1f);

    public void Init(Level level)
    {
        isWin = false;
        isLose = false;

        float _posY = 0;

        float _fCameraMaxX = 0;
        float _fCameraMinY = 0;

        Tile _tile = null;
        Element _element = null;

        this.level = level;
        arrTileType = new eTileType[level.nHeight, level.nWidth];
        arrElementType = new eElementType[level.nHeight, level.nWidth];

        int _nTileIndex = 0;
        for (int i = 0; i < level.nHeight; ++i)
        {
            for (int j = 0; j < level.nWidth; ++j)
            {
                arrTileType[i, j] = level.lisTileType[_nTileIndex];
                arrElementType[i, j] = level.lisElementType[_nTileIndex];
                ++_nTileIndex;
            }
        }

        isTileMove = false;
        arrTile = new Tile[arrTileType.GetLength(0), arrTileType.GetLength(1)];
        arrElement = new Element[arrTileType.GetLength(0), arrTileType.GetLength(1)];

        int _nChocolate = 0;
        //레벨에 맞게 타일 세팅
        for (int y = 0; y < arrTileType.GetLength(0); ++y)
        {
            float _posX = 0;
            for (int x = 0; x < arrTileType.GetLength(1); ++x)
            {
                if (arrTileType[y, x] == eTileType.RandomCandy)
                {
                    arrTileType[y, x] = ChangeRandomType(x, y);
                }
                _tile = InGameScene.instance.tilePool.GetTileType_Tile(arrTileType[y, x]);
                _element = InGameScene.instance.elementPool.GetElement(arrElementType[y, x]);

                if (_tile != null)
                {
                    _tile.Init(x, y);
                    _tile.transform.SetParent(transform);
                    _tile.transform.position = new Vector3(_posX, _posY, 0);

                    InGameScene.instance.tilePool.GetTileBg().position = _tile.transform.position;
                    if (!dicTile.ContainsKey(_tile.gameObject))
                        dicTile.Add(_tile.gameObject, _tile);

                    if (_tile.tileType == eTileType.Chocolate)
                    {
                        ++_nChocolate;
                    }
                }

                if (_element != null)
                {
                    _element.Init(x, y);
                    _element.transform.position = _tile.transform.position;
                    _tile.transform.SetParent(transform);

                    switch (_element.elementType)
                    {
                        case eElementType.Honey:
                            lisElement_Honey.Add(_element);
                            break;
                        case eElementType.Ice:
                            lisElement_Ice.Add(_element);
                            break;
                        case eElementType.Syrup1:
                            lisElement_Syrup1.Add(_element);
                            break;
                        case eElementType.Syrup2:
                            lisElement_Syrup2.Add(_element);
                            break;
                    }
                }

                _fCameraMaxX = Mathf.Max(_fCameraMaxX, _posX);
                _posX += fSpacePos;
                arrTile[y, x] = _tile;
                arrElement[y, x] = _element;
            }
            _fCameraMinY = Mathf.Min(_fCameraMinY, _posY);
            _posY -= fSpacePos;
        }

        for (int i = 0; i < level.lisGoal.Count; ++i)
        {
            if (level.lisGoal[i].tileType == eTileType.Chocolate)
            {
                level.lisGoal[i].nCount = _nChocolate;
                InGameScene.instance.ChangeChocolateText(level.lisGoal[i].nCount);
                break;
            }
        }

        for (int i = 0; i < arrTile.GetLength(1); ++i)
        {
            if (arrTileType[0, i] != eTileType.None)
            {
                lisRespawnX.Add(i);
            }
        }

        foreach (KeyValuePair<GameObject, Tile> tile in dicTile)
        {
            dicFixPos.Add(new Vector2Int(tile.Value.nPosY, tile.Value.nPosX), tile.Key.transform.position);
        }
        nTileMax = dicTile.Count;

        //오브젝트 카메라 조정
        Camera.main.transform.position = new Vector3(_fCameraMaxX * 0.5f, _fCameraMinY * 0.5f, -10);
        //플립과 다른 기기 구분
        if (Screen.width * 1.0f / Screen.height <= 0.37f)
        {
            Camera.main.orthographicSize = level.nWidth * 0.65f;
        }
        else
        {
            Camera.main.orthographicSize = level.nWidth * 1.5f;
        }

        StartCoroutine(MoveUpdate());
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetTile();
        }
        if (InGameScene.instance.isPuase)
            return;

        if (InGameScene.instance.isEnd)
            return;

        fSupportTime += Time.deltaTime;

        if (fSupportTime >= fSupportTimeMax)
        {
            if (supportCoro == null)
            {
                supportCoro = StartCoroutine(SupportMatch());
                fSupportTime = 0;
            }
        }

        //타일의 터치 및 드래그
        if (!isPlayAni && !isTileMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectTile = null;
                nCombo = 0;

                SupportReset();

                var _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (_hit.collider != null && dicTile.ContainsKey(_hit.collider.gameObject))
                {
                    if (InGameScene.instance.isApplyItem)
                    {
                        switch (InGameScene.instance.GetItemType())
                        {
                            case eItemType.Lollipop:
                                RemoveLollipop(dicTile[_hit.collider.gameObject]);
                                break;
                            case eItemType.Switch:
                                selectTile = dicTile[_hit.collider.gameObject];
                                selectTile.animator.SetTrigger("Pressed");
                                break;
                            case eItemType.All:
                                if (dicTile[_hit.collider.gameObject].tileLine == eTileLine.Normal)
                                    MakeItem(dicTile[_hit.collider.gameObject], eItemType.All);
                                break;
                            case eItemType.ColorBomb:
                                MakeItem(dicTile[_hit.collider.gameObject], eItemType.ColorBomb);
                                break;
                        }

                        if (InGameScene.instance.GetItemType() == eItemType.Switch)
                            InGameScene.instance.EndItemMode(true, true);
                        else
                            InGameScene.instance.EndItemMode(true, false);

                        return;
                    }
                    selectTile = dicTile[_hit.collider.gameObject];
                    selectTile.animator.SetTrigger("Pressed");
                }
                else
                {
                    if (InGameScene.instance.isApplyItem)
                    {
                        InGameScene.instance.EndItemMode(false, false);
                        return;
                    }
                }
            }

            if (selectTile != null && moveAniCoro == null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    var _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (_hit.collider != null && dicTile.ContainsKey(_hit.collider.gameObject))
                    {
                        if (selectTile.gameObject != _hit.collider.gameObject)
                        {
                            bool _isMove = false;

                            if ((Math.Abs(selectTile.nPosX - dicTile[_hit.collider.gameObject].nPosX) <= 1 && selectTile.nPosY == dicTile[_hit.collider.gameObject].nPosY) ||
                                (selectTile.nPosX == dicTile[_hit.collider.gameObject].nPosX && Math.Abs(selectTile.nPosY - dicTile[_hit.collider.gameObject].nPosY) == 1))
                            {
                                _isMove = true;
                            }
                            else
                                return;

                            if (_isMove && moveAniCoro == null)
                            {
                                if (IsIce(selectTile.nPosX, selectTile.nPosY) || IsIce(dicTile[_hit.collider.gameObject].nPosX, dicTile[_hit.collider.gameObject].nPosY))
                                {
                                    selectTile = null;
                                    return;
                                }

                                if (InGameScene.instance.isApplyItem)
                                {
                                    moveAniCoro = StartCoroutine(SwitchAni(dicTile[selectTile.gameObject], dicTile[_hit.collider.gameObject]));
                                    InGameScene.instance.EndItemMode(false, false);
                                    return;
                                }

                                moveAniCoro = StartCoroutine(MoveTileAni(selectTile, dicTile[_hit.collider.gameObject]));
                            }
                            else
                                selectTile = null;

                        }
                    }
                }
            }
        }

    }

    //타일의 이동 후 해당 타일과 같은 타일 찾는 함수
    public void DragMatch(Tile tile, bool isBomb = false)
    {
        if (lisExplodeTile.Contains(tile))
            return;

        if (tile.tileType == eTileType.Chocolate)
            return;

        if (tile.tileType == eTileType.ColorBomb)
        {
            lisExplodeTile.Add(tile);
            return;
        }
        List<TileMatch> _lisTileMatch = new List<TileMatch>();

        var _searchMatch = searchMatch.HandleMatch(this, tile);
        if (_searchMatch.Count >= 2)
        {
            for (int i = 0; i < _searchMatch.Count; ++i)
            {
                if (!lisExplodeTile.Contains(_searchMatch[i]))
                {
                    lisExplodeTile.Add(_searchMatch[i]);
                }
            }

            _lisTileMatch.Add(searchMatch);

            if (_searchMatch.Count >= 4)
            {
                bool _isNormal = true;
                for (int i = 0; i < _searchMatch.Count; ++i)
                {
                    if (_searchMatch[i].tileLine != eTileLine.Normal)
                    {
                        _isNormal = false;
                        break;
                    }
                }

                if (!_isNormal)
                    return;

                if (dicSpecialList.ContainsKey(tile.tileType))
                    return;

                SearchMatch tempMatch = new SearchMatch();
                tempMatch.nTileCount = searchMatch.nTileCount;
                tempMatch.isWight = searchMatch.isWight;
                tempMatch.isHeight = searchMatch.isHeight;
                dicSpecialList.Add(tile.tileType, tempMatch);
            }
        }
    }
    #region TileMove
    //타일의 이동 코루틴
    public IEnumerator MoveUpdate()
    {
        while (true)
        {
            yield return null;

            if (lisTileMove.Count > 0)
            {
                isTileMove = true;
                for (int i = 0; i < lisTileMove.Count; ++i)
                {
                    lisTileMove[i].Move(fMoveSpeed);
                    if (lisTileMove[i].lisNextPos.Count <= 0)
                    {
                        if (NextTile(lisTileMove[i]) == false)
                        {
                            lisTempMove.Add(lisTileMove[i]);
                        }
                    }
                }
                for (int i = 0; i < lisTempMove.Count; ++i)
                {
                    if (lisTileMove.Contains(lisTempMove[i]))
                        lisTileMove.Remove(lisTempMove[i]);
                }
            }
            else
            {
                if (dicTile.Count + nEmptyCount >= nTileMax)
                {
                    if (GravityCoro == null)
                    {
                        if (lisTileMove.Count <= 0 && lisRespawnTile.Count <= 0)
                        {
                            if (lisTempMove.Count > 0)
                            {
                                for (int i = 0; i < lisTempMove.Count; ++i)
                                {
                                    DragMatch(lisTempMove[i]);
                                }
                                lisTempMove.Clear();
                            }
                        }
                        if (lisExplodeTile.Count > 0)
                        {
                            nCombo += 1;
                            Explode();
                        }
                        else
                        {
                            isPlayAni = false;
                        }

                        if (isWin)
                        {
                            InGameScene.instance.isEnd = true;

                            yield return endTileWait;

                            WinPopup _uIPopup = UIManager.Instance.OpenPopup(sWinPopupPath, InGameScene.instance.uiPopupPos).GetComponent<WinPopup>();
                            nStarCount = _uIPopup.SetFigure(level.nLevel, nScore, level.nScore1, level.nScore2, level.nScore3);
                            _uIPopup.Open(delegate
                            {
                                if (level.nLevel == SaveManager.Instance.localGameData.nStageLevel)
                                {
                                    GameManager.Instance.isStageMove = true;
                                    SaveManager.Instance.localGameData.nStageLevel = level.nLevel + 1;
                                    SaveManager.Instance.localGameData.lisStageStar.Add(nStarCount);
                                    SaveManager.Instance.Save();
                                }
                                else
                                {
                                    if (SaveManager.Instance.localGameData.lisStageStar[level.nLevel - 1] < nStarCount)
                                    {
                                        SaveManager.Instance.localGameData.lisStageStar[level.nLevel - 1] = nStarCount;
                                        SaveManager.Instance.Save();
                                    }
                                }
                                GameManager.Instance.NextScene(eScene.LevelScene);
                            });

                            isWin = false;
                            break;
                        }
                        else if (isLose && level.lisGoal.Count > 0)
                        {
                            yield return endTileWait;

                            LosePopup _uIPopup = UIManager.Instance.OpenPopup(sLosePopupPath, InGameScene.instance.uiPopupPos).GetComponent<LosePopup>();
                            _uIPopup.SetFigure(nScore, level);
                            _uIPopup.Open(delegate
                            {
                                GameManager.Instance.NextScene(eScene.LevelScene);
                            });

                            isLose = false;
                            break;
                        }
                        isTileMove = false;
                    }
                }
            }
        }
    }
    //타일의 이동 경로 결정 =>  아래 => 오른쪽 아래 => 왼쪽 아래 로 탐색
    public bool NextTile(Tile tile)
    {
        Tile _tempTile = tile;

        int _nNextY = tile.nPosY + 1;
        int _nLeftX = tile.nPosX - 1;
        int _nRightX = tile.nPosX + 1;

        if (arrTile.GetLength(0) <= _nNextY)
            return false;

        if (IsMoveCenter(tile.nPosX, _nNextY))
        {
            arrTile[_tempTile.nPosY, _tempTile.nPosX] = null;
            arrTile[_nNextY, _tempTile.nPosX] = _tempTile;
            arrTile[_nNextY, _tempTile.nPosX].AddNextPos(_tempTile.nPosX, _nNextY, dicFixPos[new Vector2Int(_nNextY, _tempTile.nPosX)]);
            return true;
        }
        else if (IsMoveRight(_nRightX, _nNextY) && GetTile(_nRightX, _tempTile.nPosY) == null)
        {
            arrTile[_tempTile.nPosY, _tempTile.nPosX] = null;
            arrTile[_nNextY, _nRightX] = _tempTile;
            arrTile[_nNextY, _nRightX].AddNextPos(_nRightX, _nNextY, dicFixPos[new Vector2Int(_nNextY, _nRightX)]);
            return true;
        }
        else if (IsMoveLeft(_nLeftX, _nNextY) && GetTile(_nLeftX, _tempTile.nPosY) == null)
        {
            arrTile[_tempTile.nPosY, _tempTile.nPosX] = null;
            arrTile[_nNextY, _nLeftX] = _tempTile;
            arrTile[_nNextY, _nLeftX].AddNextPos(_nLeftX, _nNextY, dicFixPos[new Vector2Int(_nNextY, _nLeftX)]);
            return true;
        }

        return false;
    }

    //폭파 후 타일을 밑으로 내리는 코루틴
    public IEnumerator ApplyGravity()
    {
        if (InGameScene.instance.isEnd)
        {
            StopCoroutine(GravityCoro);
        }
        SupportReset();
        List<Tile> _lisTemp = new List<Tile>();

        yield return applyGravityWait;

        nEmptyCount = 0;

        for (int i = 0; i < lisEmptyPos.Count; ++i)
        {
            for (int j = lisEmptyPos[i].y; j >= 0; --j)
            {
                Tile _tempTile = GetTile(lisEmptyPos[i].x, j);
                if (_tempTile != null && (arrElement[j, lisEmptyPos[i].x] == null || arrElement[j, lisEmptyPos[i].x].elementType != eElementType.Ice))
                {
                    bool _isMove = false;
                    while (true)
                    {
                        int _nNextY = _tempTile.nPosY + 1;
                        if (IsMoveCenter(_tempTile.nPosX, _nNextY))
                        {
                            arrTile[_tempTile.nPosY, _tempTile.nPosX] = null;
                            arrTile[_nNextY, _tempTile.nPosX] = _tempTile;
                            arrTile[_nNextY, _tempTile.nPosX].AddNextPos(_tempTile.nPosX, _nNextY, dicFixPos[new Vector2Int(_nNextY, _tempTile.nPosX)]);

                            _isMove = true;
                        }
                        else
                        {
                            if (_isMove)
                            {
                                if (!lisTileMove.Contains(_tempTile))
                                    lisTileMove.Add(_tempTile);
                            }

                            break;
                        }
                    }
                }
            }
        }

        for (int i = arrTile.GetLength(0) - 1; i >= 0; --i)
        {
            for (int j = 0; j < arrTile.GetLength(1); ++j)
            {
                bool _isAdd = false;
                Tile _tempTile = GetTile(j, i);

                //if (((j - 1 >= 0 && j + 1 < arrTileType.GetLength(1) &&
                //    (arrTileType[i, j - 1] == eTileType.None || arrTileType[i, j + 1] == eTileType.None)) ||
                //    GetTile(j, i - 1) == null) &&
                if (_tempTile != null && arrElement[i, j] == null)
                {
                    while (true)
                    {
                        if (NextTile(_tempTile) == false)
                        {
                            break;
                        }
                        _isAdd = true;
                    }

                    if (_isAdd)
                    {
                        if (!_lisTemp.Contains(_tempTile))
                            _lisTemp.Add(_tempTile);
                    }
                }
            }
        }

        if (_lisTemp.Count > 0)
        {
            yield return StartCoroutine(DelayAddTile(_lisTemp));
        }

        if (lisElement_Ice.Count > 0)
        {
            for (int i = 0; i < lisElement_Ice.Count; ++i)
            {
                for (int j = lisElement_Ice[i].nPosY + 1; j < arrTileType.GetLength(0); ++j)
                {
                    if (j < arrTileType.GetLength(0) && arrTileType[j, lisElement_Ice[i].nPosX] != eTileType.None && GetTile(lisElement_Ice[i].nPosX, j) == null && !IsIce(lisElement_Ice[i].nPosX, j))
                    {
                        ++nEmptyCount;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        int _nTileCount = nTileMax - dicTile.Count - nEmptyCount;

        for (int i = 0; i < _nTileCount; ++i)
        {
            TileRespawn();
        }

        for (int i = 0; i < lisRespawnTile.Count; ++i)
        {
            if (lisEmptyPos.Count == 0)
                break;
            yield return respawnWait;
            lisRespawnTile[i].gameObject.SetActive(true);
            lisTileMove.Add(lisRespawnTile[i]);
        }
        lisRespawnTile.Clear();

        if (level.nMove <= 0)
        {
            isLose = true;
        }

        GravityCoro = null;
    }
    //딜레이를 넣어 이동중인 타일을 이동 시킨 후 리스트의 타일을 이동시키는 코루틴
    public IEnumerator DelayAddTile(List<Tile> lisTile)
    {
        yield return gravityDownWait;

        if (!lisTileMove.Contains(lisTile[0]))
            lisTileMove.Add(lisTile[0]);

        yield return gravityDownWait;

        for (int i = 1; i < lisTile.Count; ++i)
        {
            if (!lisTileMove.Contains(lisTile[i]))
                lisTileMove.Add(lisTile[i]);

            if (i != lisTile.Count - 1)
                yield return gravityDownWait;
        }
    }
    //타일 이동 후 타일 재배치하는 코루틴
    private IEnumerator MoveTileAni(Tile _selectTile, Tile swapTile, bool isMatch = true)
    {
        SoundsManager.Instance.Play(sMoveSoundsPath);

        Vector3 _vecSelect = _selectTile.transform.position;
        Vector3 _vecSwap = swapTile.transform.position;

        SupportReset();

        while (true)
        {
            yield return null;

            _selectTile.transform.position = Vector3.MoveTowards(_selectTile.transform.position, _vecSwap, Time.deltaTime * fMoveSpeed);
            swapTile.transform.position = Vector3.MoveTowards(swapTile.transform.position, _vecSelect, Time.deltaTime * fMoveSpeed);

            if (_selectTile.transform.position == _vecSwap)
            {
                Tile _tempSelectTile = _selectTile;

                int _tempSwapX = swapTile.nPosX;
                int _tempSwapY = swapTile.nPosY;

                int _tempSelectX = _tempSelectTile.nPosX;
                int _tempSelectY = _tempSelectTile.nPosY;

                arrTile[_tempSelectY, _tempSelectX] = swapTile;
                arrTile[_tempSelectY, _tempSelectX].ChangeTile(_tempSelectX, _tempSelectY);

                arrTile[_tempSwapY, _tempSwapX] = _tempSelectTile;
                arrTile[_tempSwapY, _tempSwapX].ChangeTile(_tempSwapX, _tempSwapY);

                if (isMatch)
                {
                    lisExplodeTile.Clear();

                    DragMatch(_selectTile, true);
                    DragMatch(swapTile, true);

                    --level.nMove;
                    InGameScene.instance.SetMoveCount(level.nMove);

                    if (level.nMove <= 0)
                        isLose = true;

                    moveAniCoro = null;

                    if (lisExplodeTile.Count < 3)
                    {
                        if (_selectTile.tileType == eTileType.ColorBomb || swapTile.tileType == eTileType.ColorBomb)
                            Explode();
                        else
                        {
                            StartCoroutine(MoveTileAni(swapTile, _selectTile, false));
                            SoundsManager.Instance.Play(sFallingSoundsPath);
                        }
                    }
                    else
                    {
                        Explode();
                    }
                }
                else
                {
                    foreach (KeyValuePair<GameObject, Tile> _tile in dicTile)
                    {
                        if (_tile.Value.tileType == eTileType.Chocolate)
                        {
                            ChangeChocolate();
                            break;
                        }
                    }
                }
                break;
            }
        }
    }
    #endregion
    #region ChangeTile
    public eTileType ChangeRandomType(int x, int y)
    {
        List<eTileType> _lisColor = new List<eTileType>();

        for (int w = 1; w < (int)eTileType.OrangeCandy; ++w)
        {
            _lisColor.Add((eTileType)w);
        }

        if (x - 2 >= 0)
        {
            if (arrTileType[y, x - 1] == arrTileType[y, x - 2] && arrTileType[y, x - 1] != eTileType.None)
            {
                if (_lisColor.Contains(arrTileType[y, x - 1]))
                    _lisColor.Remove(arrTileType[y, x - 1]);
            }
        }

        if (y - 2 >= 0)
        {
            if (arrTileType[y - 1, x] == arrTileType[y - 2, x] && arrTileType[y - 1, x] != eTileType.None)
            {
                if (_lisColor.Contains(arrTileType[y - 1, x]))
                    _lisColor.Remove(arrTileType[y - 1, x]);
            }
        }

        return _lisColor[UnityEngine.Random.Range(0, _lisColor.Count)];
    }
    //초콜릿으로 타일을 변경
    public void ChangeChocolate()
    {
        Tile _changeTile = null;
        foreach (KeyValuePair<GameObject, Tile> _tile in dicTile)
        {
            if (_tile.Value.tileType == eTileType.Chocolate)
            {
                List<Tile> _temp = new List<Tile>();
                List<Tile> _lisTile = new List<Tile>();

                int _nPosX = _tile.Value.nPosX;
                int _nPosY = _tile.Value.nPosY;

                _temp.Add(GetTile(_nPosX - 1, _nPosY - 1));
                _temp.Add(GetTile(_nPosX, _nPosY - 1));
                _temp.Add(GetTile(_nPosX + 1, _nPosY - 1));
                _temp.Add(GetTile(_nPosX - 1, _nPosY));
                _temp.Add(GetTile(_nPosX + 1, _nPosY));
                _temp.Add(GetTile(_nPosX - 1, _nPosY + 1));
                _temp.Add(GetTile(_nPosX, _nPosY + 1));
                _temp.Add(GetTile(_nPosX + 1, _nPosY + 1));

                for (int i = 0; i < _temp.Count; ++i)
                {
                    if (_temp[i] != null && _temp[i].tileType != eTileType.Chocolate)
                        _lisTile.Add(_temp[i]);
                }
                if (_lisTile.Count > 0)
                {
                    _changeTile = _lisTile[UnityEngine.Random.Range(0, _lisTile.Count)];
                    break;
                }
            }
        }

        if (_changeTile != null)
            ObstacleTileRespawn(_changeTile, eTileType.Chocolate);

        for (int i = 0; i < level.lisGoal.Count; ++i)
        {
            if (level.lisGoal[i].tileType == eTileType.Chocolate)
            {
                ++level.lisGoal[i].nCount;
                InGameScene.instance.ChangeChocolateText(level.lisGoal[i].nCount);
                if (level.lisGoal[i].nCount - 3 >= nTileMax)
                {
                    isLose = true;
                }
                break;
            }
        }
        SoundsManager.Instance.Play(sChocolateRespawnSoundsPath);
    }
    //허니에 있는 타일을 랜덤으로 변경

    public void ChangeHoney()
    {
        if (lisElement_Honey.Count > 0)
        {
            List<eTileType> _lisTempType = new List<eTileType>();
            List<int> _lisIndex = new List<int>();

            for (int i = 0; i < lisElement_Honey.Count; ++i)
            {
                if (GetTile(lisElement_Honey[i].nPosX, lisElement_Honey[i].nPosY) != null)
                {
                    _lisIndex.Add(i);
                }
            }

            if (_lisIndex.Count <= 0)
            {
                return;
            }

            int _nRandom = UnityEngine.Random.Range(0, _lisIndex.Count);

            int _nPosX = lisElement_Honey[_lisIndex[_nRandom]].nPosX;
            int _nPosY = lisElement_Honey[_lisIndex[_nRandom]].nPosY;

            Tile _mainTile = GetTile(_nPosX, _nPosY);

            if (_mainTile == null)
            {
                return;
            }

            for (int i = 1; i < (int)eTileType.RandomCandy; ++i)
            {
                eTileType _tileType = (eTileType)i;

                if ((GetTile(_nPosX, _nPosY - 1) != null && GetTile(_nPosX, _nPosY - 1).tileType == _tileType) ||
                    (GetTile(_nPosX - 1, _nPosY) != null && GetTile(_nPosX - 1, _nPosY).tileType == _tileType) ||
                  (GetTile(_nPosX + 1, _nPosY) != null && GetTile(_nPosX + 1, _nPosY).tileType == _tileType) ||
                    (GetTile(_nPosX, _nPosY + 1) != null && GetTile(_nPosX, _nPosY + 1).tileType == _tileType))
                {
                    continue;
                }

                _lisTempType.Add(_tileType);
            }
            Tile _tile = InGameScene.instance.tilePool.GetTile(_lisTempType[UnityEngine.Random.Range(0, _lisTempType.Count)]);
            _tile.transform.SetParent(transform);
            _tile.Init(_nPosX, _nPosY);
            _tile.transform.position = _mainTile.transform.position;

            if (!dicTile.ContainsKey(_tile.gameObject))
                dicTile.Add(_tile.gameObject, _tile);

            arrTile[_nPosY, _nPosX] = _tile;

            CallSpawnFx(_mainTile);
            if (dicTile.ContainsKey(_mainTile.gameObject))
                dicTile.Remove(_mainTile.gameObject);

            SoundsManager.Instance.Play(sChangeSoundsPath);
        }
    }
    //이동 할 타일이 없을 경우 타일을 교체
    public void ResetTile()
    {
        List<eTileColor> _lisColor = new List<eTileColor>();
        List<Tile> _lisTile = new List<Tile>();

        foreach (KeyValuePair<GameObject, Tile> temp in dicTile)
        {
            if (temp.Value.tileLine == eTileLine.Normal)
                _lisTile.Add(temp.Value);
        }

        _lisTile = _lisTile.OrderBy(_ => _.nPosY).ThenBy(_ => _.nPosX).ToList();
        for (int w = 1; w < (int)eTileColor.Max; ++w)
        {
            _lisColor.Add((eTileColor)w);
        }
        for (int i = 0; i < _lisTile.Count; ++i)
        {
            _lisColor.Clear();
            for (int w = 1; w < (int)eTileColor.Max; ++w)
            {
                _lisColor.Add((eTileColor)w);
            }
            Tile _tile_1 = GetTile(_lisTile[i].nPosX - 1, _lisTile[i].nPosY);
            Tile _tile_2 = GetTile(_lisTile[i].nPosX - 2, _lisTile[i].nPosY);
            if (_tile_1 != null && _tile_2 != null && _tile_1.eTileColor == _tile_2.eTileColor)
            {
                if (_lisColor.Contains(_tile_1.eTileColor))
                    _lisColor.Remove(_tile_1.eTileColor);
            }
            _tile_1 = GetTile(_lisTile[i].nPosX, _lisTile[i].nPosY - 1);
            _tile_2 = GetTile(_lisTile[i].nPosX, _lisTile[i].nPosY - 2);
            if (_tile_1 != null && _tile_2 != null && _tile_1.eTileColor == _tile_2.eTileColor)
            {
                if (_lisColor.Contains(_tile_1.eTileColor))
                    _lisColor.Remove(_tile_1.eTileColor);
            }
            Tile _tile = null;
            TilePool tilePool = InGameScene.instance.tilePool;
            switch (_lisColor[UnityEngine.Random.Range(0, _lisColor.Count)])
            {
                case eTileColor.Red:
                    _tile = tilePool.GetTile(eTileType.RedCandy);
                    break;
                case eTileColor.Green:
                    _tile = tilePool.GetTile(eTileType.GreenCandy);
                    break;
                case eTileColor.Blue:
                    _tile = tilePool.GetTile(eTileType.BlueCandy);
                    break;
                case eTileColor.Yellow:
                    _tile = tilePool.GetTile(eTileType.YellowCandy);
                    break;
                case eTileColor.Purple:
                    _tile = tilePool.GetTile(eTileType.PurpleCandy);
                    break;
                case eTileColor.Orange:
                    _tile = tilePool.GetTile(eTileType.OrangeCandy);
                    break;
            }
            _tile.Init(_lisTile[i].nPosX, _lisTile[i].nPosY);
            _tile.transform.SetParent(transform);
            _tile.transform.position = _lisTile[i].transform.position;

            if (!dicTile.ContainsKey(_tile.gameObject))
                dicTile.Add(_tile.gameObject, _tile);

            arrTile[_lisTile[i].nPosY, _lisTile[i].nPosX] = _tile;

            dicTile.Remove(_lisTile[i].gameObject);
            _lisTile[i].DieAction();
        }
        SupportReset();
    }

    public IEnumerator ChangeBonusTile()
    {
        List<Tile> _lisTemp = new List<Tile>();
        foreach (KeyValuePair<GameObject, Tile> tile in dicTile)
        {
            if (tile.Value.tileLine == eTileLine.Normal)
                _lisTemp.Add(tile.Value);
        }

        for (int i = 0; i < _lisTemp.Count; ++i)
        {
            int _nRandom = UnityEngine.Random.Range(0, _lisTemp.Count);

            Tile _temp = _lisTemp[i];
            _lisTemp[i] = _lisTemp[_nRandom];
            _lisTemp[_nRandom] = _temp;
        }
        int _nMoveCount = level.nMove;
        int _index = 0;

        while (true)
        {
            yield return changeTileWait;

            if (level.nMove <= 0)
            {
                isLose = false;
                isWin = true;
                break;
            }
            --level.nMove;
            SoundsManager.Instance.Play(sChangeSoundsPath);

            InGameScene.instance.SetMoveCountTxt(level.nMove);

            int _nPosX = _lisTemp[_index].nPosX;
            int _nPosY = _lisTemp[_index].nPosY;

            Tile _mainTile = GetTile(_nPosX, _nPosY);
            Tile _tile = InGameScene.instance.tilePool.GetPackTile(_mainTile.eTileColor);
            _tile.transform.SetParent(transform);
            _tile.Init(_nPosX, _nPosY);
            _tile.transform.position = _mainTile.transform.position;

            if (!dicTile.ContainsKey(_tile.gameObject))
                dicTile.Add(_tile.gameObject, _tile);

            arrTile[_nPosY, _nPosX] = _tile;

            CallSpawnFx(_mainTile);
            if (dicTile.ContainsKey(_mainTile.gameObject))
                dicTile.Remove(_mainTile.gameObject);

            ++_index;
            --_nMoveCount;
            if (_nMoveCount <= 0 || _index >= _lisTemp.Count)
            {
                yield return endTileWait;
                BonusTime();

                isLose = false;
                isWin = true;
                break;
            }
        }
    }

    #endregion
    #region Explode
    //매치된 타일을 폭파
    public void Explode()
    {
        if (InGameScene.instance.isEnd)
            return;

        isPlayAni = true;

        fSupportTime = 0;

        lisEmptyPos.Clear();
        ExplodeObstacle();
        ExplodeSpecial();
        for (int i = 0; i < lisExplodeTile.Count; ++i)
        {
            //특수 블록 제작
            if (dicSpecialList.ContainsKey(lisExplodeTile[i].tileType))
            {
                SearchMatch _searchMatch = dicSpecialList[lisExplodeTile[i].tileType];
                SpecialTileRespawn(lisExplodeTile[i], _searchMatch);

                dicSpecialList.Remove(lisExplodeTile[i].tileType);
            }

            Element _element = arrElement[lisExplodeTile[i].nPosY, lisExplodeTile[i].nPosX];
            lisExplodeTile[i].Die(arrElement[lisExplodeTile[i].nPosY, lisExplodeTile[i].nPosX]);
            eElementType elementType = ElementRemove(arrElement[lisExplodeTile[i].nPosY, lisExplodeTile[i].nPosX]);

            InGameScene.instance.CheckGoalTile(lisExplodeTile[i], _element);

            arrTile[lisExplodeTile[i].nPosY, lisExplodeTile[i].nPosX] = null;
            lisEmptyPos.Add(new Vector2Int(lisExplodeTile[i].nPosX, lisExplodeTile[i].nPosY));

            if (dicTile.ContainsKey(lisExplodeTile[i].gameObject))
                dicTile.Remove(lisExplodeTile[i].gameObject);

            if (elementType == eElementType.None)
            {
                switch (lisExplodeTile[i].tileType)
                {
                    case eTileType.Chocolate:
                        SoundsManager.Instance.Play(sChocolateSoundsPath);
                        break;
                    case eTileType.Marshmallow:
                        SoundsManager.Instance.Play(sMarshmallowSoundsPath);
                        break;
                    case eTileType.ColorBomb:
                        SoundsManager.Instance.Play(sColorBombSoundsPath);
                        break;
                    default:
                        SoundsManager.Instance.Play(sExplodeSoundsPath);
                        break;
                }
            }
            else
            {
                switch (elementType)
                {
                    case eElementType.Honey:
                        SoundsManager.Instance.Play(sHoneySoundsPath);
                        break;
                    case eElementType.Ice:
                        SoundsManager.Instance.Play(sIceSoundsPath);
                        break;
                    case eElementType.Syrup1:
                    case eElementType.Syrup2:
                        SoundsManager.Instance.Play(sSyrupSoundsPath);
                        break;
                }
            }
        }

        lisEmptyPos = lisEmptyPos.OrderBy(_ => _.x).ThenByDescending(_ => _.y).ToList();

        nScore += lisExplodeTile.Count * nBaseScore + (nCombo * nComboScore);
        InGameScene.instance.SetScore(nScore, nCombo);

        lisExplodeTile.Clear();

        if (GravityCoro != null)
            StopCoroutine(GravityCoro);
        GravityCoro = StartCoroutine(ApplyGravity());

        dicSpecialList.Clear();
        if (lisElement_Honey.Count > 0)
            ChangeHoney();
    }
    //폭파되는 타일의 주변의 장애물을 폭파
    public void ExplodeObstacle()
    {
        List<Tile> _temp = new List<Tile>();
        for (int i = 0; i < lisExplodeTile.Count; ++i)
        {
            int _nPosX = lisExplodeTile[i].nPosX;
            int _nPosY = lisExplodeTile[i].nPosY;

            _temp.Add(GetTile(_nPosX, _nPosY - 1));
            _temp.Add(GetTile(_nPosX - 1, _nPosY));
            _temp.Add(GetTile(_nPosX + 1, _nPosY));
            _temp.Add(GetTile(_nPosX, _nPosY + 1));
        }

        for (int w = 0; w < _temp.Count; ++w)
        {
            if (_temp[w] == null)
                continue;

            if (_temp[w].tileType == eTileType.Marshmallow || _temp[w].tileType == eTileType.Chocolate)
            {
                if (!lisExplodeTile.Contains(_temp[w]))
                    lisExplodeTile.Add(_temp[w]);
            }
        }
    }
    //특수 타일의 주변을 폭파 시키기 위해 폭파될 타일 탐색
    public void ExplodeSpecial()
    {
        List<Tile> _tempTile = new List<Tile>();
        for (int i = 0; i < lisExplodeTile.Count; ++i)
        {
            switch (lisExplodeTile[i].tileLine)
            {
                case eTileLine.Horizontal:
                    for (int j = 0; j < arrTile.GetLength(1); ++j)
                    {
                        if (GetTile(j, lisExplodeTile[i].nPosY) != null && !lisExplodeTile.Contains(arrTile[lisExplodeTile[i].nPosY, j]))
                            lisExplodeTile.Add(arrTile[lisExplodeTile[i].nPosY, j]);
                    }
                    break;
                case eTileLine.Vertical:
                    for (int j = 0; j < arrTile.GetLength(0); ++j)
                    {
                        if (GetTile(lisExplodeTile[i].nPosX, j) != null && !lisExplodeTile.Contains(arrTile[j, lisExplodeTile[i].nPosX]))
                            lisExplodeTile.Add(arrTile[j, lisExplodeTile[i].nPosX]);
                    }
                    break;
                case eTileLine.Pack:
                    List<Tile> _temp = new List<Tile>();
                    int _nPosX = lisExplodeTile[i].nPosX;
                    int _nPosY = lisExplodeTile[i].nPosY;

                    _temp.Add(GetTile(_nPosX - 1, _nPosY - 1));
                    _temp.Add(GetTile(_nPosX, _nPosY - 1));
                    _temp.Add(GetTile(_nPosX + 1, _nPosY - 1));
                    _temp.Add(GetTile(_nPosX - 1, _nPosY));
                    _temp.Add(GetTile(_nPosX + 1, _nPosY));
                    _temp.Add(GetTile(_nPosX - 1, _nPosY + 1));
                    _temp.Add(GetTile(_nPosX, _nPosY + 1));
                    _temp.Add(GetTile(_nPosX + 1, _nPosY + 1));

                    for (int w = 0; w < _temp.Count; ++w)
                    {
                        if (_temp[w] != null)
                        {
                            if (!lisExplodeTile.Contains(_temp[w]))
                                lisExplodeTile.Add(_temp[w]);
                        }
                    }
                    break;
                case eTileLine.All:
                    if (lisExplodeTile[i].tileLine == eTileLine.All)
                    {
                        foreach (KeyValuePair<GameObject, Tile> tile in dicTile)
                        {
                            if (tile.Value.eTileColor == lisExplodeTile[i].eTileColor)
                            {
                                if (!lisExplodeTile.Contains(tile.Value))
                                    lisExplodeTile.Add(tile.Value);
                            }
                        }
                    }
                    break;
            }
        }
    }
    public void BonusTime()
    {
        List<Tile> _tempTile = new List<Tile>();
        foreach (KeyValuePair<GameObject, Tile> tile in dicTile)
        {
            switch (tile.Value.tileLine)
            {
                case eTileLine.Horizontal:
                    if (!lisExplodeTile.Contains(tile.Value))
                        lisExplodeTile.Add(tile.Value);
                    for (int j = 0; j < arrTile.GetLength(1); ++j)
                    {
                        if (GetTile(j, tile.Value.nPosY) != null && !lisExplodeTile.Contains(arrTile[tile.Value.nPosY, j]))
                        {
                            if (!lisExplodeTile.Contains(arrTile[tile.Value.nPosY, j]))
                                lisExplodeTile.Add(arrTile[tile.Value.nPosY, j]);

                        }
                    }
                    break;
                case eTileLine.Vertical:
                    if (!lisExplodeTile.Contains(tile.Value))
                        lisExplodeTile.Add(tile.Value);
                    for (int j = 0; j < arrTile.GetLength(0); ++j)
                    {
                        if (GetTile(tile.Value.nPosX, j) != null && !lisExplodeTile.Contains(arrTile[j, tile.Value.nPosX]))
                        {
                            if (!lisExplodeTile.Contains(arrTile[j, tile.Value.nPosX]))
                                lisExplodeTile.Add(arrTile[j, tile.Value.nPosX]);
                        }
                    }
                    break;
                case eTileLine.Pack:
                    if (!lisExplodeTile.Contains(tile.Value))
                        lisExplodeTile.Add(tile.Value);

                    List<Tile> _temp = new List<Tile>();
                    int _nPosX = tile.Value.nPosX;
                    int _nPosY = tile.Value.nPosY;

                    _temp.Add(GetTile(_nPosX - 1, _nPosY - 1));
                    _temp.Add(GetTile(_nPosX, _nPosY - 1));
                    _temp.Add(GetTile(_nPosX + 1, _nPosY - 1));
                    _temp.Add(GetTile(_nPosX - 1, _nPosY));
                    _temp.Add(GetTile(_nPosX + 1, _nPosY));
                    _temp.Add(GetTile(_nPosX - 1, _nPosY + 1));
                    _temp.Add(GetTile(_nPosX, _nPosY + 1));
                    _temp.Add(GetTile(_nPosX + 1, _nPosY + 1));

                    for (int w = 0; w < _temp.Count; ++w)
                    {
                        if (_temp[w] != null)
                        {
                            if (!lisExplodeTile.Contains(_temp[w]))
                                lisExplodeTile.Add(_temp[w]);
                        }
                    }
                    break;
                case eTileLine.All:
                    if (!lisExplodeTile.Contains(tile.Value))
                        lisExplodeTile.Add(tile.Value);

                    if (tile.Value.tileLine == eTileLine.All)
                    {
                        foreach (KeyValuePair<GameObject, Tile> all in dicTile)
                        {
                            if (all.Value.eTileColor == tile.Value.eTileColor)
                            {
                                if (!lisExplodeTile.Contains(tile.Value))
                                    lisExplodeTile.Add(tile.Value);
                            }
                        }
                    }
                    break;
            }
        }
        if (lisExplodeTile.Count > 0)
            Explode();
    }

    //아이스, 허니, 시럽을 제거하는 함수
    public eElementType ElementRemove(Element element)
    {
        if (element == null)
            return eElementType.None;

        if (IsIce(element.nPosX, element.nPosY))
        {
            lisElement_Ice.Remove(element);
            arrElement[element.nPosY, element.nPosX] = null;
            return eElementType.Ice;
        }
        else if (IsHoney(element.nPosX, element.nPosY))
        {
            lisElement_Honey.Remove(element);
            arrElement[element.nPosY, element.nPosX] = null;
            return eElementType.Honey;

        }
        else if (IsSyrup1(element.nPosX, element.nPosY))
        {
            lisElement_Syrup1.Remove(element);
            arrElement[element.nPosY, element.nPosX] = null;
            return eElementType.Syrup1;

        }
        else if (IsSyrup2(element.nPosX, element.nPosY))
        {
            lisElement_Syrup2.Remove(element);

            Element _element = InGameScene.instance.elementPool.GetElement(eElementType.Syrup1);

            _element.Init(element.nPosX, element.nPosY);
            _element.transform.position = element.transform.position;
            _element.transform.SetParent(transform);
            lisElement_Syrup1.Add(_element);
            arrElement[_element.nPosY, _element.nPosX] = _element;
            return eElementType.Syrup2;
        }
        return eElementType.None;
    }
    #endregion
    #region Respawn
    //폭파 후 타일 재생성
    public void TileRespawn()
    {
        if (InGameScene.instance.isEnd)
        {
            return;
        }

        int nRespawnIndex = lisRespawnX[0];

        //1 ~ 6 기본타일
        eTileType tileType = (eTileType)UnityEngine.Random.Range(1, 7);
        Tile _tile = InGameScene.instance.tilePool.GetTile(tileType);
        _tile.transform.SetParent(transform);

        for (int i = 0; i < lisRespawnX.Count; ++i)
        {
            Tile _temp = GetTile(lisRespawnX[i], 0);
            if (arrTileType[0, lisRespawnX[i]] != eTileType.None && _temp == null)
            {
                nRespawnIndex = lisRespawnX[i];
                break;
            }
        }

        _tile.gameObject.SetActive(false);
        arrTile[0, nRespawnIndex] = _tile;
        _tile.Init(nRespawnIndex, 0);
        _tile.tileLine = eTileLine.Normal;
        Vector2Int vec = new Vector2Int(0, nRespawnIndex);
        _tile.transform.position = new Vector3(dicFixPos[vec].x, dicFixPos[vec].y + fSpacePos, 0);
        _tile.AddNextPos(nRespawnIndex, 0, dicFixPos[vec]);
        if (!dicTile.ContainsKey(_tile.gameObject))
            dicTile.Add(_tile.gameObject, _tile);

        while (true)
        {
            if (NextTile(_tile) == false)
            {
                break;
            }
        }
        lisRespawnTile.Add(_tile);
    }
    //특수 타일 생성
    public void SpecialTileRespawn(Tile tile, SearchMatch searchMatch)
    {
        if (lisExplodeTile.Find(_ => _.tileType == eTileType.ColorBomb))
            return;

        Tile _special = null;
        TilePool tilePool = InGameScene.instance.tilePool;

        if (searchMatch.nTileCount >= 7)
        {
            _special = tilePool.GetAllTile(tile.eTileColor);
        }
        else if (searchMatch.nTileCount >= 6)
        {
            _special = tilePool.GetTile(eTileType.ColorBomb);
        }
        else if (searchMatch.nTileCount >= 5)
        {
            _special = tilePool.GetPackTile(tile.eTileColor);
        }
        else if (searchMatch.nTileCount >= 4)
        {
            if (searchMatch.isWight)
            {
                _special = tilePool.GetStraightTile(tile.eTileColor, eTileLine.Horizontal);
            }
            else
            {
                _special = tilePool.GetStraightTile(tile.eTileColor, eTileLine.Vertical);
            }
        }

        if (_special == null)
            return;
        _special.transform.SetParent(transform);
        _special.Init(tile.nPosX, tile.nPosY);
        _special.transform.position = tile.transform.position;
        if (!dicTile.ContainsKey(_special.gameObject))
            dicTile.Add(_special.gameObject, _special);

        arrTile[tile.nPosY, tile.nPosX] = _special;

        tile.Die(arrElement[tile.nPosY, tile.nPosX]);
        ElementRemove(arrElement[tile.nPosY, tile.nPosX]);

        lisEmptyPos.Add(new Vector2Int(tile.nPosX, tile.nPosY));

        if (dicTile.ContainsKey(tile.gameObject))
            dicTile.Remove(tile.gameObject);
        lisExplodeTile.Remove(tile);
    }
    //장애물 생성
    public void ObstacleTileRespawn(Tile tile, eTileType tileType)
    {
        Tile _obstacle = InGameScene.instance.tilePool.GetTile(tileType);

        _obstacle.transform.SetParent(transform);
        _obstacle.Init(tile.nPosX, tile.nPosY);
        _obstacle.transform.position = tile.transform.position;
        if (!dicTile.ContainsKey(_obstacle.gameObject))
            dicTile.Add(_obstacle.gameObject, _obstacle);

        arrTile[tile.nPosY, tile.nPosX] = _obstacle;

        CallSpawnFx(tile);

        if (dicTile.ContainsKey(tile.gameObject))
            dicTile.Remove(tile.gameObject);
    }
    #endregion
    #region Support
    //가능한 퍼즐 찾기
    public IEnumerator SupportMatch()
    {
        SupportReset();

        yield return null;
        bool _isFind = false;

        Tile _tile_1 = null;
        Tile _tile_2 = null;

        Tile _sub_1 = null;
        Tile _sub_2 = null;
        Tile _sub_3 = null;

        int _nPosX = 0;
        int _nPosY = 0;

        lisSupportTile.Clear();
        for (int i = 0; i < level.nHeight; ++i)
        {
            for (int j = 0; j < level.nWidth; ++j)
            {
                if (arrTileType[i, j] != eTileType.None && GetTile(j, i) != null && IsMoveTile(j, i))
                {
                    _tile_1 = GetTile(j + 1, i);
                    _tile_2 = GetTile(j + 2, i);

                    if (_tile_1 != null && _tile_1.eTileColor != GetTile(j, i).eTileColor &&
                       _tile_2 != null && _tile_2.eTileColor == GetTile(j, i).eTileColor)
                    {
                        _nPosX = _tile_1.nPosX;
                        _nPosY = _tile_1.nPosY;

                        _sub_1 = GetTile(_nPosX, _nPosY - 1);
                        _sub_2 = GetTile(_nPosX, _nPosY + 1);

                        if ((_sub_1 != null && _sub_1.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_1.nPosX, _sub_1.nPosY)) ||
                            (_sub_2 != null && _sub_2.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_2.nPosX, _sub_2.nPosY)))
                        {
                            if (IsMoveTile(_tile_1.nPosX, _tile_1.nPosY))
                            {
                                _tile_1.animator.SetTrigger("SuggestedMatch");
                                lisSupportTile.Add(_tile_1);
                                if (_sub_1 != null && _sub_1.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_1.nPosX, _sub_1.nPosY))
                                {
                                    _sub_1.animator.SetTrigger("SuggestedMatch");
                                    lisSupportTile.Add(_sub_1);
                                }
                                else if (_sub_2 != null && _sub_2.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_2.nPosX, _sub_2.nPosY))
                                {
                                    _sub_2.animator.SetTrigger("SuggestedMatch");
                                    lisSupportTile.Add(_sub_2);
                                }
                                _isFind = true;
                            }
                        }
                    }
                    else if (_tile_1 != null && _tile_1.eTileColor == GetTile(j, i).eTileColor &&
                       _tile_2 != null && _tile_2.eTileColor != GetTile(j, i).eTileColor)
                    {
                        _nPosX = _tile_2.nPosX;
                        _nPosY = _tile_2.nPosY;

                        _sub_1 = GetTile(_nPosX, _nPosY - 1);
                        _sub_2 = GetTile(_nPosX, _nPosY + 1);
                        _sub_3 = GetTile(_nPosX + 1, _nPosY);

                        if ((_sub_1 != null && _sub_1.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_1.nPosX, _sub_1.nPosY)) ||
                            (_sub_2 != null && _sub_2.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_2.nPosX, _sub_2.nPosY)) ||
                            (_sub_3 != null && _sub_3.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_3.nPosX, _sub_3.nPosY)))
                        {
                            if (IsMoveTile(_tile_2.nPosX, _tile_2.nPosY))
                            {
                                _tile_2.animator.SetTrigger("SuggestedMatch");
                                lisSupportTile.Add(_tile_2);

                                if (_sub_1 != null && _sub_1.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_1.nPosX, _sub_1.nPosY))
                                {
                                    _sub_1.animator.SetTrigger("SuggestedMatch");
                                    lisSupportTile.Add(_sub_1);
                                }
                                else if (_sub_2 != null && _sub_2.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_2.nPosX, _sub_2.nPosY))
                                {
                                    _sub_2.animator.SetTrigger("SuggestedMatch");
                                    lisSupportTile.Add(_sub_2);
                                }
                                else if (_sub_3 != null && _sub_3.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_3.nPosX, _sub_3.nPosY))
                                {
                                    _sub_3.animator.SetTrigger("SuggestedMatch");
                                    lisSupportTile.Add(_sub_3);
                                }

                                _isFind = true;
                            }
                        }
                    }

                }
                if (_isFind)
                    break;
            }
            if (_isFind)
                break;
        }

        if (!_isFind)
        {
            for (int j = 0; j < level.nWidth; ++j)
            {
                for (int i = 0; i < level.nHeight; ++i)
                {
                    if (arrTileType[i, j] != eTileType.None && GetTile(j, i) != null && IsMoveTile(j, i))
                    {
                        _tile_1 = GetTile(j, i + 1);
                        _tile_2 = GetTile(j, i + 2);

                        if (_tile_1 != null && _tile_1.eTileColor != GetTile(j, i).eTileColor &&
                           _tile_2 != null && _tile_2.eTileColor == GetTile(j, i).eTileColor)
                        {
                            _nPosX = _tile_1.nPosX;
                            _nPosY = _tile_1.nPosY;

                            _sub_1 = GetTile(_nPosX - 1, _nPosY);
                            _sub_2 = GetTile(_nPosX + 1, _nPosY);

                            if ((_sub_1 != null && _sub_1.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_1.nPosX, _sub_1.nPosY)) ||
                                (_sub_2 != null && _sub_2.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_2.nPosX, _sub_2.nPosY)))
                            {
                                if (IsMoveTile(_tile_1.nPosX, _tile_1.nPosY))
                                {
                                    _tile_1.animator.SetTrigger("SuggestedMatch");
                                    lisSupportTile.Add(_tile_1);

                                    if (_sub_1 != null && _sub_1.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_1.nPosX, _sub_1.nPosY))
                                    {
                                        _sub_1.animator.SetTrigger("SuggestedMatch");
                                        lisSupportTile.Add(_sub_1);
                                    }
                                    else if (_sub_2 != null && _sub_2.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_2.nPosX, _sub_2.nPosY))
                                    {
                                        _sub_2.animator.SetTrigger("SuggestedMatch");
                                        lisSupportTile.Add(_sub_2);
                                    }
                                    _isFind = true;
                                }
                            }
                        }
                        else if (_tile_1 != null && _tile_1.eTileColor == GetTile(j, i).eTileColor &&
                           _tile_2 != null && _tile_2.eTileColor != GetTile(j, i).eTileColor)
                        {
                            _nPosX = _tile_2.nPosX;
                            _nPosY = _tile_2.nPosY;

                            _sub_1 = GetTile(_nPosX - 1, _nPosY);
                            _sub_2 = GetTile(_nPosX + 1, _nPosY);
                            _sub_3 = GetTile(_nPosX, _nPosY + 1);

                            if ((_sub_1 != null && _sub_1.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_1.nPosX, _sub_1.nPosY)) ||
                                (_sub_2 != null && _sub_2.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_2.nPosX, _sub_2.nPosY)) ||
                                (_sub_3 != null && _sub_3.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_3.nPosX, _sub_3.nPosY)))
                            {
                                if (IsMoveTile(_tile_2.nPosX, _tile_2.nPosY))
                                {
                                    _tile_2.animator.SetTrigger("SuggestedMatch");
                                    lisSupportTile.Add(_tile_2);

                                    if (_sub_1 != null && _sub_1.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_1.nPosX, _sub_1.nPosY))
                                    {
                                        _sub_1.animator.SetTrigger("SuggestedMatch");
                                        lisSupportTile.Add(_sub_1);
                                    }
                                    else if (_sub_2 != null && _sub_2.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_2.nPosX, _sub_2.nPosY))
                                    {
                                        _sub_2.animator.SetTrigger("SuggestedMatch");
                                        lisSupportTile.Add(_sub_2);
                                    }
                                    else if (_sub_3 != null && _sub_3.eTileColor == GetTile(j, i).eTileColor && IsMoveTile(_sub_3.nPosX, _sub_3.nPosY))
                                    {
                                        _sub_3.animator.SetTrigger("SuggestedMatch");
                                        lisSupportTile.Add(_sub_3);
                                    }

                                    _isFind = true;
                                }
                            }
                        }

                    }
                    if (_isFind)
                        break;
                }
                if (_isFind)
                    break;
            }
        }

        if (!_isFind)
        {
            ResetTile();
        }
    }
    public void SupportReset()
    {
        if (supportCoro != null)
        {
            StopCoroutine(supportCoro);
            supportCoro = null;
        }
        fSupportTime = 0;

        for (int i = 0; i < lisSupportTile.Count; ++i)
        {
            lisSupportTile[i].animator.SetTrigger("Reset");
        }
        lisSupportTile.Clear();
    }
    #endregion
    public Tile GetTile(int nPosX, int nPosY)
    {
        if (nPosX >= 0 && nPosX < arrTile.GetLength(1) && nPosY >= 0 && nPosY < arrTile.GetLength(0) && arrTile[nPosY, nPosX] != null)
        {
            return arrTile[nPosY, nPosX];
        }
        return null;
    }
    #region ApplyItem
    public void MakeItem(Tile tile, eItemType itemType)
    {
        Tile _special = null;
        TilePool tilePool = InGameScene.instance.tilePool;

        switch (itemType)
        {
            case eItemType.All:
                _special = tilePool.GetAllTile(tile.eTileColor);
                break;
            case eItemType.ColorBomb:
                _special = tilePool.GetTile(eTileType.ColorBomb);
                break;
        }

        _special.transform.SetParent(transform);
        _special.Init(tile.nPosX, tile.nPosY);
        _special.transform.position = tile.transform.position;
        if (!dicTile.ContainsKey(_special.gameObject))
            dicTile.Add(_special.gameObject, _special);

        arrTile[tile.nPosY, tile.nPosX] = _special;

        CallSpawnFx(tile);
        if (dicTile.ContainsKey(tile.gameObject))
            dicTile.Remove(tile.gameObject);

        SoundsManager.Instance.Play(sRespawnSoundsPath);
    }
    public void RemoveLollipop(Tile tile)
    {
        lisExplodeTile.Add(tile);
        Explode();
    }
    private IEnumerator SwitchAni(Tile _selectTile, Tile swapTile)
    {
        SoundsManager.Instance.Play(sMoveSoundsPath);

        Vector3 _vecSelect = _selectTile.transform.position;
        Vector3 _vecSwap = swapTile.transform.position;

        SupportReset();

        while (true)
        {
            yield return null;

            _selectTile.transform.position = Vector3.MoveTowards(_selectTile.transform.position, _vecSwap, Time.deltaTime * fMoveSpeed);
            swapTile.transform.position = Vector3.MoveTowards(swapTile.transform.position, _vecSelect, Time.deltaTime * fMoveSpeed);

            if (_selectTile.transform.position == _vecSwap)
            {
                Tile _tempSelectTile = _selectTile;

                int _tempSwapX = swapTile.nPosX;
                int _tempSwapY = swapTile.nPosY;

                int _tempSelectX = _tempSelectTile.nPosX;
                int _tempSelectY = _tempSelectTile.nPosY;

                arrTile[_tempSelectY, _tempSelectX] = swapTile;
                arrTile[_tempSelectY, _tempSelectX].ChangeTile(_tempSelectX, _tempSelectY);

                arrTile[_tempSwapY, _tempSwapX] = _tempSelectTile;
                arrTile[_tempSwapY, _tempSwapX].ChangeTile(_tempSwapX, _tempSwapY);

                lisExplodeTile.Clear();

                DragMatch(_selectTile, true);
                DragMatch(swapTile, true);

                if (_selectTile.tileType != eTileType.ColorBomb)
                {
                    if (lisExplodeTile.Count >= 3)
                        Explode();
                }
                else
                    Explode();

                moveAniCoro = null;

                break;
            }
        }
    }


    #endregion
    #region Is
    public bool IsMoveCenter(int posX, int posY)
    {
        if (posY < arrTileType.GetLength(0) && arrTileType[posY, posX] != eTileType.None && GetTile(posX, posY) == null)
            return true;
        return false;
    }
    public bool IsMoveLeft(int posX, int posY)
    {
        if (posX >= 0 && arrTileType[posY, posX] != eTileType.None && GetTile(posX, posY) == null && !IsIceY(posX, posY))
            return true;
        return false;
    }
    public bool IsMoveRight(int posX, int posY)
    {
        if (arrTile.GetLength(1) > posX && arrTileType[posY, posX] != eTileType.None && GetTile(posX, posY) == null && !IsIceY(posX, posY))
            return true;
        return false;
    }
    public bool IsIce(int x, int y)
    {
        if (y < arrElement.GetLength(0) && y >= 0 && x < arrElement.GetLength(1) && x >= 0 && arrElement[y, x] != null && arrElement[y, x].elementType == eElementType.Ice)
            return true;
        return false;
    }
    public bool IsMoveTile(int x, int y)
    {
        if (IsIce(x, y) || arrTile[y, x].tileLine == eTileLine.Obstacle)
            return false;
        return true;
    }
    //아이스가 있는지 Y 줄 확인
    public bool IsIceY(int x, int y)
    {
        for (int i = 0; i < arrElement.GetLength(0); ++i)
        {
            if (arrElement[i, x] != null && arrElement[i, x].elementType == eElementType.Ice)
            {
                if (i > y)
                {
                    return false;
                }
                return true;
            }
        }

        return false;
    }
    public bool IsHoney(int x, int y)
    {
        if (arrElement[y, x] != null && arrElement[y, x].elementType == eElementType.Honey)
            return true;
        return false;
    }
    public bool IsSyrup1(int x, int y)
    {
        if (arrElement[y, x] != null && arrElement[y, x].elementType == eElementType.Syrup1)
            return true;
        return false;
    }
    public bool IsSyrup2(int x, int y)
    {
        if (arrElement[y, x] != null && arrElement[y, x].elementType == eElementType.Syrup2)
            return true;
        return false;
    }
    #endregion

    public void CallSpawnFx(Tile tile)
    {
        var _spawn = InGameScene.instance.fxPool.GetReSpawnFx();
        _spawn.transform.position = tile.transform.position;

        tile.DieAction();
    }
}