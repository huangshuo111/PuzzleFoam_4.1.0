using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	private enum eBtn
	{
		Close = 0,
		Next = 1,
		Max = 2
	}

	public int[] HoToPlayPageClearStages;

	private static TutorialManager instance_;

	private Dictionary<int, TutorialData.Parameter> tutorialDict_ = new Dictionary<int, TutorialData.Parameter>();

	private Dictionary<string, GameObject> resourceDict_ = new Dictionary<string, GameObject>();

	private GameObject window_;

	private UILabel textLabel_;

	private TweenGroup openTween_;

	private TweenGroup closeTween_;

	private MessageResource msgRes_;

	private bool bPressButton_;

	private GameObject[] buttons_ = new GameObject[2];

	private bool bPlaying_;

	public bool bItemTutorial;

	public bool isPlaying
	{
		get
		{
			return bPlaying_;
		}
		private set
		{
			bPlaying_ = value;
		}
	}

	public static TutorialManager Instance
	{
		get
		{
			return instance_;
		}
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		msgRes_ = MessageResource.Instance;
	}

	public void init(TutorialData data, GameObject window)
	{
		for (int i = 0; i < data.Num; i++)
		{
			TutorialData.Parameter parameter = data.Parameters[i];
			tutorialDict_[parameter.StageNo] = parameter;
		}
		window_ = window;
		Transform[] componentsInChildren = window_.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "Label":
				textLabel_ = transform.GetComponent<UILabel>();
				break;
			case "Button":
				buttons_[0] = transform.gameObject;
				break;
			case "NextButton":
				buttons_[1] = transform.gameObject;
				break;
			}
		}
		GameObject[] array2 = buttons_;
		foreach (GameObject target in array2)
		{
			NGUIUtility.setupButton(target, base.gameObject, true);
		}
		getTweenGroupe(window_, ref openTween_, ref closeTween_);
	}

	public bool isTutorial(int stageNo, TutorialDataTable.ePlace place)
	{
		if (getPlace(stageNo) == place)
		{
			return true;
		}
		return false;
	}

	public TutorialDataTable.ePlace getPlace(int stageNo)
	{
		if (!tutorialDict_.ContainsKey(stageNo))
		{
			return TutorialDataTable.ePlace.Invalid;
		}
		return (TutorialDataTable.ePlace)tutorialDict_[stageNo].Place;
	}

	public int getHighlightBubble(int stageNo)
	{
		if (!tutorialDict_.ContainsKey(stageNo))
		{
			return -1;
		}
		return tutorialDict_[stageNo].BubbleType;
	}

	public int getHighlightGimmick(int stageNo)
	{
		if (!tutorialDict_.ContainsKey(stageNo))
		{
			return -1;
		}
		return tutorialDict_[stageNo].Gimmick;
	}

	public int getHighlightGimmick2(int stageNo)
	{
		if (!tutorialDict_.ContainsKey(stageNo))
		{
			return -1;
		}
		return tutorialDict_[stageNo].Gimmick2;
	}

	public int getItemType(int stageNo)
	{
		if (!tutorialDict_.ContainsKey(stageNo))
		{
			return -1;
		}
		return tutorialDict_[stageNo].ItemType;
	}

	public bool isLoaded(int stageNo)
	{
		if (!isTutorial(stageNo))
		{
			return false;
		}
		TutorialData.Parameter parameter = tutorialDict_[stageNo];
		return resourceDict_.ContainsKey(parameter.PrefabName);
	}

	public void load(int stageNo, GameObject uiRoot)
	{
		if (!isTutorial(stageNo))
		{
			return;
		}
		TutorialData.Parameter parameter = tutorialDict_[stageNo];
		if (resourceDict_.ContainsKey(parameter.PrefabName))
		{
			return;
		}
		GameObject gameObject = null;
		if (parameter.PrefabName.Contains("Tutorial"))
		{
			gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", parameter.PrefabName)) as GameObject;
			Utility.setParent(gameObject, uiRoot.transform, false);
			Transform transform = gameObject.transform.Find("TopUI");
			if (transform != null)
			{
				NGUIUtilScalableUIRoot.OffsetUI(transform, true);
			}
			Transform transform2 = gameObject.transform.Find("BottomUI");
			if (transform2 != null)
			{
				NGUIUtilScalableUIRoot.OffsetUI(transform2, false);
			}
			gameObject.SetActive(false);
		}
		else
		{
			gameObject = GameObject.Find(parameter.PrefabName);
		}
		resourceDict_[parameter.PrefabName] = gameObject;
	}

	private void setupException(int stageNo, GameObject obj, StageInfo.CommonInfo stageInfo, Hashtable args)
	{
		TutorialData.Parameter parameter = tutorialDict_[stageNo];
		switch (stageNo)
		{
		case 1:
		{
			UILabel component10 = obj.transform.Find("countdown/count_label").GetComponent<UILabel>();
			UILabel component11 = obj.transform.Find("countdown/count_number").GetComponent<UILabel>();
			UILabel component12 = obj.transform.Find("clear_condition/Target_Label").GetComponent<UILabel>();
			component12.text = Constant.MessageUtil.getTargetMsg(stageInfo, msgRes_, Constant.MessageUtil.eTargetType.Game);
			tk2dAnimatedSprite component13 = obj.transform.Find("chara_01/bubble00").GetComponent<tk2dAnimatedSprite>();
			string text2 = (((int)args["nextBubbleType"] <= 99) ? ((int)args["nextBubbleType"]).ToString("00") : ((int)args["nextBubbleType"]).ToString("000"));
			component13.Play("bubble_" + text2);
			if (stageInfo.Move > 0)
			{
				component11.text = MessageResource.Instance.getMessage(200);
				component10.text = stageInfo.Move.ToString();
			}
			else
			{
				component11.text = MessageResource.Instance.getMessage(201);
				component10.text = stageInfo.Time.ToString();
			}
			if (!stageInfo.IsFriendDelete)
			{
				return;
			}
			Transform transform3 = obj.transform.Find("clear_condition");
			UILabel component14 = transform3.Find("Target_Label").GetComponent<UILabel>();
			List<Bubble.eType> list = (List<Bubble.eType>)args["clearChacknList"];
			transform3.Find("condition_icon").gameObject.SetActive(false);
			Transform transform4 = transform3.Find("chakkun");
			transform4.gameObject.SetActive(true);
			component14.pivot = UIWidget.Pivot.Center;
			Transform transform5 = null;
			for (int num8 = 0; num8 < transform4.childCount; num8++)
			{
				Transform child = transform4.GetChild(num8);
				if (child.name == list.Count.ToString())
				{
					child.gameObject.SetActive(true);
					transform5 = child;
				}
				else
				{
					child.gameObject.SetActive(false);
				}
			}
			list.Sort();
			for (int num9 = 0; num9 < list.Count; num9++)
			{
				UISprite component15 = transform5.Find("pos_0" + num9).GetComponent<UISprite>();
				switch (list[num9])
				{
				case Bubble.eType.Red:
					component15.spriteName = "goal_chakkun_00";
					break;
				case Bubble.eType.Green:
					component15.spriteName = "goal_chakkun_01";
					break;
				case Bubble.eType.Blue:
					component15.spriteName = "goal_chakkun_02";
					break;
				case Bubble.eType.Yellow:
					component15.spriteName = "goal_chakkun_03";
					break;
				case Bubble.eType.Orange:
					component15.spriteName = "goal_chakkun_04";
					break;
				case Bubble.eType.Purple:
					component15.spriteName = "goal_chakkun_05";
					break;
				case Bubble.eType.White:
					component15.spriteName = "goal_chakkun_06";
					break;
				case Bubble.eType.Black:
					component15.spriteName = "goal_chakkun_07";
					break;
				case Bubble.eType.FriendRainbow:
					component15.spriteName = "goal_chakkun_08";
					break;
				case Bubble.eType.FriendBox:
					component15.spriteName = "goal_chakkun_09";
					break;
				}
				Debug.Log(list[num9]);
				component15.MakePixelPerfect();
			}
			return;
		}
		case 10:
		{
			tk2dAnimatedSprite component = obj.transform.Find("chara_01/bubble00").GetComponent<tk2dAnimatedSprite>();
			string text = (((int)args["nextBubbleType"] <= 99) ? ((int)args["nextBubbleType"]).ToString("00") : ((int)args["nextBubbleType"]).ToString("000"));
			component.Play("bubble_" + text);
			UILabel component2 = obj.transform.Find("countdown/count_label").GetComponent<UILabel>();
			UILabel component3 = obj.transform.Find("countdown/count_time").GetComponent<UILabel>();
			component3.text = MessageResource.Instance.getMessage(201);
			component2.text = stageInfo.Time.ToString();
			Transform transform = obj.transform.Find("clear_condition");
			int num = ((!stageInfo.IsFriendDelete) ? ((stageInfo.IsAllDelete || stageInfo.IsFulcrumDelete) ? 1 : 2) : 0);
			if (stageInfo.Time > 0)
			{
				num += 3;
			}
			for (int i = 0; i < 6; i++)
			{
				transform.Find("condition_icon/" + i.ToString("00")).gameObject.SetActive(i == num);
			}
			int num2 = ((stageInfo.IsAllDelete || stageInfo.IsFulcrumDelete) ? 2 : ((!stageInfo.IsFriendDelete) ? 1 : 0));
			for (int j = 0; j < 3; j++)
			{
				transform.Find("setup_paper/paper_0" + j).gameObject.SetActive(j == num2);
			}
			UILabel component4 = transform.Find("Target_Label").GetComponent<UILabel>();
			component4.text = Constant.MessageUtil.getTargetMsg(stageInfo, MessageResource.Instance, Constant.MessageUtil.eTargetType.Game);
			return;
		}
		case 13:
		case 49:
		{
			Dictionary<Constant.Item.eType, int> dictionary = new Dictionary<Constant.Item.eType, int>();
			for (int num4 = 0; num4 < stageInfo.ItemNum; num4++)
			{
				string key = "item_" + num4 + "type";
				string key2 = "item_" + num4 + "num";
				if (args.ContainsKey(key) && args.ContainsKey(key2))
				{
					Constant.Item.eType key3 = (Constant.Item.eType)(int)args[key];
					int value = (int)args[key2];
					dictionary.Add(key3, value);
				}
			}
			for (int num5 = 0; num5 < Constant.StageItemMax; num5++)
			{
				StageBoostItem component6 = obj.transform.Find("BottomUI/boost_items/sell_boost_" + num5.ToString("00")).GetComponent<StageBoostItem>();
				if (num5 >= stageInfo.StageItemNum)
				{
					component6.gameObject.SetActive(false);
					continue;
				}
				StageInfo.Item item2 = stageInfo.StageItems[num5];
				if (parameter.ItemType == item2.Type)
				{
					component6.gameObject.SetActive(true);
					Constant.Item.eType type = (Constant.Item.eType)item2.Type;
					if (dictionary.ContainsKey(type))
					{
						component6.setup(type, item2.Num, dictionary[type], true);
					}
					else
					{
						component6.setup(type, item2.Num, 0, false);
					}
				}
				else
				{
					component6.gameObject.SetActive(false);
				}
			}
			return;
		}
		case -3:
		{
			ContinueIcon continueIcon2 = obj.GetComponentsInChildren<ContinueIcon>(true)[0];
			continueIcon2.setup(stageInfo, (Part_Stage.eGameover)(int)args["GameOverType"]);
			return;
		}
		case -9:
		{
			ContinueIcon continueIcon = obj.GetComponentsInChildren<ContinueIcon>(true)[0];
			continueIcon.setup(stageInfo, (Part_Stage.eGameover)(int)args["GameOverType"]);
			return;
		}
		case -4:
		{
			int[] usedItemType = Bridge.StageData.getUsedItemType(stageInfo.StageNo - 1);
			for (int k = 0; k < Constant.SetupItemMax; k++)
			{
				BoostItem component5 = obj.transform.Find("items/item_" + k.ToString("00")).GetComponent<BoostItem>();
				if (k >= stageInfo.ItemNum)
				{
					component5.gameObject.SetActive(false);
				}
				else if ((int)args["spItemIndex"] == k)
				{
					StageInfo.Item item = stageInfo.SpecialItems[k];
					bool flag = false;
					if (usedItemType != null)
					{
						int[] array = usedItemType;
						foreach (int num3 in array)
						{
							if (item.Type == num3)
							{
								flag = true;
								break;
							}
						}
					}
					GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
					int newItemStageNo = @object.GetComponent<StageDataTable>().getNewItemStageNo((Constant.Item.eType)item.Type);
					component5.setUseNewIconFlg(newItemStageNo == stageInfo.StageNo - 1 && !flag && Bridge.StageData.getClearCount(stageInfo.StageNo) < 1);
					component5.gameObject.SetActive(true);
					component5.setup((Constant.Item.eType)item.Type, item.Num);
					component5.setPrice((Constant.eMoney)item.PriceType, item.Price);
					component5.setState(BoostItem.eState.OFF);
					bool flag2 = (bool)args["bChangeNum"];
					bool flag3 = (bool)args["bChangePrice"];
					MessageResource instance = MessageResource.Instance;
					if (!flag2 && flag3)
					{
						textLabel_.text = instance.castCtrlCode(instance.getMessage(2484), 1, instance.getMessage(item.Type - 1 + 1000));
					}
					else
					{
						textLabel_.text = instance.castCtrlCode(instance.getMessage(2483), 1, instance.getMessage(item.Type - 1 + 1000));
					}
					UITweener[] componentsInChildren = component5.GetComponentsInChildren<UITweener>(true);
					UITweener[] array2 = componentsInChildren;
					foreach (UITweener uITweener in array2)
					{
						if ((flag2 && uITweener.tweenName == "special_item") || (flag3 && uITweener.tweenName == "special_price"))
						{
							uITweener.Reset();
							uITweener.Play(true);
						}
					}
					component5.bTweenReset = false;
					UIButton[] componentsInChildren2 = component5.GetComponentsInChildren<UIButton>(true);
					UIButton[] array3 = componentsInChildren2;
					foreach (UIButton uIButton in array3)
					{
						uIButton.enabled = false;
						uIButton.GetComponent<Collider>().enabled = false;
					}
				}
				else
				{
					component5.gameObject.SetActive(false);
				}
			}
			return;
		}
		case -2:
		{
			Transform transform7 = window_.transform.Find("pickup_arrow");
			transform7.localScale = new Vector3(1f, -1f, 1f);
			transform7.localRotation = Quaternion.Euler(0f, 0f, 180f);
			return;
		}
		case -17:
		{
			Transform transform6 = window_.transform.Find("pickup_arrow");
			transform6.localScale = new Vector3(1f, -1f, 1f);
			transform6.localRotation = Quaternion.Euler(0f, 0f, 180f);
			return;
		}
		case -6:
		{
			UILabel component7 = window_.transform.Find("Label").GetComponent<UILabel>();
			component7.text = msgRes_.castCtrlCode(msgRes_.getMessage(parameter.TextMsgIDs[0]), 1, args["AddItemStageNo"].ToString());
			Transform transform2 = window_.transform.Find("pickup_arrow");
			int num6 = (int)args["AddItemIndex"];
			Vector3 localPosition = transform2.localPosition;
			localPosition.x += 126 * num6;
			transform2.localPosition = localPosition;
			for (int num7 = 0; num7 < Constant.SetupItemMax; num7++)
			{
				BoostItem component8 = obj.transform.Find("items/item_" + num7.ToString("00")).GetComponent<BoostItem>();
				if (num7 != num6)
				{
					component8.gameObject.SetActive(false);
					continue;
				}
				StageDataTable component9 = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
				int addItemStageNo = component9.getAddItemStageNo(0);
				if (stageInfo.StageNo < addItemStageNo)
				{
					localPosition.y -= 10f;
					transform2.localPosition = localPosition;
					localPosition = component8.transform.localPosition;
					localPosition.y = -10f;
					component8.transform.localPosition = localPosition;
				}
				component8.setUseNewIconFlg(false);
				component8.transform.Find("icon_new").gameObject.SetActive(false);
				component8.gameObject.SetActive(true);
				component8.setup(Constant.Item.eType.Invalid, 0);
				component8.noneItem();
				component8.disable();
				component8.setState(BoostItem.eState.OFF);
				component8.transform.Find("item_Button/Background").GetComponent<UISprite>().color = Color.gray;
			}
			return;
		}
		case -8:
		case -7:
		case 2:
			return;
		case -15:
		{
			if (parameter.Place != 1 || parameter.ItemType == -1)
			{
				return;
			}
			int[] usedItemType2 = Bridge.StageData.getUsedItemType(stageNo);
			bool flag4 = false;
			GameData gameData = GlobalData.Instance.getGameData();
			if (GlobalData.Instance.getStageData(stageInfo.StageNo) != null)
			{
				int area = GlobalData.Instance.getStageData(stageInfo.StageNo).area;
				if (gameData.saleArea != null)
				{
					int[] saleArea = gameData.saleArea;
					foreach (int num11 in saleArea)
					{
						if (num11 == area)
						{
							flag4 = true;
							break;
						}
					}
				}
			}
			for (int num12 = 0; num12 < Constant.SetupItemMax; num12++)
			{
				BoostItem component16 = obj.transform.Find("items/item_" + num12.ToString("00")).GetComponent<BoostItem>();
				if (num12 >= stageInfo.ItemNum)
				{
					component16.gameObject.SetActive(false);
					continue;
				}
				StageInfo.Item item3 = stageInfo.Items[num12];
				if (parameter.ItemType == item3.Type)
				{
					bool flag5 = false;
					if (usedItemType2 != null)
					{
						int[] array4 = usedItemType2;
						foreach (int num14 in array4)
						{
							if (item3.Type == num14)
							{
								flag5 = true;
								break;
							}
						}
					}
					int num15 = -15;
					component16.setUseNewIconFlg(num15 == stageNo && !flag5 && Bridge.StageData.getClearCount(stageNo) < 1);
					component16.gameObject.SetActive(true);
					component16.setup((Constant.Item.eType)item3.Type, item3.Num);
					if (flag4)
					{
						component16.setPrice((Constant.eMoney)item3.PriceType, item3.Price * gameData.areaSalePercent / 100);
					}
					else
					{
						component16.setPrice((Constant.eMoney)item3.PriceType, item3.Price);
					}
					if (num15 == stageNo || stageNo == -15)
					{
						component16.setState(BoostItem.eState.OFF);
					}
					UIButton[] componentsInChildren3 = component16.GetComponentsInChildren<UIButton>(true);
					UIButton[] array5 = componentsInChildren3;
					foreach (UIButton uIButton2 in array5)
					{
						uIButton2.enabled = false;
						uIButton2.GetComponent<Collider>().enabled = false;
					}
				}
				else
				{
					component16.gameObject.SetActive(false);
				}
			}
			return;
		}
		}
		if (parameter.Place != 1 || parameter.ItemType == -1)
		{
			return;
		}
		int[] usedItemType3 = Bridge.StageData.getUsedItemType(stageNo);
		bool flag6 = false;
		GameData gameData2 = GlobalData.Instance.getGameData();
		if (GlobalData.Instance.getStageData(stageInfo.StageNo) != null)
		{
			int area2 = GlobalData.Instance.getStageData(stageInfo.StageNo).area;
			if (gameData2.saleArea != null)
			{
				int[] saleArea2 = gameData2.saleArea;
				foreach (int num18 in saleArea2)
				{
					if (num18 == area2)
					{
						flag6 = true;
						break;
					}
				}
			}
		}
		for (int num19 = 0; num19 < Constant.SetupItemMax; num19++)
		{
			BoostItem component17 = obj.transform.Find("items/item_" + num19.ToString("00")).GetComponent<BoostItem>();
			if (num19 >= stageInfo.ItemNum)
			{
				component17.gameObject.SetActive(false);
				continue;
			}
			StageInfo.Item item4 = stageInfo.Items[num19];
			if (parameter.ItemType == item4.Type)
			{
				bool flag7 = false;
				if (usedItemType3 != null)
				{
					int[] array6 = usedItemType3;
					foreach (int num21 in array6)
					{
						if (item4.Type == num21)
						{
							flag7 = true;
							break;
						}
					}
				}
				GameObject object2 = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
				int newItemStageNo2 = object2.GetComponent<StageDataTable>().getNewItemStageNo((Constant.Item.eType)item4.Type);
				component17.setUseNewIconFlg(newItemStageNo2 == stageNo && !flag7 && Bridge.StageData.getClearCount(stageNo) < 1);
				component17.gameObject.SetActive(true);
				component17.setup((Constant.Item.eType)item4.Type, item4.Num);
				if (flag6)
				{
					component17.setPrice((Constant.eMoney)item4.PriceType, item4.Price * gameData2.areaSalePercent / 100);
				}
				else
				{
					component17.setPrice((Constant.eMoney)item4.PriceType, item4.Price);
				}
				if (newItemStageNo2 == stageNo || stageNo == -15)
				{
					component17.setState(BoostItem.eState.OFF);
				}
				UIButton[] componentsInChildren4 = component17.GetComponentsInChildren<UIButton>(true);
				UIButton[] array7 = componentsInChildren4;
				foreach (UIButton uIButton3 in array7)
				{
					uIButton3.enabled = false;
					uIButton3.GetComponent<Collider>().enabled = false;
				}
			}
			else
			{
				component17.gameObject.SetActive(false);
			}
		}
	}

	private void setupExceptionAfter(int stageNo, GameObject obj, StageInfo.CommonInfo stageInfo, Hashtable args)
	{
		if (stageNo == -6)
		{
			int num = (int)args["AddItemIndex"];
			BoostItem component = obj.transform.Find("items/item_" + num.ToString("00")).GetComponent<BoostItem>();
			component.transform.Find("item_Button/Background").GetComponent<UISprite>().color = Color.white;
			Vector3 localPosition = component.transform.localPosition;
			localPosition.y = 0f;
			component.transform.localPosition = localPosition;
		}
	}

	private void onChangeText(int stageNo, GameObject obj, StageInfo.CommonInfo stageInfo, Hashtable args)
	{
		TutorialData.Parameter parameter = tutorialDict_[stageNo];
		switch (stageNo)
		{
		case -3:
		{
			DialogContinue component3 = GameObject.Find("StageContinue_Panel(Clone)").GetComponent<DialogContinue>();
			component3.getContinueIcon().setPrice(0);
			ContinueIcon continueIcon = obj.GetComponentsInChildren<ContinueIcon>(true)[0];
			continueIcon.setPrice(0);
			return;
		}
		case -9:
			return;
		case -15:
		{
			if (!(GameObject.Find("Boss_Setup_Panel(Clone)") != null))
			{
				return;
			}
			DialogBossSetup component = GameObject.Find("Boss_Setup_Panel(Clone)").GetComponent<DialogBossSetup>();
			BoostItem[] items = component.getItems();
			BoostItem[] array = items;
			foreach (BoostItem boostItem in array)
			{
				if (boostItem.getItemType() == (Constant.Item.eType)parameter.ItemType)
				{
					Constant.eMoney priceType = boostItem.getPriceType();
					boostItem.setState(BoostItem.eState.ON);
					boostItem.setPrice(priceType, 0);
					component.setItemLabelText((int)(boostItem.getItemType() + 1250 - 1), boostItem.getNum());
					break;
				}
			}
			for (int j = 0; j < Constant.SetupItemMax; j++)
			{
				BoostItem component2 = obj.transform.Find("items/item_" + j.ToString("00")).GetComponent<BoostItem>();
				if (component2.getItemType() == (Constant.Item.eType)parameter.ItemType)
				{
					Constant.eMoney priceType2 = component2.getPriceType();
					component2.setState(BoostItem.eState.ON);
					component2.setPrice(priceType2, 0);
					break;
				}
			}
			return;
		}
		}
		if (!(GameObject.Find("Setup_Panel(Clone)") != null))
		{
			return;
		}
		DialogSetup component4 = GameObject.Find("Setup_Panel(Clone)").GetComponent<DialogSetup>();
		BoostItem[] items2 = component4.getItems();
		BoostItem[] array2 = items2;
		foreach (BoostItem boostItem2 in array2)
		{
			if (boostItem2.getItemType() == (Constant.Item.eType)parameter.ItemType)
			{
				Constant.eMoney priceType3 = boostItem2.getPriceType();
				boostItem2.setState(BoostItem.eState.ON);
				boostItem2.setPrice(priceType3, 0);
				component4.setItemLabelText((int)(boostItem2.getItemType() + 1250 - 1), boostItem2.getNum());
				break;
			}
		}
		for (int l = 0; l < Constant.SetupItemMax; l++)
		{
			BoostItem component5 = obj.transform.Find("items/item_" + l.ToString("00")).GetComponent<BoostItem>();
			if (component5.getItemType() == (Constant.Item.eType)parameter.ItemType)
			{
				Constant.eMoney priceType4 = component5.getPriceType();
				component5.setState(BoostItem.eState.ON);
				component5.setPrice(priceType4, 0);
				break;
			}
		}
	}

	public void unload()
	{
		resourceDict_.Clear();
	}

	public void unload(string prefabName)
	{
		if (resourceDict_.ContainsKey(prefabName))
		{
			resourceDict_.Remove(prefabName);
		}
	}

	public void unload(int stageNo)
	{
		if (tutorialDict_.ContainsKey(stageNo))
		{
			unload(tutorialDict_[stageNo].PrefabName);
		}
	}

	public void reOpenSetup(int stageNo, TutorialDataTable.ePlace place, GameObject uiRoot, StageInfo.CommonInfo stageInfo)
	{
		if (isTutorial(stageNo) && getPlace(stageNo) == place)
		{
			TutorialData.Parameter parameter = tutorialDict_[stageNo];
			GameObject resource = getResource(parameter);
			int num = 0;
			onChangeText(stageNo, resource, stageInfo, null);
			setText(parameter, num);
			setupButton(parameter, num + 1);
		}
	}

	public IEnumerator play(int stageNo, TutorialDataTable.ePlace place, GameObject uiRoot, StageInfo.CommonInfo stageInfo, Hashtable args)
	{
		bItemTutorial = false;
		if (!isTutorial(stageNo) || getPlace(stageNo) != place)
		{
			yield break;
		}
		TutorialData.Parameter param = tutorialDict_[stageNo];
		if (place == TutorialDataTable.ePlace.Setup && param.TextMsgIDs[1] > 0)
		{
			if (param.ValidRange == 1)
			{
				if (Bridge.StageData.isClear(stageNo))
				{
					yield break;
				}
			}
			else
			{
				int[] usedItemType = Bridge.StageData.getUsedItemType(stageNo);
				for (int i = 0; i < Constant.SetupItemMax; i++)
				{
					StageInfo.Item itemInfo = stageInfo.Items[i];
					if (param.ItemType != itemInfo.Type)
					{
						continue;
					}
					bool bUsed = false;
					if (usedItemType != null)
					{
						int[] array = usedItemType;
						foreach (int itemType in array)
						{
							if (itemInfo.Type == itemType)
							{
								bUsed = true;
								break;
							}
						}
					}
					if (bUsed)
					{
						yield break;
					}
					break;
				}
			}
			bItemTutorial = true;
		}
		bPressButton_ = false;
		Input.enable = false;
		isPlaying = true;
		TweenGroup openTween = null;
		TweenGroup closeTween = null;
		GameObject obj = getResource(param);
		Transform arrow = window_.transform.Find("pickup_arrow");
		setArrowPos(arrow, param);
		arrow.gameObject.SetActive(true);
		setupException(stageNo, obj, stageInfo, args);
		bool bTutorialObj = obj.name.Contains("Tutorial");
		Collider[] cols = null;
		Vector3 pos = obj.transform.localPosition;
		if (!bTutorialObj)
		{
			obj.transform.localPosition = new Vector3(pos.x, pos.y, param.PrefabPosZ);
			cols = obj.GetComponentsInChildren<Collider>(true);
			Collider[] array2 = cols;
			foreach (Collider col2 in array2)
			{
				col2.enabled = false;
			}
		}
		else
		{
			obj.SetActive(true);
		}
		getTweenGroupe(obj, ref openTween, ref closeTween);
		setWindowPos(window_, param);
		window_.SetActive(true);
		UITweener[] tweens = window_.transform.Find("Button/gritter").GetComponentsInChildren<UITweener>();
		UITweener[] array3 = tweens;
		foreach (UITweener tween in array3)
		{
			tween.Reset();
		}
		window_.transform.Find("Button/Background").GetComponent<UISpriteAnimationEx>().SetClip(0);
		setText(param, 0);
		setupButton(param, 1);
		float waitTime5 = 0f;
		if (openTween != null)
		{
			openTween.Play();
			waitTime5 = openTween.getEndTime();
		}
		openTween_.Play();
		waitTime5 = Mathf.Max(openTween_.getEndTime(), waitTime5);
		float elapsedTime2 = 0f;
		while (elapsedTime2 < waitTime5)
		{
			elapsedTime2 += Time.deltaTime;
			if (stageNo == 1 || stageNo == 10)
			{
				Color gray = Color.Lerp(b: new Color(16f / 51f, 16f / 51f, 16f / 51f), a: Color.white, t: elapsedTime2 / waitTime5);
				Mesh mesh2 = obj.transform.Find("chara_01/chara_01_02").GetComponent<MeshFilter>().mesh;
				Color[] colors2 = mesh2.colors;
				for (int k = 0; k < colors2.Length; k++)
				{
					colors2[k] = gray;
				}
				mesh2.colors = colors2;
				mesh2 = obj.transform.Find("chara_01/bubble00").GetComponent<MeshFilter>().mesh;
				colors2 = mesh2.colors;
				for (int j = 0; j < colors2.Length; j++)
				{
					colors2[j] = gray;
				}
				mesh2.colors = colors2;
			}
			yield return null;
		}
		Input.enable = true;
		int page = 0;
		DialogManager dialogManager = GameObject.FindGameObjectWithTag("DialogManager").GetComponent<DialogManager>();
		DialogBase dialogQuit = dialogManager.getDialog(DialogManager.eDialog.AppQuit);
		while (true)
		{
			waitTime5 = 0f;
			while (!bPressButton_ && waitTime5 < 10f)
			{
				if (!dialogQuit.isOpen())
				{
					waitTime5 += Time.deltaTime;
				}
				yield return 0;
			}
			Constant.SoundUtil.PlayDecideSE();
			page++;
			if (!isExistsMsg(param, page))
			{
				break;
			}
			onChangeText(stageNo, obj, stageInfo, args);
			setText(param, page);
			setupButton(param, page + 1);
			bPressButton_ = false;
		}
		Input.enable = false;
		waitTime5 = 0f;
		closeTween_.Play();
		if (closeTween != null)
		{
			closeTween.Play();
			waitTime5 = closeTween.getEndTime();
		}
		waitTime5 = Mathf.Max(closeTween_.getEndTime(), waitTime5);
		elapsedTime2 = 0f;
		while (elapsedTime2 < waitTime5)
		{
			elapsedTime2 += Time.deltaTime;
			if (stageNo == 1 || stageNo == 10)
			{
				Color gray3 = new Color(16f / 51f, 16f / 51f, 16f / 51f);
				gray3 = Color.Lerp(gray3, Color.white, elapsedTime2 / waitTime5);
				Mesh mesh4 = obj.transform.Find("chara_01/chara_01_02").GetComponent<MeshFilter>().mesh;
				Color[] colors4 = mesh4.colors;
				for (int m = 0; m < colors4.Length; m++)
				{
					colors4[m] = gray3;
				}
				mesh4.colors = colors4;
				mesh4 = obj.transform.Find("chara_01/bubble00").GetComponent<MeshFilter>().mesh;
				colors4 = mesh4.colors;
				for (int l = 0; l < colors4.Length; l++)
				{
					colors4[l] = gray3;
				}
				mesh4.colors = colors4;
			}
			yield return null;
		}
		setupExceptionAfter(stageNo, obj, stageInfo, args);
		if (!bTutorialObj)
		{
			obj.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
			if (cols != null)
			{
				Collider[] array4 = cols;
				foreach (Collider col in array4)
				{
					col.enabled = true;
				}
			}
		}
		else
		{
			obj.SetActive(false);
		}
		window_.SetActive(false);
		isPlaying = false;
		Input.enable = true;
	}

	public void SetWindowDepth(float z)
	{
		if (!(window_ == null))
		{
			Vector3 localPosition = window_.transform.localPosition;
			window_.transform.localPosition = new Vector3(localPosition.x, localPosition.y, z);
		}
	}

	private void setText(TutorialData.Parameter data, int index)
	{
		if (data.StageNo != -4 && data.StageNo != -6)
		{
			textLabel_.text = msgRes_.getMessage(data.TextMsgIDs[index]);
		}
	}

	private void setupButton(TutorialData.Parameter data, int index)
	{
		bool flag = isExistsMsg(data, index);
		setActiveButton(eBtn.Close, !flag);
		setActiveButton(eBtn.Next, flag);
	}

	private void setActiveButton(eBtn btn, bool bActive)
	{
		buttons_[(int)btn].SetActive(bActive);
	}

	private bool isExistsMsg(TutorialData.Parameter data, int index)
	{
		if (data.TextMsgIDs.Length <= index)
		{
			return false;
		}
		if (data.TextMsgIDs[index] == -1)
		{
			return false;
		}
		return true;
	}

	private void setWindowPos(GameObject window, TutorialData.Parameter data)
	{
		Vector3 localPosition = window.transform.localPosition;
		window.transform.localPosition = new Vector3(data.Pos.x, data.Pos.y, localPosition.z);
	}

	private void setArrowPos(Transform arrow, TutorialData.Parameter data)
	{
		Vector3 localPosition = arrow.localPosition;
		localPosition.x = data.ArrowPos.x - data.Pos.x;
		localPosition.y = data.ArrowPos.y - data.Pos.y;
		arrow.localPosition = localPosition;
		arrow.localScale = Vector3.one;
		arrow.localRotation = Quaternion.identity;
		if (data.ArrowOffset == 1)
		{
			NGUIUtilScalableUIRoot.OffsetUI(arrow, true);
		}
		else if (data.ArrowOffset == 2)
		{
			NGUIUtilScalableUIRoot.OffsetUI(arrow, false);
		}
	}

	private void OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Button":
		case "NextButton":
			bPressButton_ = true;
			break;
		}
	}

	private void OnDestroy()
	{
		tutorialDict_.Clear();
		unload();
		instance_ = null;
	}

	private GameObject getResource(TutorialData.Parameter param)
	{
		string prefabName = param.PrefabName;
		if (!resourceDict_.ContainsKey(prefabName))
		{
			return null;
		}
		return resourceDict_[prefabName];
	}

	private bool isTutorial(int stageNo)
	{
		if (!tutorialDict_.ContainsKey(stageNo))
		{
			return false;
		}
		if (Constant.ParkStage.isParkStage(stageNo) && Bridge.StageData.getClearCount_Park(stageNo) > 0)
		{
			return false;
		}
		if (stageNo >= 0 && Bridge.StageData.getClearCount(stageNo) > 0)
		{
			return false;
		}
		return true;
	}

	private void getTweenGroupe(GameObject target, ref TweenGroup openTween, ref TweenGroup closeTween)
	{
		TweenGroup[] componentsInChildren = target.GetComponentsInChildren<TweenGroup>(true);
		TweenGroup[] array = componentsInChildren;
		foreach (TweenGroup tweenGroup in array)
		{
			switch (tweenGroup.getGroupName())
			{
			case "In":
				openTween = tweenGroup;
				break;
			case "Out":
				closeTween = tweenGroup;
				break;
			}
		}
	}
}
