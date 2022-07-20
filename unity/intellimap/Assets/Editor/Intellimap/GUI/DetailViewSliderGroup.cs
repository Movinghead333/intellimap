using UnityEngine;

public class DetailViewSliderGroup : SliderGroup {
    private DetailView detailView;

    public DetailViewSliderGroup(int height, DetailView detailView)
        : base(4, Color.clear, Color.clear, height)
    {
        this.detailView = detailView;
        
        Texture2D rightArrowTexture = Resources.Load("Arrow_right") as Texture2D;
        Texture2D leftArrowTexture = Resources.Load("Arrow_left") as Texture2D;
        Texture2D upArrowTexture = Resources.Load("Arrow_up") as Texture2D;
        Texture2D downArrowTexture = Resources.Load("Arrow_down") as Texture2D;

        Rect textureRect = new Rect(0, 0, rightArrowTexture.width, rightArrowTexture.height);

        textureBoxes[0].SetTexture(upArrowTexture, textureRect);
        textureBoxes[1].SetTexture(rightArrowTexture, textureRect);
        textureBoxes[2].SetTexture(downArrowTexture, textureRect);
        textureBoxes[3].SetTexture(leftArrowTexture, textureRect);

        SetSliderValues(new float[]{0f, 0f, 0f, 0f});
        UpdateTextBoxes();
        UpdateTextBoxes();
        UpdateTextBoxes();
        UpdateTextBoxes();
        UpdateTextBoxes();
        UpdateTextBoxes();
    }

    public void SetWeightBox(WeightBox box) {
        SetSliderValues(box.GetWeights());
    }

    // Gets called when sliders changed
    protected override void SlidersChanged() {
        detailView.UpdateFromSliders(GetSliderValues());
    }
}
