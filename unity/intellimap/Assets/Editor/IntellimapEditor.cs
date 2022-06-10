using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class IntellimapEditor : EditorWindow {
    private Tilemap targetTilemap;
    private int targetWidth;
    private int targetHeight;
    
    [MenuItem ("Window/Intellimap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(IntellimapEditor), false, "Intellimap");
    }

    private void OnGUI() {
        // GUILayout.Label("Tilemap Manipulation Test", EditorStyles.boldLabel);

        targetTilemap = (Tilemap)EditorGUILayout.ObjectField(targetTilemap, typeof(Tilemap), true);

        targetWidth = EditorGUILayout.IntField(targetWidth);
        targetHeight = EditorGUILayout.IntField(targetHeight);

        IntellimapGUIUtil.HorizontalLine(Color.grey, leftMargin: 10, rightMargin: 10);

        if (GUILayout.Button("Generate")) {
            ShowNotification(new GUIContent("W: " + targetWidth + ", H: " + targetHeight));
        }

    }

    

}
