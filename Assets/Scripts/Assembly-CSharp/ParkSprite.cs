using UnityEngine;

public class ParkSprite
{
	public static Sprite createSprite(Texture2D texture, float x, float y, float width, float height, Vector2 pivot, Vector4 border, int pixelsPerUnit = 1)
	{
		Rect rect = default(Rect);
		rect.x = x;
		rect.y = y;
		rect.height = height;
		rect.width = width;
		return Sprite.Create(texture, rect, pivot, pixelsPerUnit, 0u, SpriteMeshType.FullRect, border);
	}

	public static Sprite createSprite(Texture2D texture, Rect rect, Vector2 pivot, Vector4 border, int pixelsPerUnit = 1)
	{
		return Sprite.Create(texture, rect, pivot, pixelsPerUnit, 0u, SpriteMeshType.FullRect, border);
	}

	public static Vector2 getPivot(UIWidget.Pivot widgetPivot)
	{
		Vector2 result = Vector2.zero;
		switch (widgetPivot)
		{
		case UIWidget.Pivot.Center:
			result = new Vector2(0.5f, 0.5f);
			break;
		case UIWidget.Pivot.Left:
			result = new Vector2(0f, 0.5f);
			break;
		case UIWidget.Pivot.Right:
			result = new Vector2(1f, 0.5f);
			break;
		case UIWidget.Pivot.Bottom:
			result = new Vector2(0.5f, 0f);
			break;
		case UIWidget.Pivot.BottomLeft:
			result = new Vector2(0f, 0f);
			break;
		case UIWidget.Pivot.BottomRight:
			result = new Vector2(1f, 0f);
			break;
		case UIWidget.Pivot.Top:
			result = new Vector2(0.5f, 1f);
			break;
		case UIWidget.Pivot.TopLeft:
			result = new Vector2(0f, 1f);
			break;
		case UIWidget.Pivot.TopRight:
			result = new Vector2(1f, 1f);
			break;
		}
		return result;
	}

	public static Sprite ConvertNGUISpriteToUnitySprite(UIAtlas atlas, UISprite sprite)
	{
		UIAtlas.Sprite sprite2 = atlas.GetSprite(sprite.spriteName);
		Rect outer = sprite2.outer;
		outer.y = (float)atlas.spriteMaterial.mainTexture.height - sprite2.outer.max.y;
		return createSprite(atlas.spriteMaterial.mainTexture as Texture2D, outer, getPivot(sprite.pivot), sprite.border);
	}

	public static Sprite ConvertNGUISpriteToUnitySprite(UIAtlas atlas, UIAtlas.Sprite sprite, Vector2 pivot, Vector4 border)
	{
		Rect outer = sprite.outer;
		outer.y = (float)atlas.spriteMaterial.mainTexture.height - sprite.outer.max.y;
		return createSprite(atlas.spriteMaterial.mainTexture as Texture2D, outer, pivot, border);
	}

	public static Sprite ConvertNGUISpriteToUnitySprite(UIAtlas atlas, Rect rect, Vector2 pivot, Vector4 border)
	{
		return createSprite(atlas.spriteMaterial.mainTexture as Texture2D, rect, pivot, border);
	}

	public static void ConvertSpritesInNGUIObject(UIAtlas atlas, GameObject nguiObject)
	{
		UISprite[] componentsInChildren = nguiObject.GetComponentsInChildren<UISprite>();
		foreach (UISprite uISprite in componentsInChildren)
		{
			SpriteRenderer spriteRenderer = uISprite.gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = ConvertNGUISpriteToUnitySprite(atlas, uISprite);
			spriteRenderer.sortingLayerName = "ParkMap";
			spriteRenderer.sortingOrder = uISprite.depth;
			UIAtlas.Sprite atlasSprite = uISprite.GetAtlasSprite();
			Vector3 localScale = spriteRenderer.transform.localScale;
			spriteRenderer.transform.localScale = new Vector3(localScale.x / atlasSprite.outer.size.x, localScale.y / atlasSprite.outer.size.y, 1f);
			Object.Destroy(uISprite);
		}
		UIPanel component = nguiObject.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
	}
}
