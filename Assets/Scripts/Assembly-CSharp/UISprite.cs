using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Sprite (Basic)")]
public class UISprite : UIWidget
{
	[HideInInspector]
	[SerializeField]
	private UIAtlas mAtlas;

	[SerializeField]
	[HideInInspector]
	private string mSpriteName;

	protected UIAtlas.Sprite mSprite;

	protected Rect mOuter;

	protected Rect mOuterUV;

	private bool mSpriteSet;

	private string mLastName = string.Empty;

	public Rect outerUV
	{
		get
		{
			UpdateUVs(false);
			return mOuterUV;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (mAtlas != value)
			{
				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;
				material = ((!(mAtlas != null)) ? null : mAtlas.spriteMaterial);
				if (string.IsNullOrEmpty(mSpriteName) && mAtlas != null && mAtlas.spriteList.Count > 0)
				{
					SetAtlasSprite(mAtlas.spriteList[0]);
					mSpriteName = mSprite.name;
				}
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					string text = mSpriteName;
					mSpriteName = string.Empty;
					spriteName = text;
					mChanged = true;
					UpdateUVs(true);
				}
			}
		}
	}

	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					mSpriteName = string.Empty;
					mSprite = null;
					mChanged = true;
				}
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				if (isValid)
				{
					UpdateUVs(true);
				}
			}
		}
	}

	public bool isValid
	{
		get
		{
			return GetAtlasSprite() != null;
		}
	}

	public override Vector2 pivotOffset
	{
		get
		{
			Vector2 zero = Vector2.zero;
			if (isValid)
			{
				Pivot pivot = base.pivot;
				switch (pivot)
				{
				case Pivot.Top:
				case Pivot.Center:
				case Pivot.Bottom:
					zero.x = (-1f - mSprite.paddingRight + mSprite.paddingLeft) * 0.5f;
					break;
				case Pivot.TopRight:
				case Pivot.Right:
				case Pivot.BottomRight:
					zero.x = -1f - mSprite.paddingRight;
					break;
				default:
					zero.x = mSprite.paddingLeft;
					break;
				}
				switch (pivot)
				{
				case Pivot.Left:
				case Pivot.Center:
				case Pivot.Right:
					zero.y = (1f + mSprite.paddingBottom - mSprite.paddingTop) * 0.5f;
					break;
				case Pivot.BottomLeft:
				case Pivot.Bottom:
				case Pivot.BottomRight:
					zero.y = 1f + mSprite.paddingBottom;
					break;
				default:
					zero.y = 0f - mSprite.paddingTop;
					break;
				}
			}
			return zero;
		}
	}

	public override Material material
	{
		get
		{
			Material material = base.material;
			if (material == null)
			{
				material = ((!(mAtlas != null)) ? null : mAtlas.spriteMaterial);
				mSprite = null;
				this.material = material;
				if (material != null)
				{
					UpdateUVs(true);
				}
			}
			return material;
		}
	}

	public virtual Vector4 border
	{
		get
		{
			return Vector4.zero;
		}
	}

	public UIAtlas.Sprite GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UIAtlas.Sprite sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UIAtlas.Sprite sprite2 = mAtlas.spriteList[0];
				if (sprite2 == null)
				{
					return null;
				}
				SetAtlasSprite(sprite2);
				if (mSprite == null)
				{
					Debug.LogError(mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				mSpriteName = mSprite.name;
			}
			if (mSprite != null)
			{
				material = mAtlas.spriteMaterial;
			}
		}
		return mSprite;
	}

	protected void SetAtlasSprite(UIAtlas.Sprite sp)
	{
		mChanged = true;
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? string.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public virtual void UpdateUVs(bool force)
	{
		if (!isValid || (!force && !(mOuter != mSprite.outer)))
		{
			return;
		}
		Texture texture = mainTexture;
		if (texture != null)
		{
			mOuter = mSprite.outer;
			mOuterUV = mOuter;
			if (mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
			{
				mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, texture.width, texture.height);
			}
			mChanged = true;
		}
	}

	public override void MakePixelPerfect()
	{
		if (!isValid)
		{
			return;
		}
		Texture texture = mainTexture;
		Vector3 localScale = base.cachedTransform.localScale;
		if (texture != null)
		{
			Rect rect = NGUIMath.ConvertToPixels(outerUV, texture.width, texture.height, false);
			float pixelSize = atlas.pixelSize;
			localScale.x = (float)Mathf.RoundToInt(rect.width * pixelSize) * Mathf.Sign(localScale.x);
			localScale.y = (float)Mathf.RoundToInt(rect.height * pixelSize) * Mathf.Sign(localScale.y);
			if (atlas.name.Contains("_Low"))
			{
				localScale *= 2f;
			}
			localScale.z = 1f;
			base.cachedTransform.localScale = localScale;
		}
		int num = Mathf.RoundToInt(Mathf.Abs(localScale.x) * (1f + mSprite.paddingLeft + mSprite.paddingRight));
		int num2 = Mathf.RoundToInt(Mathf.Abs(localScale.y) * (1f + mSprite.paddingTop + mSprite.paddingBottom));
		Vector3 localPosition = base.cachedTransform.localPosition;
		localPosition.x = Mathf.CeilToInt(localPosition.x * 4f) >> 2;
		localPosition.y = Mathf.CeilToInt(localPosition.y * 4f) >> 2;
		localPosition.z = Mathf.RoundToInt(localPosition.z);
		if (num % 2 == 1 && (base.pivot == Pivot.Top || base.pivot == Pivot.Center || base.pivot == Pivot.Bottom))
		{
			localPosition.x += 0.5f;
		}
		if (num2 % 2 == 1 && (base.pivot == Pivot.Left || base.pivot == Pivot.Center || base.pivot == Pivot.Right))
		{
			localPosition.y += 0.5f;
		}
		base.cachedTransform.localPosition = localPosition;
	}

	protected override void OnStart()
	{
		if (mAtlas != null)
		{
			UpdateUVs(true);
		}
	}

	public override bool OnUpdate()
	{
		if (mLastName != mSpriteName)
		{
			mSprite = null;
			mChanged = true;
			mLastName = mSpriteName;
			UpdateUVs(false);
			return true;
		}
		UpdateUVs(false);
		return false;
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector2 item = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		Vector2 item2 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		verts.Add(new Vector3(1f, 0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		uvs.Add(item2);
		uvs.Add(new Vector2(item2.x, item.y));
		uvs.Add(item);
		uvs.Add(new Vector2(item.x, item2.y));
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item3 = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
	}
}
