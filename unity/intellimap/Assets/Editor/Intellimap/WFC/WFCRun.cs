using UnityEngine;
using UnityEngine.Tilemaps;

public class WFCRun {
    private WFCAlgorithm wfcAlgorithm;
    private bool running = false;

    private Tilemap targetTilemap;
    private TilemapStats tilemapStats;

    public WFCRun(Tilemap targetTilemap, int targetWidth, int targetHeight, float[,,] adjacencyWeights, float[] frequencies, int seed, TilemapStats tilemapStats) {
        this.targetTilemap = targetTilemap;
        this.tilemapStats = tilemapStats;

        Vector2Int targetMapSize = new Vector2Int(targetWidth, targetHeight);
        wfcAlgorithm = new WFCAlgorithm(adjacencyWeights, frequencies, targetMapSize, seed);
    }

    // Execute the Wave Function Collapse Algorithm with the current set of input data
    public void GenerateEntireMap() {
        running = true;

        while(running) {
            Step();
        }
    }

    public void ToggleRunning() {
        running = !running;
    }

    public bool Running() {
        return running;
    }

    public void Step() {
        try {
            (Vector2Int tilePosition, int tileId)? result = wfcAlgorithm.RunSingleCellCollapse();

            if (result == null) {
                // Debug.Log("Cells collapsed by frequency: " + currentWFCInstance.numberCellsCollapsedByFrequencyHints + " | by directional probabilities: " + currentWFCInstance.numberCellsCollapsedByDirectionalProbabilities);
                running = false;
                return;
            }

            RenderSingleCell(result.Value.tilePosition, result.Value.tileId);
        }
        catch (System.Exception e) {
            // We might run into a contradiction of the WFC algorithm in this case just reset and show a message
            Debug.Log(e.Message);
            running = false;
            return;
        }

        return;
    }

    private void RenderSingleCell(Vector2Int tilePosition, int tileId) {
        targetTilemap.SetTile(new Vector3Int(tilePosition.x, tilePosition.y, 0), tilemapStats.idToTile[tileId]);
    }
}
