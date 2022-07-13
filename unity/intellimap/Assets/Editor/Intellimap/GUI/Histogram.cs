using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Tilemaps;
using System.Linq;

using static GUIUtil;

public class Histogram : SliderGroup {
    //private int changedSliderIndex;
    //private float changedSliderOriginalValue;

    public Histogram(int size)
        : base(size, Color.clear, Color.grey)
    {
        for (int i = 0; i < size; i++) {
            sliderValues[i] = 1f / size;
        }

        //changedSliderIndex = -1;

        UpdateTextBoxes();
    }

    public override void Show() {
        /*if (LeftMouseButton() && MouseUp()) {
            AdjustAllSliders();
            changedSliderIndex = -1;
        }*/

        base.Show();
    }

    protected override void ReactToSliderChange(int changedSliderIndex, float newSliderValue) {
        //AdjustOtherSliders(changedSliderIndex, newSliderValue);

        /*if (this.changedSliderIndex == -1) {
            this.changedSliderIndex = changedSliderIndex;
            changedSliderOriginalValue = sliderValues[changedSliderIndex];
        }*/
    }

    /*
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
        if (otherSlidersSum > 0) {
            for (int i = 0; i < numSliders; i++) {
                if (i == changedSliderIndex) continue;
   
                float percentChangeToSlider = otherSlidersDistancesToEnd[i] / otherSlidersSum;
                float changeToSlider = percentChangeToSlider * -diff;

                sliderValues[i] = RoundOnEdge(LimitToBounds(sliderValues[i] + changeToSlider, lower: 0f, upper: 1f));
            }
        }
    }

    private void AdjustAllSliders() {
        float diff = sliderValues[changedSliderIndex] - changedSliderOriginalValue;

        // Collect distance to end for every slider
        float[] slidersDistancesToEnd = new float[numSliders];

        if (isPositive(diff)) {
            for (int i = 0; i < numSliders; i++) {
                slidersDistancesToEnd [i] = sliderValues[i];
            }
        }
        else {
            for (int i = 0; i < numSliders; i++) {
                slidersDistancesToEnd [i] = 1f - sliderValues[i];
            }
        }

        float distancesSum = slidersDistancesToEnd.Sum();
        if (distancesSum > 0) {
            for (int i = 0; i < numSliders; i++) {
                float percentChangeToSlider = slidersDistancesToEnd [i] / distancesSum;
                float changeToSlider = percentChangeToSlider * -diff;

                sliderValues[i] = RoundOnEdge(LimitToBounds(sliderValues[i] + changeToSlider, lower: 0f, upper: 1f));
            }
        }

        UpdateTextBoxes();
    }

    */

    public float[] GetNormalizedSliderValues() {
        float[] result = new float[sliderValues.Count];

        float sum = sliderValues.Sum();

        if (sum > 0) {
            for (int i = 0; i < sliderValues.Count; i++) {
                result[i] = sliderValues[i] / sum;
            }
        }

        return result;
    }

    public override void SetSliderValues(List<float> newSliderValues) {
        for (int i = 0; i < newSliderValues.Count; i++) {
            if (newSliderValues[i] > 1f || newSliderValues[i] < 0f) {
                throw new ArgumentException("All slider values have to be between 0 and 1.");
            }
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
