using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;

public class IntellimapEditor : EditorWindow {
    private Tilemap targetTilemap;
    private int targetWidth;
    private int targetHeight;

    private Tilemap tilePalette;
    private string baseDataPath = "";

    private float slider1Value = 0.0f;
    private float slider2Value = 0.0f;
    private float slider3Value = 0.0f;
    
    // Register window as menu item
    [MenuItem ("Window/Intellimap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(IntellimapEditor), false, "Intellimap");
    }

    // Window GUI code
    private void OnGUI() {
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

        EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            slider1Value = GUILayout.VerticalSlider(slider1Value, 100, 0, GUILayout.Height(100));
            GUILayout.Space(15);
            slider2Value = GUILayout.VerticalSlider(slider2Value, 100, 0, GUILayout.Height(100));
            GUILayout.Space(15);
            slider3Value = GUILayout.VerticalSlider(slider3Value, 100, 0, GUILayout.Height(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Slider1 Value:", slider1Value.ToString());

        IntellimapGUIUtil.HorizontalLine(Color.grey);

        if (GUILayout.Button("Generate")) {
            ShowNotification(new GUIContent("W: " + targetWidth + ", H: " + targetHeight));
        }

    }

}
