using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IntellimapDraggableBox {
    Texture2D texture;
    
    private int width;
    private int height;
    private float currentPercentage;

    Color foregroundColor;
    Color backgroundColor;

    private bool dragStartedInBox;

    EditorWindow parentWindow;

    public IntellimapDraggableBox(int width, int height, Color foregroundColor, Color backgroundColor, EditorWindow parentWindow) {
        this.parentWindow = parentWindow;
        texture = new Texture2D(width, height);

        this.width = width;
        this.height = height;

        this.foregroundColor = foregroundColor;
        this.backgroundColor = backgroundColor;

        dragStartedInBox = false;

        FillTextureUpTo(height / 2);
    }

    public void Show() {
        GUIStyle style = new GUIStyle();
        style.normal.background = texture;
        style.fixedWidth = width;
        style.fixedHeight = height;

        GUILayout.Box(GUIContent.none, style);
        Rect boxRect = GUILayoutUtility.GetLastRect();

        if (LeftMouseButton()) {
            float mouseX = Event.current.mousePosition.x;
            float mouseY = Event.current.mousePosition.y;

            if (MouseDown()) {
                if (InRectangle(boxRect, mouseX, mouseY)) {
                    dragStartedInBox = true;

                    UpdateTexture(mouseY, boxRect.y);
                }
            }
            
            if (dragStartedInBox && MouseDrag()) {
                UpdateTexture(mouseY, boxRect.y);
            }

            if (MouseUp()) {
                dragStartedInBox = false;
            }
        }
    }

    private void UpdateTexture(float mouseY, float topY) {
        float freeSpaceOnTop = IntellimapGUIUtil.LimitToBounds(mouseY - topY, lower: 0, upper: height);
        float newFillHeight = height - freeSpaceOnTop;
        FillTextureUpTo(newFillHeight);
    }

    private void FillTextureUpTo(float newFillHeight) {
        currentPercentage = newFillHeight / height;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (y < newFillHeight)
                    texture.SetPixel(x, y, foregroundColor);
                else
                    texture.SetPixel(x, y, backgroundColor);
            }
        }
        texture.Apply();
        
        parentWindow.Repaint();
    }

    private bool InRectangle(Rect rect, float x, float y) {
        return x > rect.x && y > rect.y && x < rect.x + rect.width && y < rect.y + rect.height;
    }

    private bool LeftMouseButton() {
        return Event.current.button == 0;
    }

    private bool MouseDown() {
        return Event.current.type == EventType.MouseDown;
    }

    private bool MouseUp() {
        return Event.current.type == EventType.MouseUp;
    }

    private bool MouseDrag() {
        return Event.current.type == EventType.MouseDrag;
    }

    public float GetPercentage() {
        return currentPercentage;
    }

}
