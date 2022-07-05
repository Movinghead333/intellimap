using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System;

public class Matrix {
    private EditorWindow parentWindow;
    private float lastWindowWidth;
    private float lastWindowHeight;

    private WeightBoxDetailView detailView;

    private int size;
    private float maxPercentageOfWindowHeight;
    private int boxSize;
    private int minBoxSize;

    Color foregroundColor;
    Color backgroundColor;
    Color borderColor;

    private WeightBox[] boxes;

    private TextureBox axisTitleBox;
    private TextureBox[] axisBoxes;

    public Matrix(int size, Color foregroundColor, Color backgroundColor, Color borderColor,
                            float maxPercentageOfWindowHeight, int minBoxSize, EditorWindow parentWindow)
    {
        this.parentWindow = parentWindow;
        lastWindowWidth = parentWindow.position.width;
        lastWindowHeight = parentWindow.position.height;

        detailView = new WeightBoxDetailView();

        this.maxPercentageOfWindowHeight = maxPercentageOfWindowHeight;
        this.minBoxSize = minBoxSize;

        this.foregroundColor = foregroundColor;
        this.backgroundColor = backgroundColor;
        this.borderColor = borderColor;

        Init(size);
    }

    public void Init(int size) {
        this.size = size;

        boxes = new WeightBox[size * size];
        for (int i = 0; i < size * size; i++) {
            boxes[i] = new WeightBox(foregroundColor, backgroundColor, borderColor, detailView, parentWindow);
        }

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                int thisIndex = y * size + x;

                if (x > y) {
                    int otherIndex = x * size + y;
                    boxes[thisIndex].connectWith(boxes[otherIndex]);
                }
                else if (x < y) {
                    boxes[thisIndex].SetAlpha(0.6f);
                }
            }
        }

        axisTitleBox = new TextureBox(backgroundColor, borderColor);
        //axisTitleBox.SetText("Weights");

        axisBoxes = new TextureBox[size];
        for (int i = 0; i < axisBoxes.Length; i++) {
            axisBoxes[i] = new TextureBox(backgroundColor, borderColor);
        }

        UpdateBoxSize(forceResize: true);
    }

    public void Show() {
        HandleWindowResize();

        GUILayout.Space(15);
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

        detailView.Show();
    }

    public void SetBoxWeights(int x, int y, float[] weights) {
        int index = y * size + x;
        boxes[index].SetWeights(weights);
    }

    public float[] GetBoxWeights(int x, int y) {
        int index = y * size + x;
        return boxes[index].GetWeights();
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
            UpdateBoxSize(forceResize: false);

            lastWindowWidth = parentWindow.position.width;
            lastWindowHeight = parentWindow.position.height;
        }
    }

    private void UpdateBoxSize(bool forceResize) {
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
        
        if (boxSize < minBoxSize) {
            boxSize = minBoxSize;
        }

        if (forceResize || boxSize != this.boxSize) {
            this.boxSize = boxSize;
            SetBoxSize(this.boxSize);
        }
    }

    private bool WindowSizeChanged() {
        return lastWindowWidth != parentWindow.position.width || lastWindowHeight != parentWindow.position.height;
    }
}