using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class IntellimapEditor : EditorWindow {
    private TileBase testTile;

    private Tilemap targetTilemap;
    private int targetWidth;
    private int targetHeight;

    private Tilemap tilePalette;
    private string baseDataPath;

    private IntellimapHistogram histogram;
    private IntellimapDraggableBox draggableBox;
    
    // Register window as menu item
    [MenuItem ("Window/Intellimap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(IntellimapEditor), false, "Intellimap");
    }

    public void OnEnable() {
        targetWidth = 0;
        targetHeight = 0;

        baseDataPath = "";

        histogram = new IntellimapHistogram(4);

        draggableBox = new IntellimapDraggableBox(50, 50, Color.grey, Color.white, this);
    }

    // Window GUI code
    private void OnGUI() {
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

        GUILayout.Label("TODO: Matrix");

        EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);

            draggableBox.Show();
        EditorGUILayout.EndHorizontal();

        IntellimapGUIUtil.HorizontalLine(Color.grey, leftMargin: 10, rightMargin: 10);

        histogram.Show();

        IntellimapGUIUtil.HorizontalLine(Color.grey);

        if (GUILayout.Button("Generate")) {
            /*List<float> histogramValues = histogram.GetSliderValues();
            string output = "";
            for (int i = 0; i < histogramValues.Count; i++) {
                output += histogramValues[i] + " ";
            }*/
            string output = draggableBox.GetPercentage().ToString();
            ShowNotification(new GUIContent(output));

            if (targetTilemap != null && testTile != null) {
                targetTilemap.SetTile(new Vector3Int(0, 0), testTile);
            }
        }

    }

}
