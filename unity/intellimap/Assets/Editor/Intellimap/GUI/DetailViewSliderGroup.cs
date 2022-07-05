using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailViewSliderGroup : SliderGroup {
    private WeightBox box;
    private WeightBoxDetailView detailView;


    public DetailViewSliderGroup(int numSliders, int height, WeightBoxDetailView detailView)
        : base(numSliders, height)
    {
        this.detailView = detailView;
    }

    public void SetWeightBox(WeightBox box) {
        this.box = box;
        SetSliderValues(box.GetWeights());
    }

    protected override void ReactToSliderChange(int changedSliderIndex, float newSliderValue) {
        detailView.UpdateFromSliders(GetSliderValues().ToArray());
    }
}
