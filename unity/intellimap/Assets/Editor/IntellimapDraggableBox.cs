using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using static IntellimapGUIUtil;

public class IntellimapDraggableBox : IntellimapBox {
    private EditorWindow parentWindow;

    private float currentPercentage;
    private bool dragStartedInBox;

    private IntellimapDraggableBox connectedBox;

    public IntellimapDraggableBox(Color foregroundColor, Color backgroundColor, Color borderColor, EditorWindow parentWindow)
        : this(10, 10, foregroundColor, backgroundColor, borderColor, parentWindow) {}

    public IntellimapDraggableBox(int width, int height, Color foregroundColor, Color backgroundColor, Color borderColor, EditorWindow parentWindow)
        : base(width, height, foregroundColor, backgroundColor, borderColor)
    {
        this.parentWindow = parentWindow;

        currentPercentage = 0.5f;
        dragStartedInBox = false;

        UpdateTexture();

        connectedBox = null;
    }

    public override void Show() {
        base.Show();
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

    public override void Resize(int width, int height) {
        base.Resize(width, height);
        UpdateTexture();
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
                if (!DrawBorder(x, y)) {
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
