using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxPool : MonoBehaviour
{
    [Header("Normal")]
    [SerializeField] private ObjectPool blueCandyMatchParticles_Pool;
    [SerializeField] private ObjectPool greenCandyMatchParticles_Pool;
    [SerializeField] private ObjectPool redCandyMatchParticles_Pool;
    [SerializeField] private ObjectPool yellowCandyMatchParticles_Pool;
    [SerializeField] private ObjectPool purpleCandyMatchParticles_Pool;
    [SerializeField] private ObjectPool orangeCandyMatchParticles_Pool;

    [Header("Special")]
    [SerializeField] private ObjectPool verticalStripes_Pool;
    [SerializeField] private ObjectPool wrappedCandyParticles_Pool;
    [SerializeField] private ObjectPool horizontalStripes_Pool;
    [SerializeField] private ObjectPool colorBombParticles_Pool;

    [Header("Obstacle")]
    [SerializeField] private ObjectPool marshmallowParticles_Pool;
    [SerializeField] private ObjectPool chocolateParticles_Pool;

    [Header("Element")]
    [SerializeField] private ObjectPool honeyParticles_Pool;
    [SerializeField] private ObjectPool iceParticles_Pool;
    [SerializeField] private ObjectPool syrupParticles_Pool;
  
    [SerializeField] private ObjectPool spawn_Pool;

    public Dictionary<eTileType, ObjectPool> dicTypeFx = new Dictionary<eTileType, ObjectPool>();
    public Dictionary<eElementType, ObjectPool> dicElementFx = new Dictionary<eElementType, ObjectPool>();

    private Dictionary<GameObject, ExplodeFx> dicSaveFx = new Dictionary<GameObject, ExplodeFx>();

    private const int nMax = 5;
    public void Init()
    {
        dicTypeFx.Add(eTileType.RedCandy, redCandyMatchParticles_Pool.Init());
        dicTypeFx.Add(eTileType.GreenCandy, greenCandyMatchParticles_Pool.Init());
        dicTypeFx.Add(eTileType.BlueCandy, blueCandyMatchParticles_Pool.Init());
        dicTypeFx.Add(eTileType.YellowCandy, yellowCandyMatchParticles_Pool.Init());
        dicTypeFx.Add(eTileType.PurpleCandy, purpleCandyMatchParticles_Pool.Init());
        dicTypeFx.Add(eTileType.OrangeCandy, orangeCandyMatchParticles_Pool.Init());

        dicTypeFx.Add(eTileType.RedCandyHorizontalStriped, horizontalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.GreenCandyHorizontalStriped, horizontalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.BlueCandyHorizontalStriped, horizontalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.YellowCandyHorizontalStriped, horizontalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.PurpleCandyHorizontalStriped, horizontalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.OrangeCandyHorizontalStriped, horizontalStripes_Pool.Init(nMax));

        dicTypeFx.Add(eTileType.RedCandyVerticalStriped, verticalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.GreenCandyVerticalStriped, verticalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.BlueCandyVerticalStriped, verticalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.YellowCandyVerticalStriped, verticalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.PurpleCandyVerticalStriped, verticalStripes_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.OrangeCandyVerticalStriped, verticalStripes_Pool.Init(nMax));

        dicTypeFx.Add(eTileType.RedCandyWrapped, wrappedCandyParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.GreenCandyWrapped, wrappedCandyParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.BlueCandyWrapped, wrappedCandyParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.YellowCandyWrapped, wrappedCandyParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.PurpleCandyWrapped, wrappedCandyParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.OrangeCandyWrapped, wrappedCandyParticles_Pool.Init(nMax));

        dicTypeFx.Add(eTileType.RedCandy_All, redCandyMatchParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.GreenCandy_All, greenCandyMatchParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.BlueCandy_All, blueCandyMatchParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.YellowCandy_All, yellowCandyMatchParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.PurpleCandy_All, purpleCandyMatchParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.OrangeCandy_All, orangeCandyMatchParticles_Pool.Init(nMax));

        dicTypeFx.Add(eTileType.ColorBomb, colorBombParticles_Pool.Init(nMax));

        dicTypeFx.Add(eTileType.Chocolate, chocolateParticles_Pool.Init(nMax));
        dicTypeFx.Add(eTileType.Marshmallow, marshmallowParticles_Pool.Init(nMax));      

        dicElementFx.Add(eElementType.Honey, honeyParticles_Pool.Init(nMax));
        dicElementFx.Add(eElementType.Ice, iceParticles_Pool.Init(nMax));
        dicElementFx.Add(eElementType.Syrup1, syrupParticles_Pool.Init(nMax));
        dicElementFx.Add(eElementType.Syrup2, syrupParticles_Pool.Init(nMax));
    }
    public ExplodeFx GetExplodeFx(eTileType tileType)
    {
        if (tileType == eTileType.None)
            return null;

        GameObject _obj = dicTypeFx[tileType].GetObj();

        if (!dicSaveFx.ContainsKey(_obj))
        {
            dicSaveFx.Add(_obj, _obj.GetComponent<ExplodeFx>());
        }

        return dicSaveFx[_obj];
    }

    public GameObject GetReSpawnFx()
    {
        GameObject _obj = spawn_Pool.GetObj();
        return _obj;
    }

    public ExplodeFx GetElementFx(eElementType tileType)
    {
        if (tileType == eElementType.None)
            return null;

        GameObject _obj = dicElementFx[tileType].GetObj();

        if (!dicSaveFx.ContainsKey(_obj))
        {
            dicSaveFx.Add(_obj, _obj.GetComponent<ExplodeFx>());
        }

        return dicSaveFx[_obj];
    }
}
