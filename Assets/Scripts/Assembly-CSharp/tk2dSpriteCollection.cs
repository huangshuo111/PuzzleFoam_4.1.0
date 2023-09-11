using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollection")]
public class tk2dSpriteCollection : MonoBehaviour
{
	public enum NormalGenerationMode
	{
		None = 0,
		NormalsOnly = 1,
		NormalsAndTangents = 2
	}

	public enum TextureCompression
	{
		Uncompressed = 0,
		Reduced16Bit = 1,
		Compressed = 2,
		Dithered16Bit_Alpha = 3,
		Dithered16Bit_NoAlpha = 4
	}

	public const int CURRENT_VERSION = 3;

	[SerializeField]
	private tk2dSpriteCollectionDefinition[] textures;

	[SerializeField]
	private Texture2D[] textureRefs;

	public tk2dSpriteSheetSource[] spriteSheets;

	public tk2dSpriteCollectionFont[] fonts;

	public tk2dSpriteCollectionDefault defaults;

	public List<tk2dSpriteCollectionPlatform> platforms = new List<tk2dSpriteCollectionPlatform>();

	public bool managedSpriteCollection;

	public bool loadable;

	public int maxTextureSize = 1024;

	public bool forceTextureSize;

	public int forcedTextureWidth = 1024;

	public int forcedTextureHeight = 1024;

	public TextureCompression textureCompression;

	public int atlasWidth;

	public int atlasHeight;

	public bool forceSquareAtlas;

	public float atlasWastage;

	public bool allowMultipleAtlases;

	public tk2dSpriteCollectionDefinition[] textureParams;

	public tk2dSpriteCollectionData spriteCollection;

	public bool premultipliedAlpha;

	public Material[] altMaterials;

	public Material[] atlasMaterials;

	public Texture2D[] atlasTextures;

	public bool useTk2dCamera;

	public int targetHeight = 640;

	public float targetOrthoSize = 1f;

	public float globalScale = 1f;

	[SerializeField]
	private bool pixelPerfectPointSampled;

	public FilterMode filterMode = FilterMode.Bilinear;

	public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

	public bool userDefinedTextureSettings;

	public bool mipmapEnabled;

	public int anisoLevel = 1;

	public float physicsDepth = 0.1f;

	public bool disableTrimming;

	public NormalGenerationMode normalGenerationMode;

	public int padAmount = -1;

	public bool autoUpdate = true;

	public float editorDisplayScale = 1f;

	public int version;

	public string assetName = string.Empty;

	public Texture2D[] DoNotUse__TextureRefs
	{
		get
		{
			return textureRefs;
		}
		set
		{
			textureRefs = value;
		}
	}

	public bool HasPlatformData
	{
		get
		{
			return platforms.Count > 1;
		}
	}

	public void Upgrade()
	{
		if (version == 3)
		{
			return;
		}
		Debug.Log("SpriteCollection '" + base.name + "' - Upgraded from version " + version);
		if (version == 0)
		{
			if (pixelPerfectPointSampled)
			{
				filterMode = FilterMode.Point;
			}
			else
			{
				filterMode = FilterMode.Bilinear;
			}
			userDefinedTextureSettings = true;
		}
		if (version < 3 && textureRefs != null && textureParams != null && textureRefs.Length == textureParams.Length)
		{
			for (int i = 0; i < textureRefs.Length; i++)
			{
				textureParams[i].texture = textureRefs[i];
			}
			textureRefs = null;
		}
		version = 3;
	}
}
