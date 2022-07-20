using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

/*
 * This is a class that was just used for testing, mainly for the matrix and weight boxes.
 * Leaving the old debug test code in here would not be helpful though, since constructors and a lot more changed.
 * Using the debug elements now is clunky because the way the components repaint also changed.
 */
public class MatrixTestWindow : EditorWindow {
    public Tile[] testTiles;
    SerializedObject serializedObject;
    SerializedProperty serializedProperty;

    Matrix matrix;

    WeightBox testBox;
    DetailView detailView;

    [MenuItem ("Window/Matrix Test")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(MatrixTestWindow), false, "Matrix Test");
    }

    public void OnEnable() {
        Color foregroundColor = new Color(0.8f, 0.8f, 0.8f);
        matrix = new Matrix(3, foregroundColor, Color.clear, Color.grey, Color.red, 0.5f, 20, this);
        testTiles = new Tile[3];

        serializedObject = new SerializedObject(this);
        serializedProperty = serializedObject.FindProperty("testTiles");

        detailView = new DetailView();

        testBox = new WeightBox(50, 50, Color.white, Color.black, Color.blue, Color.green, detailView);
    }

    private void OnGUI() {
        EditorGUILayout.PropertyField(serializedProperty, true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Set Axis Tiles")) {
           matrix.SetAxisTiles(testTiles);
        }

        GUILayout.Space(15);

        matrix.Show();

        GUIUtil.HorizontalLine(Color.grey);

        EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            testBox.Show();
            detailView.Show();
        EditorGUILayout.EndHorizontal();
    }
}
