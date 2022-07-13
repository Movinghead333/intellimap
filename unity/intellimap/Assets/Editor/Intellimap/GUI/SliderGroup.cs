using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

using static GUIUtil;

public class SliderGroup {
    protected int numSliders;
    protected int height;
    protected List<float> sliderValues;

    protected List<TextureBox> textBoxes;
    protected List<TextureBox> textureBoxes;
    protected Color boxesBackgroundColor;
    protected Color boxesBorderColor;

    protected float startSpacing;

    public SliderGroup(int numSliders, Color boxesBackgroundColor, Color boxesBorderColor, int height = 100) {
        this.boxesBackgroundColor = boxesBackgroundColor;
        this.boxesBorderColor = boxesBorderColor;
        this.height = height;
        startSpacing = 15;
        
        Init(numSliders);
    }

    public void Init(int numSliders) {
        this.numSliders = numSliders;

        sliderValues = new List<float>();
        for (int i = 0; i < numSliders; i++) {
            sliderValues.Add(0f);
        }

        textureBoxes = new List<TextureBox>();
        for (int i = 0; i < numSliders; i++) {
            TextureBox b = new TextureBox(20, 20, boxesBackgroundColor, boxesBorderColor);
            textureBoxes.Add(b);
        }

        textBoxes = new List<TextureBox>();
        for (int i = 0; i < numSliders; i++) {
            TextureBox b = new TextureBox(20, 20, boxesBackgroundColor, Color.clear);
            b.SetContentOffset(2f, 3f);
            textBoxes.Add(b);
        }
    }

    public virtual void Show() {
        // the slider Width is 2.
        // the box Width is 20.
        float space = (20 - 2) / 2;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
            GUILayout.Space(startSpacing);

            GUILayout.Space(4);
            for (int i = 0; i < numSliders; i++) {
                textBoxes[i].Show();
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
            GUILayout.Space(startSpacing);

            for (int i = 0; i < numSliders; i++) {
                GUILayout.Space(space);

                float newSliderValue = GUILayout.VerticalSlider(sliderValues[i], 1f, 0f, GUILayout.Height(height));
                if (newSliderValue != sliderValues[i]) {
                    ReactToSliderChange(i, newSliderValue);
                    sliderValues[i] = newSliderValue;
                    SlidersChanged();
                    UpdateTextBoxes();
                }

                GUILayout.Space(space);
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
            GUILayout.Space(startSpacing);

            GUILayout.Space(4);
            for (int i = 0; i < numSliders; i++) {
                textureBoxes[i].Show();
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    public List<float> GetSliderValues() {
        return sliderValues;
    }

    public virtual void SetSliderValues(float[] newSliderValues) {
        if (sliderValues.Count != newSliderValues.Length) {
            throw new ArgumentException("SliderGroup: SetSliderValues: Lengths don't match");
        }

        for (int i = 0; i < numSliders; i++) {
            sliderValues[i] = newSliderValues[i];
        }

        UpdateTextBoxes();
    }

    public virtual void SetSliderValues(List<float> newSliderValues) {
        if (sliderValues.Count != newSliderValues.Count) {
            throw new ArgumentException("SliderGroup: SetSliderValues: Lengths don't match");
        }

        sliderValues = new List<float>(newSliderValues);

        UpdateTextBoxes();
    }

    // Used to react to slider change before it takes effect. Originally used by the histogram, now unused.
    protected virtual void ReactToSliderChange(int changedSliderIndex, float newSliderValue) {}

    // Gets called after all the changes to the sliders are made.
    protected virtual void SlidersChanged() {}

    protected void UpdateTextBoxes() {
        for (int i = 0; i < numSliders; i++) {
            float value = sliderValues[i];
            string valueString = value.ToString();
            
            textBoxes[i].SetTooltip(valueString);

            int stringLength = 3;
            if (valueString.Length > stringLength) {
                valueString = valueString.Substring(1, stringLength);
            }
            else if (valueString.Length > 1) {
                valueString = valueString.Substring(1);
            }
            valueString = valueString.Replace(',', '.');

            textBoxes[i].SetText(valueString);
        }
    }

    /*
    protected float RoundOnEdge(float f) {
        if (f < 0.0001) {
            return 0f;
        }
        if (f > 0.9999) {
            return 1f;
        }

        return f;
    }
    */
}
