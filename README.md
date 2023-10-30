# Match-3-Game
Match 3 퍼즐게임 개발 포트폴리오

## 목차
- 이동
- 파괴
- 리스폰

## 이동
MoveUpdate
이동 할 타일을 lisTileMove 리스트에 넣고 이동하고 성공, 실패를 판단하는 코루틴
<details> 
    
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
