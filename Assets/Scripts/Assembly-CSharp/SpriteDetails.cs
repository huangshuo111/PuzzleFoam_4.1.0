using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDetails : MonoBehaviour
{
	[HideInInspector]
	public SpriteRenderer renderer_;

	public string spriteName = string.Empty;

	public Rect rect;

	public Vector2 pivot = new Vector2(0.5f, 0.5f);

	public Vector4 border = Vector4.zero;

	public int pixelsPerUnits = 1;

	public SpriteRenderer spriteRenderer
	{
		get
		{
			return renderer_;
		}
		set
		{
			renderer_ = value;
		}
	}

	private void Awake()
	{
		if (renderer_ == null)
		{
			renderer_ = GetComponent<SpriteRenderer>();
		}
	}
}
