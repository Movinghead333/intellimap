using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// TODO: Allow control over boxes from outside
public class IntellimapMatrix {
    private EditorWindow parentWindow;
    private float lastWindowWidth;
    private float lastWindowHeight;

    private int size;
    private float spaceBetweenBoxes;
    private float maxPercentageOfWindowHeight;

    private List<IntellimapDraggableBox> boxes;

    public IntellimapMatrix(int size, Color foregroundColor, Color backgroundColor, Color borderColor,
                            float maxPercentageOfWindowHeight, EditorWindow parentWindow)
    {
        this.parentWindow = parentWindow;
        lastWindowWidth = parentWindow.position.width;
        lastWindowHeight = parentWindow.position.height;

        this.size = size;
        spaceBetweenBoxes = 0;
        this.maxPercentageOfWindowHeight = maxPercentageOfWindowHeight;

        boxes = new List<IntellimapDraggableBox>();
        for (int i = 0; i < size * size; i++) {
            boxes.Add(new IntellimapDraggableBox(foregroundColor, backgroundColor, borderColor, parentWindow));
        }

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                if (x > y) {
                    int thisIndex = y * size + x;
                    int otherIndex = x * size + y;
                    boxes[thisIndex].connectWith(boxes[otherIndex]);
                }
            }
        }

        UpdateBoxSize();
    }

    public void Show() {
        HandleWindowResize();

        for (int y = 0; y < size; y++) {
            GUILayout.Space(spaceBetweenBoxes);

            GUILayout.BeginHorizontal();
            
            GUILayout.Space(15);

            for (int x = 0; x < size; x++) {
                int index = y * size + x;
                boxes[index].Show();

                GUILayout.Space(spaceBetweenBoxes);
            }

            GUILayout.EndHorizontal();
        }
    }

    public void SetBoxSize(int boxSize) {
        for (int i = 0; i < boxes.Count; i++) {
            boxes[i].Resize(boxSize, boxSize);
        }
    }

    public int GetSize() {
        return size;
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
        float maxAllowedHeight = maxPercentageOfWindowHeight * windowHeight;

        float blockSizeInclSpace;
        if (maxAllowedHeight < windowWidth) {
            blockSizeInclSpace = maxAllowedHeight / size;
        }
        else {
            blockSizeInclSpace = windowWidth / size;
        }

        // (size-1) instead of size, because this way the boxes add up to be just slightly smaller than the full window width,
        // which makes it so that it doesn't constantly trigger the horizontal scrollbar
        // and also makes some space for the vertical scrollbar on the right.
        float accountingForLastSpace = spaceBetweenBoxes / (size-1);
        int boxSize = (int)(blockSizeInclSpace - spaceBetweenBoxes - accountingForLastSpace);

        SetBoxSize(boxSize);
    }

    private bool WindowSizeChanged() {
        return lastWindowWidth != parentWindow.position.width || lastWindowHeight != parentWindow.position.height;
    }
}
