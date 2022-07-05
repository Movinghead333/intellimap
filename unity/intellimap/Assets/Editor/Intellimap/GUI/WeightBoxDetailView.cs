using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeightBoxDetailView {
    private WeightBox box;
    private float value;
    private DetailViewSliderGroup weightSliders;

    public WeightBoxDetailView() {
        weightSliders = new DetailViewSliderGroup(4, 75, this);
    }

    public void Show() {
        EditorGUILayout.BeginHorizontal();

        float newValue = EditorGUILayout.FloatField("Value:", value);
        if (newValue != value) {
            newValue = GUIUtil.LimitToBounds(newValue, 0f, 1f);
            box.SetPercentage(newValue);
        }


        weightSliders.Show();

        EditorGUILayout.EndHorizontal();
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
