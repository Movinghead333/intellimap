using UnityEngine;
using UnityEditor;

public class GUIUtil {
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

    public static float LimitToBounds(float f, float lower, float upper) {
        if (f < lower) {
            return lower;
        }
        else if (f > upper) {
            return upper;
        }

        return f;
    }

    public static bool isPositive(float f) {
        return f > 0;
    }

    public static float Sum(float[] arr) {
        float result = 0;

        for (int i = 0; i < arr.Length; i++) {
            result += arr[i];
        }

        return result;
    }
}
