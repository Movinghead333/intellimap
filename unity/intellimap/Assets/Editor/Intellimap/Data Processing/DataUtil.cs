using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DataUtil {
    public static TilemapStats LoadTilemapStats(string resourcePath) {
        GameObject parentObject = Resources.Load(resourcePath) as GameObject;
        Tilemap tilemap = parentObject.GetComponent<Tilemap>();
        tilemap.CompressBounds();

        TilemapStats tilemapStats = new TilemapStats(tilemap);
        return tilemapStats;
    }
    
    public static void Zero3DArray(int[,,] arr) {
        int iLength = arr.GetLength(0);
        int jLength = arr.GetLength(1);
        int kLength = arr.GetLength(2);

        for (int i = 0; i < iLength; i++)
            for (int j = 0; j < jLength; j++)
                for (int k = 0; k < kLength; k++)
                    arr[i, j, k] = 0;
    }
}
