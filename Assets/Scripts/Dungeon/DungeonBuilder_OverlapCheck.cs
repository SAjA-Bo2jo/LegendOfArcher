using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DungeonBuilder : MonoBehaviour
{
    private List<Vector3> allOccupiedPositions = new List<Vector3>();

    // check if new obj is too close to existings
    private bool IsOverlapping(Vector3 newPos, float minDistance)
    {
        foreach (Vector3 pos in allOccupiedPositions)
        {
            if (Vector3.Distance(pos, newPos) < minDistance)
            {
                return true;
            }
        }

        return false;
    }

    // check if new obj is in Player spawn area
    private bool IsInPlayerSpawnArea(Vector3 pos)
    {
        bool result = (pos.x < -5.0f && pos.y > 2.0f) ? true : false;

        return result;
    }

}
