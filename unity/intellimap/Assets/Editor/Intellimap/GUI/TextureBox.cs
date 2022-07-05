using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TextureBox : Box {
    private Texture2D externalTexture;
    private Rect externalTextureRect;

    public TextureBox(Color backgroundColor, Color borderColor)
        : this(10, 10, backgroundColor, borderColor) {}
    
    public TextureBox(int width, int height, Color backgroundColor, Color borderColor)
        : base(width, height, Color.clear, backgroundColor, borderColor)
    {
        externalTexture = null;
    }

    public override void Show() {
        base.Show();
    }

    public void FillMagenta() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                texture.SetPixel(x, y, Color.magenta);
            }
        }
        texture.Apply();
    }

    public void SetTexture(Texture2D newTexture, Rect textureRect) {
        externalTexture = newTexture;
        externalTextureRect = textureRect;
        UpdateTexture();
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

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (!DrawBorder(x, y)) {
                    if (InRectangle(externalTextureDrawRegion, x, y)) {
                        Color pixel = externalTexture.GetPixel((int)externalTextureRect.x + x - startX, (int)externalTextureRect.y + y - startY);
                        texture.SetPixel(x, y, pixel);
                    }
                    else {
                        texture.SetPixel(x, y, backgroundColor);
                    }
                }
            }
        }

        texture.Apply();
    }

    private void DrawBackground() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (!DrawBorder(x, y)) {
                    texture.SetPixel(x, y, backgroundColor);
                }
            }
        }

        texture.Apply();
    }
    
    public override void Resize(int width, int height) {
        base.Resize(width, height);
        UpdateTexture();
    }

}
