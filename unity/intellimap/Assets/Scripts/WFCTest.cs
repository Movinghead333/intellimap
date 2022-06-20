using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFCTest : MonoBehaviour
{
    private static bool T = true;
    private static bool F = false;

    public Tilemap tilemap;

    public TileBase[] tileBases;

    private void Render(WFCOutput output)
    {
        for (int x = 0; x < output.tilemapIds.GetLength(0); x++)
            for (int y = 0; y < output.tilemapIds.GetLength(1); y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tileBases[output.tilemapIds[x, y].Value]);
            }
                
    }

    public void GenerateButtonPressed()
    {
        int numberOfTiles = 3;
        Vector2Int mapSize = new Vector2Int(50, 50);
        bool[,,] adjacencyConstraint = new bool[,,]{
            {{T, T, T, T }, {F, F, F, F }, {T, T, F, T }, }, // Adjacencies for Tile 0
            {{F, F, F, F }, {T, T, T, T }, {T, T, T, T }, }, // Adjacencies for Tile 1
            {{F, T, T, T }, {T, T, T, T }, {T, T, T, T }, }, // Adjacencies for Tile 2
        };

        // TODO: do symetry check on constraint matrix
        WFCInput input = new WFCInput(numberOfTiles, mapSize, adjacencyConstraint);

        WFCOutput output = WFCAlgorithm.Compute(input);

        Render(output);

        Debug.Log(output);
    }
}
