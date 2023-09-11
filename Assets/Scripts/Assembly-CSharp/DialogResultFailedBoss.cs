using System.Collections;
using UnityEngine;

public class DialogResultFailedBoss : DialogResultBase
{
	public BossBase bossBase_;

	private UISlider bossHPSlider;

	private UILabel hp_;

	private Transform confBtn;

	private UILabel levelLabel_;

	private UISprite plateSprite;

	private ClearCheckBox checkBox_;

	private GameObject checkMark_;

	public override void OnCreate()
	{
		base.OnCreate();
		checkBox_ = base.transform.Find("window/Checkboxs").GetComponent<ClearCheckBox>();
		checkMark_ = base.transform.Find("window/Checkboxs/Checkbox_00/Checkmark").gameObject;
		hp_ = base.transform.Find("window/Label_BossHP").GetComponent<UILabel>();
		bossHPSlider = base.transform.Find("window/Boss_gauge/Bossgauge").GetComponent<UISlider>();
		confBtn = base.transform.Find("window/ConfirmButton");
		plateSprite = base.transform.Find("window/Boss_plate/bg00").GetComponent<UISprite>();
		levelLabel_ = base.transform.Find("window/Title/Label_Number").GetComponent<UILabel>();
	}

	public IEnumerator showBossStage(StageInfo.CommonInfo stageInfo, int type, int level)
	{
		Input.enable = false;
		plateSprite.spriteName = "Bos_setup_plate_boss_" + type.ToString("00");
		plateSprite.MakePixelPerfect();
		levelLabel_.text = level.ToString();
		checkMark_.SetActive(false);
		bossHPSlider.numberOfSteps = bossBase_.bossHpSlider.numberOfSteps;
		bossHPSlider.sliderValue = bossBase_.bossHpSlider.sliderValue;
		UIButtonMessage mes = confBtn.GetComponent<UIButtonMessage>();
		mes.target = base.gameObject;
		mes.functionName = "Close";
		yield return dialogManager_.StartCoroutine(_show(Constant.Boss.convBossInfoToNo(type, 0)));
		Input.enable = true;
	}

	private void setTargetText(StageInfo.CommonInfo info)
	{
		MessageResource instance = MessageResource.Instance;
		labels_[4].text = Constant.MessageUtil.getTargetMsg(info, instance, Constant.MessageUtil.eTargetType.Continue);
	}

	private void Close()
	{
		Constant.SoundUtil.PlayDecideSE();
		dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
	}
}
