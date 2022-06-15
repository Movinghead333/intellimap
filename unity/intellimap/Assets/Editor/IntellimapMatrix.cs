using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// TODO: Give back fill status of individual boxes | Make symmetrical matrix
public class IntellimapMatrix {
    private EditorWindow parentWindow;
    private float lastWindowWidth;
    private float lastWindowHeight;

    private int width;
    private int height;
    private float spaceBetweenBoxes;
    private float maxPercentageOfWindowHeight;

    private List<IntellimapDraggableBox> boxes;

    public IntellimapMatrix(int width, int height, float spaceBetweenBoxes,
                            Color foregroundColor, Color backgroundColor,
                            float maxPercentageOfWindowHeight, EditorWindow parentWindow)
    {
        this.parentWindow = parentWindow;
        lastWindowWidth = parentWindow.position.width;
        lastWindowHeight = parentWindow.position.height;

        this.width = width;
        this.height = height;
        this.spaceBetweenBoxes = spaceBetweenBoxes;
        this.maxPercentageOfWindowHeight = maxPercentageOfWindowHeight;

        boxes = new List<IntellimapDraggableBox>();
        for (int i = 0; i < width * height; i++) {
            boxes.Add(new IntellimapDraggableBox(foregroundColor, backgroundColor, parentWindow));
        }

        UpdateBoxSize();
    }

    public void Show() {
        HandleWindowResize();

        for (int y = 0; y < height; y++) {
            GUILayout.Space(spaceBetweenBoxes);

            GUILayout.BeginHorizontal();

            for (int x = 0; x < width; x++) {
                GUILayout.Space(spaceBetweenBoxes);

                int boxIndex = y * width + x;
                boxes[boxIndex].Show();
            }

            GUILayout.EndHorizontal();
        }
    }

    public void SetBoxSize(int boxSize) {
        for (int i = 0; i < boxes.Count; i++) {
            boxes[i].Resize(boxSize, boxSize);
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    private void HandleWindowResize() {
        if (WindowSizeChanged()) {
            UpdateBoxSize();

            lastWindowWidth = parentWindow.position.width;
            lastWindowHeight = parentWindow.position.height;
        }
    }

    private void UpdateBoxSize() {
        float windowWidth = parentWindow.position.width;
        float windowHeight = parentWindow.position.height;

        float blockSizeInclSpace = windowWidth / width;

        float totalHeight = blockSizeInclSpace * height;
        float maxAllowedHeight = maxPercentageOfWindowHeight * windowHeight;
        if (totalHeight > maxAllowedHeight) {
            blockSizeInclSpace = maxAllowedHeight / height;
        }

        float accountingForLastSpace = spaceBetweenBoxes / width;
        int boxSize = (int)(blockSizeInclSpace - spaceBetweenBoxes - accountingForLastSpace);

        SetBoxSize(boxSize);
    }

    private bool WindowSizeChanged() {
        return lastWindowWidth != parentWindow.position.width || lastWindowHeight != parentWindow.position.height;
    }
}
