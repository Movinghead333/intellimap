using UnityEngine;
using UnityEditor;

public class DetailView {
    private bool foldedOut;

    private WeightBox box;

    private float textValue;
    private DetailViewSliderGroup weightSliders;
    private TextureBox arrowBoxBetweenSlidersAndText;

    public DetailView() {
        foldedOut = false;

        box = null;

        textValue = 0f;
        weightSliders = new DetailViewSliderGroup(75, this);

        arrowBoxBetweenSlidersAndText = new TextureBox(20, 20, Color.clear, Color.clear);
        Texture2D rightArrowTexture = Resources.Load("Arrow_right_empty") as Texture2D;
        Rect rightArrowTextureRect = new Rect(0, 0, rightArrowTexture.width, rightArrowTexture.height);
        arrowBoxBetweenSlidersAndText.SetTexture(rightArrowTexture, rightArrowTextureRect);
    }

    public void Show() {
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
            GUILayout.Space(IntellimapEditor.startingSpace);
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
                    float newValue = EditorGUILayout.FloatField(textValue, GUILayout.Width(72));
                    if (newValue != textValue && box != null) {
                        newValue = GUIUtil.LimitToBounds(newValue, lower: 0f, upper: 1f);
                        box.SetPercentage(newValue);
                    }
                EditorGUILayout.EndVertical();
        
                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    public void SetBox(WeightBox box) {
        this.box = box;

        if (box != null) {
            textValue = box.GetPercentage();
            weightSliders.SetWeightBox(box);
        }
    }

    public WeightBox GetBox() {
        return box;
    }

    public void UpdateFromBox() {
        textValue = box.GetPercentage();
        weightSliders.SetSliderValues(box.GetWeights());
    }

    public void UpdateFromSliders(float[] weights) {
        if (box != null) {
            box.SetWeights(weights);
            box.UpdatePercentageByWeights();
            textValue = box.GetPercentage();
        }
    }
}
