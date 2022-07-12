using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DataUtil {
    public static TilemapStats LoadTilemapStats(string resourcePath) {
        string[] fileEntries = Directory.GetFiles(resourcePath);
        //Debug.Log(resourcePath);
        var root= Path.GetFileName(resourcePath);
        TilemapStats tilemapStats = null;
        GameObject parentObject = null;
        List<Tilemap> tileArray = new List<Tilemap>();
        foreach (string file in fileEntries)
        {
            
            var newPath = root + "/" + Path.GetFileName(file);
            if(!newPath.Contains("meta"))
            {
                newPath = newPath.Split(".")[0];
                parentObject = Resources.Load(newPath) as GameObject;
            }
            Tilemap tilemap = parentObject.GetComponent<Tilemap>();
            tilemap.CompressBounds();
            tileArray.Add(tilemap);
       
        }
        tilemapStats = new TilemapStats(tileArray.ToArray());
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
