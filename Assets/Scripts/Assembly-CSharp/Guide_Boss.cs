using System.Collections.Generic;
using UnityEngine;

public class Guide_Boss : MonoBehaviour
{
	private const float GUIDE_LENGTH_DEFAULT = 300f;

	private const int GUIDE_REFLECT_COUNT = 8;

	private const float GUIDE_LENGTH_MAX = 5440f;

	private const float X_MIN = -270f;

	private const float X_MAX = 270f;

	private const float DEF_GUID_Z = -2.5f;

	private const float CloudCollOffsetMul = 1.5f;

	private const float CloudCollOffsetMetalMul = 2.6f;

	public Part_BossStage bossPart;

	public static float GUIDE_INTERVAL = 50f;

	public GameObject guideline_pos;

	private GameObject guideline;

	private Transform guidelineTrans;

	public bool isShootButton;

	private bool isSuperGuide;

	private UISprite[] lines = new UISprite[(int)(300f / GUIDE_INTERVAL)];

	private float scale;

	public BubbleBase hitBubble;

	public Vector3 hitDiff = Vector3.zero;

	public Vector3 hitPos = Vector3.zero;

	public float hitMoveLength;

	private List<Vector3> wallHitPosList_ = new List<Vector3>();

	private Transform bg_00;

	private Transform[] bg_01 = new Transform[9];

	private float x_min;

	private float x_max;

	private Transform baseTrans;

	private bool boundCeiling;

	private List<GameObject> nestList;

	public bool isNest;

	public bool isEgg;

	private GameObject egg;

	private Egg egg_;

	private List<Spiderweb> spwList;

	public bool isSpw;

	public GameObject hitSpw;

	public Vector3 shootBasePos = Vector3.zero;

	private List<ChainBubble> prechackChainList = new List<ChainBubble>();

	private List<Cloud> prechackCloudList = new List<Cloud>();

	private List<Bubble_Boss> prechackFieldList = new List<Bubble_Boss>();

	private int hitDiv = 10;

	private bool bActive_;

	private bool isMetal_;

	public List<Vector3> metalPath = new List<Vector3>();

	public Dictionary<Bubble_Boss, float> metalBreakBubbleDic = new Dictionary<Bubble_Boss, float>();

	public Dictionary<ChainBubble, float> metalBreakRockDic = new Dictionary<ChainBubble, float>();

	private bool isCeiling_;

	public bool bHitCeiling;

	private Vector3 ceilingPos_ = Vector3.zero;

	public bool inCloud { get; private set; }

	private void Start()
	{
		guidelineTrans = guideline_pos.transform.parent.parent.Find("guideline");
		lines[0] = guidelineTrans.GetChild(0).GetComponent<UISprite>();
		for (int i = 1; i < lines.Length; i++)
		{
			lines[i] = (Object.Instantiate(lines[0].gameObject) as GameObject).GetComponent<UISprite>();
			Utility.setParent(lines[i].gameObject, guidelineTrans, true);
			if (i % 2 != 0)
			{
				AnimationState animationState = lines[i].GetComponent<Animation>()[lines[i].GetComponent<Animation>().clip.name];
				animationState.time = animationState.length / 2f;
			}
		}
		guideline = guidelineTrans.gameObject;
		guidelineTrans.position = guideline_pos.transform.position;
		x_min = -270f - guidelineTrans.localPosition.x;
		x_max = 270f - guidelineTrans.localPosition.x;
		baseTrans = lines[0].transform;
		setActive(isShootButton);
	}

	public void setShootButton(bool bFlag)
	{
		isShootButton = bFlag;
		if (guideline != null)
		{
			if (isShootButton)
			{
				lineUpdate();
			}
			else
			{
				setActive(isShootButton);
			}
		}
	}

	public void lineUpdate()
	{
		if (isCeiling_)
		{
			lineUpdateCeiling();
			return;
		}
		metalUpdate();
		hitBubble = null;
		isNest = false;
		isEgg = false;
		isSpw = false;
		float num = Bubble.hitSize / (float)hitDiv;
		Vector3 vector = guideline_pos.transform.up * num;
		baseTrans.localPosition = shootBasePos;
		Vector3 nearDiff = Vector3.zero;
		int num2 = 0;
		while (hitBubble == null)
		{
			preCheckBubble();
			for (int i = 1; i <= hitDiv; i++)
			{
				Vector3 localPosition = baseTrans.localPosition + vector;
				if (localPosition.x < x_min || localPosition.x > x_max)
				{
					float num3 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
					localPosition = baseTrans.localPosition + vector * (1f - num3);
					vector.x = 0f - vector.x;
					localPosition += vector * num3;
				}
				baseTrans.localPosition = localPosition;
				if (isHitCeiling(baseTrans.position))
				{
					vector.y = 0f - vector.y;
				}
				if (baseTrans.localPosition.y < shootBasePos.y)
				{
					boundCeiling = true;
					hitPos = baseTrans.position;
					break;
				}
				boundCeiling = false;
				checkFieldBubble(ref nearDiff);
				if (hitBubble != null)
				{
					hitPos = baseTrans.position;
					break;
				}
				if (hitBubble == null && nestList != null && nestList.Count > 0 && isHitNest(ref nearDiff))
				{
					hitPos = baseTrans.position;
					break;
				}
				if (hitBubble == null && egg != null && egg_.isExist && isHitEgg(ref nearDiff))
				{
					hitPos = baseTrans.position;
					break;
				}
				if (hitBubble == null && spwList != null && spwList.Count > 0 && isHitSpw(ref nearDiff))
				{
					hitPos = baseTrans.position;
					break;
				}
				num2++;
			}
			if ((!isMetal_ && boundCeiling) || isNest || isEgg || isSpw)
			{
				break;
			}
		}
		hitDiff = nearDiff;
		hitMoveLength = (float)num2 * num;
		if (!bActive_)
		{
			setActive(true);
		}
		vector = guideline_pos.transform.up * GUIDE_INTERVAL;
		Vector3 vector2 = shootBasePos;
		if (isSuperGuide)
		{
			wallHitPosList_.Clear();
			if (isMetal_)
			{
				for (int j = 1; j < metalPath.Count; j++)
				{
					wallHitPosList_.Add(metalPath[j] / bossPart.uiScale + Vector3.up * 385f - shootBasePos);
				}
			}
			else
			{
				for (int k = 1; GUIDE_INTERVAL * (float)k <= hitMoveLength; k++)
				{
					Vector3 localPosition = vector2 + vector;
					if (localPosition.x < x_min || localPosition.x > x_max)
					{
						float num4 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
						localPosition = vector2 + vector * (1f - num4);
						wallHitPosList_.Add(localPosition - shootBasePos);
						vector.x = 0f - vector.x;
						localPosition += vector * num4;
					}
					vector2 = localPosition;
				}
				if (!wallHitPosList_.Contains(vector2 - shootBasePos))
				{
					wallHitPosList_.Add(vector2 - shootBasePos);
				}
			}
			Vector3 vector3 = Vector3.zero;
			for (int l = 0; l < bg_01.Length; l++)
			{
				if (wallHitPosList_.Count - 1 < l)
				{
					bg_01[l].gameObject.SetActive(false);
					continue;
				}
				bg_01[l].gameObject.SetActive(true);
				Vector3 vector4 = wallHitPosList_[l] - vector3;
				float z = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f - 90f;
				float num5 = vector4.magnitude;
				if (l == 0)
				{
					vector4.Normalize();
					vector3 = vector4 * 75f;
					num5 -= 75f;
				}
				if (wallHitPosList_.Count - 1 == l)
				{
					num5 -= 23f;
					if (num5 < 0f)
					{
						num5 = 0f;
					}
				}
				vector3.z = -2.5f;
				bg_01[l].localPosition = vector3;
				bg_01[l].localEulerAngles = new Vector3(0f, 0f, z);
				bg_01[l].localScale = new Vector3(bg_01[l].localScale.x, num5, 1f);
				bg_00.localPosition = wallHitPosList_[l];
				bg_00.localEulerAngles = new Vector3(0f, 0f, z);
				vector3 = wallHitPosList_[l];
				if (num5 < 15f)
				{
					bg_01[l].gameObject.SetActive(false);
				}
			}
			return;
		}
		lines[0].transform.localPosition = vector2;
		bool flag = false;
		int num6 = (int)(300f / GUIDE_INTERVAL);
		for (int m = 1; m < lines.Length; m++)
		{
			if (!flag && !isSuperGuide && m >= num6)
			{
				flag = true;
			}
			if (flag)
			{
				lines[m].enabled = false;
				continue;
			}
			Vector3 localPosition = vector2 + vector;
			if (localPosition.x < x_min || localPosition.x > x_max)
			{
				if (!isSuperGuide)
				{
					flag = true;
					lines[m].enabled = false;
					continue;
				}
				float num7 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
				localPosition = vector2 + vector * (1f - num7);
				vector.x = 0f - vector.x;
				localPosition += vector * num7;
			}
			vector2 = localPosition;
			vector2.z = -2.5f;
			lines[m].transform.localPosition = vector2;
			if (GUIDE_INTERVAL * (float)m > hitMoveLength)
			{
				flag = true;
			}
			lines[m].enabled = !flag;
		}
		for (int n = 0; n < 2; n++)
		{
			lines[n].enabled = false;
		}
	}

	private void preCheckBubble()
	{
		Vector3 zero = Vector3.zero;
		Vector3 position = baseTrans.position;
		prechackChainList.Clear();
		for (int i = 0; i < bossPart.chainBubbleDic.Count; i++)
		{
			foreach (ChainBubble item in bossPart.chainBubbleDic[i])
			{
				zero = (position - item.myTrans.position) / bossPart.uiScale;
				zero.z = 0f;
				if (!(zero.sqrMagnitude > 8100f))
				{
					prechackChainList.Add(item);
				}
			}
		}
		prechackFieldList.Clear();
		foreach (Bubble_Boss fieldBubble in bossPart.fieldBubbleList)
		{
			if (!fieldBubble.isLocked)
			{
				zero = (position - fieldBubble.myTrans.position) / bossPart.uiScale;
				zero.z = 0f;
				if (!(zero.sqrMagnitude > 8100f))
				{
					prechackFieldList.Add(fieldBubble);
				}
			}
		}
		nestList = bossPart.nestList;
		spwList = bossPart.spwList;
		if (bossPart.eggObj != null)
		{
			egg = bossPart.eggObj;
			egg_ = egg.GetComponent<Egg>();
		}
	}

	private void checkChainBubble(ref Vector3 nearDiff)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		foreach (ChainBubble prechackChain in prechackChainList)
		{
			zero = (baseTrans.position - prechackChain.myTrans.position) / bossPart.uiScale;
			zero.z = 0f;
			float sqrMagnitude = zero.sqrMagnitude;
			if (!(sqrMagnitude > Bubble.SQR_SIZE) && sqrMagnitude < num)
			{
				num = sqrMagnitude;
				nearDiff = zero;
				hitBubble = prechackChain;
			}
		}
	}

	private void setCloudMetal(Vector3 position, Cloud cloud, int priority)
	{
		inCloud = true;
		cloud.priority = priority;
		cloud.hitDiff = calcCloudDiff(position - cloud.transform.position);
	}

	private int calcCloudDiff(Vector3 diff)
	{
		float num;
		for (num = Mathf.Atan2(diff.y, diff.x) * 57.29578f + 90f; num < 0f; num += 360f)
		{
		}
		float num2 = 36.869896f;
		float num3 = 90f - num2;
		float num4 = 0f;
		num4 += num2;
		if (num < num4)
		{
			return 1;
		}
		num4 += num3 * 2f;
		if (num < num4)
		{
			return 4;
		}
		num4 += num2 * 2f;
		if (num < num4)
		{
			return 2;
		}
		num4 += num3 * 2f;
		if (num < num4)
		{
			return 3;
		}
		return 1;
	}

	private void checkFieldBubble(ref Vector3 nearDiff)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		foreach (Bubble_Boss prechackField in prechackFieldList)
		{
			zero = (baseTrans.position - prechackField.myTrans.position) / bossPart.uiScale;
			zero.z = 0f;
			float sqrMagnitude = zero.sqrMagnitude;
			if (!(sqrMagnitude > Bubble.SQR_SIZE) && sqrMagnitude < num)
			{
				num = sqrMagnitude;
				nearDiff = zero;
				hitBubble = prechackField;
			}
		}
	}

	public void setActive(bool bActive)
	{
		if (isSuperGuide)
		{
			UISprite[] array = lines;
			foreach (UISprite uISprite in array)
			{
				uISprite.enabled = false;
			}
		}
		else
		{
			UISprite[] array2 = lines;
			foreach (UISprite uISprite2 in array2)
			{
				uISprite2.enabled = bActive;
			}
		}
		bActive_ = bActive;
	}

	public void setMetal(bool isMetal)
	{
		if (isMetal_ != isMetal)
		{
			isMetal_ = isMetal;
			if (isShootButton)
			{
				lineUpdate();
			}
			else
			{
				metalUpdate();
			}
		}
	}

	private void metalUpdate()
	{
		if (isMetal_)
		{
			Vector3 position = guideline_pos.transform.position;
			Vector3 vector = guideline_pos.transform.up;
			metalBreakBubbleDic.Clear();
			metalBreakRockDic.Clear();
			metalPath.Clear();
			metalPath.Add(position);
			int i = 0;
			float radius = Bubble.hitSize * bossPart.uiScale * 0.5f;
			int layerMask = 1 << LayerMask.NameToLayer("StageCollider");
			int num = 0;
			inCloud = false;
			for (; i <= 2; i++)
			{
				RaycastHit hitInfo;
				if (!Physics.SphereCast(position, radius, vector, out hitInfo, float.PositiveInfinity, layerMask))
				{
					continue;
				}
				position += vector * hitInfo.distance;
				string text = hitInfo.transform.name.Replace("(Clone)", string.Empty);
				Vector3 inNormal = hitInfo.normal;
				switch (text)
				{
				case "Wall_L":
					inNormal = Vector3.right;
					break;
				case "Wall_R":
					inNormal = Vector3.left;
					break;
				case "ChainColliderHorizon":
					inNormal = Vector3.down;
					break;
				case "ChainColliderLeftDown":
					inNormal = Quaternion.Euler(0f, 0f, 240f) * Vector3.up;
					break;
				case "ChainColliderRightDown":
					inNormal = Quaternion.Euler(0f, 0f, 300f) * Vector3.down;
					break;
				}
				if (text == "CloudCollider")
				{
					float num2 = 0f;
					Transform transform = hitInfo.transform;
					if (transform.position.x < position.x)
					{
						Vector3 a = new Vector3(transform.position.x + transform.lossyScale.x / 2f, transform.position.y, transform.position.z);
						num2 = Vector3.Distance(a, position);
						inNormal = Vector3.right;
					}
					else
					{
						Vector3 a2 = new Vector3(transform.position.x - transform.lossyScale.x / 2f, transform.position.y, transform.position.z);
						num2 = Vector3.Distance(a2, position);
						inNormal = Vector3.left;
					}
					if (transform.position.y < position.y)
					{
						Vector3 a3 = new Vector3(transform.position.x, transform.position.y + transform.lossyScale.y / 2f, transform.position.z);
						if (num2 > Vector3.Distance(a3, position))
						{
							inNormal = Vector3.up;
						}
					}
					else
					{
						Vector3 a4 = new Vector3(transform.position.x, transform.position.y - transform.lossyScale.y / 2f, transform.position.z);
						if (num2 > Vector3.Distance(a4, position))
						{
							inNormal = Vector3.down;
						}
					}
					inCloud = true;
					setCloudMetal(position, hitInfo.transform.parent.GetComponent<Cloud>(), num);
					num++;
				}
				vector = Vector3.Reflect(vector, inNormal);
				metalPath.Add(position);
				if (text == "Floor" || text == "Ceiling")
				{
					break;
				}
			}
			List<Bubble_Boss> list = new List<Bubble_Boss>();
			float y = bossPart.uiRoot.transform.Find("StageCollider(Clone)/Ceiling").position.y;
			foreach (Bubble_Boss fieldBubble in bossPart.fieldBubbleList)
			{
				if (!fieldBubble.isLocked && !fieldBubble.inCloud && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && !(fieldBubble.myTrans.position.y > y) && fieldBubble.type != Bubble.eType.RotateFulcrumL && fieldBubble.type != Bubble.eType.RotateFulcrumR)
				{
					list.Add(fieldBubble);
				}
			}
			int num3 = 0;
			float num4 = 0f;
			float magnitude = (metalPath[1] - metalPath[0]).magnitude;
			position = metalPath[0];
			float num5 = Bubble.hitSize / (float)hitDiv;
			float num6 = 0f;
			while (true)
			{
				num4 += num5 * bossPart.uiScale;
				num6 += num5 * bossPart.uiScale;
				float num7 = num4 / magnitude;
				if (num7 >= 1f)
				{
					num3++;
					if (num3 >= metalPath.Count - 1)
					{
						break;
					}
					num4 -= magnitude;
					magnitude = (metalPath[num3 + 1] - metalPath[num3]).magnitude;
					num7 = num4 / magnitude;
				}
				position = Vector3.Lerp(metalPath[num3], metalPath[num3 + 1], num7);
				foreach (Bubble_Boss item in list)
				{
					if (!metalBreakBubbleDic.ContainsKey(item))
					{
						Vector3 vector2 = (position - item.myTrans.position) / bossPart.uiScale;
						vector2.z = 0f;
						if (!(vector2.sqrMagnitude > Bubble.SQR_SIZE))
						{
							metalBreakBubbleDic.Add(item, num6);
						}
					}
				}
				for (int j = 0; j < bossPart.chainBubbleDic.Count; j++)
				{
					foreach (ChainBubble item2 in bossPart.chainBubbleDic[j])
					{
						if (!metalBreakRockDic.ContainsKey(item2))
						{
							Vector3 vector2 = (position - item2.myTrans.position) / bossPart.uiScale;
							vector2.z = 0f;
							if (!(vector2.sqrMagnitude > Bubble.SQR_LARGE_SIZE))
							{
								metalBreakRockDic.Add(item2, num6);
							}
						}
					}
				}
			}
			position = metalPath[num3];
		}
		else
		{
			metalPath.Clear();
			metalBreakBubbleDic.Clear();
		}
	}

	public void setCeilingPos(Vector3 ceilingPos)
	{
		ceilingPos_ = ceilingPos;
	}

	public void setCeiling(bool isCeiling)
	{
		if (isCeiling_ != isCeiling)
		{
			isCeiling_ = isCeiling;
			if (isShootButton)
			{
				lineUpdate();
			}
		}
	}

	public void lineUpdateCeiling()
	{
		hitBubble = null;
		isNest = false;
		isEgg = false;
		isSpw = false;
		float num = Bubble.hitSize / (float)hitDiv;
		Vector3 vector = guideline_pos.transform.up * num;
		baseTrans.localPosition = shootBasePos;
		Vector3 nearDiff = Vector3.zero;
		int num2 = 0;
		bool flag = true;
		while (flag)
		{
			preCheckBubble();
			for (int i = 1; i <= hitDiv; i++)
			{
				Vector3 localPosition = baseTrans.localPosition + vector;
				if (localPosition.x < x_min || localPosition.x > x_max)
				{
					float num3 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
					localPosition = baseTrans.localPosition + vector * (1f - num3);
					vector.x = 0f - vector.x;
					localPosition += vector * num3;
				}
				baseTrans.localPosition = localPosition;
				if (isHitCeiling(baseTrans.position))
				{
					flag = false;
					bHitCeiling = true;
					hitPos = baseTrans.position;
					break;
				}
				checkFieldBubble(ref nearDiff);
				if (hitBubble != null)
				{
					flag = false;
					bHitCeiling = false;
					hitPos = baseTrans.position;
					break;
				}
				if (hitBubble == null && nestList != null && nestList.Count > 0 && isHitNest(ref nearDiff))
				{
					hitPos = baseTrans.position;
					break;
				}
				if (hitBubble == null && egg != null && egg_.isExist && isHitEgg(ref nearDiff))
				{
					hitPos = baseTrans.position;
					break;
				}
				if (hitBubble == null && spwList != null && spwList.Count > 0 && isHitSpw(ref nearDiff))
				{
					hitPos = baseTrans.position;
					break;
				}
				num2++;
			}
			if (isNest || isEgg)
			{
				break;
			}
		}
		hitDiff = nearDiff;
		hitMoveLength = (float)num2 * num;
		isSuperGuide = bossPart.isUsedItem(Constant.Item.eType.SuperGuide);
		if (!bActive_)
		{
			setActive(true);
		}
		vector = guideline_pos.transform.up * GUIDE_INTERVAL;
		Vector3 vector2 = shootBasePos;
		if (isSuperGuide)
		{
			wallHitPosList_.Clear();
			if (isMetal_)
			{
				for (int j = 1; j < metalPath.Count; j++)
				{
					wallHitPosList_.Add(metalPath[j] / bossPart.uiScale + Vector3.up * 385f - shootBasePos);
				}
			}
			else
			{
				for (int k = 1; GUIDE_INTERVAL * (float)k <= hitMoveLength; k++)
				{
					Vector3 localPosition = vector2 + vector;
					if (localPosition.x < x_min || localPosition.x > x_max)
					{
						float num4 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
						localPosition = vector2 + vector * (1f - num4);
						wallHitPosList_.Add(localPosition - shootBasePos);
						vector.x = 0f - vector.x;
						localPosition += vector * num4;
					}
					vector2 = localPosition;
				}
				if (!wallHitPosList_.Contains(vector2 - shootBasePos))
				{
					wallHitPosList_.Add(vector2 - shootBasePos);
				}
			}
			Vector3 vector3 = Vector3.zero;
			for (int l = 0; l < bg_01.Length; l++)
			{
				if (wallHitPosList_.Count - 1 < l)
				{
					bg_01[l].gameObject.SetActive(false);
					continue;
				}
				bg_01[l].gameObject.SetActive(true);
				Vector3 vector4 = wallHitPosList_[l] - vector3;
				float z = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f - 90f;
				float num5 = vector4.magnitude;
				if (l == 0)
				{
					vector4.Normalize();
					vector3 = vector4 * 75f;
					num5 -= 75f;
				}
				if (wallHitPosList_.Count - 1 == l)
				{
					num5 -= 23f;
					if (num5 < 0f)
					{
						num5 = 0f;
					}
				}
				vector3.z = -2.5f;
				bg_01[l].localPosition = vector3;
				bg_01[l].localEulerAngles = new Vector3(0f, 0f, z);
				bg_01[l].localScale = new Vector3(bg_01[l].localScale.x, num5, 1f);
				bg_00.localPosition = wallHitPosList_[l];
				bg_00.localEulerAngles = new Vector3(0f, 0f, z);
				vector3 = wallHitPosList_[l];
				if (num5 < 15f)
				{
					bg_01[l].gameObject.SetActive(false);
				}
			}
			return;
		}
		lines[0].transform.localPosition = vector2;
		bool flag2 = false;
		int num6 = (int)(300f / GUIDE_INTERVAL);
		for (int m = 1; m < lines.Length; m++)
		{
			if (!flag2 && !isSuperGuide && m >= num6)
			{
				flag2 = true;
			}
			if (flag2)
			{
				lines[m].enabled = false;
				continue;
			}
			Vector3 localPosition = vector2 + vector;
			if (localPosition.x < x_min || localPosition.x > x_max)
			{
				if (!isSuperGuide)
				{
					flag2 = true;
					lines[m].enabled = false;
					continue;
				}
				float num7 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
				localPosition = vector2 + vector * (1f - num7);
				vector.x = 0f - vector.x;
				localPosition += vector * num7;
			}
			vector2 = localPosition;
			vector2.z = -2.5f;
			lines[m].transform.localPosition = vector2;
			if (GUIDE_INTERVAL * (float)m > hitMoveLength)
			{
				flag2 = true;
			}
			lines[m].enabled = !flag2;
		}
		for (int n = 0; n < 2; n++)
		{
			lines[n].enabled = false;
		}
	}

	public bool isHitEgg(ref Vector3 nearDiff)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 3f;
		zero = (baseTrans.position - egg.transform.position) / bossPart.uiScale;
		zero.z = 0f;
		float sqrMagnitude = zero.sqrMagnitude;
		if (sqrMagnitude < num)
		{
			num = sqrMagnitude;
			nearDiff = zero;
			hitBubble = null;
			isEgg = true;
			return true;
		}
		return false;
	}

	public bool isHitNest(ref Vector3 nearDiff)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		foreach (GameObject nest in nestList)
		{
			zero = (baseTrans.position - nest.transform.position) / bossPart.uiScale;
			zero.z = 0f;
			float sqrMagnitude = zero.sqrMagnitude;
			if (sqrMagnitude > Bubble.SQR_SIZE || !(sqrMagnitude < num))
			{
				continue;
			}
			num = sqrMagnitude;
			nearDiff = zero;
			hitBubble = null;
			isNest = true;
			return true;
		}
		return false;
	}

	public bool isHitSpw(ref Vector3 nearDiff)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		foreach (Spiderweb spw in spwList)
		{
			if (spw.gameObject.activeSelf)
			{
				zero = (baseTrans.position - spw.transform.position) / bossPart.uiScale;
				zero.z = 0f;
				float sqrMagnitude = zero.sqrMagnitude;
				if (!(sqrMagnitude > Bubble.SQR_SIZE) && sqrMagnitude < num)
				{
					num = sqrMagnitude;
					nearDiff = zero;
					hitBubble = null;
					isSpw = true;
					hitSpw = spw.gameObject;
					return true;
				}
			}
		}
		return false;
	}

	public BubbleBase setRandomRoot(Vector3 fire_vec, Vector3 shotpos)
	{
		float num = Bubble.hitSize / (float)hitDiv;
		Vector3 vector = fire_vec * num;
		baseTrans.position = shotpos;
		Vector3 nearDiff = Vector3.zero;
		int num2 = 0;
		hitBubble = null;
		while (hitBubble == null)
		{
			preCheckBubble();
			for (int i = 1; i <= hitDiv; i++)
			{
				Vector3 localPosition = baseTrans.localPosition + vector;
				if (localPosition.x < x_min || localPosition.x > x_max)
				{
					float num3 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
					localPosition = baseTrans.localPosition + vector * (1f - num3);
					vector.x = 0f - vector.x;
					localPosition += vector * num3;
				}
				baseTrans.localPosition = localPosition;
				checkChainBubble(ref nearDiff);
				if (hitBubble != null)
				{
					hitPos = baseTrans.position;
					break;
				}
				checkFieldBubble(ref nearDiff);
				if (hitBubble != null)
				{
					hitPos = baseTrans.position;
					break;
				}
				num2++;
			}
			if (inCloud)
			{
				break;
			}
		}
		hitDiff = nearDiff;
		hitMoveLength = (float)num2 * num;
		return hitBubble;
	}

	public bool isHitCeiling(Vector3 point)
	{
		return point.y > ceilingPos_.y;
	}

	public Vector3 getCeilingPosition()
	{
		return ceilingPos_;
	}
}
