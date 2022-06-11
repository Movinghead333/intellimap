using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCInput
{
    public int numberOfTileTypes;
    public Vector2Int mapSize;
    public bool[,,] adjacencyConstraint;

    public WFCInput(int numberOfTiles, Vector2Int mapSize, bool[,,] adjacencyConstraint)
    {
        this.numberOfTileTypes = numberOfTiles;
        this.mapSize = mapSize;
        this.adjacencyConstraint = adjacencyConstraint;
    }
}
