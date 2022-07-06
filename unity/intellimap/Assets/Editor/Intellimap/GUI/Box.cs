using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box {
    protected int width;
    protected int height;
    protected Texture2D texture;
    protected GUIStyle style;
    protected GUIContent content;

    protected Color foregroundColor;
    protected Color backgroundColor;
    protected Color borderColor;

    protected Box()
        : this(10, 10, Color.clear, Color.clear, Color.clear) {}

    protected Box(int width, int height, Color foregroundColor, Color backgroundColor, Color borderColor) {
        this.width = width;
        this.height = height;

        texture = new Texture2D(width, height);

        style = new GUIStyle();
        style.normal.background = texture;
        style.normal.textColor = Color.white;
        style.fontSize = 10;
        style.fixedWidth = width;
        style.fixedHeight = height;

        content = new GUIContent();

        this.foregroundColor = foregroundColor;
        this.backgroundColor = backgroundColor;
        this.borderColor = borderColor;
    }

    public virtual void Show() {
        GUILayout.Box(content, style);
    }

    public virtual void Resize(int width, int height) {
        this.width = width;
        this.height = height;

        texture.Reinitialize(width, height);

        style.fixedWidth = width;
        style.fixedHeight = height;
    }

    public virtual void SetText(string text) {
        content.text = text;
    }

    public virtual void SetAlpha(float alpha) {
        foregroundColor = new Color(foregroundColor.r, foregroundColor.g, foregroundColor.b, alpha);
    }

    public virtual void SetContentOffset(float x, float y) {
        style.contentOffset = new Vector2(x, y);
    }

    protected bool DrawBorder(int x, int y) {
        if (y == 0 || x == 0 || y == height - 1 || x == width - 1) {
            texture.SetPixel(x, y, borderColor);
            return true;
        }
        return false;
    }

    protected bool InRectangle(Rect rect, float x, float y) {
        return x >= rect.x && y >= rect.y && x < rect.x + rect.width && y < rect.y + rect.height;
    }
}
