using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DebugMenuSytem : DebugMenuBase
{
	private enum eItem
	{
		AllReset = 0,
		FPS_Load = 1,
		Network_Load = 2,
		TimeScale = 3,
		Input = 4,
		SendMap = 5,
		Server = 6,
		Gameover = 7,
		MemberNo = 8,
		Max = 9
	}

	private GameObject fpsObj_;

	private GameObject networkObj_;

	private DialogManager dialogMng_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(9, "System"));
		fpsObj_ = GameObject.Find("FPS");
		networkObj_ = GameObject.Find("NetworkStatus");
		GameObject obj = GameObject.Find("DialogManager");
		if (!(obj == null))
		{
			dialogMng_ = obj.GetComponent<DialogManager>();
		}
	}

	public override void OnExecute()
	{
		if (IsPressCenterButton(0))
		{
			StartCoroutine(allReset());
		}
		loadDebugScene(ref fpsObj_, "FPS", "fps", eItem.FPS_Load);
		loadDebugScene(ref networkObj_, "NetworkStatus", "network", eItem.Network_Load);
		float timeScale = Time.timeScale;
		Time.timeScale = (float)Vary(3, timeScale, 1f, 1f, 99f);
		if (IsPressLeftButton(4))
		{
			Input.enable = false;
		}
		if (IsPressRightButton(4))
		{
			Input.enable = true;
		}
		if (IsPressCenterButton(5))
		{
			DialogBase dialog = dialogMng_.getDialog(DialogManager.eDialog.SendMap);
			if (dialog != null)
			{
				DialogSendMap dialogSendMap = dialog as DialogSendMap;
				dialogSendMap.setup();
				dialogMng_.StartCoroutine(dialogMng_.openDialog(dialogSendMap));
			}
		}
		if (IsPressCenterButton(7))
		{
			StartCoroutine(gameover());
		}
	}

	public override void OnDraw()
	{
		DrawItem(0, "All Reset", eItemType.CenterOnly);
		if (fpsObj_ == null)
		{
			DrawItem(1, "FPS Load", eItemType.CenterOnly);
		}
		else
		{
			DrawItem(1, "FPS UnLoad", eItemType.CenterOnly);
		}
		if (networkObj_ == null)
		{
			DrawItem(2, "Net Load", eItemType.CenterOnly);
		}
		else
		{
			DrawItem(2, "Net UnLoad", eItemType.CenterOnly);
		}
		DrawItem(3, "Time : " + Time.timeScale);
		DrawItem(4, "Input : " + Input.enableCount);
		DrawItem(5, "Open SendMapDialog", eItemType.CenterOnly);
		DrawItem(6, "REAL:203.104.129.141", eItemType.CenterOnly);
		DrawItem(7, "8 times game over", eItemType.CenterOnly);
	}

	private void loadDebugScene(ref GameObject target, string objName, string sceneName, eItem itemType)
	{
		if (target == null)
		{
			if (IsPressCenterButton((int)itemType))
			{
				Application.LoadLevelAdditive(sceneName);
				target = GameObject.Find(objName);
			}
		}
		else if (IsPressCenterButton((int)itemType))
		{
			Object.Destroy(target);
		}
	}

	private IEnumerator allReset()
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		yield return WWWWrap.create("debug/reset/");
		if (SaveData.IsInstance())
		{
			SaveData.Instance.reset();
		}
	}

	private IEnumerator gameover()
	{
		int stageNo = DummyPlayerData.Data.StageNo + 1;
		for (int i = 0; i < 8; i++)
		{
			NetworkMng.Instance.setup(Hash.StagePlay(stageNo, new List<BoostItem>()));
			yield return StartCoroutine(NetworkMng.Instance.download(API.StagePlay, true, false));
			Hashtable h = Hash.S2(stageNo, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, string.Empty);
			NetworkMng.Instance.setup(h);
			yield return StartCoroutine(NetworkMng.Instance.download(API.S2, true, false));
		}
	}
}
