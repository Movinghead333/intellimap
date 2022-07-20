using UnityEngine;

public class Box {
    protected int width;
    protected int height;

    protected Texture2D texture;
    protected GUIStyle style;
    protected GUIContent content;

    protected Color backgroundColor;
    protected Color borderColor;
    protected Color highlightBorderColor;
    protected Color originalBorderColor;

    protected Box()
        : this(10, 10, Color.clear, Color.clear) {}

    protected Box(int width, int height, Color backgroundColor, Color borderColor) {
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

        this.backgroundColor = backgroundColor;
        this.borderColor = borderColor;
        highlightBorderColor = borderColor;
        originalBorderColor = borderColor;
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

    public virtual void SetTooltip(string tooltip) {
        content.tooltip = tooltip;
    }

    public virtual void SetContentOffset(float x, float y) {
        style.contentOffset = new Vector2(x, y);
    }

    public void SetBackgroundColor(Color backgroundColor) {
        this.backgroundColor = backgroundColor;
    }

    public Color GetBackgroundColor() {
        return backgroundColor;
    }

    public void SetHighlightBorderColor(Color highlightBorderColor) {
        this.highlightBorderColor = highlightBorderColor;
    }

    // returns true if the color changed
    public bool HighlightBorderColor() {
        Color temp;
        temp.r = borderColor.r;
        temp.g = borderColor.g;
        temp.b = borderColor.b;
        temp.a = borderColor.a;

        borderColor = highlightBorderColor;

        return (temp.r != borderColor.r || temp.g != borderColor.g || temp.b != borderColor.b || temp.a != borderColor.a);
    }

    // returns true if the color changed
    public bool ResetBorderColor() {
        Color temp;
        temp.r = borderColor.r;
        temp.g = borderColor.g;
        temp.b = borderColor.b;
        temp.a = borderColor.a;

        borderColor = originalBorderColor;

        return (temp.r != borderColor.r || temp.g != borderColor.g || temp.b != borderColor.b || temp.a != borderColor.a);
    }

    protected bool OnBorder(int x, int y) {
        return (y == 0 || x == 0 || y == height - 1 || x == width - 1);
    }
}
