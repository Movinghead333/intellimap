using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * The histogram used to dynamically update while interacting with one slider.
 * We had another iteration where you could change one slider by itself, and once you let go, the histogram gets normalized again.
 * Either behaviour was really cool with a low number of tiles.
 * But we found that, using a realistic number of tiles, it was really unhelpful because most sliders would be so low that you coudn't get a feel for how they were moving.
 * So now the user can simply change the sliders without a dynamic update, and the values are normalized internally before the WFC algorithm.
 * This way it is slightly less "intelligent", but much more usable, and the user can set exactly the distribution they want without having the hassle of other sliders moving away constantly.
 */
public class Histogram : SliderGroup {
    public Histogram(int size)
        : base(size, Color.clear, Color.grey)
    {
        for (int i = 0; i < size; i++) {
            sliderValues[i] = 1f / size;
        }

        UpdateTextBoxes();
    }

    public float[] GetNormalizedSliderValues() {
        float[] result = new float[sliderValues.Length];

        float sum = sliderValues.Sum();

        if (sum > 0) {
            for (int i = 0; i < sliderValues.Length; i++) {
                result[i] = sliderValues[i] / sum;
            }
        }

        return result;
    }

    public override void SetSliderValues(float[] newSliderValues) {
        for (int i = 0; i < newSliderValues.Length; i++) {
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
                textureBoxes[i].SetNoTexture();
            }
        }
    }
}
