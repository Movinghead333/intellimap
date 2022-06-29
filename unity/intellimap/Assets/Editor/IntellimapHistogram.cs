using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

using static IntellimapGUIUtil;

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
                AdjustOtherSliders(i, newSliderValue);

                sliderValues[i] = newSliderValue;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void AdjustOtherSliders(int changedSliderIndex, float newSliderValue) {
        float diff = newSliderValue - sliderValues[changedSliderIndex];
                
        // Collect distance to end for every slider
        float changedSliderDistanceToEnd;
        float[] otherSlidersDistancesToEnd = new float[numBuckets];

        if (isPositive(diff)) {
            changedSliderDistanceToEnd = 100 - sliderValues[changedSliderIndex];

            for (int i = 0; i < numBuckets; i++) {
                if (i == changedSliderIndex)
                    otherSlidersDistancesToEnd[i] = 0;
                else
                    otherSlidersDistancesToEnd[i] = sliderValues[i];
            }
        }
        else {
            changedSliderDistanceToEnd = sliderValues[changedSliderIndex];

            for (int i = 0; i < numBuckets; i++) {
                if (i == changedSliderIndex)
                    otherSlidersDistancesToEnd[i] = 0;
                else
                    otherSlidersDistancesToEnd[i] = 100 - sliderValues[i];
            }
        }

        float otherSlidersSum = Sum(otherSlidersDistancesToEnd);

        // Adjust other sliders
        for (int i = 0; i < numBuckets; i++) {
            if (i == changedSliderIndex) continue;
   
            float percentChangeToSlider = otherSlidersDistancesToEnd[i] / otherSlidersSum;
            float changeToSlider = percentChangeToSlider * -diff;

            sliderValues[i] = RoundOnEdge(LimitToBounds(sliderValues[i] + changeToSlider, lower: 0, upper: 100));
        }
    }

    private float RoundOnEdge(float f) {
        if (f < 0.01) {
            return 0;
        }
        if (f > 99.99) {
            return 100;
        }

        return f;
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
