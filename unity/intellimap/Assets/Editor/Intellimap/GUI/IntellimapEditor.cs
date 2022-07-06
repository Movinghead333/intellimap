using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class IntellimapEditor : EditorWindow {
    private static bool T = true;
    private static bool F = false;

    private TileBase testTile;

    private Vector2 scrollPos;

    private Tilemap targetTilemap;
    private int targetWidth;
    private int targetHeight;

    private string baseDataPath;

    private Matrix matrix;
    private Histogram histogram;

    private TilemapStats tilemapStats;

    private WFCAlgorithm? currentWFCInstance;
    
    // Register window as menu item
    [MenuItem ("Window/Intellimap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(IntellimapEditor), false, "Intellimap");
    }

    public void OnEnable() {
        // This is needed to be able to register the MouseMove event
        wantsMouseMove = true;

        scrollPos = new Vector2(0, 0);

        targetWidth = 0;
        targetHeight = 0;

        baseDataPath = "";

        Color foregroundColor = new Color(0.8f, 0.8f, 0.8f);
        Color backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        matrix = new Matrix(10, foregroundColor, backgroundColor, Color.grey, 0.5f, 30, this);
        
        histogram = new Histogram(2);
    }

    // Window GUI code
    private void OnGUI() {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            testTile =(TileBase)EditorGUILayout.ObjectField("Test Tile:", testTile, typeof(TileBase), true);

            targetTilemap = (Tilemap)EditorGUILayout.ObjectField("Target Tilemap:", targetTilemap, typeof(Tilemap), true);
            targetWidth = EditorGUILayout.IntField("Width:", targetWidth);
            targetHeight = EditorGUILayout.IntField("Height:", targetHeight);

            GUIUtil.HorizontalLine(Color.grey);

            EditorGUILayout.BeginHorizontal();
                baseDataPath = EditorGUILayout.TextField("Base data:", baseDataPath);

                if (GUILayout.Button("...", GUILayout.Width(25))) {
                    baseDataPath = EditorUtility.OpenFolderPanel("Open base data folder", Application.dataPath, "");
                }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Current base data path:", baseDataPath);

            GUIUtil.HorizontalLine(Color.grey);

            if (GUILayout.Button("Analyze Base Data")) {
                TilemapStats tilemapStats = DataUtil.LoadTilemapStats("Tilemaps/TestTilemap");
                Debug.Log(tilemapStats);
                this.tilemapStats = tilemapStats;

                // Re-initialize matrix
                Tile[] axisTiles = new Tile[tilemapStats.tileCount];
                for (int i = 0; i < tilemapStats.tileCount; i++) {
                    axisTiles[i] = tilemapStats.idToTile[i];
                }

                matrix.Init(tilemapStats.tileCount);
                matrix.SetAxisTiles(axisTiles);

                // Fill matrix with normalized adjacency values
                float[] weights = new float[4];

                for (int y = 0; y < tilemapStats.tileCount; y++) {
                    for (int x = 0; x < tilemapStats.tileCount; x++) {
                        if (x >= y) {
                            for (int d = 0; d < 4; d++) {
                                weights[d] = tilemapStats.normalizedAdjacency[y, x, d];
                            }

                            matrix.SetBoxWeights(x, y, weights);
                        }
                    }
                }

                // Re-initialize histogram
                histogram.Init(tilemapStats.tileCount);
                histogram.SetBottomTiles(axisTiles);
                histogram.SetSliderValues(tilemapStats.tileFrequencies);
            }

            matrix.Show();

            GUIUtil.HorizontalLine(Color.grey, leftMargin: 10, rightMargin: 10);

            histogram.Show();

            GUIUtil.HorizontalLine(Color.grey);

            if (GUILayout.Button("Collapse entire map")) {
                CollapseEntireMapButtonPressed();
            }

            if (GUILayout.Button("Collapse single cell"))
            {
                SingleCellCollapseButtonPressed();
            }
        EditorGUILayout.EndScrollView();
    }

    // Execute the Wave Function Collapse Algorithm with the current set of input data
    private void CollapseEntireMapButtonPressed()
    {
        TryInitiliazeWFCInstance();

        if (currentWFCInstance != null)
        {
            bool running = true;
            while (running)
            {
                (Vector2Int tilePosition, int tileId)? result = currentWFCInstance.RunSingleCellCollapse();

                if (result == null)
                {
                    currentWFCInstance = null;
                    return;

                }

                RenderSingleCell(result.Value.tilePosition, result.Value.tileId);
            }
        }

        //int?[,] tileIdsMatrix = currentWFCInstance.RunCompleteCollapse();

        //Render(tileIdsMatrix);

        //currentWFCInstance = null;
    }

    private void SingleCellCollapseButtonPressed()
    {
        TryInitiliazeWFCInstance();

        if (currentWFCInstance != null)
        {
            (Vector2Int tilePosition, int tileId)? result = currentWFCInstance.RunSingleCellCollapse();

            if (result == null)
            {
                // Reset the WFC instance if we receive null as a result of single cell collapse
                currentWFCInstance = null;
            }
            else
            {
                RenderSingleCell(result.Value.tilePosition, result.Value.tileId);
            }
        }
    }

    private void TryInitiliazeWFCInstance()
    {
        if (targetTilemap == null || tilemapStats == null || currentWFCInstance != null)
            return;

        currentWFCInstance = new WFCAlgorithm(tilemapStats, new Vector2Int(targetWidth, targetHeight));
    }

    // Render the resulting tileIdMatrix to the selected Tilemap
    private void Render(int?[,] tileIdsMatrix)
    {
        for (int x = 0; x < tileIdsMatrix.GetLength(0); x++)
            for (int y = 0; y < tileIdsMatrix.GetLength(1); y++)
                if (tileIdsMatrix[x, y].HasValue)
                    RenderSingleCell(new Vector2Int(x, y), tileIdsMatrix[x, y].Value);

    }

    private void RenderSingleCell(Vector2Int tilePosition, int tileId)
    {
        targetTilemap.SetTile(new Vector3Int(tilePosition.x, tilePosition.y, 0), tilemapStats.idToTile[tileId]);
    }
}
