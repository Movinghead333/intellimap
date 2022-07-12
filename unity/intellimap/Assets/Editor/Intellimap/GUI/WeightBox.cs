using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

using static GUIUtil;

public class WeightBox : Box {
    private WeightBoxDetailView detailView;

    private float[] weights;
    private float[] startingWeights;

    private float currentPercentage;
    private float startingPercentage;
    private bool dragStartedInBox;

    private Color highlightBorderColor;
    private Color originalBorderColor;

    private WeightBox connectedBox;

    // Used to repaint the window *only* when the mouse enters or leaves a box
    // and not every time the mouse moves
    private bool mouseIn;

    public WeightBox(Color foregroundColor, Color backgroundColor, Color borderColor, Color highlightBorderColor, WeightBoxDetailView detailView)
        : this(10, 10, foregroundColor, backgroundColor, borderColor, highlightBorderColor, detailView) {}

    public WeightBox(int width, int height, Color foregroundColor, Color backgroundColor, Color borderColor, Color highlightBorderColor, WeightBoxDetailView detailView)
        : base(width, height, foregroundColor, backgroundColor, borderColor)
    {
        this.detailView = detailView;

        SetContentOffset(2, 0);

        weights = new float[4];
        startingWeights = new float[4];
        weights[0] = startingWeights[0] = 0.5f;
        weights[1] = startingWeights[1] = 0.5f;
        weights[2] = startingWeights[2] = 0.5f;
        weights[3] = startingWeights[3] = 0.5f;

        UpdatePercentageByWeights();
        dragStartedInBox = false;

        UpdateTexture();

        this.highlightBorderColor = highlightBorderColor;
        originalBorderColor = borderColor;

        connectedBox = null;

        mouseIn = false;
    }

    public override void Show() {
        base.Show();
        Rect boxRect = GUILayoutUtility.GetLastRect();
        
        float mouseX = Event.current.mousePosition.x;
        float mouseY = Event.current.mousePosition.y;

        if (MouseMove()) {
            bool inRect = InRectangle(boxRect, mouseX, mouseY);

            if (!mouseIn && inRect) {
                mouseIn = true;

                SetText(GetPercentage().ToString());
                HighlightBorderColor();

                UpdateTexture();
                IntellimapEditor.repaint = true;
            }
            else if (mouseIn && !inRect) {
                mouseIn = false;

                SetText("");
                ResetBorderColor();

                UpdateTexture();
                IntellimapEditor.repaint = true;
            }
        }

        if (LeftMouseButton()) {
            if (MouseDown()) {
                if (InRectangle(boxRect, mouseX, mouseY)) {
                    dragStartedInBox = true;

                    detailView.SetBox(this);
                    IntellimapEditor.repaint = true;
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

    public void connectWith(WeightBox otherBox) {
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

        UpdatePercentageByWeights();
        UpdateTexture();
        UpdateConnectedBox();
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
        float[] change = new float[4];

        // calculate change per weight
        if (isPositive(diff)) {
            // going up

            if (currentPercentage < startingPercentage) {
                // going up towards the starting percentage

                if (percentage > startingPercentage) {
                    // overshooting the starting percentage upwards, needs 2 steps:

                    // until the starting percentage, take the distance to the starting percentage
                    percentageDistance = startingPercentage - currentPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = startingWeights[i] - weights[i];
                    }

                    float diffPercentage = (startingPercentage - currentPercentage) / diff;

                    // calculate change
                    for (int i = 0; i < 4; i++) {
                        float speed = weightDistances[i] / percentageDistance;
                        change[i] = diffPercentage * diff * speed;
                    }

                    // the rest of the way, take the distance to the top
                    percentageDistance = 1f - startingPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = 1f - startingWeights[i];
                    }
                    
                    // calculate change
                    for (int i = 0; i < 4; i++) {
                        float speed = weightDistances[i] / percentageDistance;
                        change[i] += (1 - diffPercentage) * diff * speed;
                    }
                }
                else {
                    // take the distance up to the starting percentage
                    percentageDistance = startingPercentage - currentPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = startingWeights[i] - weights[i];
                    }

                    // calculate change
                    for (int i = 0; i < 4; i++) {
                        float speed = weightDistances[i] / percentageDistance;
                        change[i] = diff * speed;
                    }
                }
            }
            else {
                // take entire distance to the top
                percentageDistance = 1f - currentPercentage;
                for (int i = 0; i < 4; i++) {
                    weightDistances[i] = 1f - weights[i];
                }

                // calculate change
                for (int i = 0; i < 4; i++) {
                    float speed = weightDistances[i] / percentageDistance;
                    change[i] = diff * speed;
                }
            }
        }
        else {
            // going down

            if (currentPercentage > startingPercentage) {
                // going down towards the starting percentage

                if (percentage < startingPercentage) {
                    // overshooting the starting percentage downwards, needs 2 steps:
                    
                    // until the starting percentage, take the distance to the starting percentage
                    percentageDistance = currentPercentage - startingPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = weights[i] - startingWeights[i];
                    }

                    float diffPercentage = (currentPercentage - startingPercentage) / Mathf.Abs(diff);

                    // calculate change
                    for (int i = 0; i < 4; i++) {
                        float speed = weightDistances[i] / percentageDistance;
                        change[i] = (diffPercentage * diff) * speed;
                    }

                    // the rest of the way, take the distance to the bottom
                    percentageDistance = startingPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = startingWeights[i];
                    }

                    // calculate change
                    for (int i = 0; i < 4; i++) {
                        float speed = weightDistances[i] / percentageDistance;
                        change[i] += ((1 - diffPercentage) * diff) * speed;
                    }
                }
                else {
                    // take the distance down to the starting percentage
                    percentageDistance = currentPercentage - startingPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = weights[i] - startingWeights[i];
                    }

                    // calculate change
                    for (int i = 0; i < 4; i++) {
                        float speed = weightDistances[i] / percentageDistance;
                        change[i] = diff * speed;
                    }
                }
            }
            else {
                // take entire distance to the bottom
                percentageDistance = currentPercentage;
                for (int i = 0; i < 4; i++) {
                    weightDistances[i] = weights[i];
                }

                // calculate change
                for (int i = 0; i < 4; i++) {
                    float speed = weightDistances[i] / percentageDistance;
                    change[i] = diff * speed;
                }
            }
        }

        // Update the weights
        for (int i = 0; i < 4; i++) {
            weights[i] = LimitToBounds(weights[i] + change[i], lower: 0f, upper: 1f);
        }
        
        currentPercentage = percentage;

        detailView.UpdateFromBox();

        UpdateTexture();
        SetText(GetPercentage().ToString());

        UpdateConnectedBox();
    }

    public override void Resize(int width, int height) {
        base.Resize(width, height);
        UpdateTexture();
    }

    private void UpdateConnectedBox() {
        if (connectedBox != null) {
            connectedBox.weights[0] = weights[2];
            connectedBox.weights[1] = weights[3];
            connectedBox.weights[2] = weights[0];
            connectedBox.weights[3] = weights[1];

            connectedBox.currentPercentage = currentPercentage;
            connectedBox.UpdateTexture();
        }
    }

    private void HighlightBorderColor() {
        borderColor = highlightBorderColor;
    }

    private void ResetBorderColor() {
        borderColor = originalBorderColor;
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
        
        IntellimapEditor.repaint = true;
    }
}
