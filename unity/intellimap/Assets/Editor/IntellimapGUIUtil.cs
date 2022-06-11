using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IntellimapGUIUtil {

    private static List<int> histogramSizes = new List<int>();
    private static List<float> histogramSliderValues = new List<float>();

    public static void HorizontalLine(Color color, int leftMargin = 0, int rightMargin = 0, int topMargin = 10, int bottomMargin = 10) {
        GUIStyle horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(leftMargin, rightMargin, topMargin, bottomMargin);
        horizontalLine.fixedHeight = 1;

        var tempColor = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = tempColor;
    }

    public static void Histogram(int numBuckets, int numHistogram) {
        int startIndex = 0;
        for (int i = 0; i < numHistogram; i++) {
            startIndex += histogramSizes[i];
        }

        EditorGUILayout.BeginHorizontal();

        for (int i = startIndex; i < (startIndex + numBuckets); i++) {
            GUILayout.Space(15);

            // If the size is one too short, add one more value
            if (histogramSliderValues.Count == i) {
                histogramSliderValues.Add((1.0f / numBuckets) * 100);
            }

            float newSliderValue = GUILayout.VerticalSlider(histogramSliderValues[i], 100, 0, GUILayout.Height(100));
            if (newSliderValue != histogramSliderValues[i]) {
                float diff = newSliderValue - histogramSliderValues[i];

                for (int j = startIndex; j < (startIndex + numBuckets); j++) {
                    if (i == j)
                        continue;
                    
                    float changedSliderDistanceToEnd;
                    float thisSliderDistanceToCap;
                    if (diff > 0) {
                        changedSliderDistanceToEnd = 100 - histogramSliderValues[i];
                        thisSliderDistanceToCap = histogramSliderValues[j];
                    }
                    else {
                        changedSliderDistanceToEnd = histogramSliderValues[i];
                        float cap = (100 / (numBuckets - 1));
                        thisSliderDistanceToCap = cap - histogramSliderValues[j];
                    }

                    float speed = thisSliderDistanceToCap / changedSliderDistanceToEnd;
                    
                    float changeToThisSlider = -diff * speed;
                    histogramSliderValues[j] += changeToThisSlider;
                    if (histogramSliderValues[j] > 100) {
                        histogramSliderValues[j] = 100;
                    }
                    else if (histogramSliderValues[j] < 0) {
                        histogramSliderValues[j] = 0;
                    }
                }

                histogramSliderValues[i] = newSliderValue;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (histogramSizes.Count == numHistogram) {
            histogramSizes.Add(numBuckets);
        }
    }

    public static List<float> GetHistogramValues(int numHistogram) {
        List<float> result = new List<float>();

        int startIndex = 0;
        for (int i = 0; i < numHistogram; i++) {
            startIndex += histogramSizes[i];
        }

        for (int i = startIndex; i < (startIndex + histogramSizes[numHistogram]); i++) {
            result.Add(histogramSliderValues[i]);
        }

        return result;
    }
}
