using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class IntellimapHistogram {
    private int numBuckets;
    private List<float> sliderValues;

    public IntellimapHistogram(int numBuckets) {
        this.numBuckets = numBuckets;

        sliderValues = new List<float>();
        for (int i = 0; i < numBuckets; i++) {
            sliderValues.Add((1f / numBuckets) * 100);
        }
    }

    public IntellimapHistogram(int numBuckets, List<float> sliderValues) {
        this.numBuckets = numBuckets;
        SetSliderValues(sliderValues);
    }

    public void Show() {
        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < numBuckets; i++) {
            GUILayout.Space(15);

            float newSliderValue = GUILayout.VerticalSlider(sliderValues[i], 100, 0, GUILayout.Height(100));
            if (newSliderValue != sliderValues[i]) {
                float diff = newSliderValue - sliderValues[i];

                for (int j = 0; j < numBuckets; j++) {
                    if (i == j) continue;

                    float changedSliderDistanceToEnd;
                    float thisSliderDistanceToCap;

                    if (isPositive(diff)) {
                        changedSliderDistanceToEnd = 100 - sliderValues[i];
                        thisSliderDistanceToCap = sliderValues[j];
                    }
                    else {
                        changedSliderDistanceToEnd = sliderValues[i];
                        float cap = 100f / (numBuckets - 1);
                        thisSliderDistanceToCap = cap - sliderValues[j];
                    }

                    float speed = thisSliderDistanceToCap / changedSliderDistanceToEnd;
                    
                    float changeToThisSlider = -diff * speed;
                    AddToSlider(j, changeToThisSlider);
                }

                sliderValues[i] = newSliderValue;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private bool isPositive(float f) {
        return f > 0;
    }

    private void AddToSlider(int sliderIndex, float change) {
        sliderValues[sliderIndex] += change;

        if (sliderValues[sliderIndex] > 100) {
            sliderValues[sliderIndex] = 100;
        }
        else if (sliderValues[sliderIndex] < 0) {
            sliderValues[sliderIndex] = 0;
        }
    }

    public List<float> GetSliderValues() {
        return sliderValues;
    }

    public void SetSliderValues(List<float> newSliderValues) {
        for (int i = 0; i < newSliderValues.Count; i++) {
            if (newSliderValues[i] > 100 || newSliderValues[i] < 0) {
                throw new ArgumentException("All slider values have to be between 0 and 100.");
            }
            // Maybe also check for if it accumulates to 100%.
        }

        sliderValues = new List<float>(newSliderValues);
    }
}
