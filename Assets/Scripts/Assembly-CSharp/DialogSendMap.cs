using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class DialogSendMap : DialogScrollListBase
{
	public enum eSendPart
	{
		Map = 0,
		EventMap = 1,
		CollaborationMap = 2,
		ChallengeMap = 3,
		Max = 4
	}

	private Transform UserTotalStar_Label_;

	private Transform NeedStar_Label_;

	private StageIconDataTable iconTbl_;

	private int currentStage_;

	private PartBase pb;

	private eSendPart part;

	private Vector3 gridBasePos = Vector3.zero;

	public override void OnCreate()
	{
		base.OnCreate();
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "sendMap_item")) as GameObject;
		Utility.setParent(gameObject, base.transform, false);
		gameObject.SetActive(false);
		gridBasePos = grid_.transform.localPosition;
		base.init(gameObject);
		createLine();
		addLine();
	}

	private void CurrentPartSet()
	{
		switch (partManager_.currentPart)
		{
		case PartManager.ePart.Map:
			part = eSendPart.Map;
			break;
		case PartManager.ePart.EventMap:
			part = eSendPart.EventMap;
			break;
		case PartManager.ePart.CollaborationMap:
			part = eSendPart.CollaborationMap;
			break;
		case PartManager.ePart.ChallengeMap:
			part = eSendPart.ChallengeMap;
			break;
		}
	}

	public virtual void setup()
	{
		CurrentPartSet();
		int num = -1;
		if (part <= eSendPart.Map)
		{
			Part_Map part_Map = partManager_.execPart as Part_Map;
			num = part_Map.getMapNo();
		}
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		UserTotalStar_Label_ = base.transform.Find("window/UserTotalStar_Label");
		NeedStar_Label_ = base.transform.Find("window/NeedStar_Label");
		iconTbl_ = @object.GetComponent<StageIconDataTable>();
		currentStage_ = Bridge.PlayerData.getCurrentStage();
		TreasureDataTable component = @object.GetComponent<TreasureDataTable>();
		int totalStar = Bridge.StageData.getTotalStar();
		int num2 = iconTbl_.getMaxStageIconsNum() * 3;
		int treasureNum = Bridge.PlayerData.getTreasureNum();
		TreasureInfo.BoxInfo info = component.getInfo(treasureNum);
		int num3 = 0;
		int num4 = 0;
		string empty = string.Empty;
		if (info != null)
		{
			num3 = info.Star;
			num4 = num3 - totalStar;
			empty = MessageResource.Instance.getMessage(8833);
			empty = MessageResource.Instance.castCtrlCode(empty, 1, num4.ToString());
		}
		else
		{
			empty = MessageResource.Instance.getMessage(8836);
		}
		Debug.Log("合計★:" + totalStar + " / " + num2 + "__" + empty);
		UserTotalStar_Label_.GetComponent<UILabel>().text = totalStar + " / " + num2;
		NeedStar_Label_.GetComponent<UILabel>().text = empty;
		int maxMapNum = iconTbl_.getMaxMapNum();
		clear();
		for (int i = 0; i < maxMapNum; i++)
		{
			GameObject item = createItem(null);
			itemList_.Add(item);
		}
		int[] saleArea = GlobalData.Instance.getGameData().saleArea;
		int[] saleStageItemArea = GlobalData.Instance.getGameData().saleStageItemArea;
		bool[] array = new bool[maxMapNum];
		bool[] array2 = new bool[maxMapNum];
		int num5 = 6;
		for (int j = 0; j < maxMapNum; j++)
		{
			if (saleStageItemArea == null)
			{
				break;
			}
			int num6 = j * num5 + 1;
			num6 = ((num6 != 1) ? num6 : 0);
			int num7 = (j + 1) * num5;
			int[] array3 = saleStageItemArea;
			foreach (int num8 in array3)
			{
				if (num6 <= num8 && num8 <= num7)
				{
					array[j] = true;
					break;
				}
			}
		}
		for (int l = 0; l < maxMapNum; l++)
		{
			if (saleArea == null)
			{
				break;
			}
			int num9 = l * num5 + 1;
			num9 = ((num9 != 1) ? num9 : 0);
			int num10 = (l + 1) * num5;
			int[] array4 = saleArea;
			foreach (int num11 in array4)
			{
				if (num9 <= num11 && num11 <= num10)
				{
					array2[l] = true;
					break;
				}
			}
		}
		int num12 = 0;
		int num13 = 0;
		int num14 = 0;
		int num15 = 0;
		GameObject gameObject = null;
		for (int n = 0; n < maxMapNum; n++)
		{
			StageIconData dataByIndex = iconTbl_.getDataByIndex(n);
			num12 = n * 90 + 1;
			num13 = dataByIndex.IconDatas[dataByIndex.IconDatas.Length - 1].StageNo + 1;
			int num16 = 0;
			for (int num17 = num12 - 1; num17 <= num13 - 1; num17++)
			{
				Network.StageData stageData = GlobalData.Instance.getStageData(num17);
				if (stageData != null)
				{
					num16 += stageData.star;
				}
			}
			num14 = num16;
			num15 = (num13 - n * 90) * 3;
			Debug.Log("マップ:" + (n + 1) + "__ステージ" + num12 + "~" + num13 + "__★" + num14 + " / " + num15 + "\u3000セールフラグ\u3000" + array2[n]);
			gameObject = itemList_[n];
			if (n < 9)
			{
				gameObject.transform.Find("MapNo_Sprite0").GetComponent<UISprite>().spriteName = "stage_number_" + (n + 1).ToString("00");
				Debug.Log(" 111 i : " + n);
			}
			else
			{
				Debug.Log(" 222 i : " + n);
				int num18 = n + 1;
				int num19 = num18;
				num19 /= 10;
				gameObject.transform.Find("MapNo_Sprite0").GetComponent<UISprite>().spriteName = "stage_number_" + num19.ToString("00");
				gameObject.transform.Find("MapNo_Sprite1").gameObject.SetActive(true);
				num19 = num18;
				num19 %= 10;
				Vector3 position = gameObject.transform.Find("MapNo_Sprite0").gameObject.transform.position;
				position.x = -224.7f;
				position.y = 3f;
				position.z = 0f;
				gameObject.transform.Find("MapNo_Sprite1").GetComponent<UISprite>().spriteName = "stage_number_" + num19.ToString("00");
				gameObject.transform.Find("MapNo_Sprite0").localPosition = position;
			}
			gameObject.GetComponent<DialogSortNum>().SortNum = n + 1;
			string text = " ~ ";
			gameObject.transform.Find("Stage_Label").GetComponent<UILabel>().text = num12 + text + num13;
			gameObject.transform.Find("Sale").gameObject.SetActive(array2[n]);
			if (!array2[n])
			{
				gameObject.transform.Find("Sale").gameObject.SetActive(array[n]);
			}
			if (array[n])
			{
				gameObject.transform.Find("Sale/Sale_image").GetComponent<UISprite>().spriteName = "UI_shop_sale2_map";
			}
			if (array2[n])
			{
				gameObject.transform.Find("Sale/Sale_image").GetComponent<UISprite>().spriteName = "UI_shop_sale_map";
			}
			if (num15 <= num14)
			{
				gameObject.transform.Find("completed").gameObject.SetActive(true);
				Vector3 localPosition = gameObject.transform.Find("Star_Label").localPosition;
				gameObject.transform.Find("Star_Label").localPosition = new Vector3(localPosition.x, -20f, localPosition.z);
			}
			gameObject.transform.Find("Star_Label").GetComponent<UILabel>().text = num14 + " / " + num15;
			Transform transform = gameObject.transform.Find("Item_Button");
			transform.gameObject.name = transform.gameObject.name + "_" + (n + 1).ToString("00");
			UIButton component2 = transform.GetComponent<UIButton>();
			UILabel component3 = transform.Find("ReceiveLabel").gameObject.GetComponent<UILabel>();
			UISlicedSprite component4 = transform.Find("Background").gameObject.GetComponent<UISlicedSprite>();
			if (num12 <= currentStage_ + 1)
			{
				if (num == n)
				{
					transform.gameObject.SetActive(false);
					continue;
				}
				transform.gameObject.SetActive(true);
				component3.gameObject.SetActive(true);
				component4.spriteName = "UI_mapjump_button";
				transform.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			else
			{
				transform.gameObject.SetActive(true);
				component3.gameObject.SetActive(false);
				component4.spriteName = "UI_mapjump_look";
				transform.gameObject.GetComponent<BoxCollider>().enabled = false;
			}
		}
		SortName();
		for (int num20 = 0; num20 < maxMapNum; num20++)
		{
			addItem(itemList_[num20], num20);
		}
		repositionItem();
		dragPanel_.ResetPosition();
		grid_.transform.localPosition = gridBasePos / 2f;
	}

	private void SortName()
	{
		itemList_.Sort(delegate(GameObject a, GameObject b)
		{
			int num = 0;
			int sortNum = a.GetComponent<DialogSortNum>().SortNum;
			int sortNum2 = b.GetComponent<DialogSortNum>().SortNum;
			string text = sortNum.ToString("D2");
			string text2 = sortNum2.ToString("D2");
			return (text != null && text2 != null) ? string.Compare(text2, text) : 0;
		});
		itemList_.Reverse();
	}

	public void DestroyContents()
	{
		Transform[] componentsInChildren = base.transform.Find("DragPanel/contents").GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name != "contents")
			{
				Object.Destroy(transform.gameObject);
			}
		}
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("Item_Button"))
		{
			Constant.SoundUtil.PlayDecideSE();
			string buttonNameParse = trigger.name.Substring(trigger.name.LastIndexOf("_") + 1, trigger.name.Length - (trigger.name.LastIndexOf("_") + 1));
			int mapNo = int.Parse(buttonNameParse) - 1;
			if (mapNo < 0)
			{
				mapNo = 0;
			}
			if (iconTbl_.getMaxMapNum() - 1 < mapNo)
			{
				mapNo = iconTbl_.getMaxMapNum() - 1;
			}
			if (part == eSendPart.Map)
			{
				Part_Map partMap = partManager_.execPart as Part_Map;
				StartCoroutine(partMap.SendMap(mapNo, Part_Map.MapMoveDirection.Jump, false));
			}
			else
			{
				int focusStageNo = mapNo * 90;
				switch (part)
				{
				case eSendPart.EventMap:
				{
					Part_EventMap partE = partManager_.execPart as Part_EventMap;
					partE.SendMap(focusStageNo);
					break;
				}
				case eSendPart.CollaborationMap:
				{
					Part_CollaborationMap partCo = partManager_.execPart as Part_CollaborationMap;
					partCo.SendMap(focusStageNo);
					break;
				}
				case eSendPart.ChallengeMap:
				{
					Part_ChallengeMap partCha = partManager_.execPart as Part_ChallengeMap;
					partCha.SendMap(focusStageNo);
					break;
				}
				}
			}
		}
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "xxxButton":
			Constant.SoundUtil.PlayDecideSE();
			break;
		}
	}

	public override void OnClose()
	{
		clear();
	}
}
