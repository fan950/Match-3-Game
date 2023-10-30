# Match-3-Game
Match 3 퍼즐게임 개발 포트폴리오

## GameBoard.cs
### 목차
- 이동
- 파괴
- 리스폰

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
타일이 파괴 후 기존에 있던 타일을 내리거나 오른쪽 아래 또는 왼쪽 아래로 내리는 코루틴 

    
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

        for (int i = arrTile.GetLength(0) - 1; i >= 0; --i)
        {
            for (int j = 0; j < arrTile.GetLength(1); ++j)
            {
                bool _isAdd = false;
                Tile _tempTile = GetTile(j, i);

                if (GetTile(j, i - 1) == null && _tempTile != null && arrElement[i, j] == null)
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
```

</details>
