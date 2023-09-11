using System.Collections;
using Bridge;
using UnityEngine;

public class StageIcon : MonoBehaviour
{
	private const float DIGID_SIZE = 18f;

	[SerializeField]
	private UISprite Icon;

	[SerializeField]
	private UIButton Button;

	[SerializeField]
	private int StageNo = 1;

	[SerializeField]
	private GameObject Crown;

	[SerializeField]
	private GameObject StarRoot;

	[SerializeField]
	private GameObject[] Starts = new GameObject[Constant.StarMax];

	[SerializeField]
	private GameObject[] StartEffcts = new GameObject[Constant.StarMax];

	[SerializeField]
	private GameObject GrayStarRoot;

	[SerializeField]
	private UISprite[] Numbers = new UISprite[3];

	[SerializeField]
	private float Digit2Offset = 10f;

	private int star_;

	private StageInfo.Info stageInfo_;

	private void Awake()
	{
	}

	public void setSprite()
	{
		if (isEventStage() || isCollaborationStage())
		{
			int num = 0;
			Icon.spriteName = "stageicon_" + (num % 4).ToString("D3");
			return;
		}
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		if ((bool)@object)
		{
			stageInfo_ = @object.GetComponent<StageDataTable>().getInfo(getStageNo());
		}
		if (stageInfo_ != null)
		{
			int area = stageInfo_.Area;
			Icon.spriteName = "stageicon_" + (area % 4).ToString("D3");
		}
	}

	public IEnumerator setup(bool bPlayAnimation, bool bEvent)
	{
		if (getStageNo() < 0)
		{
			yield break;
		}
		if (isEventStage())
		{
			GrayStarRoot = base.transform.Find("Notes/GrayNotes").gameObject;
			for (int j = 0; j < Starts.Length; j++)
			{
				Starts[j] = base.transform.Find("Notes/Note_" + j.ToString("00")).gameObject;
				Starts[j].SetActive(false);
			}
		}
		if (!isCollaborationStage())
		{
			star_ = Bridge.StageData.getStar(getStageNo());
		}
		bool bClear = Bridge.StageData.isClear(getStageNo());
		if (isCollaborationStage() || isChallengeStage())
		{
			bClear = false;
		}
		Crown.SetActive(star_ == Constant.StarMax);
		if (isCollaborationStage() || isChallengeStage())
		{
			Crown.SetActive(Bridge.StageData.isClear(getStageNo()));
		}
		StarRoot.SetActive(bClear);
		GrayStarRoot.SetActive(bClear);
		if (bClear)
		{
			for (int i = 0; i < Constant.StarMax; i++)
			{
				Starts[i].SetActive(i < star_);
			}
		}
		if (bPlayAnimation && star_ > 0 && !isCollaborationStage() && !isChallengeStage())
		{
			GrayStarRoot.SetActive(true);
			string animeName = "GetStar" + star_ + "_Map_anm";
			showGetStarEffect(true, star_);
			StarRoot.GetComponent<Animation>().Play(animeName);
			while (StarRoot != null && StarRoot.GetComponent<Animation>().isPlaying)
			{
				yield return 0;
			}
			showGetStarEffect(false, star_);
		}
	}

	private void showGetStarEffect(bool bShow, int star)
	{
		for (int i = 0; i < star; i++)
		{
			if (StartEffcts[i] != null)
			{
				StartEffcts[i].gameObject.SetActive(bShow);
			}
		}
	}

	public IEnumerator playOpenProduct()
	{
		GameObject eff = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.StageOpenEff);
		Transform parent = eff.transform.parent;
		eff.SetActive(true);
		Utility.setParent(eff, base.transform, false);
		eff.GetComponent<Animation>().Play();
		while (eff.GetComponent<Animation>().isPlaying)
		{
			yield return 0;
		}
		eff.SetActive(false);
		Utility.setParent(eff, parent, false);
	}

	public void enable()
	{
		setSprite();
		NGUIUtility.enable(Button, false);
	}

	public void disable()
	{
		Icon.spriteName = "stageicon_999";
		NGUIUtility.disable(Button, false);
	}

	public int getStageNo()
	{
		return StageNo;
	}

	public void setStageNo(int stageNo)
	{
		StageNo = stageNo;
		stageNo++;
		if (isEventStage() || isCollaborationStage())
		{
			stageNo = stageNo % 10000 - 1;
		}
		for (int i = 0; i < 3; i++)
		{
			GameObject gameObject = Numbers[i].gameObject;
			gameObject.SetActive(false);
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.x = -18f + (float)i * 18f;
			gameObject.transform.localPosition = localPosition;
		}
		int length = stageNo.ToString().Length;
		switch (length)
		{
		case 3:
		{
			for (int k = 0; k < length; k++)
			{
				Numbers[k].gameObject.SetActive(true);
				setNumber(stageNo, k, k);
			}
			break;
		}
		case 2:
		{
			for (int j = 0; j < length; j++)
			{
				Numbers[j].gameObject.SetActive(true);
				Vector3 localPosition2 = Numbers[j].transform.localPosition;
				localPosition2.x += Digit2Offset;
				Numbers[j].transform.localPosition = localPosition2;
				setNumber(stageNo, j, j);
			}
			break;
		}
		default:
			Numbers[1].gameObject.SetActive(true);
			setNumber(stageNo, 0, 1);
			break;
		}
	}

	private void setNumber(int no, int digit, int index)
	{
		string text = no.ToString();
		no = int.Parse(text.Substring(digit, 1));
		Numbers[index].spriteName = "stage_number_" + no.ToString("00");
	}

	private bool isEventStage()
	{
		return StageNo > 10000 && StageNo < 20000;
	}

	private bool isChallengeStage()
	{
		return StageNo > 20000 && StageNo < 30000;
	}

	private bool isCollaborationStage()
	{
		return StageNo > 11 * Constant.Event.BaseEventStageNo && StageNo < 12 * Constant.Event.BaseEventStageNo;
	}

	public void setStageNum_Park(int stage_num)
	{
		StageNo = stage_num;
		stage_num++;
	}
}
