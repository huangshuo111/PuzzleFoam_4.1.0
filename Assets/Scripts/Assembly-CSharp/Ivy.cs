using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ivy : GimmickBase
{
	public enum eState
	{
		Field = 0,
		Break = 1,
		Gameover = 2
	}

	public enum eType
	{
		Right = 0,
		Left = 1
	}

	public const float OFFSET_X = 266f;

	private const float ONE_MOVE_TIME = 0.3f;

	private const float IVY_SCALE_SIZE = 0.7f;

	private const float IVY_SCALE_TIME = 0.3f;

	private const float DEF_IVY_REMOVE_TIME = 3f;

	public eType ivyType_;

	public int column_;

	public Vector3 headBubblePos_;

	public float toPosX;

	private List<GameObject> budObjList_ = new List<GameObject>();

	private StagePause stagePause_;

	private DialogManager dialogManager_;

	private bool preIsRemoved_;

	private Vector3 preLocalPosition = Vector3.zero;

	private Vector3 preScrollObjLocalPosition = Vector3.zero;

	private Vector3 preHeadBubblePos = Vector3.zero;

	private float preToX;

	public bool isRemoved_;

	private bool isBurn_;

	[SerializeField]
	private Transform ScrollTransform;

	[SerializeField]
	private Transform BudTransform;

	[SerializeField]
	private Transform IvyTransform;

	public eState state { get; private set; }

	private void Awake()
	{
		BudTransform.localScale = new Vector3(94f, 89f, 1f);
		IvyTransform.localScale = Vector3.one;
		BudTransform.gameObject.SetActive(false);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void setup(StagePause stagepause, DialogManager dialogmanager)
	{
		stagePause_ = stagepause;
		dialogManager_ = dialogmanager;
	}

	public Bud createBudObj(int row, int column, float size)
	{
		foreach (GameObject item in budObjList_)
		{
			if (item.GetComponent<Bud>().getRow() == row)
			{
				return item.GetComponent<Bud>();
			}
		}
		GameObject gameObject = Object.Instantiate(BudTransform.gameObject) as GameObject;
		float num = 0f;
		if (column % 2 == 1)
		{
			num = size / 2f;
		}
		gameObject.name = budObjList_.Count.ToString();
		gameObject.transform.parent = ScrollTransform;
		gameObject.transform.localScale = BudTransform.localScale;
		gameObject.transform.localPosition = new Vector3((float)row * size + num, -3f, -6f);
		Bud component = gameObject.GetComponent<Bud>();
		component.setType(ivyType_);
		component.setRow(row);
		component.setColumn(column);
		gameObject.SetActive(true);
		budObjList_.Add(gameObject);
		budObjList_.Sort((GameObject a, GameObject b) => (int)a.transform.localPosition.x - (int)b.transform.localPosition.x);
		return component;
	}

	public void deleteBud(int row)
	{
		GameObject gameObject = null;
		foreach (GameObject item in budObjList_)
		{
			Bud component = item.GetComponent<Bud>();
			if (component.getRow() == row)
			{
				gameObject = item;
				break;
			}
		}
		if (gameObject != null)
		{
			budObjList_.Remove(gameObject);
			Object.DestroyImmediate(gameObject);
		}
	}

	public int setBudPriority()
	{
		foreach (GameObject item in budObjList_)
		{
			Bud component = item.GetComponent<Bud>();
			item.GetComponent<UISprite>().depth += component.getColumn() * 10 + (9 - component.getRow());
		}
		Transform transform = base.transform.Find("leaf");
		UISprite component2 = transform.GetComponent<UISprite>();
		component2.depth = column_ * 10 + 20;
		return component2.depth;
	}

	public void setEditScale()
	{
		BudTransform.localScale = new Vector3(BudTransform.localScale.x * 0.344498f, BudTransform.localScale.y * 0.344498f, 1f);
		IvyTransform.localScale = new Vector3(IvyTransform.localScale.x * 0.344498f, IvyTransform.localScale.y * 0.344498f, 1f);
	}

	public void setScale()
	{
		BudTransform.localScale = new Vector3(94f, 89f, 1f);
		IvyTransform.localScale = Vector3.one;
	}

	public int getBudNum()
	{
		return budObjList_.Count;
	}

	public float getMoveDirection(float to_x)
	{
		return to_x - headBubblePos_.x;
	}

	public Vector3 getIvyPos()
	{
		return IvyTransform.localPosition;
	}

	public void move(float to_x)
	{
		StopAllCoroutines();
		StartCoroutine(moveInner(to_x));
	}

	private IEnumerator moveInner(float to_x)
	{
		setIvySe(true);
		if (ScrollTransform.GetComponent<iTween>() != null)
		{
			Object.Destroy(ScrollTransform.GetComponent<iTween>());
		}
		if (IvyTransform.GetComponent<iTween>() != null)
		{
			Object.Destroy(IvyTransform.GetComponent<iTween>());
		}
		toPosX = to_x;
		float run_time = 3f;
		float direct_x = Mathf.Abs(to_x - ScrollTransform.localPosition.x);
		run_time = direct_x / 60f * 0.3f;
		if (isBurn_)
		{
			run_time *= 0.33f;
		}
		iTween.MoveTo(ScrollTransform.gameObject, iTween.Hash("x", to_x, "easetype", iTween.EaseType.easeInSine, "time", run_time, "islocal", true));
		iTween.ScaleTo(IvyTransform.gameObject, iTween.Hash("y", 0.7f, "easetype", iTween.EaseType.easeInSine, "time", 0.3f, "loopType", "pingPong", "islocal", true));
		float s_time = 0f;
		while (s_time < run_time - 0.3f)
		{
			s_time += Time.deltaTime;
			DialogPause dialogPause = dialogManager_.getDialog(DialogManager.eDialog.Pause) as DialogPause;
			DialogStageShop dialogStageShop = dialogManager_.getDialog(DialogManager.eDialog.StageShop) as DialogStageShop;
			if (!dialogPause.isOpen() && !dialogPause.isOpen() && !dialogStageShop.isOpen())
			{
				setIvySe(true);
			}
			yield return stagePause_.sync();
		}
		Transform temp_leaf2 = null;
		if (isRemoved_)
		{
			temp_leaf2 = base.transform.Find("leaf");
			if (ivyType_ == eType.Left)
			{
				iTween.MoveTo(temp_leaf2.gameObject, iTween.Hash("x", -150, "easetype", iTween.EaseType.easeInCubic, "time", 0.8f, "islocal", true));
			}
			else
			{
				iTween.MoveTo(temp_leaf2.gameObject, iTween.Hash("x", 720, "easetype", iTween.EaseType.easeInCubic, "time", 0.8f, "islocal", true));
			}
			while (temp_leaf2 != null && temp_leaf2.GetComponent<iTween>() != null)
			{
				yield return stagePause_.sync();
			}
		}
		Object.Destroy(IvyTransform.GetComponent<iTween>());
		IvyTransform.localScale = Vector3.one;
		setIvySe(false);
		if (isRemoved_)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void setEnable(bool enable)
	{
		if (isRemoved_)
		{
			return;
		}
		if (!enable)
		{
			TweenColor[] componentsInChildren = base.gameObject.GetComponentsInChildren<TweenColor>();
			TweenColor[] array = componentsInChildren;
			foreach (TweenColor tweenColor in array)
			{
				tweenColor.enabled = true;
			}
			return;
		}
		TweenColor[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<TweenColor>();
		TweenColor[] array2 = componentsInChildren2;
		foreach (TweenColor tweenColor2 in array2)
		{
			tweenColor2.enabled = false;
		}
		UISprite[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<UISprite>();
		UISprite[] array3 = componentsInChildren3;
		foreach (UISprite uISprite in array3)
		{
			uISprite.color = Color.white;
		}
	}

	public void setPreMove()
	{
		preIsRemoved_ = isRemoved_;
		preLocalPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z);
		preHeadBubblePos = new Vector3(headBubblePos_.x, headBubblePos_.y, headBubblePos_.z);
		preToX = toPosX;
		preScrollObjLocalPosition = new Vector3(ScrollTransform.localPosition.x, ScrollTransform.localPosition.y, ScrollTransform.localPosition.z);
	}

	public void recoverPreMove()
	{
		StopAllCoroutines();
		setIvySe(false);
		if (ScrollTransform.GetComponent<iTween>() != null)
		{
			Object.Destroy(ScrollTransform.GetComponent<iTween>());
		}
		if (IvyTransform.GetComponent<iTween>() != null)
		{
			Object.Destroy(IvyTransform.GetComponent<iTween>());
			IvyTransform.localScale = Vector3.one;
		}
		isBurn_ = false;
		isRemoved_ = preIsRemoved_;
		base.transform.localPosition = preLocalPosition;
		headBubblePos_ = preHeadBubblePos;
		toPosX = preToX;
		ScrollTransform.localPosition = preScrollObjLocalPosition;
		if (ScrollTransform.localPosition.x != preToX)
		{
			ScrollTransform.localPosition = new Vector3(preToX, ScrollTransform.localPosition.y, ScrollTransform.localPosition.z);
		}
		base.gameObject.SetActive(false);
		if (!isRemoved_)
		{
			if (base.transform.Find("leaf").GetComponent<iTween>() != null)
			{
				Object.Destroy(base.transform.Find("leaf").GetComponent<iTween>());
			}
			if (ivyType_ == eType.Left)
			{
				base.transform.Find("leaf").localPosition = new Vector3(-50f, 0f, 0f);
			}
			else
			{
				base.transform.Find("leaf").localPosition = new Vector3(590f, 0f, 0f);
			}
			base.gameObject.SetActive(true);
		}
	}

	public void setIvyPos(float pos_x)
	{
		IvyTransform.localPosition = new Vector3(pos_x, IvyTransform.localPosition.y, IvyTransform.localPosition.z);
		if (ivyType_ == eType.Left)
		{
			base.transform.Find("leaf").localPosition = new Vector3(-50f, 0f, 0f);
		}
		else
		{
			base.transform.Find("leaf").localPosition = new Vector3(590f, 0f, 0f);
		}
	}

	public static void setIvySe(bool play)
	{
		if (play)
		{
			if (!Sound.Instance.isPlayingSe(Sound.eSe.SE_508_tsuta_nobiru))
			{
				Sound.Instance.playSe(Sound.eSe.SE_508_tsuta_nobiru, true);
			}
		}
		else if (Sound.Instance.isPlayingSe(Sound.eSe.SE_508_tsuta_nobiru))
		{
			Sound.Instance.stopSe(Sound.eSe.SE_508_tsuta_nobiru);
		}
	}

	public void setBurn()
	{
		isBurn_ = true;
	}

	public bool isBurn()
	{
		return isBurn_;
	}
}
