using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapStats
{
    public Dictionary<string, int> tileNameToTileIdMapping = new();
    public Dictionary<int, string> tileIdToTileNameMapping = new();

    public Dictionary<int, int> tileIdToTileFrequencyMapping = new();
    public int[,,] tileOccurences;

    public TilemapStats(Tilemap tilemap)
    {
        // Build name-id mappings
        int idCounter = 0;
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(position))
            {
                continue;
            }

            TileBase tile = tilemap.GetTile(position);

            if (!tileNameToTileIdMapping.ContainsKey(tile.name))
            {
                tileNameToTileIdMapping[tile.name] = idCounter;
                tileIdToTileNameMapping[idCounter] = tile.name;
                idCounter++;
            }
        }

        // Build tile-frequency mapping
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(position))
            {
                continue;
            }

            TileBase tile = tilemap.GetTile(position);
            //Debug.Log(tile.name);

            int tileId = tileNameToTileIdMapping[tile.name];
            if (tileIdToTileFrequencyMapping.ContainsKey(tileId))
            {
                tileIdToTileFrequencyMapping[tileId]++;
            }
            else
            {
                tileIdToTileFrequencyMapping[tileId] = 1;
            }
            
        }

        // Build tile-occurences mapping
        tileOccurences = new int[tileIdToTileFrequencyMapping.Count, tileIdToTileFrequencyMapping.Count, 4];
        for (int i = 0; i < tileIdToTileFrequencyMapping.Count; i++)
            for (int j = 0; j < tileIdToTileFrequencyMapping.Count; j++)
                for (int d = 0; d < Directions.directions.Length; d++)
                    tileOccurences[i, j, d] = 0;

        foreach (var initialTilePosition in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(initialTilePosition))
            {
                continue;
            }

            TileBase tile = tilemap.GetTile(initialTilePosition);

            for (int d = 0; d < Directions.directions.Length; d++)
            {
                Vector3Int direction = (Vector3Int)Directions.directions[d];
                Vector3Int targetPosition = initialTilePosition + direction;
                if (tilemap.HasTile(targetPosition))
                {
                    TileBase targetTile = tilemap.GetTile(targetPosition);
                    int id1 = tileNameToTileIdMapping[tile.name];
                    int id2 = tileNameToTileIdMapping[targetTile.name];
                    tileOccurences[id1, id2, d]++;
                }
            }
        }


    }

    public override string ToString()
    {
        string result = "Tile frequencies:";
        foreach ((int tileId, int freq) in tileIdToTileFrequencyMapping)
        {
            result += tileId + ": " + freq + "\n";
        }

        result += "\n";

        for (int i = 0; i < tileIdToTileFrequencyMapping.Count; i++)
        {
            result += "Adjadcencies for tileId " + i + ":\n";
            for (int d = 0; d < Directions.directions.Length; d++)
            {
                result += "\tFrequencies for direction " + Directions.directions[d] + ":\n";
                for (int j = 0; j < tileIdToTileFrequencyMapping.Count; j++)
                {
                    result += "\t\tto tileId " + j + ":  " + tileOccurences[i, j, d] + "\n";
                }
            }
        }
        return result;
    }
}
