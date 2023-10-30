using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightStraightMatch : TileMatch
{
    public override List<Tile> HandleMatch(GameBoard GameBoard, Tile tile)
    {
        List<Tile> _tempList = new List<Tile>();
        Tile _obj = null;
        int _nIndexY = -1;

        //왼쪽로 갈 경우
        while (true)
        {
            _obj = null;
            int _tempY = tile.nPosY + _nIndexY;

            if (0 > _tempY)
                break;

            _obj = GameBoard.arrTile[_tempY, tile.nPosX];

            if (_obj == null)
                break;
            if (tile.eTileColor == _obj.eTileColor)
                _tempList.Add(_obj);
            else
                break;

            --_nIndexY;
        }

        // 오른쪽로 같 경우
        _nIndexY = 1;

        while (true)
        {
            _obj = null;
            int _tempY = tile.nPosY + _nIndexY;

            if (GameBoard.arrTile.GetLength(0) <= _tempY)
                break;

            _obj = GameBoard.arrTile[_tempY, tile.nPosX];

            if (_obj == null)
                break;

            if (tile.eTileColor == _obj.eTileColor)
                _tempList.Add(_obj);
            else
                break;

            ++_nIndexY;
        }

        return _tempList;
    }
}
