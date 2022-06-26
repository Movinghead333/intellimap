using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System;

// TODO: Make a minimal size constraint for the matrix
public class IntellimapMatrix {
    private EditorWindow parentWindow;
    private float lastWindowWidth;
    private float lastWindowHeight;

    private int size;
    private float maxPercentageOfWindowHeight;
    private int boxSize;

    private IntellimapDraggableBox[] boxes;

    private IntellimapTextureBox axisTitleBox;
    private IntellimapTextureBox[] axisBoxes;

    public IntellimapMatrix(int size, Color foregroundColor, Color backgroundColor, Color borderColor,
                            float maxPercentageOfWindowHeight, EditorWindow parentWindow)
    {
        this.parentWindow = parentWindow;
        lastWindowWidth = parentWindow.position.width;
        lastWindowHeight = parentWindow.position.height;

        this.size = size;
        this.maxPercentageOfWindowHeight = maxPercentageOfWindowHeight;

        boxes = new IntellimapDraggableBox[size * size];
        for (int i = 0; i < size * size; i++) {
            boxes[i] = new IntellimapDraggableBox(foregroundColor, backgroundColor, borderColor, parentWindow);
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

        axisTitleBox = new IntellimapTextureBox(Color.clear, Color.grey);
        axisTitleBox.SetText("Weights");

        axisBoxes = new IntellimapTextureBox[size];
        for (int i = 0; i < axisBoxes.Length; i++) {
            axisBoxes[i] = new IntellimapTextureBox(Color.clear, Color.grey);
        }

        UpdateBoxSize();
    }

    public void Show() {
        HandleWindowResize();

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        axisTitleBox.Show();
        for (int x = 0; x < size; x++) {
            axisBoxes[x].Show();
        }
        GUILayout.EndHorizontal();

        for (int y = 0; y < size; y++) {
            GUILayout.BeginHorizontal();
            
            GUILayout.Space(15);

            for (int x = -1; x < size; x++) {
                if (x == -1) {
                    axisBoxes[y].Show();
                }
                else {
                    int index = y * size + x;
                    boxes[index].Show();
                }
            }

            GUILayout.EndHorizontal();
        }
    }

    public void SetBoxSize(int boxSize) {
        for (int i = 0; i < boxes.Length; i++) {
            boxes[i].Resize(boxSize, boxSize);
        }
        
        axisTitleBox.Resize(boxSize, boxSize);
        for (int i = 0; i < axisBoxes.Length; i++) {
            axisBoxes[i].Resize(boxSize, boxSize);
        }
    }

    public int GetSize() {
        return size;
    }

    public void SetAxisTiles(Tile[] tiles) {
        if (tiles.Length != axisBoxes.Length) {
            throw new ArgumentException("Array lengths don't match");
        }

        for (int i = 0; i < tiles.Length; i++) {
            if (tiles[i] != null) {
                Sprite sprite = tiles[i].sprite;
                axisBoxes[i].SetTexture(sprite.texture, sprite.textureRect);
            }
            else {
                axisBoxes[i].SetTexture(null, new Rect());
            }
        }
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

        int sizeInclAxes = size + 1;

        int boxSize;
        if (maxAllowedHeight < windowWidth) {
            boxSize = (int)maxAllowedHeight / sizeInclAxes;
        }
        else {
            boxSize = (int)windowWidth / sizeInclAxes;
        }

        // 2*15 for the hardcoded space in Show() and 5 for a potential scrollbar on the right
        float correctingForSpace = 35.0f / sizeInclAxes;
        boxSize -= (int)correctingForSpace;

        if (boxSize != this.boxSize) {
            this.boxSize = boxSize;
            SetBoxSize(this.boxSize);
        }
    }

    private bool WindowSizeChanged() {
        return lastWindowWidth != parentWindow.position.width || lastWindowHeight != parentWindow.position.height;
    }
}
