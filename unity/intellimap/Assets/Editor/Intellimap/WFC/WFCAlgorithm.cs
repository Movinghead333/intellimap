using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCAlgorithm
{
    private float[,,] directionalWeights;

    private float[] tileFrequencies;

    private int tileCount;

    private Vector2Int outputMapSize;

    // 2D array of the final generated tileIds
    private int?[,] resultTileIds;

    // bool array for each tile storing the options left for this tile
    private bool[,,] tileDomains;

    private int collapsedCellsCounter = 0;

    private int numberCellsToCollapse;

    public WFCAlgorithm(float[,,] directionalWeights, float[] tileFrequencies, Vector2Int outputMapSize)
    {
        this.directionalWeights = directionalWeights;
        this.tileFrequencies = tileFrequencies;
        tileCount = tileFrequencies.Length;
        this.outputMapSize = outputMapSize;
        numberCellsToCollapse = outputMapSize.x * outputMapSize.y;

        resultTileIds = new int?[outputMapSize.x, outputMapSize.y];
        tileDomains = new bool[outputMapSize.x, outputMapSize.y, tileCount];

        for (int x = 0; x < outputMapSize.x; x++)
            for (int y = 0; y < outputMapSize.y; y++)
                for (int t = 0; t < tileCount; t++)
                    tileDomains[x, y, t] = true;
    }

    public int?[,] RunCompleteCollapse()
    {
        // While uncollapsed cells remain, continue with collpasing cells
        // prioritizing the ones with the smallest entropy
        while(AnyCellsLeftToCollapse())
        {
            CollapseCell();
        }

        return resultTileIds;
    }

    public (Vector2Int tilePosition, int tileId)? RunSingleCellCollapse()
    {
        if (!AnyCellsLeftToCollapse())
            return null;

        return CollapseCell();
    }

    private bool AnyCellsLeftToCollapse()
    {
        return collapsedCellsCounter < numberCellsToCollapse;
    }

    private (Vector2Int, int)? CollapseCell()
    {
        collapsedCellsCounter++;

        // Collapse cell with lowest entropy
        Vector2Int? tileToCollapseNullable = FindMinimumEntropyCell();

        // If there is no valid cell left to collapse, then we are done
        if (tileToCollapseNullable == null)
            return null;

        // Get non-nullable version of position
        Vector2Int tileToCollapse = tileToCollapseNullable.Value;

        // Determine a random tileId based on the options left in the domain
        // and their probability to occur
        DetermineTileId(tileToCollapse);

        // Propagate Changes to all uncollapsed cells
        PropagateConstraints(tileToCollapse);

        //Debug.Log(EntropyMatrixToString());

        // If the collapse of the cell was successful then return the collapsed
        // position as well as the id of the newly collapsed tile
        if (resultTileIds[tileToCollapse.x, tileToCollapse.y] == null)
        {
            return null;
        }
        else
        {
            return (tileToCollapse, resultTileIds[tileToCollapse.x, tileToCollapse.y].Value);
        }
    }

    private Vector2Int? FindMinimumEntropyCell()
    {
        List<Vector2Int> minimumEntropyTiles = new List<Vector2Int>();
        float minimumEntropy = float.MaxValue;

        for (int x = 0; x < tileDomains.GetLength(0); x++)
            for (int y = 0; y < tileDomains.GetLength(1); y++)
            {
                if (resultTileIds[x, y] == null)
                {
                    float entropy = CalculateEntropyForCell(x, y);

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

    private void DetermineTileId(Vector2Int tileToCollapse)
    {
        List<int> possibleTilesIds = new List<int>();
        for (int t = 0; t < tileCount; t++)
        {
            if (tileDomains[tileToCollapse.x, tileToCollapse.y, t])
                possibleTilesIds.Add(t);
        }

        float probabilitySum = 0;
        foreach (int possibleTileId in possibleTilesIds)
        {
            probabilitySum += tileFrequencies[possibleTileId];
        }

        List<float> normalizedProbabilities = new List<float>();

        foreach (int possibleTileId in possibleTilesIds)
        {
            normalizedProbabilities.Add(tileFrequencies[possibleTileId] / probabilitySum);
        }

        float randomValue = Random.Range(0f, 1f);
        int tileId = -1;

        float aggregatedProbability = 0;

        for (int i = 0; i < possibleTilesIds.Count; i++)
        {
            aggregatedProbability += normalizedProbabilities[i];

            if (randomValue <= aggregatedProbability)
            {
                tileId = possibleTilesIds[i];
                break;
            }
        }

        // Update the domain to reflect the collapse in preparation for the propagation step
        for (int t = 0; t < tileCount; t++)
        {
            tileDomains[tileToCollapse.x, tileToCollapse.y, t] = t == tileId;
        }

        resultTileIds[tileToCollapse.x, tileToCollapse.y] = tileId;
    }

    // Propagate the domain-changes caused by a cell-collapse towards the rest
    // of the tiles.
    private void PropagateConstraints(Vector2Int collapsedTile)
    {
        // We have to track which uncollapsed cells habe already been used for
        // propagation in order to avoid "going backwards" and thus visiting
        // cells twice
        bool[,] visitedTiles = new bool[outputMapSize.x, outputMapSize.y];
        for (int x = 0; x < outputMapSize.x; x++)
            for (int y = 0; y < outputMapSize.y; y++)
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
                for (int d = 0; d < Directions.directions.Length; d++)
                {
                    // Determine the position of the neighbouring cell
                    Vector2Int direction = Directions.directions[d];
                    Vector2Int targetPosition = positionToPropagate + direction;

                    // Check if the tile tile we want to propagate to...
                    // ... is within the map bounds
                    // ... has not been collapsed yet
                    // ... and has not been visited yet by the propagation
                    //     algorithm
                    if (!LocationOutOfMapBounds(targetPosition) &&
                        resultTileIds[targetPosition.x, targetPosition.y] == null &&
                        !visitedTiles[targetPosition.x, targetPosition.y])
                    {
                        // Track if the tile actually changes because if it
                        // does not we can skip looking at it and reduce
                        // runtime this way
                        bool targetTileChanged = false;

                        // In this array we collect the possibilities resulting
                        // from the current tile's domain
                        bool[] tempStates = new bool[tileCount];
                        for (int j = 0; j < tileCount; j++)
                            tempStates[j] = false;

                        // Now go through the domain of the currently checked tile
                        for (int i = 0; i < tileCount; i++)
                        {
                            // If the tile is within the domain of the currently checked tile
                            // then also check its implications on the neighbouring cells
                            // regarding the set of constraints
                            if (tileDomains[positionToPropagate.x, positionToPropagate.y, i])
                            {
                                for (int j = 0; j < tileCount; j++)
                                {
                                    bool connectionAllowed = directionalWeights[j, i, d] > 0 ;

                                    targetTileChanged = true;
                                    // We or here to get the union of all tiles possible coming from the
                                    // set of constraints enabled by the domain of the currently checked tile
                                    tempStates[j] = tempStates[j] || connectionAllowed; // TODO: detect flip in flag
                                }
                            }
                        }

                        // We then and the result of allowed tiles from the propagation with the current state of the neighbouring tile's domain
                        for (int j = 0; j < tileCount; j++)
                            tileDomains[targetPosition.x, targetPosition.y, j] = tileDomains[targetPosition.x, targetPosition.y, j] && tempStates[j];
                        // TODO: check for chane in tile domain here

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

    // Calculate the probability for single cell given by its x and y coordinate
    private float CalculateEntropyForCell(int x, int y)
    {
        float W = 0;
        for (int t = 0; t < tileCount; t++)
            if (tileDomains[x, y, t])
                W += tileFrequencies[t];

        float entropy = 0;
        for (int t = 0; t < tileCount; t++)
            if (tileDomains[x, y, t])
            {
                float probabilityForT = (float)tileFrequencies[t] / W;
                entropy += probabilityForT * Mathf.Log(probabilityForT, 2);
            }

        entropy *= -1;
        return entropy;
    }

    // Determine if a given position is within the bounds of the targetMapSize
    private bool LocationOutOfMapBounds(Vector2Int location)
    {
        return location.x < 0 || location.y < 0 || location.x >= outputMapSize.x || location.y >= outputMapSize.y;
    }

    // Print the current entropy of all tiles as matrix
    private string EntropyMatrixToString()
    {
        string result = "";
        for (int y = 0; y < outputMapSize.y; y++)
        {
            for (int x = 0; x < outputMapSize.x; x++)
            {
                int entropy = 0;
                for (int t = 0; t < tileCount; t++)
                    entropy += tileDomains[x, y, t] ? 1 : 0;
                result += entropy + ", ";
            }
            result += "\n";
        }

        return result;     
    }
}
