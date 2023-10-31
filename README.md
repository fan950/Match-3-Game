# Match-3-Game

유니티로 개발한  Match 3 퍼즐게임 개발 포트폴리오

# 유튜브 링크

https://www.youtube.com/watch?v=AGMGpd-L96o
영상에선 사운드가 없습니다.

## GameBoard.cs

인게임에서 중요 스크립트로서 간략하게 함수 및 코루틴의 기능을 설명


### 기능
- 이동
- 폭발
- 생성



***



### 이동
MoveUpdate
<details> 
    이동 할 타일을 lisTileMove 리스트에 넣고 이동하고 성공, 실패를 판단하는 코루틴
    
```
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
```

</details>


NextTile
<details> 
    타일의 이동 경로를 정하는 함수 => 아래, 오른쪽 아래, 왼쪽 아래 순서
    
```
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
```
</details>


ApplyGravity
<details> 
타일이 폭발 후 기존에 있던 타일을 내리거나 오른쪽 아래 또는 왼쪽 아래로 내리는 코루틴 

    
```
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

        for (int i = 0; i < arrTile.GetLength(0); ++i)
        {
            for (int j = 0; j < arrTile.GetLength(1); ++j)
            {
                bool _isAdd = false;
                Tile _tempTile = GetTile(j, i);

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
                        if (GetTile(j, i - 1) != null || GetTile(j - 1, i - 1) != null || GetTile(j + 1, i - 1) != null)
                        {
                            i = -1;
                            j = -1;
                        }

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


```

</details>



***


### 폭발
Explode
<details> 
매치된 타일을 폭발시키는 함수


 ```
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

    
```
</details>


ExplodeSpecial
<details>
특수 타일의 주변을 폭파 시키기 위해 폭파 될 타일 탐색하는 함수


```
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

```

</details>


ElementRemove
<details>
아이스, 허니, 시럽을 폭파 시키기 위해 탐색하는 함수

```   
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


```

</details>



***


### 생성
TileRespawn
<details>
폭파 후 타일을 재생성하는 함수

```
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

```

</details>


SpecialTileRespawn
<details>
특수 타일을 생성시키는 함수

```

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


```

</details>


ObstacleTileRespawn
<details>
장애물을 생성시키는 함수

```

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

```

</details>
