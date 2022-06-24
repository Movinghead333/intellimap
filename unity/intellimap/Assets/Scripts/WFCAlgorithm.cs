using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCAlgorithm
{
    static Vector2Int[] directions = new Vector2Int[] {
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

            //Debug.Log("Collapsing cell " + tileToCollapse);

            List<int> possibleTilesIds = new List<int>();
            for (int t = 0; t < wfcInput.numberOfTileTypes; t++)
            {
                if (tileDomains[tileToCollapse.x, tileToCollapse.y, t])
                    possibleTilesIds.Add(t);
            }

            int randomIndex = Random.Range(0, possibleTilesIds.Count);
            int tileId = possibleTilesIds[randomIndex];

            // Update the domain to reflect the collapse in preparation for the propagation step
            for (int t = 0; t < wfcInput.numberOfTileTypes; t++)
            {
                tileDomains[tileToCollapse.x, tileToCollapse.y, t] = t == tileId;
            }

            resultTilemap[tileToCollapse.x, tileToCollapse.y] = tileId;

            // Propagate Changes to all uncollapsed cells
            PropagateConstraints(tileToCollapse);

            //Debug.Log(EntropyMatrixToString());
        }

        return new WFCOutput(resultTilemap);
    }

    // Propagate the domain-changes caused by a cell-collapse towards the rest
    // of the tiles.
    private static void PropagateConstraints(Vector2Int collapsedTile)
    {
        // We have to track which uncollapsed cells habe already been used for
        // propagation in order to avoid "going backwards" and thus visiting
        // cells twice
        bool[,] visitedTiles = new bool[wfcInput.mapSize.x, wfcInput.mapSize.y];
        for (int x = 0; x < wfcInput.mapSize.x; x++)
            for (int y = 0; y < wfcInput.mapSize.y; y++)
                visitedTiles[x, y] = false;

        // We keep a set for the currentWave which we iterate through and
        // propagate changes to other tiles
        HashSet<Vector2Int> currentWave = new();

        // All tiles which we have not yet visited and are candidates for
        // future propagation are inserted into this set which will then
        // become the next currentWave
        HashSet<Vector2Int> nextWave = new();

        // As an entrypoint we add the tile which we just collapsed in the
        // current iteration of the WFC algorithm
        currentWave.Add(collapsedTile);

        // As long as their candidates left for propagation
        while (currentWave.Count > 0)
        {
            // Go through all candidates for propagation in the current wave
            foreach (Vector2Int positionToPropagate in currentWave)
            {
                // Set the currently checked position as visited to avoid
                // looking at tiles twice
                visitedTiles[positionToPropagate.x, positionToPropagate.y] = true;

                // Go over the four directions and thus the neighbouring cells
                // whose domain we might want to change
                for (int d = 0; d < directions.Length; d++)
                {
                    // Determine the position of the neighbouring cell
                    Vector2Int direction = directions[d];
                    Vector2Int targetPosition = positionToPropagate + direction;

                    // Check if the tile tile we want to propagate to...
                    // ... is within the map bounds
                    // ... has not been collapsed yet
                    // ... and has not been visited yet by the propagation
                    //     algorithm
                    if (!LocationOutOfMapBounds(targetPosition) &&
                        resultTilemap[targetPosition.x, targetPosition.y] == null &&
                        !visitedTiles[targetPosition.x, targetPosition.y])
                    {
                        // Track if the tile actually changes because if it
                        // does not we can skip looking at it and reduce
                        // runtime this way
                        bool targetTileChanged = false;

                        // In this array we collect the possibilities resulting
                        // from the current tile's domain
                        bool[] tempStates = new bool[wfcInput.numberOfTileTypes];
                        for (int j = 0; j < wfcInput.numberOfTileTypes; j++)
                            tempStates[j] = false;

                        // Now go through the domain of the currently checked tile
                        for (int i = 0; i < wfcInput.numberOfTileTypes; i++)
                        {
                            // If the tile is within the domain of the currently checked tile
                            // then also check its implications on the neighbouring cells
                            // regarding the set of constraints
                            if (tileDomains[positionToPropagate.x, positionToPropagate.y, i])
                            {
                                for (int j = 0; j < wfcInput.numberOfTileTypes; j++)
                                {
                                    bool connectionAllowed = wfcInput.adjacencyConstraint[i, j, d];

                                    targetTileChanged = true;
                                    // We or here to get the union of all tiles possible coming from the
                                    // set of constraints enabled by the domain of the currently checked tile
                                    tempStates[j] = tempStates[j] || connectionAllowed;
                                }
                            }
                        }

                        // We then and the result of allowed tiles from the propagation with the current state of the neighbouring tile's domain
                        for (int j = 0; j < wfcInput.numberOfTileTypes; j++)
                            tileDomains[targetPosition.x, targetPosition.y, j] = tileDomains[targetPosition.x, targetPosition.y, j] && tempStates[j];

                        // If the tile did in fact change, then we have to see if there are further implications
                        if (targetTileChanged)
                        {
                            nextWave.Add(targetPosition);
                        }
                    }
                }
            }

            // Prepare the following currentWave by shifting over the tiles in nextWave and reset nextWAve
            currentWave = nextWave;
            nextWave = new HashSet<Vector2Int>();
        }
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

    private static bool LocationOutOfMapBounds(Vector2Int location)
    {
        return location.x < 0 || location.y < 0 || location.x >= wfcInput.mapSize.x || location.y >= wfcInput.mapSize.y;
    }

    // Print the current entropy of all tiles as matrix
    private static string EntropyMatrixToString()
    {
        string result = "";
        for (int y = 0; y < wfcInput.mapSize.y; y++)
        {
            for (int x = 0; x < wfcInput.mapSize.x; x++)
            {
                int entropy = 0;
                for (int t = 0; t < wfcInput.numberOfTileTypes; t++)
                    entropy += tileDomains[x, y, t] ? 1 : 0;
                result += entropy + ", ";
            }
            result += "\n";
        }

        return result;     
    }
}
