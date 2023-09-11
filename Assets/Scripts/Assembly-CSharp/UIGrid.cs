using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Grid")]
[ExecuteInEditMode]
public class UIGrid : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal = 0,
		Vertical = 1
	}

	public Arrangement arrangement;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool repositionNow;

	public bool sorted;

	public bool hideInactive = true;

	public bool isUseColliderSize;

	private bool mStarted;

	public void Start()
	{
		mStarted = true;
		Reposition();
	}

	private void Update()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public void Reposition()
	{
		if (!mStarted)
		{
			repositionNow = true;
			return;
		}
		Transform transform = base.transform;
		int num = 0;
		int num2 = 0;
		if (sorted)
		{
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if ((bool)child && (!hideInactive || NGUITools.GetActive(child.gameObject)))
				{
					list.Add(child);
				}
			}
			list.Sort(SortByName);
			float num3 = 0f;
			float num4 = 0f;
			int j = 0;
			for (int count = list.Count; j < count; j++)
			{
				Transform transform2 = list[j];
				if (!NGUITools.GetActive(transform2.gameObject) && hideInactive)
				{
					continue;
				}
				float num5 = cellWidth;
				float num6 = cellHeight;
				if (isUseColliderSize && transform2.GetComponent<Collider>() != null)
				{
					BoxCollider boxCollider = transform2.GetComponent<Collider>() as BoxCollider;
					if (boxCollider != null)
					{
						float num7 = boxCollider.size.x * transform2.localScale.x;
						float num8 = boxCollider.size.y * transform2.localScale.y;
						num5 = num7 * 0.5f;
						num6 = num8 * 0.5f;
						if (arrangement == Arrangement.Horizontal)
						{
							num5 += boxCollider.center.x;
							num3 += ((j <= 0) ? 0f : (num7 * 0.5f));
							num3 -= boxCollider.center.x;
						}
						else
						{
							num6 -= boxCollider.center.y;
							num4 -= ((j <= 0) ? 0f : (num8 * 0.5f));
						}
					}
				}
				float z = transform2.localPosition.z;
				transform2.localPosition = new Vector3(num3, num4, z);
				if (arrangement == Arrangement.Horizontal)
				{
					if (++num >= maxPerLine && maxPerLine > 0)
					{
						num = 0;
						num2++;
						num3 = 0f;
						num4 -= num6;
					}
					else
					{
						num3 += num5;
					}
				}
				else if (++num >= maxPerLine && maxPerLine > 0)
				{
					num = 0;
					num2++;
					num4 = 0f;
					num3 += num5;
				}
				else
				{
					num4 -= num6;
				}
			}
		}
		else
		{
			for (int k = 0; k < transform.childCount; k++)
			{
				Transform child2 = transform.GetChild(k);
				if (NGUITools.GetActive(child2.gameObject) || !hideInactive)
				{
					float z2 = child2.localPosition.z;
					child2.localPosition = ((arrangement != 0) ? new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, z2) : new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, z2));
					if (++num >= maxPerLine && maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
			}
		}
		UIDraggablePanel uIDraggablePanel = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
		if (uIDraggablePanel != null)
		{
			uIDraggablePanel.UpdateScrollbars(true);
		}
	}
}
