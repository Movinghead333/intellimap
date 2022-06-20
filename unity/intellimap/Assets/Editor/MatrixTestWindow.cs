using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MatrixTestWindow : EditorWindow {
    IntellimapMatrix matrix;

    [MenuItem ("Window/Matrix Test")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(MatrixTestWindow), false, "Matrix Test");
    }

    public void OnEnable() {
        matrix = new IntellimapMatrix(3, Color.gray, Color.white, Color.black, 0.8f, this);
    }

    private void OnGUI() {
        matrix.Show();
    }
}
