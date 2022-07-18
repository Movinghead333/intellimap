using System;
using UnityEngine;
using UnityEditor;

public class SliderGroup {
    protected int numSliders;
    protected int height;
    protected float[] sliderValues;

    protected TextureBox[] textBoxes;
    protected TextureBox[] textureBoxes;
    protected Color boxesBackgroundColor;
    protected Color boxesBorderColor;

    public SliderGroup(int numSliders, Color boxesBackgroundColor, Color boxesBorderColor, int height = 100) {
        this.boxesBackgroundColor = boxesBackgroundColor;
        this.boxesBorderColor = boxesBorderColor;
        this.height = height;
        
        Init(numSliders);
    }

    public void Init(int numSliders) {
        this.numSliders = numSliders;

        sliderValues = new float[numSliders];
        for (int i = 0; i < numSliders; i++) {
            sliderValues[i] = 0f;
        }

        textureBoxes = new TextureBox[numSliders];
        for (int i = 0; i < numSliders; i++) {
            textureBoxes[i] = new TextureBox(20, 20, boxesBackgroundColor, boxesBorderColor);
        }

        textBoxes = new TextureBox[numSliders];
        for (int i = 0; i < numSliders; i++) {
            textBoxes[i] = new TextureBox(20, 20, boxesBackgroundColor, Color.clear);
            textBoxes[i].SetContentOffset(2f, 3f);
        }
    }

    public virtual void Show() {
        // the slider Width is 2.
        // the box Width is 20.
        float space = (20 - 2) / 2;

        EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
                GUILayout.Space(IntellimapEditor.startingSpace);

                GUILayout.Space(4);
                for (int i = 0; i < numSliders; i++) {
                    textBoxes[i].Show();
                }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                GUILayout.Space(IntellimapEditor.startingSpace);

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
                GUILayout.Space(IntellimapEditor.startingSpace);

                GUILayout.Space(4);
                for (int i = 0; i < numSliders; i++) {
                    textureBoxes[i].Show();
                }
            EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    public float[] GetSliderValues() {
        return sliderValues;
    }

    public virtual void SetSliderValues(float[] newSliderValues) {
        if (sliderValues.Length != newSliderValues.Length) {
            throw new ArgumentException("SliderGroup: SetSliderValues: Lengths don't match");
        }

        for (int i = 0; i < numSliders; i++) {
            sliderValues[i] = newSliderValues[i];
        }

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
            else if (valueString.Length == stringLength) {
                valueString = valueString.Substring(1);
                valueString += '0';
            }
            else { // valueString.Length == 1
                valueString += '.';

                int numMissingZeros = stringLength - 2;
                valueString += new string('0', numMissingZeros);
            }

            valueString = valueString.Replace(',', '.');
            textBoxes[i].SetText(valueString);
        }
    }
}
