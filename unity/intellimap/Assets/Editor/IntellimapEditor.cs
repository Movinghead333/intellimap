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
    private string baseDataPath = "";
    
    // Register window as menu item
    [MenuItem ("Window/Intellimap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(IntellimapEditor), false, "Intellimap");
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

        IntellimapGUIUtil.HorizontalLine(Color.grey, leftMargin: 10, rightMargin: 10);

        IntellimapGUIUtil.Histogram(3, 0);

        IntellimapGUIUtil.Histogram(4, 1);

        IntellimapGUIUtil.HorizontalLine(Color.grey);

        if (GUILayout.Button("Generate")) {
            //ShowNotification(new GUIContent("W: " + targetWidth + ", H: " + targetHeight));
            List<float> histogramValues = IntellimapGUIUtil.GetHistogramValues(0);
            string output = "";
            for (int i = 0; i < histogramValues.Count; i++) {
                output += histogramValues[i] + " ";
            }
            ShowNotification(new GUIContent(output));

            targetTilemap.SetTile(new Vector3Int(0, 0), testTile);
        }

    }

}
