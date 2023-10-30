using System.IO;
using UnityEngine;

public enum eScene 
{
    LevelScene=0,
    InGameScene,
}

public enum eTileType
{
    None = 0,
    RedCandy = 1,
    GreenCandy = 2,
    BlueCandy = 3,
    YellowCandy = 4,
    PurpleCandy = 5,
    OrangeCandy = 6,
    RandomCandy = 7,

    RedCandyHorizontalStriped = 11,
    GreenCandyHorizontalStriped = 12,
    BlueCandyHorizontalStriped = 13,
    YellowCandyHorizontalStriped = 14,
    PurpleCandyHorizontalStriped = 15,
    OrangeCandyHorizontalStriped = 16,

    RedCandyVerticalStriped = 21,
    GreenCandyVerticalStriped = 22,
    BlueCandyVerticalStriped = 23,
    YellowCandyVerticalStriped = 24,
    PurpleCandyVerticalStriped = 25,
    OrangeCandyVerticalStriped = 26,

    RedCandyWrapped = 31,
    GreenCandyWrapped = 32,
    BlueCandyWrapped = 33,
    YellowCandyWrapped = 34,
    PurpleCandyWrapped = 35,
    OrangeCandyWrapped = 36,

    RedCandy_All = 51,
    GreenCandy_All = 52,
    BlueCandy_All = 53,
    YellowCandy_All = 54,
    PurpleCandy_All = 55,
    OrangeCandy_All = 56,

    Chocolate,
    Marshmallow,
    ColorBomb,
    Max
}
public enum eElementType
{
    None,
    Honey,
    Ice,
    Syrup1,
    Syrup2,
    Max
}

public enum eTileColor
{
    None = 0,
    Red = 1,
    Green = 2,
    Blue = 3,
    Yellow = 4,
    Purple = 5,
    Orange = 6,
    Max = 7,
}
public enum eTileLine
{
    None = 0,
    Normal,
    Vertical,
    Horizontal,
    Pack,
    All,
    Obstacle,
    Max
}
public enum eAtlasType
{
    None = 0,
    Tile,
    InGame_UI,
    Level_UI,
    Max
}

public enum eItemType
{
    Lollipop,
    All,
    Switch,
    ColorBomb
}
