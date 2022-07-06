using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class IntellimapEditor : EditorWindow {
    private TileBase testTile;

    private Vector2 scrollPos;

    private Tilemap targetTilemap;
    private int targetWidth;
    private int targetHeight;

    private string baseDataPath;

    private Matrix matrix;
    private Histogram histogram;
    
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

            if (GUILayout.Button("Generate")) {
                List<float> histogramValues = histogram.GetSliderValues();
                string output = "";
                for (int i = 0; i < histogramValues.Count; i++) {
                    output += histogramValues[i] + " ";
                }
                //string output = draggableBox.GetPercentage().ToString();
            
                ShowNotification(new GUIContent(output));

                if (targetTilemap != null && testTile != null) {
                    targetTilemap.SetTile(new Vector3Int(0, 0), testTile);
                }
            }
        EditorGUILayout.EndScrollView();
    }

}
