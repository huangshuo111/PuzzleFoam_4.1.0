using System;
using System.Collections.Generic;
using UnityEngine;
using tk2dRuntime;

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollectionData")]
public class tk2dSpriteCollectionData : MonoBehaviour
{
	public const int CURRENT_VERSION = 3;

	public int version;

	public bool materialIdsValid;

	public bool needMaterialInstance;

	public tk2dSpriteDefinition[] spriteDefinitions;

	private Dictionary<string, int> spriteNameLookupDict;

	public bool premultipliedAlpha;

	public Material material;

	public Material[] materials;

	[NonSerialized]
	public Material[] materialInsts;

	public Texture[] textures;

	public bool allowMultipleAtlases;

	public string spriteCollectionGUID;

	public string spriteCollectionName;

	public string assetName = string.Empty;

	public bool loadable;

	public float invOrthoSize = 1f;

	public float halfTargetHeight = 1f;

	public int buildKey;

	public string dataGuid = string.Empty;

	public bool managedSpriteCollection;

	public bool hasPlatformData;

	public string[] spriteCollectionPlatforms;

	public string[] spriteCollectionPlatformGUIDs;

	private tk2dSpriteCollectionData platformSpecificData;

	public bool Transient { get; set; }

	public int Count
	{
		get
		{
			return inst.spriteDefinitions.Length;
		}
	}

	public tk2dSpriteDefinition FirstValidDefinition
	{
		get
		{
			tk2dSpriteDefinition[] array = inst.spriteDefinitions;
			foreach (tk2dSpriteDefinition tk2dSpriteDefinition2 in array)
			{
				if (tk2dSpriteDefinition2.Valid)
				{
					return tk2dSpriteDefinition2;
				}
			}
			return null;
		}
	}

	public int FirstValidDefinitionIndex
	{
		get
		{
			tk2dSpriteCollectionData tk2dSpriteCollectionData2 = inst;
			for (int i = 0; i < tk2dSpriteCollectionData2.spriteDefinitions.Length; i++)
			{
				if (tk2dSpriteCollectionData2.spriteDefinitions[i].Valid)
				{
					return i;
				}
			}
			return -1;
		}
	}

	public tk2dSpriteCollectionData inst
	{
		get
		{
			if (platformSpecificData == null)
			{
				if (hasPlatformData)
				{
					string currentPlatform = tk2dSystem.CurrentPlatform;
					string text = string.Empty;
					for (int i = 0; i < spriteCollectionPlatforms.Length; i++)
					{
						if (spriteCollectionPlatforms[i] == currentPlatform)
						{
							text = spriteCollectionPlatformGUIDs[i];
							break;
						}
					}
					if (text.Length == 0)
					{
						text = spriteCollectionPlatformGUIDs[0];
					}
					platformSpecificData = tk2dSystem.LoadResourceByGUID<tk2dSpriteCollectionData>(text);
				}
				else
				{
					platformSpecificData = this;
				}
			}
			platformSpecificData.Init();
			return platformSpecificData;
		}
	}

	public int GetSpriteIdByName(string name)
	{
		return GetSpriteIdByName(name, 0);
	}

	public int GetSpriteIdByName(string name, int defaultValue)
	{
		inst.InitDictionary();
		int value = defaultValue;
		if (!inst.spriteNameLookupDict.TryGetValue(name, out value))
		{
			return defaultValue;
		}
		return value;
	}

	public tk2dSpriteDefinition GetSpriteDefinition(string name)
	{
		int spriteIdByName = GetSpriteIdByName(name, -1);
		if (spriteIdByName == -1)
		{
			return null;
		}
		return spriteDefinitions[spriteIdByName];
	}

	public void InitDictionary()
	{
		if (spriteNameLookupDict == null)
		{
			spriteNameLookupDict = new Dictionary<string, int>(spriteDefinitions.Length);
			for (int i = 0; i < spriteDefinitions.Length; i++)
			{
				spriteNameLookupDict[spriteDefinitions[i].name] = i;
			}
		}
	}

	public void InitMaterialIds()
	{
		if (inst.materialIdsValid)
		{
			return;
		}
		int num = -1;
		Dictionary<Material, int> dictionary = new Dictionary<Material, int>();
		for (int i = 0; i < inst.materials.Length; i++)
		{
			if (num == -1 && inst.materials[i] != null)
			{
				num = i;
			}
			dictionary[materials[i]] = i;
		}
		if (num == -1)
		{
			Debug.LogError("Init material ids failed.");
			return;
		}
		tk2dSpriteDefinition[] array = inst.spriteDefinitions;
		foreach (tk2dSpriteDefinition tk2dSpriteDefinition2 in array)
		{
			if (!dictionary.TryGetValue(tk2dSpriteDefinition2.material, out tk2dSpriteDefinition2.materialId))
			{
				tk2dSpriteDefinition2.materialId = num;
			}
		}
		inst.materialIdsValid = true;
	}

	private void Init()
	{
		if (materialInsts != null)
		{
			return;
		}
		if (spriteDefinitions == null)
		{
			spriteDefinitions = new tk2dSpriteDefinition[0];
		}
		if (materials == null)
		{
			materials = new Material[0];
		}
		materialInsts = new Material[materials.Length];
		if (needMaterialInstance)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				materialInsts[i] = UnityEngine.Object.Instantiate(materials[i]) as Material;
			}
			for (int j = 0; j < spriteDefinitions.Length; j++)
			{
				tk2dSpriteDefinition tk2dSpriteDefinition2 = spriteDefinitions[j];
				tk2dSpriteDefinition2.materialInst = materialInsts[tk2dSpriteDefinition2.materialId];
			}
		}
		else
		{
			for (int k = 0; k < spriteDefinitions.Length; k++)
			{
				tk2dSpriteDefinition tk2dSpriteDefinition3 = spriteDefinitions[k];
				tk2dSpriteDefinition3.materialInst = tk2dSpriteDefinition3.material;
			}
		}
	}

	public static tk2dSpriteCollectionData CreateFromTexture(Texture2D texture, SpriteCollectionSize size, string[] names, Rect[] regions, Vector2[] anchors)
	{
		return SpriteCollectionGenerator.CreateFromTexture(texture, size, names, regions, anchors);
	}

	public void ResetPlatformData()
	{
		if (hasPlatformData && (bool)platformSpecificData)
		{
			platformSpecificData = null;
		}
		materialInsts = null;
	}

	private void OnDestroy()
	{
		if (Transient)
		{
			Material[] array = materials;
			foreach (Material obj in array)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
		}
		else if (needMaterialInstance)
		{
			Material[] array2 = materialInsts;
			foreach (Material obj2 in array2)
			{
				UnityEngine.Object.DestroyImmediate(obj2);
			}
		}
		ResetPlatformData();
	}
}
