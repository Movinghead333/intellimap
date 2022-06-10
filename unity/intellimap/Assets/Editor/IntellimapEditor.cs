using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class IntellimapEditor : EditorWindow {
    public Tilemap tilemap;
    public TileBase tile;

    [MenuItem ("Window/Intellimap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(IntellimapEditor), false, "Intellimap");
    }

    private void OnGUI() {
        GUILayout.Label("Tilemap Manipulation Test", EditorStyles.boldLabel);

        tilemap = (Tilemap)EditorGUILayout.ObjectField(tilemap, typeof(Tilemap), true);
        tile = (TileBase)EditorGUILayout.ObjectField(tile, typeof(TileBase), true);

        if (GUILayout.Button("Write first tile")) {
            if (tilemap == null || tile == null) {
                ShowNotification(new GUIContent("Please select all objects"));
            }
            else {
                tilemap.CompressBounds();
                tilemap.SetTile(new Vector3Int(0, 0, 0), tile);
            }
        }

    }

}
