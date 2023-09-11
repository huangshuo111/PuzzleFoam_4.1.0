using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
	private const float GUIDE_LENGTH_DEFAULT = 300f;

	private const int GUIDE_REFLECT_COUNT = 8;

	private const float GUIDE_LENGTH_MAX = 5440f;

	private const float X_MIN = -270f;

	private const float X_MAX = 270f;

	private const float DEF_GUID_Z = -11.1f;

	private const float GUIDLINE_Z = -1.5f;

	private const float CloudCollOffsetMul = 1.5f;

	private const float CloudCollOffsetMetalMul = 2.6f;

	public Part_Stage part;

	public Part_BonusStage bonusPart;

	public Part_RankingStage rankingPart;

	public float uiScale;

	public float GUIDE_INTERVAL = 50f;

	public GameObject guideline_pos;

	private GameObject guideline;

	private Transform guidelineTrans;

	public bool isShootButton;

	private bool isSuperGuide;

	private UISprite[] lines;

	private float scale;

	public BubbleBase hitBubble;

	public BubbleBase tunnelInBubble;

	public Bubble tunnelInBubbleMetal;

	public Vector3 hitDiff = Vector3.zero;

	public Vector3 hitPos = Vector3.zero;

	public float hitMoveLength;

	public float hitMoveLocal;

	private GameObject guideline_supergurd;

	private List<Vector3> wallHitPosList_ = new List<Vector3>();

	private Transform bg_00;

	private Transform[] bg_01 = new Transform[9];

	private float x_min;

	private float x_max;

	private Transform baseTrans;

	public Vector3 shootBasePos = Vector3.zero;

	private float guideStretchMultiple = 1f;

	private List<ChainBubble> prechackChainList = new List<ChainBubble>();

	private List<Bubble> prechackFieldList = new List<Bubble>();

	private List<Bubble> prechackInObjectList = new List<Bubble>();

	private int hitDiv = 10;

	private bool bActive_;

	private bool isMetal_;

	public Bubble counterbalanceBubble;

	public List<Vector3> metalPath = new List<Vector3>();

	public Dictionary<Bubble, float> metalBreakBubbleDic = new Dictionary<Bubble, float>();

	public Dictionary<ChainBubble, float> metalBreakRockDic = new Dictionary<ChainBubble, float>();

	public int metalPathIndex = -1;

	private bool isCeiling_;

	public bool bHitCeiling;

	private Vector3 ceilingPos_ = Vector3.zero;

	public bool inCloud { get; private set; }

	private void Start()
	{
		lines = new UISprite[(int)(300f / GUIDE_INTERVAL)];
		guidelineTrans = guideline_pos.transform.parent.parent.Find("guideline");
		if (part != null && part.bGuideStretch)
		{
			guideStretchMultiple = part.GuideStretchMultiple + 1.5f;
			lines = new UISprite[(int)(guideStretchMultiple * 2f)];
			guidelineTrans.GetChild(0).GetComponent<UISprite>().spriteName = "guide_red";
			GUIDE_INTERVAL /= 2f;
		}
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
		if (part != null)
		{
			guideline_supergurd = guideline_pos.transform.parent.parent.Find("guideline_supergurd").gameObject;
			bg_01[0] = guideline_supergurd.transform.Find("bg_01");
			for (int j = 1; j < bg_01.Length; j++)
			{
				bg_01[j] = (Object.Instantiate(bg_01[0].gameObject) as GameObject).transform;
				Utility.setParent(bg_01[j].gameObject, bg_01[0].parent, false);
			}
			bg_00 = guideline_supergurd.transform.Find("bg_00");
			if (part != null)
			{
				isSuperGuide = part.isUsedItem(Constant.Item.eType.SuperGuide);
			}
		}
		else if (rankingPart != null)
		{
			guideline_supergurd = guideline_pos.transform.parent.parent.Find("guideline_supergurd").gameObject;
			bg_01[0] = guideline_supergurd.transform.Find("bg_01");
			for (int k = 1; k < bg_01.Length; k++)
			{
				bg_01[k] = (Object.Instantiate(bg_01[0].gameObject) as GameObject).transform;
				Utility.setParent(bg_01[k].gameObject, bg_01[0].parent, false);
			}
			bg_00 = guideline_supergurd.transform.Find("bg_00");
			if (rankingPart != null)
			{
				isSuperGuide = rankingPart.isUsedItem(Constant.Item.eType.SuperGuide);
			}
		}
		guidelineTrans.position = guideline_pos.transform.position;
		if (part != null || rankingPart != null)
		{
			guideline_supergurd.transform.position = guidelineTrans.position;
		}
		x_min = -270f - guidelineTrans.localPosition.x;
		x_max = 270f - guidelineTrans.localPosition.x;
		baseTrans = lines[0].transform;
		guidelineTrans.localPosition = new Vector3(guidelineTrans.localPosition.x, guidelineTrans.localPosition.y, -1.5f);
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
		tunnelInBubble = null;
		float num = Bubble.hitSize / (float)hitDiv;
		Vector3 vector = guideline_pos.transform.up * num;
		baseTrans.localPosition = shootBasePos;
		Vector3 zero = Vector3.zero;
		float num2 = Bubble.SQR_SIZE * 2f;
		Vector3 nearDiff = Vector3.zero;
		Vector3 nearDiff2 = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		int num3 = 0;
		int num4 = 0;
		while (hitBubble == null)
		{
			preCheckBubble();
			for (int i = 1; i <= hitDiv; i++)
			{
				Vector3 localPosition = baseTrans.localPosition + vector;
				if (localPosition.x < x_min || localPosition.x > x_max)
				{
					float num5 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
					localPosition = baseTrans.localPosition + vector * (1f - num5);
					vector.x = 0f - vector.x;
					localPosition += vector * num5;
				}
				baseTrans.localPosition = localPosition;
				if (part != null)
				{
					if (!isMetal_)
					{
						checkCloud(ref nearDiff2, baseTrans.position);
						if (inCloud)
						{
							hitPos = baseTrans.position;
							break;
						}
					}
					checkChainBubble(ref nearDiff);
					if (tunnelInBubble == null && !isMetal_)
					{
						checkTunnel(ref nearDiff, baseTrans.position);
					}
					if (hitBubble != null)
					{
						if (hitBubble.name.IndexOf("120") < 0 || tunnelInBubble != null)
						{
							hitPos = baseTrans.position;
							break;
						}
						if (hitBubble.GetComponent<Bubble>().OutObject != null)
						{
							baseTrans.position = hitBubble.GetComponent<Bubble>().OutObject.transform.position + hitBubble.GetComponent<Bubble>().OutObject.transform.Find("AS_spr_bubble").transform.up * Bubble.mHitSize * uiScale;
							vector = hitBubble.GetComponent<Bubble>().OutObject.transform.Find("AS_spr_bubble").transform.up * num;
							num3++;
							num4++;
							tunnelInBubble = hitBubble;
							hitBubble = null;
						}
						continue;
					}
				}
				checkFieldBubble(ref nearDiff);
				if (hitBubble != null)
				{
					hitPos = baseTrans.position;
					break;
				}
				if (baseTrans.localPosition.y <= 0f)
				{
					hitPos = baseTrans.position;
					break;
				}
				if (tunnelInBubble == null)
				{
					num4++;
				}
				num3++;
			}
			if ((!isMetal_ && inCloud) || baseTrans.localPosition.y <= 0f)
			{
				break;
			}
		}
		hitDiff = nearDiff;
		hitMoveLocal = (float)num4 * num;
		hitMoveLength = (float)num3 * num;
		if (part != null)
		{
			isSuperGuide = part.isUsedItem(Constant.Item.eType.SuperGuide);
		}
		else if (rankingPart != null)
		{
			isSuperGuide = rankingPart.isUsedItem(Constant.Item.eType.SuperGuide);
		}
		if (!bActive_)
		{
			setActive(true);
		}
		vector = guideline_pos.transform.up * GUIDE_INTERVAL;
		Vector3 vector2 = shootBasePos;
		if (isSuperGuide)
		{
			guideline_supergurd.transform.localPosition = shootBasePos;
			wallHitPosList_.Clear();
			if (isMetal_)
			{
				for (int j = 1; j < metalPath.Count; j++)
				{
					wallHitPosList_.Add(metalPath[j] / uiScale + Vector3.up * 385f - shootBasePos);
					if (metalPathIndex != -1 && metalPathIndex == j)
					{
						break;
					}
				}
			}
			else
			{
				for (int k = 1; GUIDE_INTERVAL * (float)k <= hitMoveLocal; k++)
				{
					Vector3 localPosition = vector2 + vector;
					if (localPosition.x < x_min || localPosition.x > x_max)
					{
						float num6 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
						localPosition = vector2 + vector * (1f - num6);
						wallHitPosList_.Add(localPosition - shootBasePos);
						vector.x = 0f - vector.x;
						localPosition += vector * num6;
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
				float num7 = vector4.magnitude;
				if (l == 0)
				{
					vector4.Normalize();
					vector3 = vector4 * 75f;
					num7 -= 75f;
				}
				if (wallHitPosList_.Count - 1 == l)
				{
					num7 -= 23f;
					if (num7 < 0f)
					{
						num7 = 0f;
					}
				}
				vector3.z = -11.1f;
				bg_01[l].localPosition = vector3;
				bg_01[l].localEulerAngles = new Vector3(0f, 0f, z);
				bg_01[l].localScale = new Vector3(bg_01[l].localScale.x, num7, 1f);
				bg_00.localPosition = wallHitPosList_[l];
				bg_00.localEulerAngles = new Vector3(0f, 0f, z);
				vector3 = wallHitPosList_[l];
				if (num7 < 15f)
				{
					bg_01[l].gameObject.SetActive(false);
				}
			}
			return;
		}
		lines[0].transform.localPosition = vector2;
		bool flag = false;
		int num8 = (int)(300f / GUIDE_INTERVAL);
		if (part != null && part.bGuideStretch)
		{
			num8 = (int)(guideStretchMultiple * 2f);
		}
		for (int m = 1; m < lines.Length; m++)
		{
			if (!flag && !isSuperGuide && m >= num8)
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
				if ((part == null && !isSuperGuide) || (part != null && !part.bGuideStretch && !isSuperGuide))
				{
					flag = true;
					lines[m].enabled = false;
					continue;
				}
				float num9 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
				localPosition = vector2 + vector * (1f - num9);
				vector.x = 0f - vector.x;
				localPosition += vector * num9;
			}
			vector2 = localPosition;
			vector2.z = -11.1f;
			lines[m].transform.localPosition = vector2;
			if (isMetal_)
			{
				if (GUIDE_INTERVAL * (float)m > hitMoveLength)
				{
					flag = true;
				}
			}
			else if (GUIDE_INTERVAL * (float)m > hitMoveLocal)
			{
				flag = true;
			}
			lines[m].enabled = !flag;
		}
		for (int n = 0; n < 2; n++)
		{
			lines[n].enabled = false;
		}
		if (part != null && part.bGuideStretch)
		{
			lines[2].enabled = false;
		}
	}

	private void preCheckBubble()
	{
		Vector3 zero = Vector3.zero;
		Vector3 position = baseTrans.position;
		prechackChainList.Clear();
		if (part != null)
		{
			for (int i = 0; i < part.chainBubbleDic.Count; i++)
			{
				foreach (ChainBubble item in part.chainBubbleDic[i])
				{
					zero = (position - item.myTrans.position) / uiScale;
					zero.z = 0f;
					if (!(zero.sqrMagnitude > 8100f))
					{
						prechackChainList.Add(item);
					}
				}
			}
		}
		prechackFieldList.Clear();
		prechackInObjectList.Clear();
		if (part != null)
		{
			foreach (Bubble fieldBubble in part.fieldBubbleList)
			{
				if (!fieldBubble.isLocked)
				{
					zero = (position - fieldBubble.myTrans.position) / uiScale;
					zero.z = 0f;
					if (!(zero.sqrMagnitude > 8100f))
					{
						prechackFieldList.Add(fieldBubble);
					}
				}
			}
			return;
		}
		if (bonusPart != null)
		{
			foreach (Bubble fieldBubble2 in bonusPart.fieldBubbleList)
			{
				if (!fieldBubble2.isLocked)
				{
					zero = (position - fieldBubble2.myTrans.position) / uiScale;
					zero.z = 0f;
					if (!(zero.sqrMagnitude > 8100f))
					{
						prechackFieldList.Add(fieldBubble2);
					}
				}
			}
			return;
		}
		if (!(rankingPart != null))
		{
			return;
		}
		foreach (Bubble fieldBubble3 in rankingPart.fieldBubbleList)
		{
			if (!fieldBubble3.isLocked)
			{
				zero = (position - fieldBubble3.myTrans.position) / uiScale;
				zero.z = 0f;
				if (!(zero.sqrMagnitude > 8100f))
				{
					prechackFieldList.Add(fieldBubble3);
				}
			}
		}
	}

	private void checkChainBubble(ref Vector3 nearDiff)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		foreach (ChainBubble prechackChain in prechackChainList)
		{
			zero = (baseTrans.position - prechackChain.myTrans.position) / uiScale;
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

	private void checkTunnel(ref Vector3 nearDiff, Vector3 temppos)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		foreach (Bubble prechackField in prechackFieldList)
		{
			zero = (baseTrans.position - prechackField.myTrans.position) / uiScale;
			zero.z = 0f;
			float sqrMagnitude = zero.sqrMagnitude;
			if (!(sqrMagnitude > Bubble.SQR_SIZE) && sqrMagnitude < num)
			{
				num = sqrMagnitude;
				nearDiff = zero;
				hitBubble = prechackField;
			}
		}
		nearDiff = nearDiff;
	}

	private void checkTunnel(ref Vector3 nearDiff)
	{
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		foreach (Bubble prechackField in prechackFieldList)
		{
			if (prechackField.GetComponent<Bubble>().OutObject != null)
			{
				hitBubble = prechackField;
			}
		}
	}

	private void checkCloud(ref Vector3 nearDiff, Vector3 position)
	{
		if (isMetal_ || part == null)
		{
			return;
		}
		inCloud = false;
		Vector3 zero = Vector3.zero;
		float num = Bubble.SQR_SIZE * 2f;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		foreach (Cloud cloud in part.cloudList)
		{
			cloud.hitDiff = 0;
			float num2 = (cloud.cloudMaxY - cloud.cloudMinY) / 4f * uiScale;
			float num3 = (cloud.cloudMaxX - cloud.cloudMinX) / 3f * uiScale;
			zero2 = cloud.transform.position;
			float num4 = zero2.x - num3;
			float num5 = zero2.y - num2 * 1.5f;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					zero3.x = num4 + (float)j * num3;
					zero3.y = num5 + (float)i * num2;
					zero = (position - zero3) / uiScale;
					zero.z = 0f;
					float sqrMagnitude = zero.sqrMagnitude;
					if (!(sqrMagnitude > Bubble.SQR_SIZE * 1.5f) && sqrMagnitude < num)
					{
						num = sqrMagnitude;
						nearDiff = zero;
						inCloud = true;
						break;
					}
				}
			}
			if (inCloud)
			{
				cloud.hitDiff = calcCloudDiff(position - zero2);
				break;
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
		foreach (Bubble prechackField in prechackFieldList)
		{
			zero = (baseTrans.position - prechackField.myTrans.position) / uiScale;
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

	public bool isActive()
	{
		return bActive_;
	}

	public void setActive(bool bActive)
	{
		if (part != null || bonusPart != null || rankingPart != null)
		{
			if (isSuperGuide)
			{
				if (part != null || rankingPart != null)
				{
					guideline_supergurd.SetActive(bActive);
				}
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
				if (part != null || rankingPart != null)
				{
					guideline_supergurd.SetActive(false);
				}
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
			if ((bool)part)
			{
				part.clearRotateFulcrumMetal();
			}
			if (part != null && part.state == Part_Stage.eState.Wait)
			{
				foreach (Cloud cloud in part.cloudList)
				{
					cloud.priority = -1;
					cloud.hitDiff = 0;
				}
			}
			Vector3 vector = guideline_pos.transform.position;
			Vector3 vector2 = guideline_pos.transform.up;
			metalBreakBubbleDic.Clear();
			metalBreakRockDic.Clear();
			metalPath.Clear();
			metalPath.Add(vector);
			int i = 0;
			int num = 0;
			float radius = Bubble.hitSize * uiScale * 0.5f;
			int layerMask = 1 << LayerMask.NameToLayer("StageCollider");
			int num2 = 0;
			inCloud = false;
			bool flag = false;
			metalPathIndex = -1;
			int[] array = new int[5];
			int num3 = -1;
			for (; i - num <= 2; i++)
			{
				RaycastHit hitInfo;
				if (!Physics.SphereCast(vector, radius, vector2, out hitInfo, float.PositiveInfinity, layerMask))
				{
					continue;
				}
				vector += vector2 * hitInfo.distance;
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
					float num4 = 0f;
					Transform transform = hitInfo.transform;
					if (transform.position.x < vector.x)
					{
						Vector3 a = new Vector3(transform.position.x + transform.lossyScale.x / 2f, transform.position.y, transform.position.z);
						num4 = Vector3.Distance(a, vector);
						inNormal = Vector3.right;
					}
					else
					{
						Vector3 a2 = new Vector3(transform.position.x - transform.lossyScale.x / 2f, transform.position.y, transform.position.z);
						num4 = Vector3.Distance(a2, vector);
						inNormal = Vector3.left;
					}
					if (transform.position.y < vector.y)
					{
						Vector3 a3 = new Vector3(transform.position.x, transform.position.y + transform.lossyScale.y / 2f, transform.position.z);
						if (num4 > Vector3.Distance(a3, vector))
						{
							inNormal = Vector3.up;
						}
					}
					else
					{
						Vector3 a4 = new Vector3(transform.position.x, transform.position.y - transform.lossyScale.y / 2f, transform.position.z);
						if (num4 > Vector3.Distance(a4, vector))
						{
							inNormal = Vector3.down;
						}
					}
					inCloud = true;
					setCloudMetal(vector, hitInfo.transform.parent.GetComponent<Cloud>(), num2);
					num2++;
				}
				if (text == "warpInCollider" && metalPathIndex <= -1)
				{
					num++;
					if (hitInfo.transform.parent.gameObject.GetComponent<Bubble>().OutObject != null)
					{
						Vector3 vector3 = hitInfo.transform.parent.gameObject.GetComponent<Bubble>().OutObject.transform.position + hitInfo.transform.parent.gameObject.GetComponent<Bubble>().OutObject.transform.Find("AS_spr_bubble").transform.up * Bubble.mHitSize;
						metalPath.Add(vector);
						vector = hitInfo.transform.parent.gameObject.GetComponent<Bubble>().OutObject.transform.position + hitInfo.transform.parent.gameObject.GetComponent<Bubble>().OutObject.transform.Find("AS_spr_bubble").transform.up * -1f * (Bubble.mHitSize / 3f * uiScale);
						tunnelInBubbleMetal = hitInfo.transform.parent.gameObject.GetComponent<Bubble>();
						flag = true;
						if (metalPathIndex == -1)
						{
							metalPathIndex = i + 1;
						}
						if (num3 + 1 < array.Length)
						{
							array[++num3] = i + 1;
						}
					}
				}
				else if (text == "FulcrumCollider")
				{
					Bubble component = hitInfo.transform.parent.gameObject.GetComponent<Bubble>();
					if (component != null && (component.type == Bubble.eType.RotateFulcrumL || component.type == Bubble.eType.RotateFulcrumR) && (bool)part)
					{
						part.setRotateFulcrumMetal(component);
					}
				}
				if (!flag)
				{
					vector2 = Vector3.Reflect(vector2, inNormal);
				}
				else
				{
					vector2 = hitInfo.transform.parent.gameObject.GetComponent<Bubble>().OutObject.transform.Find("AS_spr_bubble").transform.up;
					flag = false;
				}
				metalPath.Add(vector);
				if (text == "MorganaCollider")
				{
					counterbalanceBubble = hitInfo.transform.parent.GetComponent<Bubble>();
				}
				else
				{
					counterbalanceBubble = null;
				}
				switch (text)
				{
				default:
					continue;
				case "Floor":
				case "Ceiling":
				case "MorganaCollider":
					break;
				}
				break;
			}
			List<Bubble> list = new List<Bubble>();
			float num5 = 1f;
			if (part != null)
			{
				num5 = part.uiRoot.transform.Find("StageCollider(Clone)/Ceiling").position.y;
			}
			else if (rankingPart != null)
			{
				num5 = rankingPart.uiRoot.transform.Find("StageCollider(Clone)/Ceiling").position.y;
			}
			if (part != null)
			{
				foreach (Bubble fieldBubble in part.fieldBubbleList)
				{
					if (!fieldBubble.isLocked && !fieldBubble.inCloud && fieldBubble.type != Bubble.eType.Blank && fieldBubble.type != Bubble.eType.Fulcrum && (fieldBubble.type < Bubble.eType.TunnelIn || fieldBubble.type > Bubble.eType.TunnelOutRightDown) && !(fieldBubble.myTrans.position.y > num5))
					{
						list.Add(fieldBubble);
					}
				}
			}
			else if (rankingPart != null)
			{
				foreach (Bubble fieldBubble2 in rankingPart.fieldBubbleList)
				{
					if (!fieldBubble2.isLocked && !fieldBubble2.inCloud && fieldBubble2.type != Bubble.eType.Blank && fieldBubble2.type != Bubble.eType.Fulcrum && fieldBubble2.type != Bubble.eType.RotateFulcrumL && fieldBubble2.type != Bubble.eType.RotateFulcrumR && !(fieldBubble2.myTrans.position.y > num5))
					{
						list.Add(fieldBubble2);
					}
				}
			}
			int num6 = 0;
			float num7 = 0f;
			float magnitude = (metalPath[1] - metalPath[0]).magnitude;
			vector = metalPath[0];
			float num8 = Bubble.hitSize / (float)hitDiv;
			float num9 = 0f;
			int num10 = 0;
			while (true)
			{
				num7 += num8 * uiScale;
				num9 += num8 * uiScale;
				float num11 = num7 / magnitude;
				if (num11 >= 1f)
				{
					num6++;
					if (0 <= num3 && num10 < array.Length && array[num10] == num6)
					{
						num10++;
						num11 = 0f;
						continue;
					}
					if (num6 >= metalPath.Count - 1)
					{
						break;
					}
					num7 -= magnitude;
					magnitude = (metalPath[num6 + 1] - metalPath[num6]).magnitude;
					num11 = num7 / magnitude;
				}
				vector = Vector3.Lerp(metalPath[num6], metalPath[num6 + 1], num11);
				foreach (Bubble item in list)
				{
					if (!metalBreakBubbleDic.ContainsKey(item))
					{
						Vector3 vector4 = (vector - item.myTrans.position) / uiScale;
						vector4.z = 0f;
						if (!(vector4.sqrMagnitude > Bubble.SQR_SIZE))
						{
							metalBreakBubbleDic.Add(item, num9);
						}
					}
				}
				if (!(part != null))
				{
					continue;
				}
				for (int j = 0; j < part.chainBubbleDic.Count; j++)
				{
					foreach (ChainBubble item2 in part.chainBubbleDic[j])
					{
						if (!metalBreakRockDic.ContainsKey(item2))
						{
							Vector3 vector4 = (vector - item2.myTrans.position) / uiScale;
							vector4.z = 0f;
							if (!(vector4.sqrMagnitude > Bubble.SQR_LARGE_SIZE))
							{
								metalBreakRockDic.Add(item2, num9);
							}
						}
					}
				}
			}
			vector = metalPath[num6];
		}
		else
		{
			metalPath.Clear();
			metalBreakBubbleDic.Clear();
			if ((bool)part)
			{
				part.clearRotateFulcrumMetal();
			}
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
		float num = Bubble.hitSize / (float)hitDiv;
		Vector3 vector = guideline_pos.transform.up * num;
		baseTrans.localPosition = shootBasePos;
		Vector3 zero = Vector3.zero;
		float num2 = Bubble.SQR_SIZE * 2f;
		Vector3 nearDiff = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		tunnelInBubble = null;
		int num3 = 0;
		int num4 = 0;
		bool flag = true;
		while (flag)
		{
			preCheckBubble();
			for (int i = 1; i <= hitDiv; i++)
			{
				Vector3 localPosition = baseTrans.localPosition + vector;
				if (localPosition.x < x_min || localPosition.x > x_max)
				{
					float num5 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
					localPosition = baseTrans.localPosition + vector * (1f - num5);
					vector.x = 0f - vector.x;
					localPosition += vector * num5;
				}
				baseTrans.localPosition = localPosition;
				if (part != null)
				{
					checkCloud(ref nearDiff, baseTrans.position);
					if (inCloud)
					{
						flag = false;
						bHitCeiling = false;
						hitPos = baseTrans.position;
						break;
					}
				}
				if (part != null)
				{
					if (tunnelInBubble == null)
					{
						checkTunnel(ref nearDiff, baseTrans.position);
					}
					if (hitBubble != null)
					{
						if (hitBubble.name.IndexOf("120") >= 0 && !(tunnelInBubble != null))
						{
							if (hitBubble.GetComponent<Bubble>().OutObject != null)
							{
								vector = hitBubble.GetComponent<Bubble>().OutObject.transform.Find("AS_spr_bubble").transform.up * num;
								baseTrans.position = hitBubble.GetComponent<Bubble>().OutObject.transform.position + hitBubble.GetComponent<Bubble>().OutObject.transform.Find("AS_spr_bubble").transform.up * Bubble.mHitSize * uiScale;
								num4++;
								num3++;
								tunnelInBubble = hitBubble;
								hitBubble = null;
							}
							break;
						}
						flag = false;
						bHitCeiling = false;
						hitPos = baseTrans.position;
					}
				}
				if (isHitCeiling(baseTrans.position))
				{
					flag = false;
					bHitCeiling = true;
					hitPos = baseTrans.position;
					break;
				}
				if (part != null)
				{
					checkChainBubble(ref nearDiff);
					if (hitBubble != null)
					{
						flag = false;
						bHitCeiling = false;
						hitPos = baseTrans.position;
						break;
					}
				}
				checkFieldBubble(ref nearDiff);
				if (hitBubble != null)
				{
					flag = false;
					bHitCeiling = false;
					hitPos = baseTrans.position;
					break;
				}
				if (baseTrans.localPosition.y <= 0f)
				{
					hitPos = baseTrans.position;
					break;
				}
				if (tunnelInBubble == null)
				{
					num4++;
				}
				num3++;
			}
			if (baseTrans.localPosition.y <= 0f)
			{
				break;
			}
		}
		hitDiff = nearDiff;
		hitMoveLocal = (float)num4 * num;
		hitMoveLength = (float)num3 * num;
		if (part != null)
		{
			isSuperGuide = part.isUsedItem(Constant.Item.eType.SuperGuide);
		}
		else if (rankingPart != null)
		{
			isSuperGuide = rankingPart.isUsedItem(Constant.Item.eType.SuperGuide);
		}
		if (!bActive_)
		{
			setActive(true);
		}
		vector = guideline_pos.transform.up * GUIDE_INTERVAL;
		Vector3 vector2 = shootBasePos;
		if (isSuperGuide)
		{
			guideline_supergurd.transform.localPosition = shootBasePos;
			wallHitPosList_.Clear();
			if (isMetal_)
			{
				for (int j = 1; j < metalPath.Count; j++)
				{
					wallHitPosList_.Add(metalPath[j] / uiScale + Vector3.up * 385f - shootBasePos);
					if (metalPathIndex != -1 && metalPathIndex == j)
					{
						break;
					}
				}
			}
			else
			{
				for (int k = 1; GUIDE_INTERVAL * (float)k <= hitMoveLocal; k++)
				{
					Vector3 localPosition = vector2 + vector;
					if (localPosition.x < x_min || localPosition.x > x_max)
					{
						float num6 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
						localPosition = vector2 + vector * (1f - num6);
						wallHitPosList_.Add(localPosition - shootBasePos);
						vector.x = 0f - vector.x;
						localPosition += vector * num6;
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
				float num7 = vector4.magnitude;
				if (l == 0)
				{
					vector4.Normalize();
					vector3 = vector4 * 75f;
					num7 -= 75f;
				}
				if (wallHitPosList_.Count - 1 == l)
				{
					num7 -= 23f;
					if (num7 < 0f)
					{
						num7 = 0f;
					}
				}
				vector3.z = -11.1f;
				bg_01[l].localPosition = vector3;
				bg_01[l].localEulerAngles = new Vector3(0f, 0f, z);
				bg_01[l].localScale = new Vector3(bg_01[l].localScale.x, num7, 1f);
				bg_00.localPosition = wallHitPosList_[l];
				bg_00.localEulerAngles = new Vector3(0f, 0f, z);
				vector3 = wallHitPosList_[l];
				if (num7 < 15f)
				{
					bg_01[l].gameObject.SetActive(false);
				}
			}
			return;
		}
		lines[0].transform.localPosition = vector2;
		bool flag2 = false;
		int num8 = (int)(300f / GUIDE_INTERVAL);
		if (part != null && part.bGuideStretch)
		{
			num8 = (int)(guideStretchMultiple * 2f);
		}
		for (int m = 1; m < lines.Length; m++)
		{
			if (!flag2 && !isSuperGuide && m >= num8)
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
				if (!part.bGuideStretch && !isSuperGuide)
				{
					flag2 = true;
					lines[m].enabled = false;
					continue;
				}
				float num9 = ((!(vector.x < 0f)) ? ((localPosition.x - x_max) / vector.x) : ((localPosition.x - x_min) / vector.x));
				localPosition = vector2 + vector * (1f - num9);
				vector.x = 0f - vector.x;
				localPosition += vector * num9;
			}
			vector2 = localPosition;
			vector2.z = -11.1f;
			lines[m].transform.localPosition = vector2;
			if (isMetal_)
			{
				if (GUIDE_INTERVAL * (float)m > hitMoveLength)
				{
					flag2 = true;
				}
			}
			else if (GUIDE_INTERVAL * (float)m > hitMoveLocal)
			{
				flag2 = true;
			}
			lines[m].enabled = !flag2;
		}
		for (int n = 0; n < 2; n++)
		{
			lines[n].enabled = false;
		}
		if (part != null && part.bGuideStretch)
		{
			lines[2].enabled = false;
		}
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
				if (part != null)
				{
					checkCloud(ref nearDiff, baseTrans.position);
				}
				if (inCloud)
				{
					hitPos = baseTrans.position;
					break;
				}
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

	private bool isHitCeiling(Vector3 point)
	{
		return point.y > ceilingPos_.y;
	}
}
