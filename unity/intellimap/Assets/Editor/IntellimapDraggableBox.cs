using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using static IntellimapGUIUtil;

public class IntellimapDraggableBox {
    private EditorWindow parentWindow;

    private int width;
    private int height;
    private float currentPercentage;
    private bool dragStartedInBox;

    private Texture2D texture;
    private Color foregroundColor;
    private Color backgroundColor;
    private GUIStyle style;

    public IntellimapDraggableBox(Color foregroundColor, Color backgroundColor, EditorWindow parentWindow)
        : this(2, 2, foregroundColor, backgroundColor, parentWindow) {}

    public IntellimapDraggableBox(int width, int height, Color foregroundColor, Color backgroundColor, EditorWindow parentWindow) {
        this.parentWindow = parentWindow;

        this.width = width;
        this.height = height;
        dragStartedInBox = false;

        texture = new Texture2D(width, height);
        this.foregroundColor = foregroundColor;
        this.backgroundColor = backgroundColor;
        FillTextureUpTo(height / 2);

        style = new GUIStyle();
        style.normal.background = texture;
        style.fixedWidth = width;
        style.fixedHeight = height;
    }

    public void Show() {
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
        float freeSpaceOnTop = LimitToBounds(mouseY - topY, lower: 0, upper: height);
        float newFillHeight = height - freeSpaceOnTop;
        FillTextureUpTo(newFillHeight);
    }

    // TODO: Make more efficient (give the entire array of data at once instead of setting every pixel individually)
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

    public float GetPercentage() {
        return currentPercentage;
    }

    public void Resize(int width, int height) {
        this.width = width;
        this.height = height;
        texture.Reinitialize(width, height);

        FillTextureUpTo(currentPercentage * height);

        style.fixedWidth = width;
        style.fixedHeight = height;
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
}
