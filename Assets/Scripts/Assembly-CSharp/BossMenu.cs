using System;
using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class BossMenu : MonoBehaviour
{
	private Vector3 basePos_;

	private Vector3 prevMapPos_;

	private Transform map_;

	private int duration_;

	public UILabel keyLabel_;

	public BoxCollider buttonColl_;

	public Animation gateAnm_;

	public UIButton button_;

	private GameData gameData_;

	private FadeMng fadeMng_;

	private bool bInitialized;

	private void Start()
	{
		basePos_ = base.transform.localPosition;
		gameData_ = GlobalData.Instance.getGameData();
		updateKeyLabel();
		bInitialized = true;
	}

	private void Update()
	{
		if (map_ == null)
		{
			map_ = base.transform.parent.parent.Find("DragCamera(Clone)/DragObject/MapCamera");
			prevMapPos_ = map_.localPosition;
		}
		if (bInitialized)
		{
			base.transform.localPosition += (prevMapPos_ - map_.localPosition) * 0.5f;
			prevMapPos_ = map_.localPosition;
			Vector3 vector = basePos_ - base.transform.localPosition;
			vector.z = 0f;
			float num = Time.deltaTime;
			if (num > 1f)
			{
				num = 1f;
			}
			base.transform.localPosition += vector * num;
		}
	}

	public void updateEnable(PartManager.ePart part, FadeMng fade)
	{
		fadeMng_ = fade;
		bool flag = true;
		bool flag2 = false;
		if (gameData_ == null)
		{
			gameData_ = GlobalData.Instance.getGameData();
		}
		flag = gameData_.isBossOpen;
		BossStageInfo bossData = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>().getBossData();
		flag2 = isTermsStageClear(bossData.Infos[0]);
		if (flag && flag2)
		{
			updateKeyLabel();
		}
		base.gameObject.SetActive(flag && flag2);
		if (base.gameObject.activeSelf)
		{
			base.gameObject.transform.Find("BossButton").gameObject.SetActive(true);
		}
	}

	public static bool isTermsStageClear(BossStageInfo.Info info)
	{
		int entryTerms = info.EntryTerms;
		if (entryTerms == -1)
		{
			return true;
		}
		if (!Bridge.StageData.isClear(entryTerms - 1))
		{
			return false;
		}
		return true;
	}

	public static bool isPrevLevelClear(EventStageInfo.Info info)
	{
		if (info.Level != 1)
		{
			int stageNo = info.Common.StageNo - 1;
			if (!Bridge.StageData.isClear(stageNo))
			{
				return false;
			}
		}
		return true;
	}

	private int getNowTimeBySeconds()
	{
		return DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
	}

	public void updateKeyLabel()
	{
		if (!(keyLabel_ == null))
		{
			KeyBubbleData keyBubbleData = GlobalData.Instance.getKeyBubbleData();
			keyLabel_.text = keyBubbleData.keyBubbleCount + "/" + keyBubbleData.keyBubbleMax;
		}
	}

	public void setButtonEnable(bool enable)
	{
		if (!(buttonColl_ == null))
		{
			buttonColl_.enabled = enable;
		}
	}

	public bool isKeyShortage()
	{
		KeyBubbleData keyBubbleData = GlobalData.Instance.getKeyBubbleData();
		return keyBubbleData.keyBubbleCount < keyBubbleData.keyBubbleMax;
	}

	public IEnumerator checkGateOpenEffect(bool bEffect)
	{
		if (!base.gameObject.activeSelf || gateAnm_ == null)
		{
			yield break;
		}
		KeyBubbleData keyData = GlobalData.Instance.getKeyBubbleData();
		if (keyData.keyBubbleCount >= keyData.keyBubbleMax)
		{
			if (bEffect)
			{
				base.transform.localPosition += Vector3.back * 355f;
				fadeMng_.setActive(FadeMng.eType.AllMask, true);
				yield return StartCoroutine(fadeMng_.startFade(FadeMng.eType.AllMask, 0f, 0.6f, 0.8f));
				Sound.Instance.playSe(Sound.eSe.SE_245_door);
				gateAnm_.Stop();
				gateAnm_.cullingType = AnimationCullingType.AlwaysAnimate;
				gateAnm_.Play("Boss_button_unlock_anm");
				while (gateAnm_.IsPlaying("Boss_button_unlock_anm"))
				{
					yield return 0;
				}
				yield return StartCoroutine(fadeMng_.startFade(FadeMng.eType.AllMask, 0.6f, 0f, 0.8f));
				fadeMng_.setActive(FadeMng.eType.AllMask, false);
				base.transform.localPosition += Vector3.forward * 355f;
			}
			UISprite sprite2 = gateAnm_.transform.Find("lock_bg").GetComponent<UISprite>();
			sprite2.enabled = false;
		}
		else
		{
			UISprite sprite = gateAnm_.transform.Find("lock_bg").GetComponent<UISprite>();
			sprite.enabled = true;
			gateAnm_.cullingType = AnimationCullingType.AlwaysAnimate;
			gateAnm_.clip = gateAnm_.GetClip("Boss_button_lock_anm");
			gateAnm_.Play("Boss_button_lock_anm");
			Color tempcolor = sprite.color;
			tempcolor.a = 1f;
			sprite.color = tempcolor;
		}
	}
}
