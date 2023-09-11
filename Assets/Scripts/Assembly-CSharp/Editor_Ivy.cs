using System.Collections.Generic;
using UnityEngine;

public class Editor_Ivy : MonoBehaviour
{
	public static GameObject rightObj;

	public static GameObject leftObj;

	public static GameObject[] objs;

	public static Ivy.eType select_type;

	public List<Ivy> ivyList = new List<Ivy>();

	public List<Bud> budList = new List<Bud>();

	private static Editor_Ivy instance;

	public static Editor_Ivy Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		if (rightObj == null)
		{
			rightObj = Resources.Load("Prefabs/Common/Object_16_R") as GameObject;
		}
		if (leftObj == null)
		{
			leftObj = Resources.Load("Prefabs/Common/Object_16_L") as GameObject;
		}
		Editor_UIButtonMessage component = base.transform.GetComponent<Editor_UIButtonMessage>();
		component.target = base.gameObject;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnButtonEvy(GameObject caller, bool isDelete)
	{
		int num = int.Parse(caller.name.Replace("bubble_", string.Empty));
		int row = num % 10;
		int column = num / 10;
		if (!isDelete)
		{
			setBud(row, column);
			return;
		}
		deleteBud(row, column);
		Ivy sameColumnIvy = getSameColumnIvy(column);
		if (sameColumnIvy != null && sameColumnIvy.getBudNum() == 0)
		{
			deleteIvy(column);
		}
	}

	public void setBud(int row, int column)
	{
		float num = 0f;
		float num2 = 0f;
		if (column % 2 == 1)
		{
			num = 10f;
		}
		num2 = (float)row * 21f + num;
		if (getRightBubble(column) == null || (select_type == Ivy.eType.Right && num2 < getRightBubble(column).transform.localPosition.x))
		{
			Debug.Log("this position can't create ivy.");
			return;
		}
		if (getLeftBubble(column) == null || (select_type == Ivy.eType.Left && num2 > getLeftBubble(column).transform.localPosition.x))
		{
			Debug.Log("this position can't create ivy.");
			return;
		}
		Ivy ivy = getSameColumnIvy(column);
		if (ivy != null && ivy.ivyType_ != select_type)
		{
			deleteIvy(column);
			ivy = null;
		}
		if (ivy == null)
		{
			ivy = createIvy(column);
		}
		foreach (Bud bud2 in budList)
		{
			if (bud2.getColumn() == column && bud2.getRow() == row)
			{
				return;
			}
		}
		Bud bud = ivy.createBudObj(row, column, 21f);
		bud.GetComponent<TweenScale>().enabled = false;
		bud.gameObject.transform.localScale = new Vector3(21f, 21f, 1f);
		budList.Add(bud);
	}

	public Ivy createIvy(int column)
	{
		GameObject gameObject = ((select_type != 0) ? (Object.Instantiate(leftObj) as GameObject) : (Object.Instantiate(rightObj) as GameObject));
		gameObject.transform.parent = base.transform.parent;
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<Ivy>().setEditScale();
		gameObject.transform.localPosition = new Vector3(0f, (float)column * -18.186533f, -100f);
		TweenScale[] componentsInChildren = gameObject.GetComponentsInChildren<TweenScale>();
		TweenScale[] array = componentsInChildren;
		foreach (TweenScale tweenScale in array)
		{
			tweenScale.enabled = false;
		}
		Ivy component = gameObject.GetComponent<Ivy>();
		component.ivyType_ = select_type;
		component.column_ = column;
		ivyList.Add(component);
		setupRomPos(component, ((select_type != 0) ? getLeftBubble(column).transform.localPosition : getRightBubble(column).transform.localPosition).x);
		return component;
	}

	public void setupRomPos(Ivy iv, float pos_x)
	{
		float num = 85f;
		if (iv.ivyType_ == Ivy.eType.Left)
		{
			num = 0f - num;
		}
		iv.setIvyPos(pos_x + num);
	}

	public void deleteIvy(int column)
	{
		foreach (Bud bud in budList)
		{
			if (bud.getColumn() == column)
			{
				budList.Remove(bud);
				deleteIvy(column);
				return;
			}
		}
		Ivy sameColumnIvy = getSameColumnIvy(column);
		ivyList.Remove(sameColumnIvy);
		Object.DestroyImmediate(sameColumnIvy.gameObject);
	}

	public void deleteBud(int row, int column)
	{
		foreach (Bud bud in budList)
		{
			if (bud.getColumn() == column && bud.getRow() == row)
			{
				budList.Remove(bud);
				getSameColumnIvy(column).deleteBud(row);
				break;
			}
		}
	}

	public Ivy getSameColumnIvy(int column)
	{
		foreach (Ivy ivy in ivyList)
		{
			if (ivy.column_ == column)
			{
				return ivy;
			}
		}
		return null;
	}

	public void setIvyVisible(bool enable)
	{
		foreach (Ivy ivy in ivyList)
		{
			ivy.gameObject.SetActive(enable);
		}
	}

	private GameObject getRightBubble(int height)
	{
		for (int i = 0; i < 10; i++)
		{
			int num = 10 * height + i;
			tk2dAnimatedSprite componentInChildren = objs[num].GetComponentInChildren<tk2dAnimatedSprite>();
			tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = null;
			if (componentInChildren != null)
			{
				tk2dSpriteAnimationClip2 = componentInChildren.CurrentClip;
			}
			if (tk2dSpriteAnimationClip2 != null && tk2dSpriteAnimationClip2.name != "bubble_99")
			{
				return objs[num];
			}
		}
		return null;
	}

	private GameObject getLeftBubble(int height)
	{
		for (int num = 9; num >= 0; num--)
		{
			int num2 = 10 * height + num;
			tk2dAnimatedSprite componentInChildren = objs[num2].GetComponentInChildren<tk2dAnimatedSprite>();
			tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = null;
			if (componentInChildren != null)
			{
				tk2dSpriteAnimationClip2 = componentInChildren.CurrentClip;
			}
			if (tk2dSpriteAnimationClip2 != null && tk2dSpriteAnimationClip2.name != "bubble_99")
			{
				return objs[num2];
			}
		}
		return null;
	}

	public void reSetupIvy()
	{
		if (ivyList.Count <= 0)
		{
			return;
		}
		foreach (Ivy ivy in ivyList)
		{
			GameObject gameObject = ((ivy.ivyType_ != 0) ? getLeftBubble(ivy.column_) : getRightBubble(ivy.column_));
			if (gameObject == null)
			{
				deleteIvy(ivy.column_);
				reSetupIvy();
				break;
			}
			reSetupBud(ivy, gameObject.transform.localPosition.x);
			setupRomPos(ivy, gameObject.transform.localPosition.x);
		}
	}

	public void reSetupBud(Ivy iv, float limit_x)
	{
		foreach (Bud bud in budList)
		{
			if (bud.getColumn() == iv.column_)
			{
				if (iv.ivyType_ == Ivy.eType.Right && bud.transform.localPosition.x < limit_x - 12f)
				{
					deleteBud(bud.getRow(), bud.getColumn());
					reSetupBud(iv, limit_x);
					return;
				}
				if (iv.ivyType_ == Ivy.eType.Left && bud.transform.localPosition.x > limit_x + 12f)
				{
					deleteBud(bud.getRow(), bud.getColumn());
					reSetupBud(iv, limit_x);
					return;
				}
			}
		}
		foreach (Bud bud2 in budList)
		{
			if (bud2.getColumn() == iv.column_)
			{
				return;
			}
		}
		deleteIvy(iv.column_);
	}

	public void reset()
	{
		if (ivyList.Count <= 0)
		{
			return;
		}
		using (List<Ivy>.Enumerator enumerator = ivyList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Ivy current = enumerator.Current;
				deleteIvy(current.column_);
				reset();
				return;
			}
		}
		ivyList.Clear();
		budList.Clear();
	}

	public void onChangeLineNum(int lineNum)
	{
		foreach (Ivy ivy in ivyList)
		{
			if (ivy.column_ >= lineNum)
			{
				deleteIvy(ivy.column_);
				onChangeLineNum(lineNum);
				break;
			}
		}
	}
}
