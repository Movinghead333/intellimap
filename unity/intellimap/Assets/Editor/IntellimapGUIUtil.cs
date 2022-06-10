using UnityEngine;
using UnityEditor;

public class IntellimapGUIUtil : EditorWindow {
    public static void HorizontalLine(Color color, int leftMargin = 0, int rightMargin = 0, int topMargin = 10, int bottomMargin = 10) {
        GUIStyle horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(leftMargin, rightMargin, topMargin, bottomMargin);
        horizontalLine.fixedHeight = 1;

        var tempColor = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = tempColor;
    }
}
