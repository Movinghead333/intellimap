using System;
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

    public static bool CenteredButton(string text, float width, float height) {
        bool pressed = false;

        EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(text, GUILayout.Width(width), GUILayout.Height(height))) {
                pressed = true;
            }

            GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        return pressed;
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

    public static int GetTimestamp() {
        return (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }

    public static bool InRectangle(Rect rect, float x, float y) {
        return x >= rect.x && y >= rect.y && x < rect.x + rect.width && y < rect.y + rect.height;
    }

    public static bool LeftMouseButton() {
        return Event.current.button == 0;
    }

    public static bool MouseDown() {
        return Event.current.type == EventType.MouseDown;
    }

    public static bool MouseUp() {
        return Event.current.type == EventType.MouseUp;
    }

    public static bool MouseDrag() {
        return Event.current.type == EventType.MouseDrag;
    }

    public static bool MouseMove() {
        return Event.current.type == EventType.MouseMove;
    }

    public static Vector2 MouseScroll() {
        if (Event.current.isScrollWheel) {
            return Event.current.delta;        
        }
        else {
            return Vector2.zero;
        }
    }

    public static bool CtrlHeld() {
        return Event.current.control;
    }
}
