using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class Matrix {
    private EditorWindow parentWindow;
    private float lastWindowWidth;
    private float lastWindowHeight;

    private DetailView detailView;

    private int size;
    private float maxPercentageOfWindowHeight;
    private int boxSize;
    private int minBoxSize;

    Color foregroundColor;
    Color backgroundColor;
    Color borderColor;
    Color highlightBorderColor;

    private WeightBox[] boxes;

    private TextureBox axisTitleBox;
    private TextureBox[] axisBoxes;

    public Matrix(int size, Color foregroundColor, Color backgroundColor, Color borderColor, Color highlightBorderColor,
                  float maxPercentageOfWindowHeight, int minBoxSize, EditorWindow parentWindow)
    {
        this.parentWindow = parentWindow;
        lastWindowWidth = parentWindow.position.width;
        lastWindowHeight = parentWindow.position.height;

        this.maxPercentageOfWindowHeight = maxPercentageOfWindowHeight;
        this.minBoxSize = minBoxSize;

        this.foregroundColor = foregroundColor;
        this.backgroundColor = backgroundColor;
        this.borderColor = borderColor;
        this.highlightBorderColor = highlightBorderColor;

        Init(size);
    }

    public void Init(int size) {
        this.size = size;

        detailView = new DetailView();

        Color weightBoxBorderColor = new Color(borderColor.r, borderColor.g, borderColor.b, 0.3f);

        boxes = new WeightBox[size * size];
        for (int i = 0; i < size * size; i++) {
            boxes[i] = new WeightBox(foregroundColor, backgroundColor, weightBoxBorderColor, highlightBorderColor, detailView);
        }

        Color diagonalBackgroundColor = new Color(backgroundColor.r + 0.05f, backgroundColor.g + 0.05f, backgroundColor.b + 0.05f);

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                int thisIndex = y * size + x;

                if (x > y) {
                    int otherIndex = x * size + y;
                    boxes[thisIndex].connectWith(boxes[otherIndex]);
                }
                else if (x == y) {
                    boxes[thisIndex].SetBackgroundColor(diagonalBackgroundColor);
                }
            }
        }

        axisTitleBox = new TextureBox(backgroundColor, borderColor);

        axisBoxes = new TextureBox[size];
        for (int i = 0; i < axisBoxes.Length; i++) {
            axisBoxes[i] = new TextureBox(backgroundColor, borderColor);
        }

        UpdateBoxSize(forceResize: true);
    }

    public void Show() {
        HandleWindowResize();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
            GUILayout.Space(IntellimapEditor.startingSpace);

            axisTitleBox.Show();
            for (int x = 0; x < size; x++) {
                axisBoxes[x].Show();
            }
        GUILayout.EndHorizontal();

        for (int y = 0; y < size; y++) {
            GUILayout.BeginHorizontal();
                GUILayout.Space(IntellimapEditor.startingSpace);

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

    public float[,,] GetAllBoxWeights() {
        float[,,] result = new float[size, size, 4];

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float[] boxWeights = GetBoxWeights(x, y);
                result[x, y, 0] = boxWeights[0];
                result[x, y, 1] = boxWeights[1];
                result[x, y, 2] = boxWeights[2];
                result[x, y, 3] = boxWeights[3];
            }
        }

        return result;
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
                axisBoxes[i].SetNoTexture();
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

        float boxSize;
        if (maxAllowedHeight < windowWidth) {
            boxSize = maxAllowedHeight / sizeInclAxes;
        }
        else {
            boxSize = windowWidth / sizeInclAxes;
        }

        // plus 5 for a potential scrollbar on the right
        float correctingForSpace = (2 * IntellimapEditor.startingSpace + 5) / sizeInclAxes;
        boxSize -= correctingForSpace;

        if (boxSize < minBoxSize) {
            boxSize = minBoxSize;
        }

        if (forceResize || boxSize != this.boxSize) {
            this.boxSize = (int)boxSize;
            SetBoxSize(this.boxSize);
        }
    }

    private bool WindowSizeChanged() {
        return lastWindowWidth != parentWindow.position.width || lastWindowHeight != parentWindow.position.height;
    }
}
