using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Tilemaps;
using System.Linq;

using static GUIUtil;

public class Histogram : SliderGroup {
    public Histogram(int size)
        : base(size, Color.clear, Color.grey)
    {
        for (int i = 0; i < size; i++) {
            sliderValues[i] = 1f / size;
        }

        UpdateTextBoxes();
    }

    protected override void ReactToSliderChange(int changedSliderIndex, float newSliderValue) {
        AdjustOtherSliders(changedSliderIndex, newSliderValue);
    }

    private void AdjustOtherSliders(int changedSliderIndex, float newSliderValue) {
        float diff = newSliderValue - sliderValues[changedSliderIndex];

        // Collect distance to end for every slider
        float[] otherSlidersDistancesToEnd = new float[numSliders];

        if (isPositive(diff)) {
            for (int i = 0; i < numSliders; i++) {
                if (i == changedSliderIndex)
                    otherSlidersDistancesToEnd[i] = 0f;
                else
                    otherSlidersDistancesToEnd[i] = sliderValues[i];
            }
        }
        else {
            for (int i = 0; i < numSliders; i++) {
                if (i == changedSliderIndex)
                    otherSlidersDistancesToEnd[i] = 0f;
                else
                    otherSlidersDistancesToEnd[i] = 1f - sliderValues[i];
            }
        }

        float otherSlidersSum = otherSlidersDistancesToEnd.Sum();

        // Adjust other sliders
        for (int i = 0; i < numSliders; i++) {
            if (i == changedSliderIndex) continue;
   
            float percentChangeToSlider = otherSlidersDistancesToEnd[i] / otherSlidersSum;
            float changeToSlider = percentChangeToSlider * -diff;

            sliderValues[i] = RoundOnEdge(LimitToBounds(sliderValues[i] + changeToSlider, lower: 0f, upper: 1f));
        }
    }

    public override void SetSliderValues(List<float> newSliderValues) {
        for (int i = 0; i < newSliderValues.Count; i++) {
            if (newSliderValues[i] > 1 || newSliderValues[i] < 0) {
                throw new ArgumentException("All slider values have to be between 0 and 1.");
            }
            // Maybe also check for if it accumulates to 100%.
        }

        base.SetSliderValues(newSliderValues);
    }

    public void SetBottomTiles(Tile[] tiles) {
        if (tiles.Length != numSliders) {
            throw new ArgumentException("Array lengths don't match");
        }

        for (int i = 0; i < tiles.Length; i++) {
            if (tiles[i] != null) {
                Sprite sprite = tiles[i].sprite;
                textureBoxes[i].SetTexture(sprite.texture, sprite.textureRect);
            }
            else {
                textureBoxes[i].SetTexture(null, new Rect());
            }
        }
    }
}
