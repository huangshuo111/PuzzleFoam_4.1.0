using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGrid : MonoBehaviour
{
	private const string LAYOUT_GRID_PREFAB_NAME = "P_Choice_0000";

	private const string LAYOUT_GRID_SPRITE_NAME = "Choice_01";

	public const int SORTING_ORDER_MAX = 30767;

	private const int GRID_WIDTH = 8;

	private const int GRID_HEIGHT = 8;

	private static readonly Color PLACABLE_COLOR = new Color(0.781f, 1f, 0f, 0.781f);

	private static readonly Color UNPLACEABLE_COLOR = new Color(1f, 0f, 0f, 0.8593f);

	private ParkStructures.Size currentSize_;

	private Dictionary<ParkStructures.IntegerXY, SpriteRenderer> renderers_ = new Dictionary<ParkStructures.IntegerXY, SpriteRenderer>();

	public static LayoutGrid createObject(Transform parent)
	{
		GameObject gameObject = new GameObject("LayoutGrid");
		gameObject.transform.SetParent(parent, false);
		return gameObject.AddComponent<LayoutGrid>();
	}

	public IEnumerator setup(ParkStructures.Size gridSize)
	{
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		GameObject prefab = objectManager.findPrefab("P_Choice_0000");
		UIAtlas.Sprite atlasSprite = null;
		UIAtlas atlas = objectManager.findOtherAtlas("Choice_01", ref atlasSprite);
		Vector3 startPosition = new Vector3(0f, 2f);
		Vector3 position = Vector3.zero;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				GameObject layoutGrid = Object.Instantiate(prefab) as GameObject;
				layoutGrid.SetActive(true);
				ParkObjectManager.Instance.ConvertSpritesWithSpriteDetails(atlas, layoutGrid);
				layoutGrid.transform.SetParent(base.transform, false);
				layoutGrid.transform.localPosition = startPosition + position;
				SpriteRenderer renderer = layoutGrid.GetComponent<SpriteRenderer>();
				renderers_.Add(new ParkStructures.IntegerXY(i, j), renderer);
				renderer.sortingOrder = 151;
				position.x += gridSize.width / 2;
				position.y += gridSize.height / 2;
			}
			startPosition.x -= gridSize.width / 2;
			startPosition.y += gridSize.height / 2;
			position = Vector3.zero;
		}
		base.transform.localPosition = new Vector3(-1500f, -1500f);
		yield break;
	}

	public void setVisible(ParkStructures.Size objectSize, int sortingOrder = 30767)
	{
		ForceUnvisible();
		for (int i = 0; i < objectSize.width; i++)
		{
			for (int j = 0; j < objectSize.height; j++)
			{
				SpriteRenderer value = null;
				renderers_.TryGetValue(new ParkStructures.IntegerXY(i, j), out value);
				if (value != null)
				{
					value.gameObject.SetActive(true);
					value.sortingOrder = sortingOrder;
				}
			}
		}
		currentSize_ = objectSize;
	}

	public void setUnplaceableGrid(Building building)
	{
		if (ParkObjectManager.Instance.enableDetaildLayout)
		{
			setColor(true);
			List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
			ParkObjectManager.getRelationalIndices(building.index, currentSize_, ParkObject.eDirection.Default, ref indices, false);
			for (int i = 0; i < indices.Count; i++)
			{
				ParkStructures.IntegerXY integerXY = indices[i];
				int num = i / currentSize_.width;
				int num2 = i % currentSize_.width;
				ParkStructures.IntegerXY key = new ParkStructures.IntegerXY(num2, num);
				SpriteRenderer value = null;
				if ((num2 < 0 || num2 >= ParkObjectManager.Instance.mapGridCount.width) && renderers_.TryGetValue(key, out value))
				{
					value.color = UNPLACEABLE_COLOR;
					continue;
				}
				if ((num < 0 || num >= ParkObjectManager.Instance.mapGridCount.height) && renderers_.TryGetValue(key, out value))
				{
					value.color = UNPLACEABLE_COLOR;
					continue;
				}
				Grid grid = ParkObjectManager.Instance.getGrid(integerXY.x, integerXY.y);
				if (grid == null && renderers_.TryGetValue(key, out value))
				{
					value.color = UNPLACEABLE_COLOR;
				}
				else if (!grid.isReleased && renderers_.TryGetValue(key, out value))
				{
					value.color = UNPLACEABLE_COLOR;
				}
				else if (ParkObjectManager.CheckGridObjects(grid, building) && renderers_.TryGetValue(key, out value))
				{
					value.color = UNPLACEABLE_COLOR;
				}
			}
		}
		else
		{
			setColor(false);
		}
	}

	public void ForceUnvisible()
	{
		foreach (SpriteRenderer value in renderers_.Values)
		{
			value.gameObject.SetActive(false);
		}
	}

	public void setColor(bool placable)
	{
		Color color = PLACABLE_COLOR;
		if (!placable)
		{
			color = UNPLACEABLE_COLOR;
		}
		foreach (SpriteRenderer value in renderers_.Values)
		{
			value.color = color;
		}
	}

	public void setPosition(Vector3 position)
	{
		base.transform.localPosition = position;
	}
}
