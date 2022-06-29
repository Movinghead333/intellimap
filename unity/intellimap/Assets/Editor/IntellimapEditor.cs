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

    private Tilemap tilePalette;
    private string baseDataPath;

    private IntellimapHistogram histogram;
    private IntellimapMatrix matrix;
    
    // Register window as menu item
    [MenuItem ("Window/Intellimap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(IntellimapEditor), false, "Intellimap");
    }

    public void OnEnable() {
        scrollPos = new Vector2(0, 0);

        targetWidth = 0;
        targetHeight = 0;

        baseDataPath = "";

        histogram = new IntellimapHistogram(4);

        Color foregroundColor = new Color(0.8f, 0.8f, 0.8f);
        Color backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        matrix = new IntellimapMatrix(10, foregroundColor, backgroundColor, Color.grey, 0.7f, 30, this);
    }

    // Window GUI code
    private void OnGUI() {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        testTile =(TileBase)EditorGUILayout.ObjectField("Test Tile:", testTile, typeof(TileBase), true);

        targetTilemap = (Tilemap)EditorGUILayout.ObjectField("Target Tilemap:", targetTilemap, typeof(Tilemap), true);
        targetWidth = EditorGUILayout.IntField("Width:", targetWidth);
        targetHeight = EditorGUILayout.IntField("Height:", targetHeight);

        IntellimapGUIUtil.HorizontalLine(Color.grey);

        tilePalette = (Tilemap)EditorGUILayout.ObjectField("Tile Palette:", tilePalette, typeof(Tilemap), true);

        EditorGUILayout.BeginHorizontal();
            baseDataPath = EditorGUILayout.TextField("Base data:", baseDataPath);

            if (GUILayout.Button("...", GUILayout.Width(25))) {
                baseDataPath = EditorUtility.OpenFolderPanel("Open base data folder", Application.dataPath, "");
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Current base data path:", baseDataPath);

        IntellimapGUIUtil.HorizontalLine(Color.grey);

        matrix.Show();

        IntellimapGUIUtil.HorizontalLine(Color.grey, leftMargin: 10, rightMargin: 10);

        histogram.Show();

        IntellimapGUIUtil.HorizontalLine(Color.grey);

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
