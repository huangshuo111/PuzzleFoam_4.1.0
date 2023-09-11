using UnityEngine;

namespace tk2dRuntime
{
	internal static class SpriteCollectionGenerator
	{
		public static tk2dSpriteCollectionData CreateFromTexture(Texture2D texture, SpriteCollectionSize size, Rect region, Vector2 anchor)
		{
			return CreateFromTexture(texture, size, new string[1] { "Unnamed" }, new Rect[1] { region }, new Vector2[1] { anchor });
		}

		public static tk2dSpriteCollectionData CreateFromTexture(Texture2D texture, SpriteCollectionSize size, string[] names, Rect[] regions, Vector2[] anchors)
		{
			GameObject gameObject = new GameObject("SpriteCollection");
			tk2dSpriteCollectionData tk2dSpriteCollectionData = gameObject.AddComponent<tk2dSpriteCollectionData>();
			tk2dSpriteCollectionData.Transient = true;
			tk2dSpriteCollectionData.version = 3;
			tk2dSpriteCollectionData.invOrthoSize = 1f / size.orthoSize;
			tk2dSpriteCollectionData.halfTargetHeight = size.targetHeight * 0.5f;
			tk2dSpriteCollectionData.premultipliedAlpha = false;
			string name = "tk2d/BlendVertexColor";
			tk2dSpriteCollectionData.material = new Material(Shader.Find(name));
			tk2dSpriteCollectionData.material.mainTexture = texture;
			tk2dSpriteCollectionData.materials = new Material[1] { tk2dSpriteCollectionData.material };
			tk2dSpriteCollectionData.textures = new Texture[1] { texture };
			float scale = 2f * size.orthoSize / size.targetHeight;
			tk2dSpriteCollectionData.spriteDefinitions = new tk2dSpriteDefinition[regions.Length];
			for (int i = 0; i < regions.Length; i++)
			{
				tk2dSpriteCollectionData.spriteDefinitions[i] = CreateDefinitionForRegionInTexture(texture, scale, regions[i], anchors[i]);
				tk2dSpriteCollectionData.spriteDefinitions[i].name = names[i];
			}
			tk2dSpriteDefinition[] spriteDefinitions = tk2dSpriteCollectionData.spriteDefinitions;
			foreach (tk2dSpriteDefinition tk2dSpriteDefinition in spriteDefinitions)
			{
				tk2dSpriteDefinition.material = tk2dSpriteCollectionData.material;
			}
			return tk2dSpriteCollectionData;
		}

		private static tk2dSpriteDefinition CreateDefinitionForRegionInTexture(Texture2D texture, float scale, Rect uvRegion, Vector2 anchor)
		{
			float height = uvRegion.height;
			float width = uvRegion.width;
			Vector3 vector = new Vector3((0f - anchor.x) * scale, (0f - (height - anchor.y)) * scale, 0f);
			Vector3 vector2 = vector + new Vector3(width * scale, height * scale, 0f);
			tk2dSpriteDefinition tk2dSpriteDefinition = new tk2dSpriteDefinition();
			tk2dSpriteDefinition.flipped = false;
			tk2dSpriteDefinition.extractRegion = false;
			tk2dSpriteDefinition.name = texture.name;
			tk2dSpriteDefinition.colliderType = tk2dSpriteDefinition.ColliderType.Unset;
			float num = texture.width;
			float num2 = texture.height;
			Vector2 vector3 = new Vector2(0.001f, 0.001f);
			Vector2 vector4 = new Vector2((uvRegion.x + vector3.x) / num, 1f - (uvRegion.y + uvRegion.height + vector3.y) / num2);
			Vector2 vector5 = new Vector2((uvRegion.x + uvRegion.width - vector3.x) / num, 1f - (uvRegion.y - vector3.y) / num2);
			Vector3 lhs = vector;
			Vector3 rhs = vector2;
			tk2dSpriteDefinition.positions = new Vector3[4]
			{
				new Vector3(vector.x, vector.y, 0f),
				new Vector3(vector2.x, vector.y, 0f),
				new Vector3(vector.x, vector2.y, 0f),
				new Vector3(vector2.x, vector2.y, 0f)
			};
			tk2dSpriteDefinition.uvs = new Vector2[4]
			{
				new Vector2(vector4.x, vector4.y),
				new Vector2(vector5.x, vector4.y),
				new Vector2(vector4.x, vector5.y),
				new Vector2(vector5.x, vector5.y)
			};
			tk2dSpriteDefinition.normals = new Vector3[0];
			tk2dSpriteDefinition.tangents = new Vector4[0];
			tk2dSpriteDefinition.indices = new int[6] { 0, 3, 1, 2, 3, 0 };
			Vector3 vector6 = Vector3.Min(lhs, rhs);
			Vector3 vector7 = Vector3.Max(lhs, rhs);
			tk2dSpriteDefinition.boundsData = new Vector3[2]
			{
				(vector7 + vector6) / 2f,
				vector7 - vector6
			};
			tk2dSpriteDefinition.untrimmedBoundsData = new Vector3[2]
			{
				(vector7 + vector6) / 2f,
				vector7 - vector6
			};
			tk2dSpriteDefinition.texelSize = new Vector2(scale, scale);
			return tk2dSpriteDefinition;
		}
	}
}
