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

    Matrix matrix;

    WeightBox testBox;
    WeightBoxDetailView detailView;

    [MenuItem ("Window/Matrix Test")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(MatrixTestWindow), false, "Matrix Test");
    }

    public void OnEnable() {
        Color foregroundColor = new Color(0.8f, 0.8f, 0.8f);
        matrix = new Matrix(3, foregroundColor, Color.clear, Color.grey, 0.5f, 20, this);
        testTiles = new Tile[3];

        serializedObject = new SerializedObject(this);
        serializedProperty = serializedObject.FindProperty("testTiles");

        detailView = new WeightBoxDetailView();

        testBox = new WeightBox(50, 50, Color.white, Color.black, Color.blue, detailView, this);
    }

    private void OnGUI() {
        //EditorGUILayout.PropertyField(serializedProperty, true);
        //serializedObject.ApplyModifiedProperties();

        //if (GUILayout.Button("Set Axis Tiles")) {
        //   matrix.SetAxisTiles(testTiles);
        //}

        //GUILayout.Space(15);

        //matrix.Show();

        GUIUtil.HorizontalLine(Color.grey);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(15);
        testBox.Show();
        detailView.Show();

        EditorGUILayout.EndHorizontal();
    }
}
