using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TilemapStats {
    public int tileCount;

    public Dictionary<int, Tile> idToTile;
    public Dictionary<Tile, int> tileToId;

    public int[] tileOccurrences;
    public float[] tileFrequencies;

    public int[,,] totalAdjacency;
    public float[,,] normalizedAdjacency;

    public TilemapStats(Tilemap tilemap) {
        BuildIdMappings(tilemap);
        tileCount = idToTile.Count;

        BuildFrequencyMapping(tilemap);

        ExtractAdjacency(tilemap);
    }

    private void BuildIdMappings(Tilemap tilemap) {
        idToTile = new Dictionary<int, Tile>();
        tileToId = new Dictionary<Tile, int>();

        int idCounter = 0;
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin) {
            if (!tilemap.HasTile(position))
                continue;

            Tile tile = tilemap.GetTile<Tile>(position);
            if (!tileToId.ContainsKey(tile)) {
                tileToId[tile] = idCounter;
                idToTile[idCounter] = tile;
                idCounter++;
            }
        }
    }

    private void BuildFrequencyMapping(Tilemap tilemap) {
        tileOccurrences = new int[tileCount];
        tileFrequencies = new float[tileCount];

        for (int i = 0; i < tileCount; i++) {
            tileOccurrences[i] = 0;
        }

        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin) {
            if (!tilemap.HasTile(position))
                continue;

            Tile tile = tilemap.GetTile<Tile>(position);
            int tileId = tileToId[tile];

            tileOccurrences[tileId]++;
        }

        int occurrenceSum = tileOccurrences.Sum();
        for (int i = 0; i < tileCount; i++) {
            tileFrequencies[i] = (float)tileOccurrences[i] / occurrenceSum;
        }
    }

    private void ExtractAdjacency(Tilemap tilemap) {
        totalAdjacency = new int[tileCount, tileCount, 4];
        DataUtil.Zero3DArray(totalAdjacency);

        foreach (Vector3Int initialTilePosition in tilemap.cellBounds.allPositionsWithin) {
            if (!tilemap.HasTile(initialTilePosition))
                continue;

            Tile tile = tilemap.GetTile<Tile>(initialTilePosition);

            for (int d = 0; d < Directions.directions.Length; d++) {
                Vector3Int direction = (Vector3Int)Directions.directions[d];
                Vector3Int targetPosition = initialTilePosition + direction;

                if (tilemap.HasTile(targetPosition)) {
                    Tile targetTile = tilemap.GetTile<Tile>(targetPosition);
                    int id1 = tileToId[tile];
                    int id2 = tileToId[targetTile];
                    totalAdjacency[id1, id2, d]++;
                }
            }
        }

        NormalizeAdjacency();
    }

    private void NormalizeAdjacency() {
        int[,] adjacencySum = new int[tileCount, 4];
        normalizedAdjacency = new float[tileCount, tileCount, 4];
        
        // Get the sum for every tile in every direction
        for (int i = 0; i < tileCount; i++) {
            for (int d = 0; d < 4; d++) {
                adjacencySum[i, d] = 0;

                for (int j = 0; j < tileCount; j++) {
                    adjacencySum[i, d] += totalAdjacency[i, j, d];
                }
            }
        }

        // Divide each total by the sum
        for (int i = 0; i < tileCount; i++) {
            for (int j = 0; j < tileCount; j++) {
                for (int d = 0; d < 4; d++) {
                    normalizedAdjacency[i, j, d] = (float)totalAdjacency[i, j, d] / adjacencySum[i, d];
                }
            }
        }
    }

    public override string ToString()
    {
        string result = "Tile frequencies:";
        for (int tileId = 0; tileId < tileCount; tileId++) {
            result += tileId + ": " + tileFrequencies[tileId] + "\n";
        }

        result += "\n";

        for (int i = 0; i < tileCount; i++)
        {
            result += "Adjadcencies for tileId " + i + ":\n";
            for (int d = 0; d < Directions.directions.Length; d++)
            {
                result += "\tFrequencies for direction " + Directions.directions[d] + ":\n";
                for (int j = 0; j < tileCount; j++)
                {
                    result += "\t\tto tileId " + j + ":  " + totalAdjacency[i, j, d] + "\n";
                }
            }
        }
        return result;
    }
}
