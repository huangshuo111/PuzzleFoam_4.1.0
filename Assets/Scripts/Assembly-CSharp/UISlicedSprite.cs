using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Sprite (Sliced)")]
public class UISlicedSprite : UISprite
{
	[SerializeField]
	[HideInInspector]
	private bool mFillCenter = true;

	protected Rect mInner;

	protected Rect mInnerUV;

	protected Vector3 mScale = Vector3.one;

	public Rect innerUV
	{
		get
		{
			UpdateUVs(false);
			return mInnerUV;
		}
	}

	public bool fillCenter
	{
		get
		{
			return mFillCenter;
		}
		set
		{
			if (mFillCenter != value)
			{
				mFillCenter = value;
				MarkAsChanged();
			}
		}
	}

	public override Vector4 border
	{
		get
		{
			UIAtlas.Sprite atlasSprite = GetAtlasSprite();
			if (atlasSprite == null)
			{
				return Vector2.zero;
			}
			Rect rect = atlasSprite.outer;
			Rect rect2 = atlasSprite.inner;
			Texture texture = mainTexture;
			if (base.atlas.coordinates == UIAtlas.Coordinates.TexCoords && texture != null)
			{
				rect = NGUIMath.ConvertToPixels(rect, texture.width, texture.height, true);
				rect2 = NGUIMath.ConvertToPixels(rect2, texture.width, texture.height, true);
			}
			return new Vector4(rect2.xMin - rect.xMin, rect2.yMin - rect.yMin, rect.xMax - rect2.xMax, rect.yMax - rect2.yMax) * base.atlas.pixelSize;
		}
	}

	public override Vector2 pivotOffset
	{
		get
		{
			Vector2 zero = Vector2.zero;
			Pivot pivot = base.pivot;
			switch (pivot)
			{
			case Pivot.Top:
			case Pivot.Center:
			case Pivot.Bottom:
				zero.x = -0.5f;
				break;
			case Pivot.TopRight:
			case Pivot.Right:
			case Pivot.BottomRight:
				zero.x = -1f;
				break;
			}
			switch (pivot)
			{
			case Pivot.Left:
			case Pivot.Center:
			case Pivot.Right:
				zero.y = 0.5f;
				break;
			case Pivot.BottomLeft:
			case Pivot.Bottom:
			case Pivot.BottomRight:
				zero.y = 1f;
				break;
			}
			return zero;
		}
	}

	public override void UpdateUVs(bool force)
	{
		if (base.cachedTransform.localScale != mScale)
		{
			mScale = base.cachedTransform.localScale;
			mChanged = true;
		}
		if (!base.isValid || (!force && !(mInner != mSprite.inner) && !(mOuter != mSprite.outer)))
		{
			return;
		}
		Texture texture = mainTexture;
		if (texture != null)
		{
			mInner = mSprite.inner;
			mOuter = mSprite.outer;
			mInnerUV = mInner;
			mOuterUV = mOuter;
			if (base.atlas.coordinates == UIAtlas.Coordinates.Pixels)
			{
				mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, texture.width, texture.height);
				mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, texture.width, texture.height);
			}
		}
	}

	public override void MakePixelPerfect()
	{
		Vector3 localPosition = base.cachedTransform.localPosition;
		localPosition.x = Mathf.RoundToInt(localPosition.x);
		localPosition.y = Mathf.RoundToInt(localPosition.y);
		localPosition.z = Mathf.RoundToInt(localPosition.z);
		base.cachedTransform.localPosition = localPosition;
		Vector3 localScale = base.cachedTransform.localScale;
		localScale.x = Mathf.RoundToInt(localScale.x * 0.5f) << 1;
		localScale.y = Mathf.RoundToInt(localScale.y * 0.5f) << 1;
		localScale.z = 1f;
		base.cachedTransform.localScale = localScale;
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mOuterUV == mInnerUV)
		{
			base.OnFill(verts, uvs, cols);
			return;
		}
		Vector2[] array = new Vector2[4];
		Vector2[] array2 = new Vector2[4];
		Texture texture = mainTexture;
		array[0] = Vector2.zero;
		array[1] = Vector2.zero;
		array[2] = new Vector2(1f, -1f);
		array[3] = new Vector2(1f, -1f);
		if (texture != null)
		{
			float pixelSize = base.atlas.pixelSize;
			float num = (mInnerUV.xMin - mOuterUV.xMin) * pixelSize;
			float num2 = (mOuterUV.xMax - mInnerUV.xMax) * pixelSize;
			float num3 = (mInnerUV.yMax - mOuterUV.yMax) * pixelSize;
			float num4 = (mOuterUV.yMin - mInnerUV.yMin) * pixelSize;
			if (base.atlas.name.Contains("_Low"))
			{
				num *= 2f;
				num2 *= 2f;
				num3 *= 2f;
				num4 *= 2f;
			}
			Vector3 localScale = base.cachedTransform.localScale;
			localScale.x = Mathf.Max(0f, localScale.x);
			localScale.y = Mathf.Max(0f, localScale.y);
			Vector2 vector = new Vector2(localScale.x / (float)texture.width, localScale.y / (float)texture.height);
			Vector2 vector2 = new Vector2(num / vector.x, num3 / vector.y);
			Vector2 vector3 = new Vector2(num2 / vector.x, num4 / vector.y);
			Pivot pivot = base.pivot;
			if (pivot == Pivot.Right || pivot == Pivot.TopRight || pivot == Pivot.BottomRight)
			{
				array[0].x = Mathf.Min(0f, 1f - (vector3.x + vector2.x));
				array[1].x = array[0].x + vector2.x;
				array[2].x = array[0].x + Mathf.Max(vector2.x, 1f - vector3.x);
				array[3].x = array[0].x + Mathf.Max(vector2.x + vector3.x, 1f);
			}
			else
			{
				array[1].x = vector2.x;
				array[2].x = Mathf.Max(vector2.x, 1f - vector3.x);
				array[3].x = Mathf.Max(vector2.x + vector3.x, 1f);
			}
			if (pivot == Pivot.Bottom || pivot == Pivot.BottomLeft || pivot == Pivot.BottomRight)
			{
				array[0].y = Mathf.Max(0f, -1f - (vector3.y + vector2.y));
				array[1].y = array[0].y + vector2.y;
				array[2].y = array[0].y + Mathf.Min(vector2.y, -1f - vector3.y);
				array[3].y = array[0].y + Mathf.Min(vector2.y + vector3.y, -1f);
			}
			else
			{
				array[1].y = vector2.y;
				array[2].y = Mathf.Min(vector2.y, -1f - vector3.y);
				array[3].y = Mathf.Min(vector2.y + vector3.y, -1f);
			}
			array2[0] = new Vector2(mOuterUV.xMin, mOuterUV.yMax);
			array2[1] = new Vector2(mInnerUV.xMin, mInnerUV.yMax);
			array2[2] = new Vector2(mInnerUV.xMax, mInnerUV.yMin);
			array2[3] = new Vector2(mOuterUV.xMax, mOuterUV.yMin);
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				array2[i] = Vector2.zero;
			}
		}
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!base.atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (int j = 0; j < 3; j++)
		{
			int num5 = j + 1;
			for (int k = 0; k < 3; k++)
			{
				if (mFillCenter || j != 1 || k != 1)
				{
					int num6 = k + 1;
					verts.Add(new Vector3(array[num5].x, array[k].y, 0f));
					verts.Add(new Vector3(array[num5].x, array[num6].y, 0f));
					verts.Add(new Vector3(array[j].x, array[num6].y, 0f));
					verts.Add(new Vector3(array[j].x, array[k].y, 0f));
					uvs.Add(new Vector2(array2[num5].x, array2[k].y));
					uvs.Add(new Vector2(array2[num5].x, array2[num6].y));
					uvs.Add(new Vector2(array2[j].x, array2[num6].y));
					uvs.Add(new Vector2(array2[j].x, array2[k].y));
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
				}
			}
		}
	}
}
