using System.Collections;
using UnityEngine;

public class DebugMenuGacha : DebugMenuBase
{
	private enum eItem
	{
		Avatar = 0,
		ThrowCharacter = 1,
		SupportCharacter = 2,
		LocalDataClear = 3,
		AvatarComp = 4,
		AvatarLevelMax = 5,
		UISetting = 6,
		Max = 7
	}

	private PartBase part_;

	private DialogManager dialogManager_;

	private MessageResource msg;

	private int throwCharaID;

	private string throwCharaName = string.Empty;

	private int supportCharaID;

	private string supportCharaName = string.Empty;

	private string[] throwCharaNames = new string[12]
	{
		"Bubblen", "Bubblen March", "Kururun", "Drank", "Zenchan", "Maita", "Chackn", "Bobblen", "Pirates Bubblen", "Jitome Bubblen",
		"Dranko", "Babby"
	};

	private string[] supportCharaNames = new string[12]
	{
		"Bobblen", "Bobblen March", "Cororon", "Red Zenchan", "Red Maita", "Chackn", "Drank", "Kururun", "Bubblen", "Jitome Bobblen",
		"Bobby", "Pirates Bobblen"
	};

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(7, "Avatar"));
		part_ = Object.FindObjectOfType(typeof(Part_Map)) as Part_Map;
		if (part_ == null)
		{
			part_ = Object.FindObjectOfType(typeof(Part_EventMap)) as Part_EventMap;
		}
		if (part_ == null)
		{
			part_ = Object.FindObjectOfType(typeof(Part_CollaborationMap)) as Part_CollaborationMap;
		}
		dialogManager_ = Object.FindObjectOfType(typeof(DialogManager)) as DialogManager;
		throwCharaName = throwCharaNames[throwCharaID];
	}

	public override void OnExecute()
	{
		if (part_ != null)
		{
			if (IsPressCenterButton(1))
			{
				SetThrowCharacter(throwCharaNames[throwCharaID]);
			}
			throwCharaID = (int)Vary(1, throwCharaID, 1, 0, throwCharaNames.Length - 1);
			if (IsPressCenterButton(2))
			{
				SetSupportCharacter(supportCharaNames[supportCharaID]);
			}
			supportCharaID = (int)Vary(2, supportCharaID, 1, 0, supportCharaNames.Length - 1);
		}
	}

	private IEnumerator OpenAvatarGacha()
	{
		DialogAvatarCollection collection = dialogManager_.getDialog(DialogManager.eDialog.AvatarCollection) as DialogAvatarCollection;
		if (collection.isOpen())
		{
			collection.DestroyContents();
			yield return StartCoroutine(collection.close());
		}
		DialogAvatarGacha gacha = dialogManager_.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
		if (!gacha.isOpen())
		{
			bool isFree2 = false;
			isFree2 = GlobalData.Instance.getGameData().gachaTicket > 0 || GlobalData.Instance.getGameData().isFirstGacha;
			gacha.setup(isFree2);
			yield return StartCoroutine(gacha.open());
		}
	}

	private IEnumerator OpenAvatarCollection()
	{
		DialogAvatarGacha gacha = dialogManager_.getDialog(DialogManager.eDialog.AvatarGacha) as DialogAvatarGacha;
		if (gacha.isOpen())
		{
			yield return StartCoroutine(gacha.close());
		}
		DialogAvatarCollection collection = dialogManager_.getDialog(DialogManager.eDialog.AvatarCollection) as DialogAvatarCollection;
		if (!collection.isOpen())
		{
			collection.setup();
			yield return StartCoroutine(collection.open());
		}
	}

	private void SetThrowCharacter(string charaName)
	{
		int num = -1;
		switch (charaName)
		{
		case "Bubblen":
			num = 0;
			break;
		case "Bubblen March":
			num = 1;
			break;
		case "Kururun":
			num = 2;
			break;
		case "Drank":
			num = 3;
			break;
		case "Zenchan":
			num = 4;
			break;
		case "Maita":
			num = 5;
			break;
		case "Chackn":
			num = 6;
			break;
		case "Bobblen":
			num = 7;
			break;
		case "Pirates Bubblen":
			num = 8;
			break;
		case "Jitome Bubblen":
			num = 9;
			break;
		case "Dranko":
			num = 10;
			break;
		case "Babby":
			num = 11;
			break;
		}
		if (num >= 0)
		{
			PlayerPrefs.SetInt("ThrowChara", num);
		}
		Debug.Log("<color=yellow>投げキャラクターを ID" + num + " : " + charaName + "に変更しました!</color>");
	}

	private void SetSupportCharacter(string charaName)
	{
		int num = -1;
		switch (charaName)
		{
		case "Bobblen":
			num = 0;
			break;
		case "Bobblen March":
			num = 1;
			break;
		case "Cororon":
			num = 2;
			break;
		case "Red Drank":
			num = 3;
			break;
		case "Red Zenchan":
			num = 4;
			break;
		case "Red Maita":
			num = 5;
			break;
		case "Chackn":
			num = 6;
			break;
		case "Drank":
			num = 7;
			break;
		case "Kururun":
			num = 8;
			break;
		case "Bubblen":
			num = 9;
			break;
		case "Jitome Bobblen":
			num = 10;
			break;
		case "Bobby":
			num = 11;
			break;
		case "Pirates Bobblen":
			num = 12;
			break;
		}
		if (num >= 0)
		{
			PlayerPrefs.SetInt("SupportChara", num);
		}
		Debug.Log("<color=yellow>渡すキャラクターを ID" + num + " : " + charaName + "に変更しました!</color>");
	}

	public override void OnDraw()
	{
		if (part_ != null)
		{
			DrawItem(1, "Throw :\n" + throwCharaNames[throwCharaID]);
			DrawItem(2, "Support :\n" + supportCharaNames[supportCharaID]);
		}
	}
}
