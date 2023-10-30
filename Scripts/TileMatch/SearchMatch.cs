using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchMatch : TileMatch
{
    private WidthStraightMatch wigthStraightMatch = new WidthStraightMatch();
    private HeightStraightMatch heightStraightMatch = new HeightStraightMatch();
    public int nTileCount = 0;
    //특수 블록 생성시키기 위한 bool
    public bool isWight = false;
    public bool isHeight = false;
    public override List<Tile> HandleMatch(GameBoard GameBoard, Tile tile)
    {
        List<Tile> _tempList = new List<Tile>();
        var _wight = wigthStraightMatch.HandleMatch(GameBoard, tile);
        _wight.Add(tile);

        isWight = false;
        isHeight = false;

        if (_wight.Count >= 3)
        {
            if (_wight.Count >= 4)
                isWight = true;
            for (int i = 0; i < _wight.Count; ++i)
            {
                if (!_tempList.Contains(_wight[i]))
                    _tempList.Add(_wight[i]);
                var _wightlis = heightStraightMatch.HandleMatch(GameBoard, _wight[i]);
                if (_wightlis.Count >= 4)
                    isHeight = true;
                for (int j = 0; j < _wightlis.Count; ++j)
                {
                    if (_wightlis.Count >= 2)
                    {
                        if (!_tempList.Contains(_wightlis[j]))
                            _tempList.Add(_wightlis[j]);
                    }
                }
            }
        }

        var _height = heightStraightMatch.HandleMatch(GameBoard, tile);
        _height.Add(tile);
        if (_height.Count >= 3)
        {
            if (_height.Count >= 4)
                isHeight = true;
            for (int i = 0; i < _height.Count; ++i)
            {
                if (!_tempList.Contains(_height[i]))
                    _tempList.Add(_height[i]);
                var _heightlis = wigthStraightMatch.HandleMatch(GameBoard, _height[i]);
                if (_heightlis.Count >= 4)
                    isWight = true;
                for (int j = 0; j < _heightlis.Count; ++j)
                {
                    if (_heightlis.Count >= 2)
                    {
                        if (!_tempList.Contains(_heightlis[j]))
                            _tempList.Add(_heightlis[j]);
                    }
                }
            }
        }
        //특수 블록이 있으면 또다른 특수 블록이 안생기게 탐색
        for (int i = 0; i < _tempList.Count; ++i)
        {
            if (_tempList[i].tileLine != eTileLine.Normal)
            {
                isWight = false;
                isHeight = false;
                break;
            }
        }
        nTileCount = _tempList.Count;
        return _tempList;
    }

}
