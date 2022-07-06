using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeightBoxDetailView {
    private WeightBox box;

    private bool foldedOut;
    private float value;
    private DetailViewSliderGroup weightSliders;
    private TextureBox arrowBoxBetweenSlidersAndText;

    public WeightBoxDetailView() {
        weightSliders = new DetailViewSliderGroup(75, this);
        
        arrowBoxBetweenSlidersAndText = new TextureBox(20, 20, Color.clear, Color.clear);

        Texture2D rightArrowTexture = Resources.Load("Arrow_right_empty") as Texture2D;
        Rect textureRect = new Rect(0, 0, rightArrowTexture.width, rightArrowTexture.height);

        arrowBoxBetweenSlidersAndText.SetTexture(rightArrowTexture, textureRect);
    }

    public void Show() {
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            foldedOut = EditorGUILayout.Foldout(foldedOut, "Detail View");
        EditorGUILayout.EndHorizontal();

        if (foldedOut) {
            EditorGUILayout.BeginHorizontal();
                weightSliders.Show();
                GUILayout.Space(15);

                EditorGUILayout.BeginVertical();
                    GUILayout.Space(50);
                    arrowBoxBetweenSlidersAndText.Show();
                EditorGUILayout.EndVertical();

                GUILayout.Space(15);

                EditorGUILayout.BeginVertical();
                    GUILayout.Space(42.5f);

                    GUILayout.Label("Value:");
                    float newValue = EditorGUILayout.FloatField(value, GUILayout.Width(72));
                    if (newValue != value) {
                        newValue = GUIUtil.LimitToBounds(newValue, 0f, 1f);
                        box.SetPercentage(newValue);
                    }
                EditorGUILayout.EndVertical();
        
                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    public void SetBox(WeightBox box) {
        this.box = box;
        value = box.GetPercentage();
        weightSliders.SetWeightBox(box);
    }

    public void UpdateFromBox() {
        value = box.GetPercentage();
        weightSliders.SetSliderValues(box.GetWeights());
    }

    public void UpdateFromSliders(float[] weights) {
        box.SetWeights(weights);
        value = box.GetPercentage();
    }
}
