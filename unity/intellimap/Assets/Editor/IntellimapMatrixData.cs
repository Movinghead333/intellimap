using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This might be obsolete if every box has its own data anyway
public class IntellimapMatrixData {
    public float[,,] weights;

    public IntellimapMatrixData(int size) {
        weights = new float[size, size, 4];
    }

    public void CopyWeights(float[,,] weights) {
        this.weights = weights.Clone() as float[,,];
    }
}
