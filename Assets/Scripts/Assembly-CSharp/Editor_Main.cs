using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class Editor_Main : MonoBehaviour
{
	public enum eEditorLayer
	{
		BubbleLayer = 0,
		IvyLayer = -1,
		CloudLayer = -2,
		BossLayer = -3,
		ChainLayerBegin = 1,
		ChainLayerEnd = 10,
		AllLayer = 99
	}

	public GameObject arrangeBase;

	public GameObject chainBase;

	public GameObject ivyBase;

	public GameObject counterBase;

	private int numW;

	private int numH;

	private int chainNum;

	private int chainTempNum = -1;

	public UILabel stageLabel;

	public UILabel lineNumLabel;

	public UILabel chainNumLabel;

	public GameObject exchange;

	public GameObject exchange_ivy;

	public tk2dAnimatedSprite chainSelect;

	public GameObject bubbleSelectRoot;

	public GameObject chainSelectRoot;

	public GameObject ivySelectRoot;

	public GameObject cloudSelectRoot;

	public GameObject bossSelectRoot;

	public GameObject bgOnOffButton;

	private GameObject[] objs;

	private GameObject[][] chainObjs;

	private GameObject ivyParent;

	private StageDataTable infoTable;

	private int[] cloudArea = new int[2];

	private int cloudCnt;

	private GameObject cloudObj;

	public static int editLayer = 0;

	public static string[] skelltonColorName = new string[8] { "赤", "緑", "青", "黄", "橙", "紫", "白", "黒" };

	public static Color[] skelltonColor;

	[HideInInspector]
	public static string[] bossObjectNames = new string[3] { "egg", "nest", "spiderweb" };

	private GameObject stage;

	private void Start()
	{
		infoTable = base.gameObject.AddComponent<StageDataTable>();
		infoTable.loadStageData();
		infoTable.loadEventData();
		bgOnOffButton.SetActive(false);
		Editor_Chain.select = chainSelect;
		numW = 10;
		numH = 42;
		lineNumLabel.text = numH.ToString();
		chainNum = 0;
		chainNumLabel.text = chainNum.ToString();
		create(true);
		createIvy();
		createChain();
		cloudObj = Resources.Load("Prefabs/Common/Object_19_Cloud") as GameObject;
		skelltonColor = new Color[8]
		{
			new Color(1f, 0f, 0f, 1f),
			new Color(0f, 1f, 0f, 1f),
			new Color(0f, 0f, 1f, 1f),
			new Color(1f, 1f, 0f, 1f),
			new Color(1f, 0.5f, 0f, 1f),
			new Color(1f, 0f, 1f, 1f),
			new Color(1f, 1f, 1f, 1f),
			new Color(0f, 0f, 0f, 1f)
		};
	}

	private void Update()
	{
	}

	private void load(GameObject obj)
	{
		Object.Destroy(stage);
		bgOnOffButton.SetActive(false);
		string dataPath = getDataPath();
		int num = int.Parse(stageLabel.text) - 1;
		Debug.Log("path:" + dataPath);
		StageData stageData;
		if (File.Exists(dataPath))
		{
			Debug.Log("path(exist):" + dataPath);
			stageData = Xml.DeserializeObject<StageData>(File.ReadAllText(dataPath)) as StageData;
			if (num >= Constant.Event.BaseEventStageNo)
			{
				if (num >= 1 * Constant.Event.BaseEventStageNo && num < 2 * Constant.Event.BaseEventStageNo)
				{
					infoTable.loadEventData("event");
				}
				else if (num >= 2 * Constant.Event.BaseEventStageNo && num < 3 * Constant.Event.BaseEventStageNo)
				{
					infoTable.loadEventData("challenge");
				}
				else if (num >= 11 * Constant.Event.BaseEventStageNo && num < 12 * Constant.Event.BaseEventStageNo)
				{
					infoTable.loadEventData("collaboration");
				}
			}
		}
		else
		{
			stageData = new StageData();
			stageData.lineNum = 42;
			stageData.bubbleTypes = new byte[10 * stageData.lineNum];
			stageData.chainLayerNum = 0;
			stageData.chainTypes = null;
			for (int i = 0; i < stageData.bubbleTypes.Length; i++)
			{
				stageData.bubbleTypes[i] = 99;
			}
		}
		numH = stageData.lineNum;
		lineNumLabel.text = numH.ToString();
		create(true);
		chainNum = -1;
		OnChangeChainNum(stageData.chainLayerNum.ToString());
		chainNumLabel.text = chainNum.ToString();
		for (int j = 0; j < numH; j++)
		{
			for (int k = 0; k < numW; k++)
			{
				int num2 = numW * j + k;
				tk2dAnimatedSprite component = objs[num2].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>();
				string text = ((stageData.bubbleTypes[num2] <= 99) ? stageData.bubbleTypes[num2].ToString("00") : stageData.bubbleTypes[num2].ToString("000"));
				component.Play("bubble_" + text);
				component.Pause();
				UILabel component2 = objs[num2].transform.Find("label").GetComponent<UILabel>();
				if (stageData.bubbleTypes[num2] == 78)
				{
					component2.text = "A";
					component2.color = Color.white;
					component2.transform.localScale = new Vector3(20f, 20f, 1f);
				}
				else if (stageData.bubbleTypes[num2] == 88)
				{
					component2.text = "B";
					component2.color = Color.white;
					component2.transform.localScale = new Vector3(20f, 20f, 1f);
				}
				else
				{
					component2.text = string.Empty;
				}
				Editor_Arrange component3 = objs[num2].GetComponent<Editor_Arrange>();
				if (stageData.eggRow != null && component3.transform.Find(bossObjectNames[0]) == null)
				{
					for (int l = 0; l < stageData.eggRow.Length; l++)
					{
						if (stageData.eggRow[l] == j && stageData.eggColumn[l] == k)
						{
							component3.createBossObject(0);
						}
					}
				}
				if (stageData.nestRow != null && component3.transform.Find(bossObjectNames[1]) == null)
				{
					for (int m = 0; m < stageData.nestRow.Length; m++)
					{
						if (stageData.nestRow[m] == j && stageData.nestColumn[m] == k)
						{
							component3.createBossObject(1);
						}
					}
				}
				if (stageData.spiderwebRow == null || !(component3.transform.Find(bossObjectNames[2]) == null))
				{
					continue;
				}
				for (int n = 0; n < stageData.spiderwebRow.Length; n++)
				{
					if (stageData.spiderwebRow[n] == j && stageData.spiderwebColumn[n] == k)
					{
						component3.createBossObject(2);
					}
				}
			}
		}
		for (int num3 = 0; num3 < chainNum; num3++)
		{
			for (int num4 = 0; num4 < numH; num4++)
			{
				for (int num5 = 0; num5 < numW; num5++)
				{
					int num6 = numW * num4 + num5;
					tk2dAnimatedSprite component4 = chainObjs[num3][num6].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>();
					string text2 = ((stageData.chainTypes[numW * numH * num3 + num6] <= 99) ? stageData.chainTypes[numW * numH * num3 + num6].ToString("00") : stageData.chainTypes[numW * numH * num3 + num6].ToString("000"));
					component4.Play("bubble_" + text2);
					component4.Pause();
				}
			}
		}
		if (stageData.counteIndex != null)
		{
			int num7 = 0;
			for (int num8 = 0; num8 < numH; num8++)
			{
				for (int num9 = 0; num9 < numW; num9++)
				{
					int num10 = numW * num8 + num9;
					if (stageData.bubbleTypes[num10] == 77 || (stageData.bubbleTypes[num10] >= 100 && stageData.bubbleTypes[num10] <= 107))
					{
						objs[num10].GetComponent<Editor_Arrange>().createCounter(stageData.counteCount[num7]);
						num7++;
					}
				}
			}
		}
		if (stageData.skeltonIndex != null)
		{
			int num11 = 0;
			for (int num12 = 0; num12 < numH; num12++)
			{
				for (int num13 = 0; num13 < numW; num13++)
				{
					int num14 = numW * num12 + num13;
					if (stageData.bubbleTypes[num14] == 87)
					{
						UILabel component5 = objs[num14].transform.Find("label").GetComponent<UILabel>();
						component5.text = skelltonColorName[stageData.skeltonColor[num11]];
						component5.color = skelltonColor[stageData.skeltonColor[num11]];
						component5.transform.localScale = new Vector3(20f, 20f, 1f);
						num11++;
					}
				}
			}
		}
		Editor_Ivy instance = Editor_Ivy.Instance;
		instance.reset();
		if (stageData.ivyTypes != null)
		{
			for (int num15 = 0; num15 < stageData.ivyTypes.Length; num15++)
			{
				Editor_Ivy.select_type = (Ivy.eType)stageData.ivyTypes[num15];
				instance.setBud(stageData.ivyRow[num15], stageData.ivyColumn[num15]);
			}
		}
		clearCloud();
		if (stageData.cloudArea != null)
		{
			loadCloud(stageData.cloudArea);
		}
		UILabel[] componentsInChildren = exchange_ivy.GetComponentsInChildren<UILabel>();
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			uILabel.effectColor = ((!(uILabel.name == "00")) ? Color.black : Color.red);
		}
		changeDispImp(0);
		if (num < 10000)
		{
			StageInfo.Info info = infoTable.getInfo(int.Parse(stageLabel.text) - 1);
			if (info != null)
			{
				stage = Object.Instantiate(Resources.Load("Prefabs/Common/Stage_" + info.Common.Bg.ToString("00"))) as GameObject;
				Utility.setParent(stage, base.transform, true);
				stage.transform.localScale = new Vector3(0.344498f, 0.344498f, 1f);
				stage.transform.localPosition = Vector3.down * 224f;
				bgOnOffButton.SetActive(true);
			}
		}
		else
		{
			int num17 = num % 10000;
			if (num17 >= infoTable.getEventData().Infos.Length)
			{
				return;
			}
			EventStageInfo.Info info2 = infoTable.getEventData().Infos[num17];
			if (info2 != null)
			{
				stage = Object.Instantiate(Resources.Load("Prefabs/Common/Stage_" + info2.Common.Bg.ToString("00"))) as GameObject;
				Utility.setParent(stage, base.transform, true);
				stage.transform.localScale = new Vector3(0.344498f, 0.344498f, 1f);
				stage.transform.localPosition = Vector3.down * 224f;
				bgOnOffButton.SetActive(true);
			}
		}
		Debug.Log("load " + stageLabel.text);
	}

	private void save(GameObject obj)
	{
		StageData stageData = new StageData();
		stageData.lineNum = numH;
		stageData.bubbleTypes = new byte[numW * numH];
		stageData.chainLayerNum = chainNum;
		stageData.chainTypes = new byte[numW * numH * chainNum];
		ArrayList arrayList = new ArrayList();
		arrayList.Clear();
		ArrayList arrayList2 = new ArrayList();
		arrayList2.Clear();
		ArrayList arrayList3 = new ArrayList();
		arrayList3.Clear();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < numH; i++)
		{
			for (int j = 0; j < numW; j++)
			{
				int num3 = numW * i + j;
				string s = objs[num3].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>().CurrentClip.name.Replace("bubble_", string.Empty);
				int length = ((int.Parse(s) <= 99) ? 2 : 3);
				stageData.bubbleTypes[num3] = byte.Parse(objs[num3].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>().CurrentClip.name.Substring("bubble_".Length, length));
				if (stageData.bubbleTypes[num3] == 77 || (stageData.bubbleTypes[num3] >= 100 && stageData.bubbleTypes[num3] <= 107))
				{
					num++;
				}
				if (stageData.bubbleTypes[num3] == 87)
				{
					num2++;
				}
				if (objs[num3].transform.Find(bossObjectNames[0]) != null)
				{
					int[] value = new int[2] { i, j };
					arrayList.Add(value);
				}
				if (objs[num3].transform.Find(bossObjectNames[1]) != null)
				{
					int[] value2 = new int[2] { i, j };
					arrayList2.Add(value2);
				}
				if (objs[num3].transform.Find(bossObjectNames[2]) != null)
				{
					int[] value3 = new int[2] { i, j };
					arrayList3.Add(value3);
				}
			}
		}
		for (int k = 0; k < chainNum; k++)
		{
			for (int l = 0; l < numH; l++)
			{
				for (int m = 0; m < numW; m++)
				{
					int num4 = numW * l + m;
					stageData.chainTypes[numW * numH * k + num4] = byte.Parse(chainObjs[k][num4].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>().CurrentClip.name.Substring("bubble_".Length, 2));
				}
			}
		}
		if (num > 0)
		{
			stageData.counteIndex = new int[num];
			stageData.counteCount = new byte[num];
			int num5 = 0;
			for (int n = 0; n < numH; n++)
			{
				for (int num6 = 0; num6 < numW; num6++)
				{
					int num7 = numW * n + num6;
					if (stageData.bubbleTypes[num7] == 77 || (stageData.bubbleTypes[num7] >= 100 && stageData.bubbleTypes[num7] <= 107))
					{
						stageData.counteIndex[num5] = num7;
						stageData.counteCount[num5] = (byte)objs[num7].GetComponentInChildren<Editor_Counter>().getCount();
						num5++;
					}
				}
			}
		}
		if (num2 > 0)
		{
			stageData.skeltonIndex = new int[num2];
			stageData.skeltonColor = new int[num2];
			int num8 = 0;
			for (int num9 = 0; num9 < numH; num9++)
			{
				for (int num10 = 0; num10 < numW; num10++)
				{
					int num11 = numW * num9 + num10;
					if (stageData.bubbleTypes[num11] != 87)
					{
						continue;
					}
					stageData.skeltonIndex[num8] = num11;
					int num12 = 0;
					string text = objs[num11].transform.Find("label").GetComponent<UILabel>().text;
					for (int num13 = 0; num13 < skelltonColorName.Length; num13++)
					{
						if (text == skelltonColorName[num13])
						{
							num12 = num13;
							break;
						}
					}
					stageData.skeltonColor[num8] = num12;
					num8++;
				}
			}
		}
		Editor_Ivy component = ivyParent.GetComponent<Editor_Ivy>();
		if (component.budList.Count > 0)
		{
			stageData.ivyTypes = new byte[component.budList.Count];
			stageData.ivyColumn = new byte[component.budList.Count];
			stageData.ivyRow = new byte[component.budList.Count];
			for (int num14 = 0; num14 < component.budList.Count; num14++)
			{
				stageData.ivyTypes[num14] = (byte)component.budList[num14].getType();
				stageData.ivyColumn[num14] = (byte)component.budList[num14].getColumn();
				stageData.ivyRow[num14] = (byte)component.budList[num14].getRow();
			}
		}
		if (cloudCnt > 0)
		{
			stageData.cloudArea = new int[2];
			for (int num15 = 0; num15 < 2; num15++)
			{
				stageData.cloudArea[num15] = cloudArea[num15];
			}
		}
		else
		{
			stageData.cloudArea = null;
		}
		if (arrayList.Count > 0)
		{
			stageData.eggRow = new byte[arrayList.Count];
			stageData.eggColumn = new byte[arrayList.Count];
			for (int num16 = 0; num16 < arrayList.Count; num16++)
			{
				int[] array = (int[])arrayList[num16];
				stageData.eggRow[num16] = (byte)array[0];
				stageData.eggColumn[num16] = (byte)array[1];
			}
		}
		if (arrayList2.Count > 0)
		{
			stageData.nestRow = new byte[arrayList2.Count];
			stageData.nestColumn = new byte[arrayList2.Count];
			for (int num17 = 0; num17 < arrayList2.Count; num17++)
			{
				int[] array2 = (int[])arrayList2[num17];
				stageData.nestRow[num17] = (byte)array2[0];
				stageData.nestColumn[num17] = (byte)array2[1];
			}
		}
		if (arrayList3.Count > 0)
		{
			stageData.spiderwebRow = new byte[arrayList3.Count];
			stageData.spiderwebColumn = new byte[arrayList3.Count];
			for (int num18 = 0; num18 < arrayList3.Count; num18++)
			{
				int[] array3 = (int[])arrayList3[num18];
				stageData.spiderwebRow[num18] = (byte)array3[0];
				stageData.spiderwebColumn[num18] = (byte)array3[1];
			}
		}
		string dataPath = getDataPath();
		Directory.CreateDirectory(dataPath.Remove(dataPath.LastIndexOf('/')));
		File.WriteAllText(dataPath, Xml.SerializeObject<StageData>(stageData));
		Debug.Log("save " + stageLabel.text);
	}

	private void bgOnOff()
	{
		if (stage != null)
		{
			stage.SetActive(!stage.activeSelf);
		}
	}

	private void create(bool recreate)
	{
		Transform transform = base.transform.Find("Arrange");
		transform.localPosition = new Vector3(-93f, 400f, 0f);
		GameObject[] array = new GameObject[numW * numH];
		for (int i = 0; i < numH; i++)
		{
			int num = 0;
			if (i % 2 == 1)
			{
				num = 10;
			}
			for (int j = 0; j < numW; j++)
			{
				GameObject gameObject = Object.Instantiate(arrangeBase) as GameObject;
				gameObject.transform.parent = transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localPosition = new Vector3(j * 21 + num, (float)i * -18.186533f, 0f);
				gameObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_99");
				gameObject.name = "bubble_" + (numW * i + j).ToString("000");
				array[numW * i + j] = gameObject;
			}
		}
		if (objs != null)
		{
			for (int k = 0; k < numH; k++)
			{
				for (int l = 0; l < numW; l++)
				{
					int num2 = numW * k + l;
					if (num2 >= objs.Length)
					{
						break;
					}
					tk2dAnimatedSprite component = array[num2].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>();
					component.Play(objs[num2].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>().CurrentClip.name);
					component.Pause();
					Transform transform2 = objs[num2].transform.Find("count");
					if (transform2 != null)
					{
						if (recreate)
						{
							Object.DestroyImmediate(transform2.gameObject);
						}
						else
						{
							transform2.parent = array[num2].transform;
						}
					}
				}
			}
			for (int m = 0; m < objs.Length; m++)
			{
				Object.Destroy(objs[m]);
			}
		}
		objs = array;
		Editor_Ivy.objs = objs;
	}

	private void createChain()
	{
		Transform parent = base.transform.Find("Arrange");
		GameObject[][] array = new GameObject[chainNum][];
		for (int i = 0; i < chainNum; i++)
		{
			array[i] = new GameObject[numW * numH];
		}
		for (int j = 0; j < chainNum; j++)
		{
			for (int k = 0; k < numH; k++)
			{
				int num = 0;
				if (k % 2 == 1)
				{
					num = 10;
				}
				for (int l = 0; l < numW; l++)
				{
					GameObject gameObject = Object.Instantiate(chainBase) as GameObject;
					gameObject.transform.parent = parent;
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localPosition = new Vector3(l * 21 + num, (float)k * -18.186533f, -0.01f * (float)(j + 1));
					gameObject.GetComponentInChildren<tk2dAnimatedSprite>().Play("bubble_99");
					gameObject.name = "chain_" + j.ToString("00") + "_" + (numW * k + l).ToString("000");
					array[j][numW * k + l] = gameObject;
				}
			}
		}
		if (chainObjs != null)
		{
			for (int m = 0; m < chainNum && m < chainObjs.Length; m++)
			{
				for (int n = 0; n < numH; n++)
				{
					for (int num2 = 0; num2 < numW; num2++)
					{
						int num3 = numW * n + num2;
						if (num3 >= chainObjs[m].Length)
						{
							break;
						}
						tk2dAnimatedSprite component = array[m][num3].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>();
						component.Play(chainObjs[m][num3].transform.Find("AS_spr_bubble").GetComponent<tk2dAnimatedSprite>().CurrentClip.name);
						component.Pause();
					}
				}
			}
			for (int num4 = 0; num4 < chainObjs.Length; num4++)
			{
				for (int num5 = 0; num5 < chainObjs[num4].Length; num5++)
				{
					Object.Destroy(chainObjs[num4][num5]);
				}
			}
		}
		chainObjs = array;
		changeDispImp(0);
	}

	private void createIvy()
	{
		Transform parent = base.transform.Find("Arrange");
		ivyParent = Object.Instantiate(ivyBase) as GameObject;
		ivyParent.transform.parent = parent;
		ivyParent.transform.localScale = Vector3.one;
		ivyParent.transform.localScale = Vector3.zero;
	}

	private void OnChangeLineNum(string inputString)
	{
		if (numH != int.Parse(inputString))
		{
			numH = int.Parse(inputString);
			create(false);
			createChain();
			Editor_Ivy.Instance.onChangeLineNum(numH);
		}
	}

	private void OnChangeChainNum(string inputString)
	{
		if (chainNum == int.Parse(inputString))
		{
			return;
		}
		chainNum = int.Parse(inputString);
		createChain();
		if (chainNum > 0)
		{
			exchange.SetActive(true);
			for (int i = 0; i <= 10; i++)
			{
				exchange.transform.Find(i.ToString("00")).gameObject.SetActive(i <= chainNum);
			}
			exchange_ivy.SetActive(false);
		}
		else
		{
			exchange.SetActive(false);
			exchange_ivy.SetActive(true);
		}
	}

	private void changeDisp(GameObject obj)
	{
		int num = int.Parse(obj.name);
		if (Input.GetKey(KeyCode.LeftControl))
		{
			if (chainTempNum == -1)
			{
				return;
			}
			if (num != 99 && num != 0 && num != chainTempNum)
			{
				GameObject[] array = chainObjs[chainTempNum - 1];
				chainObjs[chainTempNum - 1] = chainObjs[num - 1];
				chainObjs[num - 1] = array;
				for (int i = 0; i < chainNum; i++)
				{
					for (int j = 0; j < numH; j++)
					{
						int num2 = 0;
						if (j % 2 == 1)
						{
							num2 = 10;
						}
						for (int k = 0; k < numW; k++)
						{
							chainObjs[i][numW * j + k].transform.localPosition = new Vector3(k * 21 + num2, (float)j * -18.186533f, -0.01f * (float)(i + 1));
						}
					}
				}
			}
			UILabel[] componentsInChildren = obj.transform.parent.GetComponentsInChildren<UILabel>(true);
			UILabel[] array2 = componentsInChildren;
			foreach (UILabel uILabel in array2)
			{
				uILabel.effectColor = ((!(uILabel.name == obj.name)) ? Color.black : Color.red);
			}
			changeDispImp(num);
			chainTempNum = -1;
		}
		else
		{
			if (num != 0 && num != 99 && num != -1 && num != -2)
			{
				chainTempNum = num;
			}
			UILabel[] componentsInChildren2 = obj.transform.parent.GetComponentsInChildren<UILabel>(true);
			UILabel[] array3 = componentsInChildren2;
			foreach (UILabel uILabel2 in array3)
			{
				uILabel2.effectColor = ((!(uILabel2.name == obj.name)) ? Color.black : Color.red);
			}
			changeDispImp(num);
		}
	}

	private void changeIvyType(GameObject obj)
	{
		if (obj.name.Contains("right"))
		{
			Editor_Ivy.select_type = Ivy.eType.Right;
		}
		else
		{
			Editor_Ivy.select_type = Ivy.eType.Left;
		}
		UILabel[] componentsInChildren = obj.transform.parent.GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			uILabel.effectColor = ((!(uILabel.name == obj.name)) ? Color.black : Color.red);
		}
	}

	private void changeBossObjType(GameObject obj)
	{
		if (obj.name.Contains("egg"))
		{
			Editor_ObjectSelect.selectIndex = 0;
		}
		else if (obj.name.Contains("nest"))
		{
			Editor_ObjectSelect.selectIndex = 1;
		}
		else if (obj.name.Contains("spiderweb"))
		{
			Editor_ObjectSelect.selectIndex = 2;
		}
		UILabel[] componentsInChildren = obj.transform.parent.GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			uILabel.effectColor = ((!(uILabel.name == obj.name)) ? Color.black : Color.red);
		}
	}

	private void decideCloudArea(GameObject obj)
	{
		int num = 0;
		UILabel uILabel = null;
		GameObject gameObject = null;
		if (obj.name.Equals("area_1"))
		{
			num = 1;
		}
		else if (obj.name.Equals("area_2"))
		{
			num = 2;
		}
		else if (obj.name.Equals("area_3"))
		{
			num = 3;
		}
		else if (obj.name.Equals("area_4"))
		{
			num = 4;
		}
		else if (obj.name.Equals("area_5"))
		{
			num = 5;
		}
		else if (obj.name.Equals("area_6"))
		{
			num = 6;
		}
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		if (num >= 4 && num <= 6)
		{
			num2 = num - 3;
			num3 = 1;
		}
		else
		{
			num2 = num;
			num3 = 0;
		}
		if (numH >= 10)
		{
			num4 = numH - 9;
		}
		for (int i = 0; i < 2; i++)
		{
			if (cloudArea[i] == 0)
			{
				cloudArea[i] = num;
				uILabel = obj.transform.GetComponent<UILabel>();
				uILabel.effectColor = Color.red;
				gameObject = Object.Instantiate(cloudObj) as GameObject;
				gameObject.transform.parent = base.transform.Find("Arrange");
				gameObject.transform.localScale = new Vector3(0.344498f, 0.344498f, 1f);
				gameObject.transform.localPosition = new Vector3(30f + 63f * (float)(num2 - 1), -43f - (float)num4 * 18.186533f - 72f * (float)num3, -5f);
				gameObject.name = "cloud_" + obj.name;
				cloudCnt++;
				break;
			}
			if (cloudArea[i] == num)
			{
				cloudArea[i] = 0;
				uILabel = obj.transform.GetComponent<UILabel>();
				uILabel.effectColor = Color.black;
				gameObject = base.transform.Find("Arrange/cloud_" + obj.name).gameObject;
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
				if (i == 0)
				{
					cloudArea[i] = cloudArea[i + 1];
					cloudArea[i + 1] = 0;
				}
				cloudCnt--;
				break;
			}
		}
	}

	private void loadCloud(int[] _area)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		GameObject gameObject = null;
		if (numH >= 10)
		{
			num3 = numH - 9;
		}
		for (int i = 0; i < 2; i++)
		{
			if (_area[i] != 0)
			{
				cloudArea[i] = _area[i];
				if (_area[i] >= 4 && _area[i] <= 6)
				{
					num = _area[i] - 3;
					num2 = 1;
				}
				else
				{
					num = _area[i];
					num2 = 0;
				}
				gameObject = Object.Instantiate(cloudObj) as GameObject;
				gameObject.transform.parent = base.transform.Find("Arrange");
				gameObject.transform.localScale = new Vector3(0.344498f, 0.344498f, 1f);
				gameObject.transform.localPosition = new Vector3(30f + 63f * (float)(num - 1), -43f - (float)num3 * 18.186533f - 72f * (float)num2, -5f);
				gameObject.name = "cloud_area_" + _area[i];
				cloudCnt++;
			}
		}
	}

	private void clearCloud()
	{
		for (int i = 0; i < 7; i++)
		{
			Transform transform = base.transform.Find("Arrange/cloud_area_" + (i + 1));
			if (transform != null)
			{
				Object.DestroyImmediate(transform.gameObject);
			}
		}
		for (int j = 0; j < 2; j++)
		{
			cloudArea[j] = 0;
		}
	}

	private void setCloudEnable(bool enable)
	{
		for (int i = 0; i < 7; i++)
		{
			Transform transform = base.transform.Find("Arrange/cloud_area_" + (i + 1));
			if (transform != null)
			{
				transform.gameObject.SetActive(enable);
			}
		}
	}

	private void changeDispImp(int num)
	{
		switch (num)
		{
		case 99:
		{
			for (int num10 = 0; num10 < objs.Length; num10++)
			{
				objs[num10].SetActive(num10 % (numW * 2) != numW * 2 - 1);
				objs[num10].GetComponent<Collider>().enabled = false;
			}
			for (int num11 = 0; num11 < chainObjs.Length; num11++)
			{
				for (int num12 = 0; num12 < chainObjs[num11].Length; num12++)
				{
					chainObjs[num11][num12].SetActive(num12 % (numW * 2) != numW * 2 - 1);
					if (chainObjs[num11][num12].activeSelf && chainObjs[num11][num12].GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip.name == "bubble_99")
					{
						chainObjs[num11][num12].SetActive(false);
					}
					else if (chainObjs[num11][num12].activeSelf && chainObjs[num11][num12].GetComponentInChildren<tk2dAnimatedSprite>().CurrentClip.name == "bubble_47")
					{
						chainObjs[num11][num12].transform.localPosition = new Vector3(chainObjs[num11][num12].transform.localPosition.x, chainObjs[num11][num12].transform.localPosition.y, chainObjs[num11][num12].transform.localPosition.z - 0.1f);
					}
					chainObjs[num11][num12].GetComponent<Collider>().enabled = false;
				}
			}
			Editor_Ivy component5 = ivyParent.GetComponent<Editor_Ivy>();
			component5.setIvyVisible(true);
			setCloudEnable(true);
			bubbleSelectRoot.SetActive(false);
			chainSelectRoot.SetActive(false);
			ivySelectRoot.SetActive(false);
			cloudSelectRoot.SetActive(false);
			bossSelectRoot.SetActive(false);
			break;
		}
		case 0:
		{
			for (int num7 = 0; num7 < objs.Length; num7++)
			{
				objs[num7].SetActive(num7 % (numW * 2) != numW * 2 - 1);
				objs[num7].GetComponent<Collider>().enabled = true;
			}
			for (int num8 = 0; num8 < chainObjs.Length; num8++)
			{
				for (int num9 = 0; num9 < chainObjs[num8].Length; num9++)
				{
					chainObjs[num8][num9].SetActive(false);
				}
			}
			Editor_Ivy component4 = ivyParent.GetComponent<Editor_Ivy>();
			component4.setIvyVisible(false);
			setCloudEnable(false);
			bubbleSelectRoot.SetActive(true);
			chainSelectRoot.SetActive(false);
			ivySelectRoot.SetActive(false);
			cloudSelectRoot.SetActive(false);
			bossSelectRoot.SetActive(false);
			break;
		}
		case -1:
		{
			for (int num4 = 0; num4 < objs.Length; num4++)
			{
				objs[num4].SetActive(num4 % (numW * 2) != numW * 2 - 1);
				objs[num4].GetComponent<Collider>().enabled = true;
			}
			for (int num5 = 0; num5 < chainObjs.Length; num5++)
			{
				for (int num6 = 0; num6 < chainObjs[num5].Length; num6++)
				{
					chainObjs[num5][num6].SetActive(false);
				}
			}
			ivyParent.SetActive(true);
			Editor_Ivy component3 = ivyParent.GetComponent<Editor_Ivy>();
			component3.setIvyVisible(true);
			setCloudEnable(false);
			bubbleSelectRoot.SetActive(false);
			chainSelectRoot.SetActive(false);
			ivySelectRoot.SetActive(true);
			cloudSelectRoot.SetActive(false);
			bossSelectRoot.SetActive(false);
			break;
		}
		case -2:
		{
			for (int l = 0; l < objs.Length; l++)
			{
				objs[l].SetActive(l % (numW * 2) != numW * 2 - 1);
				objs[l].GetComponent<Collider>().enabled = true;
			}
			for (int m = 0; m < chainObjs.Length; m++)
			{
				for (int n = 0; n < chainObjs[m].Length; n++)
				{
					chainObjs[m][n].SetActive(false);
				}
			}
			ivyParent.SetActive(false);
			Editor_Ivy component2 = ivyParent.GetComponent<Editor_Ivy>();
			component2.setIvyVisible(false);
			setCloudEnable(true);
			UILabel[] componentsInChildren = cloudSelectRoot.transform.GetComponentsInChildren<UILabel>(true);
			UILabel[] array = componentsInChildren;
			foreach (UILabel uILabel in array)
			{
				for (int num3 = 0; num3 < 2; num3++)
				{
					if (uILabel.name == "area_" + cloudArea[num3])
					{
						uILabel.effectColor = Color.red;
						break;
					}
					uILabel.effectColor = Color.black;
				}
			}
			bubbleSelectRoot.SetActive(false);
			chainSelectRoot.SetActive(false);
			ivySelectRoot.SetActive(false);
			cloudSelectRoot.SetActive(true);
			bossSelectRoot.SetActive(false);
			break;
		}
		case -3:
			bubbleSelectRoot.SetActive(false);
			chainSelectRoot.SetActive(false);
			ivySelectRoot.SetActive(false);
			cloudSelectRoot.SetActive(false);
			bossSelectRoot.SetActive(true);
			break;
		default:
		{
			for (int i = 0; i < objs.Length; i++)
			{
				if (objs[i].GetComponentsInChildren<tk2dAnimatedSprite>(true)[0].CurrentClip.name == "bubble_99")
				{
					objs[i].SetActive(false);
				}
			}
			for (int j = 0; j < chainObjs.Length; j++)
			{
				for (int k = 0; k < chainObjs[j].Length; k++)
				{
					chainObjs[j][k].SetActive(j + 1 == num && k % (numW * 2) != numW * 2 - 1);
					chainObjs[j][k].GetComponent<Collider>().enabled = true;
				}
			}
			Editor_Ivy component = ivyParent.GetComponent<Editor_Ivy>();
			component.setIvyVisible(false);
			setCloudEnable(false);
			bubbleSelectRoot.SetActive(false);
			chainSelectRoot.SetActive(true);
			ivySelectRoot.SetActive(false);
			cloudSelectRoot.SetActive(false);
			bossSelectRoot.SetActive(false);
			break;
		}
		}
		editLayer = num;
	}

	private string getDataPath()
	{
		int num = int.Parse(stageLabel.text);
		StringBuilder stringBuilder = new StringBuilder(Application.dataPath);
		stringBuilder.Append("/../StageData/");
		stringBuilder.Append(num.ToString("00000"));
		stringBuilder.Append(".xml");
		return stringBuilder.ToString();
	}

	private void play(GameObject obj)
	{
		Application.LoadLevel("main");
	}

	private void directPlay(GameObject obj)
	{
	}

	private void reset()
	{
		Application.LoadLevel("stage_editor");
	}
}
