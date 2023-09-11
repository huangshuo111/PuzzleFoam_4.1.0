using Bridge;
using UnityEngine;

public class ExpMenu : MonoBehaviour
{
	private enum eText
	{
		Lv = 0,
		Exp = 1
	}

	[SerializeField]
	private float ChangeTime = 300f;

	[SerializeField]
	private UILabel LvLabel;

	[SerializeField]
	private UILabel PercentLabel;

	[SerializeField]
	private GameObject Max;

	[SerializeField]
	private UISlider Slider;

	private RewardDataTable rewardTable_;

	private float startTime_;

	private eText currentText_;

	private void Awake()
	{
		rewardTable_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<RewardDataTable>();
		startTime_ = Time.time;
		changeText(eText.Lv);
	}

	public void changeText()
	{
		changeText((currentText_ == eText.Lv) ? eText.Exp : eText.Lv);
	}

	private void changeText(eText type)
	{
		currentText_ = type;
		startTime_ = Time.time;
		if (currentText_ == eText.Lv)
		{
			LvLabel.transform.parent.gameObject.SetActive(true);
			PercentLabel.transform.parent.gameObject.SetActive(false);
			Max.SetActive(false);
		}
		else
		{
			LvLabel.transform.parent.gameObject.SetActive(false);
		}
		update();
	}

	public void update()
	{
		if (GlobalData.Instance.getGameData() != null)
		{
			int level = PlayerData.getLevel();
			int exp = PlayerData.getExp();
			float num = 0f;
			bool flag = level >= rewardTable_.getLvCap();
			if (currentText_ == eText.Exp)
			{
				PercentLabel.transform.parent.gameObject.SetActive(!flag);
				Max.SetActive(flag);
			}
			if (!flag)
			{
				int exp2 = rewardTable_.getExp(level);
				num = (float)exp / (float)exp2;
				Slider.sliderValue = num;
			}
			else
			{
				Slider.sliderValue = 1f;
			}
			if (currentText_ == eText.Lv)
			{
				LvLabel.text = level.ToString();
			}
			else
			{
				PercentLabel.text = ((int)(num * 100f)).ToString();
			}
		}
	}

	private void Update()
	{
		float num = Time.time - startTime_;
		if (num > ChangeTime)
		{
			startTime_ = Time.time;
			num = 0f;
			changeText((currentText_ == eText.Lv) ? eText.Exp : eText.Lv);
		}
	}

	private void OnEnable()
	{
		startTime_ = Time.time;
	}
}
