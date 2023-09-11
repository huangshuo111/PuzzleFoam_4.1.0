using UnityEngine;
using tk2dRuntime;

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/Sprite/tk2dSprite")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class tk2dSprite : tk2dBaseSprite
{
	private Mesh mesh;

	private Vector3[] meshVertices;

	private Vector3[] meshNormals;

	private Vector4[] meshTangents;

	private Color[] meshColors;

	private new void Awake()
	{
		base.Awake();
		mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		GetComponent<MeshFilter>().mesh = mesh;
		if ((bool)base.Collection)
		{
			if (_spriteId < 0 || _spriteId >= base.Collection.Count)
			{
				_spriteId = 0;
			}
			Build();
		}
	}

	protected void OnDestroy()
	{
		if ((bool)mesh)
		{
			Object.Destroy(mesh);
		}
		if ((bool)meshColliderMesh)
		{
			Object.Destroy(meshColliderMesh);
		}
	}

	public override void Build()
	{
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[base.spriteId];
		meshVertices = new Vector3[tk2dSpriteDefinition2.positions.Length];
		meshColors = new Color[tk2dSpriteDefinition2.positions.Length];
		meshNormals = new Vector3[0];
		meshTangents = new Vector4[0];
		if (tk2dSpriteDefinition2.normals != null && tk2dSpriteDefinition2.normals.Length > 0)
		{
			meshNormals = new Vector3[tk2dSpriteDefinition2.normals.Length];
		}
		if (tk2dSpriteDefinition2.tangents != null && tk2dSpriteDefinition2.tangents.Length > 0)
		{
			meshTangents = new Vector4[tk2dSpriteDefinition2.tangents.Length];
		}
		SetPositions(meshVertices, meshNormals, meshTangents);
		SetColors(meshColors);
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
			GetComponent<MeshFilter>().mesh = mesh;
		}
		mesh.Clear();
		mesh.vertices = meshVertices;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.colors = meshColors;
		mesh.uv = tk2dSpriteDefinition2.uvs;
		mesh.triangles = tk2dSpriteDefinition2.indices;
		UpdateMaterial();
		CreateCollider();
	}

	public static tk2dSprite AddComponent(GameObject go, tk2dSpriteCollectionData spriteCollection, int spriteId)
	{
		return tk2dBaseSprite.AddComponent<tk2dSprite>(go, spriteCollection, spriteId);
	}

	public static tk2dSprite AddComponent(GameObject go, tk2dSpriteCollectionData spriteCollection, string spriteName)
	{
		return tk2dBaseSprite.AddComponent<tk2dSprite>(go, spriteCollection, spriteName);
	}

	public static GameObject CreateFromTexture(Texture2D texture, SpriteCollectionSize size, Rect region, Vector2 anchor)
	{
		return tk2dBaseSprite.CreateFromTexture<tk2dSprite>(texture, size, region, anchor);
	}

	protected override void UpdateGeometry()
	{
		UpdateGeometryImpl();
	}

	protected override void UpdateColors()
	{
		UpdateColorsImpl();
	}

	protected override void UpdateVertices()
	{
		UpdateVerticesImpl();
	}

	protected void UpdateColorsImpl()
	{
		SetColors(meshColors);
		mesh.colors = meshColors;
	}

	protected void UpdateVerticesImpl()
	{
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[base.spriteId];
		if (tk2dSpriteDefinition2.normals.Length != meshNormals.Length)
		{
			meshNormals = ((tk2dSpriteDefinition2.normals == null || tk2dSpriteDefinition2.normals.Length <= 0) ? new Vector3[0] : new Vector3[tk2dSpriteDefinition2.normals.Length]);
		}
		if (tk2dSpriteDefinition2.tangents.Length != meshTangents.Length)
		{
			meshTangents = ((tk2dSpriteDefinition2.tangents == null || tk2dSpriteDefinition2.tangents.Length <= 0) ? new Vector4[0] : new Vector4[tk2dSpriteDefinition2.tangents.Length]);
		}
		SetPositions(meshVertices, meshNormals, meshTangents);
		mesh.vertices = meshVertices;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.uv = tk2dSpriteDefinition2.uvs;
		mesh.bounds = GetBounds();
	}

	protected void UpdateGeometryImpl()
	{
		if (mesh == null)
		{
			Build();
		}
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[base.spriteId];
		if (meshVertices == null || meshVertices.Length != tk2dSpriteDefinition2.positions.Length)
		{
			meshVertices = new Vector3[tk2dSpriteDefinition2.positions.Length];
			meshNormals = ((tk2dSpriteDefinition2.normals == null || tk2dSpriteDefinition2.normals.Length <= 0) ? new Vector3[0] : new Vector3[tk2dSpriteDefinition2.normals.Length]);
			meshTangents = ((tk2dSpriteDefinition2.tangents == null || tk2dSpriteDefinition2.tangents.Length <= 0) ? new Vector4[0] : new Vector4[tk2dSpriteDefinition2.tangents.Length]);
			meshColors = new Color[tk2dSpriteDefinition2.positions.Length];
		}
		SetPositions(meshVertices, meshNormals, meshTangents);
		SetColors(meshColors);
		mesh.Clear();
		mesh.vertices = meshVertices;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.colors = meshColors;
		mesh.uv = tk2dSpriteDefinition2.uvs;
		mesh.bounds = GetBounds();
		mesh.triangles = tk2dSpriteDefinition2.indices;
	}

	protected override void UpdateMaterial()
	{
		if (base.GetComponent<Renderer>().sharedMaterial != collectionInst.spriteDefinitions[base.spriteId].materialInst)
		{
			base.GetComponent<Renderer>().material = collectionInst.spriteDefinitions[base.spriteId].materialInst;
		}
		if (base.GetComponent<Renderer>().sharedMaterial.mainTexture == null)
		{
			base.GetComponent<Renderer>().sharedMaterial.mainTexture = ResourceLoader.Instance.loadFromGameResource(base.GetComponent<Renderer>().sharedMaterial.name);
		}
	}

	protected override int GetCurrentVertexCount()
	{
		if (meshVertices == null)
		{
			Build();
		}
		return meshVertices.Length;
	}

	public override void ForceBuild()
	{
		base.ForceBuild();
		GetComponent<MeshFilter>().mesh = mesh;
	}
}
