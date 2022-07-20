using UnityEngine;

public class TextureBox : Box {
    private Texture2D externalTexture;
    private Rect externalTextureRect;

    public TextureBox(Color backgroundColor, Color borderColor)
        : this(10, 10, backgroundColor, borderColor) {}
    
    public TextureBox(int width, int height, Color backgroundColor, Color borderColor)
        : base(width, height, backgroundColor, borderColor)
    {
        SetNoTexture();
    }

    public void SetTexture(Texture2D newTexture, Rect textureRect) {
        externalTexture = newTexture;
        externalTextureRect = textureRect;
        UpdateTexture();
    }

    public void SetNoTexture() {
        externalTexture = null;
        DrawBackground();
    }

    private void UpdateTexture() {
        if (externalTexture != null) {
            DrawExternalTextureOnBackground();
        }
        else {
            DrawBackground();
        }
    }

    private void DrawExternalTextureOnBackground() {
        int startX = (int)(width / 2 - externalTextureRect.width / 2);
        int startY = (int)(height / 2 - externalTextureRect.height / 2);
        Rect externalTextureDrawRegion = new Rect(startX, startY, externalTextureRect.width, externalTextureRect.height);

        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int index = y * width + x;

                if (OnBorder(x, y)) {
                    pixels[index] = borderColor;
                }
                else if (GUIUtil.InRectangle(externalTextureDrawRegion, x, y)) {
                    int externalX = (int)externalTextureRect.x + x - startX;
                    int externalY = (int)externalTextureRect.y + y - startY;
                    pixels[index] = externalTexture.GetPixel(externalX, externalY);
                }
                else {
                    pixels[index] = backgroundColor;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }

    private void DrawBackground() {
        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int index = y * width + x;

                if (OnBorder(x, y)) {
                    pixels[index] = borderColor;
                }
                else {
                    pixels[index] = backgroundColor;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }
    
    public override void Resize(int width, int height) {
        base.Resize(width, height);
        UpdateTexture();
    }
}
