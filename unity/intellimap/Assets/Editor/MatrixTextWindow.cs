using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MatrixTextWindow : EditorWindow {
    private IntellimapDraggableBox draggableBox;
    
    [MenuItem ("Window/Matrix Test")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(MatrixTextWindow), false, "Matrix Test");
    }

    public void OnEnable() {
        //wantsMouseMove = true;
        draggableBox = new IntellimapDraggableBox(100, 100, Color.grey, Color.white, this);
    }

    private void OnGUI() {
        draggableBox.Show();
    }
}
