using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCOutput
{
    public int?[,] tilemapIds;

    public WFCOutput(int?[,] tilemapIds)
    {
        this.tilemapIds = tilemapIds;
    }

    public override string ToString()
    {
        string result = "";
        int yMaxIndex = tilemapIds.GetLength(1) - 1;
        for (int y = yMaxIndex; y >= 0; y--) 
        {
            for (int x = 0; x < tilemapIds.GetLength(0); x++)
            {
                result += tilemapIds[x, y] + ",";
            }
            result += "\n";
        }

        return result;
    }
}
