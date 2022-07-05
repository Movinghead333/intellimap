using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

using static IntellimapGUIUtil;

public class IntellimapWeightBox : IntellimapBox {
    private EditorWindow parentWindow;

    float[] weights;
    float[] startingWeights;

    private float currentPercentage;
    private float startingPercentage;
    private bool dragStartedInBox;

    private IntellimapWeightBox connectedBox;

    // DEBUG
    public IntellimapHistogram debugHistogram;

    public IntellimapWeightBox(Color foregroundColor, Color backgroundColor, Color borderColor, EditorWindow parentWindow)
        : this(10, 10, foregroundColor, backgroundColor, borderColor, parentWindow) {}

    public IntellimapWeightBox(int width, int height, Color foregroundColor, Color backgroundColor, Color borderColor, EditorWindow parentWindow)
        : base(width, height, foregroundColor, backgroundColor, borderColor)
    {
        this.parentWindow = parentWindow;

        // Every DraggableBox has 4 weights that are automatically updated.
        // Just initialize them to 50%.
        weights = new float[4];
        startingWeights = new float[4];
        weights[0] = startingWeights[0] = 0.5f;
        weights[1] = startingWeights[1] = 0.5f;
        weights[2] = startingWeights[2] = 0.5f;
        weights[3] = startingWeights[3] = 0.5f;

        UpdatePercentageByWeights();
        dragStartedInBox = false;

        UpdateTexture();

        connectedBox = null;

        debugHistogram = null;
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

    public void connectWith(IntellimapWeightBox otherBox) {
        connectedBox = otherBox;
        otherBox.connectedBox = this;
    }

    public float[] GetWeights() {
        return weights;
    }

    public void SetWeights(float[] weights) {
        if (weights.Length != 4) {
            throw new ArgumentException("Weight arrays for draggable boxes must have length 4");
        }

        for (int i = 0; i < 4; i++) {
            this.weights[i] = startingWeights[i] = weights[i];
        }

        if (debugHistogram != null) {
            List<float> l = new List<float>();
            l.Add(weights[0] * 100);
            l.Add(weights[1] * 100);
            l.Add(weights[2] * 100);
            l.Add(weights[3] * 100);

            debugHistogram.SetSliderValues(l);
        }

        UpdatePercentageByWeights();
        UpdateTexture();
    }

    public float GetPercentage() {
        return currentPercentage;
    }

    public void SetPercentage(float percentage) {
        float diff = percentage - currentPercentage;
        if (diff == 0) {
            return;
        }

        float percentageDistance;
        float[] weightDistances = new float[4];

        // TODO: Make this prettier
        if (currentPercentage == startingPercentage) {
            percentageDistance = (isPositive(diff)) ? 1f - currentPercentage : currentPercentage;
            for (int i = 0; i < 4; i++) {
                weightDistances[i] = (isPositive(diff)) ? 1f - weights[i] : weights[i];
            }
        }
        else if (currentPercentage < startingPercentage) {
            percentageDistance = (isPositive(diff)) ? startingPercentage - currentPercentage : currentPercentage;
            for (int i = 0; i < 4; i++) {
                weightDistances[i] = (isPositive(diff)) ? startingWeights[i] - weights[i] : weights[i];
            }
        }
        else {
            percentageDistance = (isPositive(diff)) ? 1f - currentPercentage : currentPercentage - startingPercentage;
            for (int i = 0; i < 4; i++) {
                weightDistances[i] = (isPositive(diff)) ? 1f - weights[i] : weights[i] - startingWeights[i];
            }
        }

        for (int i = 0; i < 4; i++) {
            float speed = weightDistances[i] / percentageDistance;
            weights[i] = LimitToBounds(weights[i] + diff * speed, lower: 0f, upper: 1f);
        }
        
        if (debugHistogram != null) {
            List<float> l = new List<float>() {weights[0] * 100, weights[1] * 100, weights[2] * 100, weights[3] * 100};
            debugHistogram.SetSliderValues(l);
        }

        currentPercentage = percentage;
        UpdateTexture();

        if (connectedBox != null) {
            for (int i = 0; i < 4; i++) {
                connectedBox.weights[i] = weights[i];
            }

            connectedBox.currentPercentage = percentage;
            connectedBox.UpdateTexture();
        }
    }

    public override void Resize(int width, int height) {
        base.Resize(width, height);
        UpdateTexture();
    }

    private void UpdatePercentageByWeights() {
        currentPercentage = startingPercentage = weights.Average();
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
