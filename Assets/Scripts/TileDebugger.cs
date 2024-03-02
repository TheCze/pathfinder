using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDebugger : MonoBehaviour
{
    public float heuristicCost;
    public float costToReach;
    public float totalCost;

    public void SetTile(Tile t)
    {
        heuristicCost = t.heuristicCost;
        costToReach = t.costToReach;
        totalCost = t.GetTotalCost();
    }
}
