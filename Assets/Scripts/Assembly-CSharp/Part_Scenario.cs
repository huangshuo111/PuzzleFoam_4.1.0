using System.Collections;
using UnityEngine;

public class Part_Scenario : PartBase
{
	private Scenario scenario_;

	private Hashtable args_ = new Hashtable();

	private Scenario.ePlace place_ = Scenario.ePlace.Invalid;

	private bool isCollaboration;

	public override IEnumerator setup(Hashtable args)
	{
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable stageTbl = dataTable.GetComponent<StageDataTable>();
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.Main, 10, false, 0, 5));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ScenarioChara, 0, false, 1, 5));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ScenarioChara, 1, false, 2, 5));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ScenarioChara, 2, false, 3, 5));
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.ScenarioChara, 3, false, 4, 5));
		foreach (string key in args.Keys)
		{
			args_[key] = args[key];
		}
		scenario_ = Scenario.Instance;
		int stageNo = (int)args["StageNo"];
		place_ = (Scenario.ePlace)(int)args["Place"];
		yield return StartCoroutine(scenario_.load(stageNo, place_, uiRoot, stageTbl));
		scenario_.setup();
		isCollaboration = scenario_.isCollaboration;
		if (place_ == Scenario.ePlace.Begin)
		{
			Sound.Instance.playBgm(Sound.eBgm.BGM_006_Event, true);
		}
		else
		{
			Sound.Instance.playBgm(Sound.eBgm.BGM_005_Ending, true);
		}
		StartCoroutine(execute());
	}

	private IEnumerator execute()
	{
		while (!Input.enable)
		{
			yield return 0;
		}
		yield return StartCoroutine(scenario_.play());
		int stageNo = (int)args_["StageNo"];
		if (place_ == Scenario.ePlace.Begin)
		{
			if (stageNo == 500000)
			{
				partManager.requestTransition(PartManager.ePart.Park, null, FadeMng.eType.Cutout, true);
			}
			else
			{
				partManager.requestTransition(PartManager.ePart.Stage, args_, FadeMng.eType.Cutout, true);
			}
		}
		else if (isCollaboration)
		{
			partManager.requestTransition(PartManager.ePart.CollaborationMap, args_, true);
		}
		else if (Scenario.Instance.isScenario_Park(stageNo, Scenario.ePlace.End))
		{
			partManager.requestTransition(PartManager.ePart.Park, args_, true);
		}
		else
		{
			partManager.requestTransition(PartManager.ePart.Map, args_, true);
		}
	}

	public override IEnumerator OnDestroyCB()
	{
		scenario_.cleanup();
		yield break;
	}
}
