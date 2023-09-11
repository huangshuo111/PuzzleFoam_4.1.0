using System.Collections;
using Network;
using UnityEngine;

public class DialogCrossMission : DialogBase
{
	private GameObject CrossMission01;

	private GameObject CrossMission02;

	private GameObject CrossMission03;

	private CollaboBBInfo BBInfo;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnCreate()
	{
		CrossMission01 = base.transform.Find("window/CrossMission_01").gameObject;
		CrossMission02 = base.transform.Find("window/CrossMission_02").gameObject;
		CrossMission03 = base.transform.Find("window/CrossMission_03").gameObject;
	}

	public void setup(CollaboBBInfo _BBinfo)
	{
		BBInfo = _BBinfo;
		SetCrossMission(CrossMission01, 0);
		SetCrossMission(CrossMission02, 1);
		SetCrossMission(CrossMission03, 2);
	}

	private void SetCrossMission(GameObject _CrossMission, int _Num)
	{
		if (BBInfo.missionList.Length > _Num)
		{
			string text = BBInfo.missionList[_Num].description + " (" + BBInfo.missionList[_Num].achieve + "/" + BBInfo.missionList[_Num].goal + ")";
			_CrossMission.transform.Find("CrossMission_Label").GetComponent<UILabel>().text = text;
			_CrossMission.transform.Find("Reward_Text").GetComponent<UILabel>().text = "x" + BBInfo.missionList[_Num].rewardValue;
			if (BBInfo.missionList[_Num].rewardType == 1)
			{
				_CrossMission.transform.Find("Reward_Coin").GetComponent<UISprite>().gameObject.SetActive(true);
				_CrossMission.transform.Find("Reward_Jewel").GetComponent<UISprite>().gameObject.SetActive(false);
			}
			else if (BBInfo.missionList[_Num].rewardType == 2)
			{
				_CrossMission.transform.Find("Reward_Coin").GetComponent<UISprite>().gameObject.SetActive(false);
				_CrossMission.transform.Find("Reward_Jewel").GetComponent<UISprite>().gameObject.SetActive(true);
			}
			if (BBInfo.missionList[_Num].rewarded)
			{
				_CrossMission.transform.Find("Checkmark").GetComponent<UISprite>().gameObject.SetActive(true);
			}
			else
			{
				_CrossMission.transform.Find("Checkmark").GetComponent<UISprite>().gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator OnButton(GameObject trig)
	{
		switch (trig.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton":
		{
			Constant.SoundUtil.PlayDecideSE();
			bool bLaunchPackage = false;
			for (int i = 0; i < BBInfo.androidPackageNameList.Length; i++)
			{
				if (GKUnityPluginController.Instance.InstalledPackage(BBInfo.androidPackageNameList[i]))
				{
					GKUnityPluginController.Instance.LaunchPackage(BBInfo.androidPackageNameList[i]);
					bLaunchPackage = true;
					break;
				}
			}
			if (!bLaunchPackage)
			{
				Application.OpenURL(BBInfo.downloadUrl);
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
		}
	}
}
