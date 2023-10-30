using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidthStraightMatch : TileMatch
{
    public override List<Tile> HandleMatch(GameBoard GameBoard, Tile tile)
    {
        List<Tile> _tempList = new List<Tile>();
        Tile _obj = null;
        int _nIndexX = -1;

        //왼쪽로 갈 경우
        while (true)
        {
            _obj = null;

            int _tempX = tile.nPosX + _nIndexX;
            if (0 > _tempX)
                break;

            _obj = GameBoard.arrTile[tile.nPosY, _tempX];

            if (_obj == null)
                break;
            if (tile.eTileColor == _obj.eTileColor)
                _tempList.Add(_obj);
            else
                break;

            --_nIndexX;
        }

        // 오른쪽로 같 경우
        _nIndexX = 1;

        while (true)
        {
            _obj = null;
            int _tempX = tile.nPosX + _nIndexX;

            if (GameBoard.arrTile.GetLength(1) <= _tempX)
                break;

            _obj = GameBoard.arrTile[tile.nPosY, _tempX];

            if (_obj == null)
                break;

            if (tile.eTileColor == _obj.eTileColor)
                _tempList.Add(_obj);
            else
                break;

            ++_nIndexX;
        }

        return _tempList;
    }
}
