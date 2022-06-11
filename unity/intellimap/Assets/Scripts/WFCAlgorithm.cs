using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCAlgorithm
{
    static Vector2Int[] offsets = new Vector2Int[] {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
    };

    // 2D array of the final generated tileIds
    static int?[,] resultTilemap;

    static WFCInput wfcInput;

    // bool array for each tile storing the options left for this tile
    static bool[,,] tileDomains;

    public static WFCOutput Compute(WFCInput pWFCInput)
    {
        wfcInput = pWFCInput;

        // Init the result based on the desired mapSize
        resultTilemap = new int?[wfcInput.mapSize.x, wfcInput.mapSize.y];
        for (int x = 0; x < wfcInput.mapSize.x; x++)
            for (int y = 0; y < wfcInput.mapSize.y; y++)
                // Init the tileDomains to maximum entropy in the beginning
                tileDomains = new bool[wfcInput.mapSize.x, wfcInput.mapSize.y, wfcInput.numberOfTileTypes];

        for (int x = 0; x < wfcInput.mapSize.x; x++)
            for (int y = 0; y < wfcInput.mapSize.y; y++)
                for (int t = 0; t < wfcInput.numberOfTileTypes; t++)
                    tileDomains[x, y, t] = true;
        
        // While uncollapsed cells remain, continue with collpasing cells
        while(true)
        {
            // Collapse cell with lowest entropy
            Vector2Int? tileToCollapseNullable = FindMinimumEntropyCell();

            // If there is no valid cell left to collapse, then we are done
            if (tileToCollapseNullable == null)
                break;

            Vector2Int tileToCollapse = tileToCollapseNullable.Value;

            Debug.Log("Collapsing cell " + tileToCollapse);

            List<int> possibleTilesIds = new List<int>();
            for (int t = 0; t < tileDomains.GetLength(2); t++)
            {
                if (tileDomains[tileToCollapse.x, tileToCollapse.y, t])
                    possibleTilesIds.Add(t);
            }

            int randomIndex = Random.Range(0, possibleTilesIds.Count);
            int tileId = possibleTilesIds[randomIndex];
            resultTilemap[tileToCollapse.x, tileToCollapse.y] = tileId;

            // Propagate Changes to all uncollapsed cells
            Queue<(Vector2Int, Vector2Int)> positionsToCheck = new Queue<(Vector2Int, Vector2Int)>();
            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2Int targetPosition = tileToCollapse + offsets[i];
                if (targetPosition.x < 0 || targetPosition.y < 0 || targetPosition.x >= wfcInput.mapSize.x || targetPosition.y >= wfcInput.mapSize.y)
                {
                    continue;
                }

                positionsToCheck.Enqueue((tileToCollapse, offsets[i]));
            }

            while (positionsToCheck.Count > 0)
            {
                (Vector2Int nextPosition, Vector2Int offset) = positionsToCheck.Dequeue();
                Vector2Int targetPosition = nextPosition + offset;

                int dirIndex = 0;
                for (int i = 0; i < offsets.Length; i++)
                {
                    if (offset == offsets[i])
                    {
                        dirIndex = i;
                        break;
                    }
                }


                for (int i = 0; i < wfcInput.numberOfTileTypes; i++)
                {
                    bool connectionAllowed = wfcInput.adjacencyConstraint[tileId, i, dirIndex];

                    if (!connectionAllowed)
                    {
                        tileDomains[targetPosition.x, targetPosition.y, i] = false;
                    }
                }
            }
        }

        return new WFCOutput(resultTilemap);
    }

    private static Vector2Int? FindMinimumEntropyCell()
    {
        List<Vector2Int> minimumEntropyTiles = new List<Vector2Int>();
        int minimumEntropy = int.MaxValue;

        for (int x = 0; x < tileDomains.GetLength(0); x++)
            for (int y = 0; y < tileDomains.GetLength(1); y++)
            {
                if (resultTilemap[x, y] == null)
                {
                    int entropy = 0;
                    for (int t = 0; t < tileDomains.GetLength(2); t++)
                    {
                        entropy += tileDomains[x, y, t] ? 1 : 0;
                    }

                    if (entropy < minimumEntropy)
                    {
                        minimumEntropy = entropy;
                        minimumEntropyTiles = new List<Vector2Int>();
                        minimumEntropyTiles.Add(new Vector2Int(x, y));
                    }
                    else if (entropy == minimumEntropy)
                    {
                        minimumEntropyTiles.Add(new Vector2Int(x, y));
                    }
                }
            }

        if (minimumEntropyTiles.Count != 0)
        {
            int index = Random.Range(0, minimumEntropyTiles.Count);
            return minimumEntropyTiles[index];
        }
        else
        {
            return null;
        } 
    }
}
