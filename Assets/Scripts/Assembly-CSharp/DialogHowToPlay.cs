using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class DialogHowToPlay : DialogBase
{
	private enum eLabel
	{
		Page = 0,
		Main = 1,
		Max = 2
	}

	private enum eArrow
	{
		Left = 0,
		Right = 1,
		Max = 2
	}

	private const float FadeTime = 0.3f;

	private const float MoveValue = 60f;

	private Dictionary<int, UITexture> imageList_ = new Dictionary<int, UITexture>();

	private UILabel[] labels_ = new UILabel[2];

	private GameObject[] arrows_ = new GameObject[2];

	private int currentPage_;

	private GameObject window_;

	private int pageMax_;

	private List<int> numList_ = new List<int>();

	private DialogHowToPlayIndex indexDialog_;

	private void Awake()
	{
	}

	public override void OnCreate()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "Label_Detail":
				labels_[1] = transform.GetComponent<UILabel>();
				break;
			case "Label_Page":
				labels_[0] = transform.GetComponent<UILabel>();
				break;
			case "arrow_00":
				arrows_[0] = transform.gameObject;
				break;
			case "arrow_01":
				arrows_[1] = transform.gameObject;
				break;
			case "window":
				window_ = transform.gameObject;
				break;
			}
		}
	}

	public void init()
	{
	}

	public void setup(int page, DialogHowToPlayIndex indexDialog, bool _isChallenge)
	{
		currentPage_ = page;
		indexDialog_ = indexDialog;
		pageMax_ = 0;
		numList_.Clear();
		TutorialManager instance = TutorialManager.Instance;
		int num = Bridge.PlayerData.getCurrentStage() + 1;
		bool flag = false;
		if (partManager_.currentPart == PartManager.ePart.Park)
		{
			flag = true;
		}
		else if (partManager_.currentPart == PartManager.ePart.Stage)
		{
			Part_Stage part_Stage = Object.FindObjectOfType<Part_Stage>();
			if ((bool)part_Stage && part_Stage.isParkStage)
			{
				flag = true;
				_isChallenge = true;
			}
		}
		int[] hoToPlayPageClearStages = instance.HoToPlayPageClearStages;
		foreach (int num2 in hoToPlayPageClearStages)
		{
			if (!_isChallenge)
			{
				if (num >= num2)
				{
					numList_.Add(pageMax_);
					pageMax_++;
				}
			}
			else
			{
				numList_.Add(pageMax_);
				pageMax_++;
			}
		}
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (!_isChallenge)
		{
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialTreasure))
			{
				numList_.Add(400);
				pageMax_++;
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialLineFriend))
			{
				numList_.Add(401);
				pageMax_++;
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialEvent))
			{
				numList_.Add(402);
				pageMax_++;
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialEventMap))
			{
				numList_.Add(403);
				pageMax_++;
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialCoinBubble))
			{
				numList_.Add(404);
				pageMax_++;
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialKeyBubble))
			{
				numList_.Add(405);
				pageMax_++;
			}
			if (otherData.isFlag(SaveOtherData.eFlg.BossMenu))
			{
				numList_.Add(406);
				pageMax_++;
			}
			BossDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
			BossListData bossData = component.getBossData();
			for (int j = 0; j < bossData.bossList.Length; j++)
			{
				if (component.isPlayed(j, 1))
				{
					numList_.Add(600 + j);
					pageMax_++;
				}
			}
			if (component.isPlayed(0, 2) || component.isPlayed(1, 1))
			{
				numList_.Add(408);
				pageMax_++;
			}
			bool flag2 = ResourceLoader.Instance.isJapanResource();
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialGachaMukCollabo))
			{
				numList_.Add(416);
				pageMax_++;
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialStageSkip))
			{
				numList_.Add(418);
				pageMax_++;
			}
		}
		if (flag)
		{
			numList_.Add(650);
			pageMax_++;
			numList_.Add(651);
			pageMax_++;
			numList_.Add(653);
			pageMax_++;
			if (Bridge.StageData.getPlayCount(500003) > 0)
			{
				numList_.Add(652);
				pageMax_++;
			}
		}
		foreach (KeyValuePair<int, UITexture> item in imageList_)
		{
			item.Value.gameObject.SetActive(false);
		}
		UITexture image = getImage(numList_[currentPage_]);
		image.gameObject.SetActive(true);
		if ((bool)image.gameObject.GetComponent<iTween>())
		{
			Object.Destroy(image.GetComponent<iTween>());
		}
		image.color = new Color(1f, 1f, 1f, 1f);
		Vector3 localPosition = image.transform.localPosition;
		image.transform.localPosition = new Vector3(0f, localPosition.y, localPosition.z);
		setPageText(currentPage_);
		setMainText(currentPage_);
		arrows_[1].SetActive(currentPage_ < pageMax_ - 1);
		arrows_[0].SetActive(currentPage_ > 0);
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "arrow_00":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(scrollPrev());
			break;
		case "arrow_01":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(scrollNext());
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private IEnumerator scrollNext()
	{
		yield return StartCoroutine(scroll(currentPage_ + 1, true));
	}

	private IEnumerator scrollPrev()
	{
		yield return StartCoroutine(scroll(currentPage_ - 1, false));
	}

	private IEnumerator scroll(int next, bool bNext)
	{
		Input.enable = false;
		GameObject[] array = arrows_;
		foreach (GameObject arrow in array)
		{
			arrow.SetActive(false);
		}
		StartCoroutine(imageOut(currentPage_, bNext));
		yield return new WaitForSeconds(0.15f);
		StartCoroutine(imageIn(next, bNext));
		yield return new WaitForSeconds(0.15f);
		yield return new WaitForEndOfFrame();
		getImage(numList_[currentPage_]).gameObject.SetActive(false);
		setPageText(next);
		setMainText(next);
		currentPage_ = next;
		indexDialog_.disableNew(currentPage_);
		GameObject[] array2 = arrows_;
		foreach (GameObject arrow2 in array2)
		{
			arrow2.SetActive(true);
		}
		if (currentPage_ >= pageMax_ - 1)
		{
			arrows_[1].SetActive(false);
		}
		else if (currentPage_ <= 0)
		{
			arrows_[0].SetActive(false);
		}
		Input.enable = true;
	}

	private IEnumerator imageOut(int index, bool bNext)
	{
		index = numList_[index];
		if (bNext)
		{
			StartCoroutine(move(getImage(index).gameObject, 0f, -60f));
		}
		else
		{
			StartCoroutine(move(getImage(index).gameObject, 0f, 60f));
		}
		StartCoroutine(fade(getImage(index), 1f, 0f));
		yield break;
	}

	private IEnumerator imageIn(int index, bool bNext)
	{
		index = numList_[index];
		getImage(index).gameObject.SetActive(true);
		if (bNext)
		{
			StartCoroutine(move(getImage(index).gameObject, 60f, 0f));
		}
		else
		{
			StartCoroutine(move(getImage(index).gameObject, -60f, 0f));
		}
		StartCoroutine(fade(getImage(index), 0f, 1f));
		yield break;
	}

	private IEnumerator move(GameObject obj, float from, float to)
	{
		Vector3 pos = obj.transform.localPosition;
		pos.x = from;
		obj.transform.localPosition = pos;
		iTween.MoveTo(obj, iTween.Hash("x", to, "time", 0.3f, "islocal", true));
		while ((bool)obj.GetComponent<iTween>())
		{
			yield return 0;
		}
	}

	private IEnumerator fade(UIWidget widget, float from, float to)
	{
		Color color = widget.color;
		color.a = from;
		widget.color = color;
		float startTime = Time.time;
		while (Time.time - startTime < 0.3f)
		{
			color.a = Mathf.Lerp(from, to, (Time.time - startTime) / 0.3f);
			widget.color = color;
			yield return null;
		}
		color.a = to;
		widget.color = color;
	}

	private void setMainText(int page)
	{
		MessageResource instance = MessageResource.Instance;
		string message = instance.getMessage(1500 + numList_[page]);
		labels_[1].text = message;
	}

	private void setPageText(int page)
	{
		MessageResource instance = MessageResource.Instance;
		string message = instance.getMessage(1474);
		message = instance.castCtrlCode(message, 1, (page + 1).ToString());
		message = instance.castCtrlCode(message, 2, pageMax_.ToString());
		labels_[0].text = message;
	}

	private UITexture getImage(int index)
	{
		if (imageList_.ContainsKey(index))
		{
			return imageList_[index];
		}
		Debug.Log("index = " + index);
		string text = ((index <= 99) ? index.ToString("00") : index.ToString("000"));
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "HowToPlayImage_" + text)) as GameObject;
		string text2 = gameObject.name.Replace("HowToPlayImage", "howtoplay").Replace("(Clone)", string.Empty);
		if (!ResourceLoader.Instance.isJapanResource())
		{
			text2 += "_en";
		}
		Debug.Log("file_path = " + text2);
		if (gameObject.GetComponent<UITexture>().mainTexture == null)
		{
			gameObject.GetComponent<UITexture>().mainTexture = ResourceLoader.Instance.loadFromGameResource(text2);
		}
		Utility.setParent(gameObject, window_.transform, false);
		UITexture component = gameObject.GetComponent<UITexture>();
		imageList_.Add(index, component);
		return component;
	}
}
