using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SliderGroup {
    protected int numSliders;
    protected int height;
    protected List<float> sliderValues;

    public SliderGroup(int numSliders)
        : this(numSliders, 100) {}

    public SliderGroup(int numSliders, int height) {
        this.numSliders = numSliders;
        this.height = height;

        sliderValues = new List<float>();
        for (int i = 0; i < numSliders; i++) {
            sliderValues.Add(0f);
        }
    }

    public virtual void Show() {
        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < numSliders; i++) {
            GUILayout.Space(15);

            float newSliderValue = GUILayout.VerticalSlider(sliderValues[i], 1f, 0f, GUILayout.Height(height));
            if (newSliderValue != sliderValues[i]) {
                ReactToSliderChange(i, newSliderValue);

                sliderValues[i] = newSliderValue;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    public List<float> GetSliderValues() {
        return sliderValues;
    }

    public virtual void SetSliderValues(float[] newSliderValues) {
        if (sliderValues.Count != newSliderValues.Length) {
            throw new ArgumentException("SliderGroup: SetSliderValues: Lenghts don't match");
        }

        for (int i = 0; i < numSliders; i++) {
            sliderValues[i] = newSliderValues[i];
        }
    }

    public virtual void SetSliderValues(List<float> newSliderValues) {
        if (sliderValues.Count != newSliderValues.Count) {
            throw new ArgumentException("SliderGroup: SetSliderValues: Lenghts don't match");
        }

        sliderValues = new List<float>(newSliderValues);
    }

    protected virtual void ReactToSliderChange(int changedSliderIndex, float newSliderValue) {}

    protected float RoundOnEdge(float f) {
        if (f < 0.001) {
            return 0f;
        }
        if (f > 0.999) {
            return 1f;
        }

        return f;
    }
}
