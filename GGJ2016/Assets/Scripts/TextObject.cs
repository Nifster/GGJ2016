using UnityEngine;
using System.Collections;

public class TextObject : MonoBehaviour {
    
	protected Renderer thisRenderer;

	protected TextMesh textMesh;
	public TextMesh TextMesh{get{return textMesh;}}

	private Transform thisTransform;

	private Color color;

    void Start() {
        InitialiseAttributes();
    }

    protected void InitialiseAttributes () {
		if (textMesh != null) return;
		textMesh = this.GetComponent<TextMesh>();
		thisRenderer = GetComponent<Renderer>();
		thisTransform = this.transform;
		color = textMesh.color;
	}

    public void InheritFontProperties(GUIStyle style)
    {
        InitialiseAttributes();
        //textMesh.font = style.font;
        textMesh.anchor = style.alignment;
        textMesh.color = style.normal.textColor;
        textMesh.fontSize = style.fontSize;
        textMesh.fontStyle = style.fontStyle;
        textMesh.richText = style.richText;
    }

	public void Initialise() {
		InitialiseAttributes();
	}

	public void Hide() {
		InitialiseAttributes();
		thisRenderer.enabled = false;
	}

	public void Show() {
		InitialiseAttributes();
		thisRenderer.enabled = true;
	}

	public void SetColour(Color toColor)
    {
        color.r = toColor.r;
        color.g = toColor.g;
        color.b = toColor.b;
        color.a = toColor.a;
	    textMesh.color = color;
    }

    public Color GetColour()
    {
        InitialiseAttributes();
        return TextMesh.color;
    }

    public void Reposition(Vector2 position) {
		InitialiseAttributes();
		thisTransform.position = position;
	}
	
	public void SetLocalPosition(Vector2 position) {
		InitialiseAttributes();
		thisTransform.localPosition = position;
	}
	
	public void SetAlpha(float alpha) {
		color.a = alpha;
		textMesh.color = color;
	}

	public void UpdateString(string message) {
		InitialiseAttributes();
	    TextChanged();
		textMesh.text = message;
	}

    protected virtual void TextChanged()
    {
    }

    public void SmallFont()
    {
        TextMesh.fontSize = 40;
    }
}