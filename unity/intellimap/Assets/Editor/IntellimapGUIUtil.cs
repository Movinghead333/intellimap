using UnityEngine;
using UnityEditor;

public class IntellimapGUIUtil : EditorWindow {
    private static GUIStyle horizontalLine = null;

    public static void HorizontalLine(Color color, int leftMargin = 0, int rightMargin = 0, int topMargin = 10, int bottomMargin = 10) {
        if (horizontalLine == null) {
            horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.fixedHeight = 1;
        }

        horizontalLine.margin = new RectOffset(leftMargin, rightMargin, topMargin, bottomMargin);

        var tempColor = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = tempColor;
    }
}
