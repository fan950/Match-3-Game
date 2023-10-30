# Match-3-Game
Match 3 퍼즐게임 개발 포트폴리오

## 목차
- 이동
- 파괴
- 리스폰

## 이동

```    
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
        tile.DieAction();
    }
}
```
