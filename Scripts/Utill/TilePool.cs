using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePool : MonoBehaviour
{
    [Header("Tile")]
    [SerializeField] private ObjectPool redCandy_Pool;
    [SerializeField] private ObjectPool greenCandy_Pool;
    [SerializeField] private ObjectPool blueCandy_Pool;
    [SerializeField] private ObjectPool yellowCandy_Pool;
    [SerializeField] private ObjectPool purpleCandy_Pool;
    [SerializeField] private ObjectPool orangeCandy_Pool;

    [SerializeField] private ObjectPool stripedHorizontalRedCandy_Pool;
    [SerializeField] private ObjectPool stripedHorizontalGreenCandy_Pool;
    [SerializeField] private ObjectPool stripedHorizontalBlueCandy_Pool;
    [SerializeField] private ObjectPool stripedHorizontalYellowCandy_Pool;
    [SerializeField] private ObjectPool stripedHorizontalPurpleCandy_Pool;
    [SerializeField] private ObjectPool stripedHorizontalOramge_Pool;

    [SerializeField] private ObjectPool stripedVerticalRedCandy_Pool;
    [SerializeField] private ObjectPool stripedVerticalGreenCandy_Pool;
    [SerializeField] private ObjectPool stripedVerticalBlueCandy_Pool;
    [SerializeField] private ObjectPool stripedVerticalYellowCandy_Pool;
    [SerializeField] private ObjectPool stripedVerticalPurpleCandy_Pool;
    [SerializeField] private ObjectPool stripedVerticalOrangeCandy_Pool;

    [SerializeField] private ObjectPool wrappedRedCandy_Pool;
    [SerializeField] private ObjectPool wrappedGreenCandy_Pool;
    [SerializeField] private ObjectPool wrappedBlueCandy_Pool;
    [SerializeField] private ObjectPool wrappedYellowCandy_Pool;
    [SerializeField] private ObjectPool wrappedPurpleCandy_Pool;
    [SerializeField] private ObjectPool wrappedOrangeCandy_Pool;

    [SerializeField] private ObjectPool bomb_normal_Pool;

    [SerializeField] private ObjectPool red_all_Pool;
    [SerializeField] private ObjectPool green_all_Pool;
    [SerializeField] private ObjectPool blue_all_Pool;
    [SerializeField] private ObjectPool yellow_all_Pool;
    [SerializeField] private ObjectPool purple_all_Pool;
    [SerializeField] private ObjectPool orange_all_Pool;

    [Header("Obstacle")]
    [SerializeField] private ObjectPool chocolate_normal_Pool;
    [SerializeField] private ObjectPool marshmallow_normal_Pool;

    [Header("Bg")]
    [SerializeField] private ObjectPool Tile_block_Bg_1_Pool;
    [SerializeField] private ObjectPool Tile_block_Bg_2_Pool;
    private bool isBg = false;

    private Dictionary<eTileType, ObjectPool> dicTileTypePool = new Dictionary<eTileType, ObjectPool>();

    private Dictionary<eTileColor, ObjectPool> dicHorizontalStraightPool = new Dictionary<eTileColor, ObjectPool>();
    private Dictionary<eTileColor, ObjectPool> dicVerticalStraightPool = new Dictionary<eTileColor, ObjectPool>();
    private Dictionary<eTileColor, ObjectPool> dicPackPool = new Dictionary<eTileColor, ObjectPool>();
    private Dictionary<eTileColor, ObjectPool> dicAllPool = new Dictionary<eTileColor, ObjectPool>();

    private Dictionary<GameObject, Tile> dicSaveTile = new Dictionary<GameObject, Tile>();
    public void Init()
    {
        dicTileTypePool.Add(eTileType.RedCandy, redCandy_Pool.Init());
        dicTileTypePool.Add(eTileType.GreenCandy, greenCandy_Pool.Init());
        dicTileTypePool.Add(eTileType.BlueCandy, blueCandy_Pool.Init());
        dicTileTypePool.Add(eTileType.YellowCandy, yellowCandy_Pool.Init());
        dicTileTypePool.Add(eTileType.PurpleCandy, purpleCandy_Pool.Init());
        dicTileTypePool.Add(eTileType.OrangeCandy, orangeCandy_Pool.Init());

        dicHorizontalStraightPool.Add(eTileColor.Red, stripedHorizontalRedCandy_Pool.Init());
        dicHorizontalStraightPool.Add(eTileColor.Green, stripedHorizontalGreenCandy_Pool.Init());
        dicHorizontalStraightPool.Add(eTileColor.Blue, stripedHorizontalBlueCandy_Pool.Init());
        dicHorizontalStraightPool.Add(eTileColor.Yellow, stripedHorizontalYellowCandy_Pool.Init());
        dicHorizontalStraightPool.Add(eTileColor.Purple, stripedHorizontalPurpleCandy_Pool.Init());
        dicHorizontalStraightPool.Add(eTileColor.Orange, stripedHorizontalOramge_Pool.Init());

        dicVerticalStraightPool.Add(eTileColor.Red, stripedVerticalRedCandy_Pool.Init());
        dicVerticalStraightPool.Add(eTileColor.Green, stripedVerticalGreenCandy_Pool.Init());
        dicVerticalStraightPool.Add(eTileColor.Blue, stripedVerticalBlueCandy_Pool.Init());
        dicVerticalStraightPool.Add(eTileColor.Yellow, stripedVerticalYellowCandy_Pool.Init());
        dicVerticalStraightPool.Add(eTileColor.Purple, stripedVerticalPurpleCandy_Pool.Init());
        dicVerticalStraightPool.Add(eTileColor.Orange, stripedVerticalOrangeCandy_Pool.Init());

        dicPackPool.Add(eTileColor.Red, wrappedRedCandy_Pool.Init());
        dicPackPool.Add(eTileColor.Green, wrappedGreenCandy_Pool.Init());
        dicPackPool.Add(eTileColor.Blue, wrappedBlueCandy_Pool.Init());
        dicPackPool.Add(eTileColor.Yellow, wrappedYellowCandy_Pool.Init());
        dicPackPool.Add(eTileColor.Purple, wrappedPurpleCandy_Pool.Init());
        dicPackPool.Add(eTileColor.Orange, wrappedOrangeCandy_Pool.Init());

        dicAllPool.Add(eTileColor.Red, red_all_Pool.Init());
        dicAllPool.Add(eTileColor.Green, green_all_Pool.Init());
        dicAllPool.Add(eTileColor.Blue, blue_all_Pool.Init());
        dicAllPool.Add(eTileColor.Yellow, yellow_all_Pool.Init());
        dicAllPool.Add(eTileColor.Purple, purple_all_Pool.Init());
        dicAllPool.Add(eTileColor.Orange, orange_all_Pool.Init());

        dicTileTypePool.Add(eTileType.Chocolate, chocolate_normal_Pool.Init());
        dicTileTypePool.Add(eTileType.Marshmallow, marshmallow_normal_Pool.Init());

        dicTileTypePool.Add(eTileType.ColorBomb, bomb_normal_Pool.Init());

        Tile_block_Bg_1_Pool.Init();
        Tile_block_Bg_2_Pool.Init();
    }
    public Tile GetTileType_Tile(eTileType tileType)
    {
        Tile _tile = null;
        switch (tileType)
        {
            case eTileType.None:
            case eTileType.RedCandy:
            case eTileType.GreenCandy:
            case eTileType.BlueCandy:
            case eTileType.YellowCandy:
            case eTileType.PurpleCandy:
            case eTileType.OrangeCandy:
            case eTileType.Chocolate:
            case eTileType.Marshmallow:
            case eTileType.ColorBomb:
                _tile = GetTile(tileType);
                break;
            case eTileType.RandomCandy:
                _tile = GetTile((eTileType)(Random.Range(1, (int)eTileType.RandomCandy)));
                break;
            case eTileType.RedCandyHorizontalStriped:
                _tile = GetStraightTile(eTileColor.Red, eTileLine.Horizontal);
                break;
            case eTileType.GreenCandyHorizontalStriped:
                _tile = GetStraightTile(eTileColor.Green, eTileLine.Horizontal);
                break;
            case eTileType.BlueCandyHorizontalStriped:
                _tile = GetStraightTile(eTileColor.Blue, eTileLine.Horizontal);
                break;
            case eTileType.YellowCandyHorizontalStriped:
                _tile = GetStraightTile(eTileColor.Yellow, eTileLine.Horizontal);
                break;
            case eTileType.PurpleCandyHorizontalStriped:
                _tile = GetStraightTile(eTileColor.Purple, eTileLine.Horizontal);
                break;
            case eTileType.OrangeCandyHorizontalStriped:
                _tile = GetStraightTile(eTileColor.Orange, eTileLine.Horizontal);
                break;
            case eTileType.RedCandyVerticalStriped:
                _tile = GetStraightTile(eTileColor.Red, eTileLine.Vertical);
                break;
            case eTileType.GreenCandyVerticalStriped:
                _tile = GetStraightTile(eTileColor.Green, eTileLine.Vertical);
                break;
            case eTileType.BlueCandyVerticalStriped:
                _tile = GetStraightTile(eTileColor.Blue, eTileLine.Vertical);
                break;
            case eTileType.YellowCandyVerticalStriped:
                _tile = GetStraightTile(eTileColor.Yellow, eTileLine.Vertical);
                break;
            case eTileType.PurpleCandyVerticalStriped:
                _tile = GetStraightTile(eTileColor.Purple, eTileLine.Vertical);
                break;
            case eTileType.OrangeCandyVerticalStriped:
                _tile = GetStraightTile(eTileColor.Orange, eTileLine.Vertical);
                break;
            case eTileType.RedCandyWrapped:
                _tile = GetPackTile(eTileColor.Red);
                break;
            case eTileType.GreenCandyWrapped:
                _tile = GetPackTile(eTileColor.Green);
                break;
            case eTileType.BlueCandyWrapped:
                _tile = GetPackTile(eTileColor.Blue);
                break;
            case eTileType.YellowCandyWrapped:
                _tile = GetPackTile(eTileColor.Yellow);
                break;
            case eTileType.PurpleCandyWrapped:
                _tile = GetPackTile(eTileColor.Purple);
                break;
            case eTileType.OrangeCandyWrapped:
                _tile = GetPackTile(eTileColor.Orange);
                break;
            case eTileType.RedCandy_All:
                _tile = GetAllTile(eTileColor.Red);
                break;
            case eTileType.GreenCandy_All:
                _tile = GetAllTile(eTileColor.Green);
                break;
            case eTileType.BlueCandy_All:
                _tile = GetAllTile(eTileColor.Blue);
                break;
            case eTileType.YellowCandy_All:
                _tile = GetAllTile(eTileColor.Yellow);
                break;
            case eTileType.PurpleCandy_All:
                _tile = GetAllTile(eTileColor.Purple);
                break;
            case eTileType.OrangeCandy_All:
                _tile = GetAllTile(eTileColor.Orange);
                break;
            default:
                _tile = null;
                break;
        }

        return _tile;
    }
    public Tile GetTile(eTileType tileType)
    {
        if (tileType == eTileType.None)
            return null;

        GameObject _obj = dicTileTypePool[tileType].GetObj();
        if (!dicSaveTile.ContainsKey(_obj))
        {
            dicSaveTile.Add(_obj, _obj.GetComponent<Tile>());
        }
        return dicSaveTile[_obj];
    }
    public Tile GetStraightTile(eTileColor tileColor, eTileLine tileLine)
    {
        GameObject _obj = null;
        switch (tileLine)
        {
            case eTileLine.Vertical:
                _obj = dicVerticalStraightPool[tileColor].GetObj();
                break;
            case eTileLine.Horizontal:
                _obj = dicHorizontalStraightPool[tileColor].GetObj();
                break;
        }

        if (!dicSaveTile.ContainsKey(_obj))
        {
            dicSaveTile.Add(_obj, _obj.GetComponent<Tile>());
        }

        return dicSaveTile[_obj];
    }
    public Tile GetPackTile(eTileColor tileColor)
    {
        GameObject _obj = dicPackPool[tileColor].GetObj();

        if (!dicSaveTile.ContainsKey(_obj))
        {
            dicSaveTile.Add(_obj, _obj.GetComponent<Tile>());
        }
        return dicSaveTile[_obj];
    }
    public Tile GetAllTile(eTileColor tileColor)
    {
        GameObject _obj = dicAllPool[tileColor].GetObj();

        if (!dicSaveTile.ContainsKey(_obj))
        {
            dicSaveTile.Add(_obj, _obj.GetComponent<Tile>());
        }
        return dicSaveTile[_obj];
    }
    public Transform GetTileBg()
    {
        if (isBg)
        {
            isBg = false;
            return Tile_block_Bg_1_Pool.GetObj().transform;
        }
        else
        {
            isBg = true;
            return Tile_block_Bg_2_Pool.GetObj().transform;
        }
    }
}
