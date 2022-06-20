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
    private Color borderColor;
    private GUIStyle style;

    private IntellimapDraggableBox connectedBox;

    public IntellimapDraggableBox(Color foregroundColor, Color backgroundColor, Color borderColor, EditorWindow parentWindow)
        : this(2, 2, foregroundColor, backgroundColor, borderColor, parentWindow) {}

    public IntellimapDraggableBox(int width, int height, Color foregroundColor, Color backgroundColor, Color borderColor, EditorWindow parentWindow) {
        this.parentWindow = parentWindow;

        this.width = width;
        this.height = height;
        currentPercentage = 0.5f;
        dragStartedInBox = false;

        texture = new Texture2D(width, height);
        this.foregroundColor = foregroundColor;
        this.backgroundColor = backgroundColor;
        this.borderColor = borderColor;
        UpdateTexture();

        style = new GUIStyle();
        style.normal.background = texture;
        style.fixedWidth = width;
        style.fixedHeight = height;

        connectedBox = null;
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

                    HandleMouseDragEvent(mouseY, boxRect.y);
                }
            }
            
            if (dragStartedInBox && MouseDrag()) {
                HandleMouseDragEvent(mouseY, boxRect.y);
            }

            if (MouseUp()) {
                dragStartedInBox = false;
            }
        }
    }

    public void connectWith(IntellimapDraggableBox otherBox) {
        connectedBox = otherBox;
        otherBox.connectedBox = this;
    }

    public float GetPercentage() {
        return currentPercentage;
    }

    public void SetPercentage(float percentage) {
        currentPercentage = percentage;
        UpdateTexture();

        if (connectedBox != null) {
            connectedBox.currentPercentage = percentage;
            connectedBox.UpdateTexture();
        }
    }

    public void Resize(int width, int height) {
        this.width = width;
        this.height = height;
        texture.Reinitialize(width, height);

        UpdateTexture();

        style.fixedWidth = width;
        style.fixedHeight = height;
    }

    private void HandleMouseDragEvent(float mouseY, float topY) {
        float freeSpaceOnTop = LimitToBounds(mouseY - topY, lower: 0, upper: height);
        float newFillHeight = height - freeSpaceOnTop;

        SetPercentage(newFillHeight / height);
    }

    // TODO: Make more efficient (give the entire array of data at once instead of setting every pixel individually)
    private void UpdateTexture() {
        int fillHeight = (int)(currentPercentage * height);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (y == 0 || x == 0 || y == height - 1 || x == width - 1) {
                    texture.SetPixel(x, y, borderColor);
                }
                else {
                    if (y < fillHeight)
                        texture.SetPixel(x, y, foregroundColor);
                    else
                        texture.SetPixel(x, y, backgroundColor);
                }

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
}
