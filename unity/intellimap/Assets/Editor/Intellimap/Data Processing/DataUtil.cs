using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DataUtil {
    public static TilemapStats LoadTilemapStats(string resourcePath) {
        string[] fileEntries = Directory.GetFiles(resourcePath);
        List<Tilemap> tilemaps = new List<Tilemap>();

        foreach (string filePath in fileEntries) {
            string[] filePathSplit = filePath.Split("/");

            string simpleFileName = filePathSplit[filePathSplit.Length - 1];
            string[] simpleFileNameSplit = simpleFileName.Split(".");
            string fileExtension = simpleFileNameSplit[simpleFileNameSplit.Length - 1];
            // TODO: Deal with x.x.extension files
            string simpleFileNameWithoutExtension = simpleFileNameSplit[0];

            int resourcesFolderIndex = Array.IndexOf(filePathSplit, "Resources");
            string pathInResourcesFolder = "";
            for (int i = resourcesFolderIndex + 1; i < filePathSplit.Length - 1; i++) {
                pathInResourcesFolder += filePathSplit[i] + "/";
            }
            pathInResourcesFolder += simpleFileNameWithoutExtension;

            if (fileExtension == "prefab") {
                GameObject parentObject = Resources.Load(pathInResourcesFolder) as GameObject;

                Tilemap tilemap = parentObject.GetComponent<Tilemap>();
                if (tilemap != null) {
                    tilemap.CompressBounds();
                    tilemaps.Add(tilemap);
                }
            }

        }

        TilemapStats tilemapStats = new TilemapStats(tilemaps.ToArray());
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
