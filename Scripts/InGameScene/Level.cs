using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Level
{
    public int nLevel;

    public int nWidth;
    public int nHeight;

    public int nScore1;
    public int nScore2;
    public int nScore3;

    public int nMove;

    public List<eElementType> lisElementType = new List<eElementType>();
    public List<eTileType> lisTileType = new List<eTileType>();
    public List<Goal> lisGoal = new List<Goal>();
    public void SetTileType(Dictionary<int, eTileType> dicType, Dictionary<int, eElementType> diceElementType)
    {
        foreach (KeyValuePair<int, eTileType> tile in dicType)
        {
            lisTileType.Add(tile.Value);
        }

        foreach (KeyValuePair<int, eElementType> tile in diceElementType)
        {
            lisElementType.Add(tile.Value);
        }
    }
    public void SetGoal(Dictionary<int, Goal> _goal)
    {
        foreach (KeyValuePair<int, Goal> temp in _goal)
        {
            lisGoal.Add(temp.Value);
        }
    }
}
[Serializable]
public class Goal
{
    //true => tile , false => element
    public bool isTile;
    public eTileType tileType;
    public eElementType elementType;
    public int nCount;
}
