using System;
using System.Linq;
using UnityEngine;

using static GUIUtil;
using static IntellimapInput;

public class WeightBox : Box {
    private DetailView detailView;

    private float[] weights;
    private float[] startingWeights;

    private float currentPercentage;
    private float startingPercentage;

    private bool dragStartedInBox;

    private Color foregroundColor;
    private Color highlightBorderColor;
    private Color originalBorderColor;

    private WeightBox connectedBox;

    // Used to repaint the window *only* when the mouse enters or leaves a box
    private bool mouseIn;

    public WeightBox(Color foregroundColor, Color backgroundColor, Color borderColor, Color highlightBorderColor, DetailView detailView)
        : this(10, 10, foregroundColor, backgroundColor, borderColor, highlightBorderColor, detailView) {}

    public WeightBox(int width, int height, Color foregroundColor, Color backgroundColor, Color borderColor, Color highlightBorderColor, DetailView detailView)
        : base(width, height, backgroundColor, borderColor)
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

        this.foregroundColor = foregroundColor;
        this.highlightBorderColor = highlightBorderColor;
        originalBorderColor = borderColor;

        connectedBox = null;

        mouseIn = false;

        UpdateTexture();
    }

    public override void Show() {
        base.Show();
        Rect boxRect = GUILayoutUtility.GetLastRect();
        
        float mouseX = Event.current.mousePosition.x;
        float mouseY = Event.current.mousePosition.y;

        bool inRect = InRectangle(boxRect, mouseX, mouseY);

        if (LeftMouseButton()) {
            if (MouseDown() && inRect) {
                dragStartedInBox = true;

                detailView.SetBox(this);
                IntellimapEditor.repaint = true;
            }
            
            if (dragStartedInBox && MouseDrag()) {
                HandleMouseDragEvent(mouseY, boxRect.y);
            }

            if (MouseUp()) {
                dragStartedInBox = false;
            }
        }

        if (MouseMove()) {
            if (!mouseIn && inRect) {
                mouseIn = true;

                SetText(GetPercentage().ToString());
                HighlightBorderColor();
                UpdateTexture();

                if (connectedBox != null) {
                    connectedBox.HighlightBorderColor();
                    connectedBox.UpdateTexture();
                }

                IntellimapEditor.repaint = true;
            }
            else if (mouseIn && !inRect) {
                mouseIn = false;

                SetText("");
                ResetBorderColor();
                UpdateTexture();

                if (connectedBox != null) {
                    connectedBox.ResetBorderColor();
                    connectedBox.UpdateTexture();
                }

                IntellimapEditor.repaint = true;
            }
        }

        if (inRect) {
            float scrollAmount = MouseScroll().y;
            if (scrollAmount != 0f && CtrlHeld()) {
                if (scrollAmount > 0) {
                    SetPercentage(currentPercentage - 0.001f);
                }
                else {
                    SetPercentage(currentPercentage + 0.001f);
                }
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
        percentage = LimitToBounds(percentage, lower: 0f, upper: 1f);
        if (percentage == currentPercentage) {
            return;
        }

        float[] change = calculateWeightChange(percentage);
        for (int i = 0; i < 4; i++) {
            weights[i] = LimitToBounds(weights[i] + change[i], lower: 0f, upper: 1f);
        }
        
        currentPercentage = percentage;

        if (detailView.GetBox() != this) {
            detailView.SetBox(this);
        }
        detailView.UpdateFromBox();

        UpdateTexture();
        SetText(GetPercentage().ToString());

        UpdateConnectedBox();
    }

    private float[] calculateWeightChange(float newPercentage) {
        float[] change = new float[4];

        float diff = newPercentage - currentPercentage;

        float percentageDistance;
        float[] weightDistances = new float[4];

        if (isPositive(diff)) {
            // going up

            if (currentPercentage < startingPercentage) {
                // going up towards the starting percentage

                if (newPercentage > startingPercentage) {
                    // overshooting the starting percentage upwards, needs 2 steps:

                    // until the starting percentage, take the distance to the starting percentage
                    percentageDistance = startingPercentage - currentPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = startingWeights[i] - weights[i];
                    }

                    float diffPercentage = (startingPercentage - currentPercentage) / diff;

                    for (int i = 0; i < 4; i++) {
                        float speed = weightDistances[i] / percentageDistance;
                        change[i] = diffPercentage * diff * speed;
                    }

                    // the rest of the way, take the distance to the top
                    percentageDistance = 1f - startingPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = 1f - startingWeights[i];
                    }
                    
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

                if (newPercentage < startingPercentage) {
                    // overshooting the starting percentage downwards, needs 2 steps:
                    
                    // until the starting percentage, take the distance to the starting percentage
                    percentageDistance = currentPercentage - startingPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = weights[i] - startingWeights[i];
                    }

                    float diffPercentage = (currentPercentage - startingPercentage) / Mathf.Abs(diff);

                    for (int i = 0; i < 4; i++) {
                        float speed = weightDistances[i] / percentageDistance;
                        change[i] = (diffPercentage * diff) * speed;
                    }

                    // the rest of the way, take the distance to the bottom
                    percentageDistance = startingPercentage;
                    for (int i = 0; i < 4; i++) {
                        weightDistances[i] = startingWeights[i];
                    }

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

                for (int i = 0; i < 4; i++) {
                    float speed = weightDistances[i] / percentageDistance;
                    change[i] = diff * speed;
                }
            }
        }

        return change;
    }

    public void SetAlpha(float alpha) {
        foregroundColor = new Color(foregroundColor.r, foregroundColor.g, foregroundColor.b, alpha);
        
        //borderColor = new Color(borderColor.r, borderColor.g, borderColor.b, alpha);
        //originalBorderColor = borderColor;
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

    public void UpdatePercentageByWeights() {
        currentPercentage = startingPercentage = weights.Average();
    }

    private void HandleMouseDragEvent(float mouseY, float topY) {
        float freeSpaceOnTop = LimitToBounds(mouseY - topY, lower: 0, upper: height);
        float newFillHeight = height - freeSpaceOnTop;

        SetPercentage(newFillHeight / height);
    }

    private void UpdateTexture() {
        int fillHeight = (int)(currentPercentage * height);

        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int index = y * width + x;

                if (OnBorder(x, y)) {
                    pixels[index] = borderColor;
                }
                else if (y < fillHeight) {
                    pixels[index] = foregroundColor;
                }
                else {
                    pixels[index] = backgroundColor;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        IntellimapEditor.repaint = true;
    }
}
