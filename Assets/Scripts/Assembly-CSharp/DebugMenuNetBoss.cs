using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class DebugMenuNetBoss : DebugMenuBase
{
	private enum eItem
	{
		Clear = 0,
		Boss = 1,
		Max = 2
	}

	private enum eType
	{
		Owl = 0,
		Crab = 1,
		Skeleton = 2
	}

	private int levelCount = 5;

	private eType clearType;

	private int clearLevel;

	private string boss;

	private StageDataTable stageTbl_;

	private int bossTypes_;

	private int bossLevel_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(2, "NetBoss"));
		if (Bridge.PlayerData.isInstance())
		{
			GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			stageTbl_ = dataTable.GetComponent<StageDataTable>();
			clearType = eType.Owl;
			clearLevel = 1;
			boss = "Clear: " + clearType.ToString() + "  Lv." + clearLevel;
		}
	}

	public override void OnDraw()
	{
		DrawItem(0, "All Clear", eItemType.CenterOnly);
		DrawItem(1, boss, eItemType.Default);
	}

	public override void OnExecute()
	{
		if (IsPressCenterButton(0))
		{
			StartCoroutine(clearAllBoss());
		}
		if (IsPressRightButton(1))
		{
			if (clearLevel < levelCount)
			{
				clearLevel++;
			}
			else if (clearType != eType.Skeleton)
			{
				clearType++;
				clearLevel = 1;
			}
			boss = "Clear: " + clearType.ToString() + "  Lv." + clearLevel;
			DrawItem(1, boss, eItemType.Default);
		}
		if (IsPressLeftButton(1))
		{
			if (clearLevel > 1)
			{
				clearLevel--;
			}
			else if (clearType != 0)
			{
				clearType--;
				clearLevel = levelCount;
			}
			boss = "Clear: " + clearType.ToString() + "  Lv." + clearLevel;
			DrawItem(1, boss, eItemType.Default);
		}
		if (IsPressCenterButton(1))
		{
			StartCoroutine(clearBoss((int)clearType, clearLevel + 1));
		}
	}

	private IEnumerator clearAllBoss()
	{
		Input.enable = false;
		BossDataTable bossDataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		if (bossDataTable != null)
		{
			yield return StartCoroutine(bossDataTable.download(true, true));
			BossListData bossDatas = bossDataTable.getBossData();
			for (int i = 0; i < bossDatas.bossList.Length; i++)
			{
				BossListData.BossLevelData[] bossLevelList = bossDatas.bossList[i].bossLevelList;
				foreach (BossListData.BossLevelData lvData in bossLevelList)
				{
					bossTypes_ = i;
					bossLevel_ = lvData.level;
					yield return StartCoroutine(NetworkMng.Instance.download(BossStageClear, false, false));
				}
			}
			Input.enable = true;
		}
		else
		{
			Input.enable = true;
		}
	}

	private IEnumerator clearBoss(int type, int level)
	{
		Input.enable = false;
		BossDataTable bossDataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		if (bossDataTable != null)
		{
			yield return StartCoroutine(bossDataTable.download(true, true));
			BossListData bossDatas = bossDataTable.getBossData();
			bossTypes_ = type;
			bossLevel_ = level - 1;
			yield return StartCoroutine(NetworkMng.Instance.download(BossStageClear, false, false));
			Input.enable = true;
		}
		else
		{
			Input.enable = true;
		}
	}

	private WWW BossStageClear(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string empty = string.Empty;
		WWWWrap.addPostParameter("bossNo", Constant.Boss.convBossInfoToNo(bossTypes_, 0));
		WWWWrap.addPostParameter("bossLv", bossLevel_);
		return WWWWrap.create("debug/bossclear/");
	}
}
