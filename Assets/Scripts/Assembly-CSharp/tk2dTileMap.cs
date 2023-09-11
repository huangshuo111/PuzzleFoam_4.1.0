using System;
using System.Collections.Generic;
using UnityEngine;
using tk2dRuntime;
using tk2dRuntime.TileMap;

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/TileMap/TileMap")]
public class tk2dTileMap : MonoBehaviour, ISpriteCollectionForceBuild
{
	[Flags]
	public enum BuildFlags
	{
		Default = 0,
		EditMode = 1,
		ForceBuild = 2
	}

	public string editorDataGUID = string.Empty;

	public tk2dTileMapData data;

	public GameObject renderData;

	[SerializeField]
	private tk2dSpriteCollectionData spriteCollection;

	private tk2dSpriteCollectionData _spriteCollectionInst;

	[SerializeField]
	private int spriteCollectionKey;

	public int width = 128;

	public int height = 128;

	public int partitionSizeX = 32;

	public int partitionSizeY = 32;

	[SerializeField]
	private Layer[] layers;

	[SerializeField]
	private ColorChannel colorChannel;

	public int buildKey;

	[SerializeField]
	private bool _inEditMode;

	public bool serializeRenderData;

	public string serializedMeshPath;

	public tk2dSpriteCollectionData Editor__SpriteCollection
	{
		get
		{
			return spriteCollection;
		}
		set
		{
			_spriteCollectionInst = null;
			spriteCollection = value;
			if (spriteCollection != null)
			{
				_spriteCollectionInst = spriteCollection.inst;
			}
		}
	}

	public tk2dSpriteCollectionData SpriteCollectionInst
	{
		get
		{
			if (_spriteCollectionInst == null && spriteCollection != null)
			{
				_spriteCollectionInst = spriteCollection.inst;
			}
			return _spriteCollectionInst;
		}
	}

	public bool AllowEdit
	{
		get
		{
			return _inEditMode;
		}
	}

	public Layer[] Layers
	{
		get
		{
			return layers;
		}
		set
		{
			layers = value;
		}
	}

	public ColorChannel ColorChannel
	{
		get
		{
			return colorChannel;
		}
		set
		{
			colorChannel = value;
		}
	}

	private void Awake()
	{
		if (spriteCollection != null)
		{
			_spriteCollectionInst = spriteCollection.inst;
		}
		bool flag = true;
		if ((bool)SpriteCollectionInst && SpriteCollectionInst.buildKey != spriteCollectionKey)
		{
			flag = false;
		}
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			if ((Application.isPlaying && _inEditMode) || !flag)
			{
				Build(BuildFlags.ForceBuild);
			}
		}
		else if (_inEditMode)
		{
			Debug.LogError("Tilemap " + base.name + " is still in edit mode. Please fix.Building overhead will be significant.");
			Build(BuildFlags.ForceBuild);
		}
		else if (!flag)
		{
			Build(BuildFlags.ForceBuild);
		}
	}

	public void Build()
	{
		Build(BuildFlags.Default);
	}

	public void ForceBuild()
	{
		Build(BuildFlags.ForceBuild);
	}

	private void ClearSpawnedInstances()
	{
		if (layers == null)
		{
			return;
		}
		for (int i = 0; i < layers.Length; i++)
		{
			Layer layer = layers[i];
			for (int j = 0; j < layer.spriteChannel.chunks.Length; j++)
			{
				SpriteChunk spriteChunk = layer.spriteChannel.chunks[j];
				if (!(spriteChunk.gameObject == null))
				{
					Transform transform = spriteChunk.gameObject.transform;
					List<Transform> list = new List<Transform>();
					for (int k = 0; k < transform.GetChildCount(); k++)
					{
						list.Add(transform.GetChild(k));
					}
					for (int l = 0; l < list.Count; l++)
					{
						UnityEngine.Object.DestroyImmediate(list[l].gameObject);
					}
				}
			}
		}
	}

	public void Build(BuildFlags buildFlags)
	{
		if (spriteCollection != null)
		{
			_spriteCollectionInst = spriteCollection.inst;
		}
		if (data != null)
		{
			if (data.tilePrefabs == null)
			{
				data.tilePrefabs = new UnityEngine.Object[SpriteCollectionInst.Count];
			}
			else if (data.tilePrefabs.Length != SpriteCollectionInst.Count)
			{
				Array.Resize(ref data.tilePrefabs, SpriteCollectionInst.Count);
			}
			BuilderUtil.InitDataStore(this);
			if ((bool)SpriteCollectionInst)
			{
				SpriteCollectionInst.InitMaterialIds();
			}
			bool flag = (buildFlags & BuildFlags.EditMode) != 0;
			bool flag2 = (buildFlags & BuildFlags.ForceBuild) != 0;
			if ((bool)SpriteCollectionInst && SpriteCollectionInst.buildKey != spriteCollectionKey)
			{
				flag2 = true;
			}
			if (flag2)
			{
				ClearSpawnedInstances();
			}
			BuilderUtil.CreateRenderData(this, flag);
			RenderMeshBuilder.Build(this, flag, flag2);
			if (!flag)
			{
				ColliderBuilder.Build(this, flag2);
				BuilderUtil.SpawnPrefabs(this);
			}
			Layer[] array = layers;
			foreach (Layer layer in array)
			{
				layer.ClearDirtyFlag();
			}
			if (colorChannel != null)
			{
				colorChannel.ClearDirtyFlag();
			}
			buildKey = UnityEngine.Random.Range(0, int.MaxValue);
			if ((bool)SpriteCollectionInst)
			{
				spriteCollectionKey = SpriteCollectionInst.buildKey;
			}
		}
	}

	public bool GetTileAtPosition(Vector3 position, out int x, out int y)
	{
		float x2;
		float y2;
		bool tileFracAtPosition = GetTileFracAtPosition(position, out x2, out y2);
		x = (int)x2;
		y = (int)y2;
		return tileFracAtPosition;
	}

	public bool GetTileFracAtPosition(Vector3 position, out float x, out float y)
	{
		switch (data.tileType)
		{
		case tk2dTileMapData.TileType.Rectangular:
		{
			Vector3 vector2 = base.transform.worldToLocalMatrix.MultiplyPoint(position);
			x = (vector2.x - data.tileOrigin.x) / data.tileSize.x;
			y = (vector2.y - data.tileOrigin.y) / data.tileSize.y;
			return x >= 0f && x <= (float)width && y >= 0f && y <= (float)height;
		}
		case tk2dTileMapData.TileType.Isometric:
		{
			if (data.tileSize.x == 0f)
			{
				break;
			}
			float num = Mathf.Atan2(data.tileSize.y, data.tileSize.x / 2f);
			Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint(position);
			x = (vector.x - data.tileOrigin.x) / data.tileSize.x;
			y = (vector.y - data.tileOrigin.y) / data.tileSize.y;
			float num2 = y * 0.5f;
			int num3 = (int)num2;
			float num4 = num2 - (float)num3;
			float num5 = x % 1f;
			x = (int)x;
			y = num3 * 2;
			if (num5 > 0.5f)
			{
				if (num4 > 0.5f && Mathf.Atan2(1f - num4, (num5 - 0.5f) * 2f) < num)
				{
					y += 1f;
				}
				else if (num4 < 0.5f && Mathf.Atan2(num4, (num5 - 0.5f) * 2f) < num)
				{
					y -= 1f;
				}
			}
			else if (num5 < 0.5f)
			{
				if (num4 > 0.5f && Mathf.Atan2(num4 - 0.5f, num5 * 2f) > num)
				{
					y += 1f;
					x -= 1f;
				}
				if (num4 < 0.5f && Mathf.Atan2(num4, (0.5f - num5) * 2f) < num)
				{
					y -= 1f;
					x -= 1f;
				}
			}
			return x >= 0f && x <= (float)width && y >= 0f && y <= (float)height;
		}
		}
		x = 0f;
		y = 0f;
		return false;
	}

	public Vector3 GetTilePosition(int x, int y)
	{
		Vector3 v = new Vector3((float)x * data.tileSize.x + data.tileOrigin.x, (float)y * data.tileSize.y + data.tileOrigin.y, 0f);
		return base.transform.localToWorldMatrix.MultiplyPoint(v);
	}

	public int GetTileIdAtPosition(Vector3 position, int layer)
	{
		if (layer < 0 || layer >= layers.Length)
		{
			return -1;
		}
		int x;
		int y;
		if (!GetTileAtPosition(position, out x, out y))
		{
			return -1;
		}
		return layers[layer].GetTile(x, y);
	}

	public TileInfo GetTileInfoForTileId(int tileId)
	{
		return data.GetTileInfoForSprite(tileId);
	}

	public Color GetInterpolatedColorAtPosition(Vector3 position)
	{
		Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint(position);
		int num = (int)((vector.x - data.tileOrigin.x) / data.tileSize.x);
		int num2 = (int)((vector.y - data.tileOrigin.y) / data.tileSize.y);
		if (colorChannel == null || colorChannel.IsEmpty)
		{
			return Color.white;
		}
		if (num < 0 || num >= width || num2 < 0 || num2 >= height)
		{
			return colorChannel.clearColor;
		}
		int offset;
		ColorChunk colorChunk = colorChannel.FindChunkAndCoordinate(num, num2, out offset);
		if (colorChunk.Empty)
		{
			return colorChannel.clearColor;
		}
		int num3 = partitionSizeX + 1;
		Color a = colorChunk.colors[offset];
		Color b = colorChunk.colors[offset + 1];
		Color a2 = colorChunk.colors[offset + num3];
		Color b2 = colorChunk.colors[offset + num3 + 1];
		float num4 = (float)num * data.tileSize.x + data.tileOrigin.x;
		float num5 = (float)num2 * data.tileSize.y + data.tileOrigin.y;
		float t = (vector.x - num4) / data.tileSize.x;
		float t2 = (vector.y - num5) / data.tileSize.y;
		Color a3 = Color.Lerp(a, b, t);
		Color b3 = Color.Lerp(a2, b2, t);
		return Color.Lerp(a3, b3, t2);
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		return spriteCollection == this.spriteCollection || _spriteCollectionInst == spriteCollection;
	}

	public Mesh GetOrCreateMesh()
	{
		return new Mesh();
	}

	public void TouchMesh(Mesh mesh)
	{
	}

	public void DestroyMesh(Mesh mesh)
	{
		UnityEngine.Object.DestroyImmediate(mesh);
	}
}
