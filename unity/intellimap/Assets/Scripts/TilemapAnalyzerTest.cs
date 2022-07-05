using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapAnalyzerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Load a tilemap prefab
        GameObject mapObject = Resources.Load("Tilemaps/TestTilemap") as GameObject;

        // Get the tilemap component and compress the bounds
        Tilemap tilemap = mapObject.GetComponent<Tilemap>();
        tilemap.CompressBounds();
        Debug.Log(tilemap.size);

        // Analyze the tilemap and store its stats for later use in WFC
        TilemapStats tilemapStats = new TilemapStats(tilemap);

        Debug.Log(tilemapStats);
    }
}
