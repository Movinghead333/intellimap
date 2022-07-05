using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Tilemaps;

public class MatrixTestWindow : EditorWindow {
    public Tile[] testTiles;
    SerializedObject serializedObject;
    SerializedProperty serializedProperty;

    IntellimapMatrix matrix;

    IntellimapWeightBox testBox;
    IntellimapHistogram testHistogram;

    [MenuItem ("Window/Matrix Test")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(MatrixTestWindow), false, "Matrix Test");
    }

    public void OnEnable() {
        Color foregroundColor = new Color(0.8f, 0.8f, 0.8f);
        matrix = new IntellimapMatrix(3, foregroundColor, Color.clear, Color.grey, 0.5f, 20, this);
        testTiles = new Tile[3];

        serializedObject = new SerializedObject(this);
        serializedProperty = serializedObject.FindProperty("testTiles");

        testBox = new IntellimapWeightBox(50, 50, Color.white, Color.black, Color.blue, this);
        testHistogram = new IntellimapHistogram(4, new List<float>{50f, 50f, 50f, 50f});
        testBox.debugHistogram = testHistogram;

        testBox.SetWeights(new float[]{0.1f, 0.2f, 0.3f, 0.4f});
    }

    private void OnGUI() {
        EditorGUILayout.PropertyField(serializedProperty, true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Set Axis Tiles")) {
            matrix.SetAxisTiles(testTiles);
        }

        GUILayout.Space(15);

        matrix.Show();

        IntellimapGUIUtil.HorizontalLine(Color.grey);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(15);
        testBox.Show();
        testHistogram.Show();

        EditorGUILayout.EndHorizontal();
    }
}
