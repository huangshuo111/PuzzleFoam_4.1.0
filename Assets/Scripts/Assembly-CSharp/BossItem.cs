using UnityEngine;

public class BossItem : MonoBehaviour
{
	public class ContentsData
	{
		public int bossType;

		public int level;
	}

	private const int BossNameMessage = 8200;

	public UILabel Name_Label;

	public UILabel Level_Lavel;

	public UILabel Coin_Lavel;

	public UILabel Heart_Lavel;

	public UILabel Jewel_Lavel;

	public UILabel Item_Num_Lavel;

	public UILabel HP_Label;

	public UISlider Hp_Slider;

	public Transform Unlock_Trans;

	public Transform Lock_Trans;

	public UISprite Boss_Sprite;

	public UISprite Item_Sprite;

	private ContentsData contentData;

	public void setup(int maxHP, int currentHP, int level, bool isBattle, BossStageInfo.BossData bossData)
	{
		MessageResource instance = MessageResource.Instance;
		Name_Label.text = instance.getMessage(8200 + bossData.BossType);
		Level_Lavel.text = instance.getMessage(22) + " " + level;
		Unlock_Trans.Find("bg_root/battle_icon").gameObject.SetActive(isBattle);
		Boss_Sprite.spriteName = "Bos_item_boss_icon_" + bossData.BossType.ToString("00");
		Boss_Sprite.MakePixelPerfect();
		int num = ((currentHP != 0) ? currentHP : maxHP);
		Hp_Slider.numberOfSteps = 20;
		Hp_Slider.sliderValue = (float)num / (float)maxHP;
		HP_Label.text = num + "/" + maxHP;
		BossStageInfo.LevelInfo levelInfo = bossData.LevelInfos[level - 1];
		Coin_Lavel.text = getRewardNum(levelInfo, 1).ToString("N0");
		Heart_Lavel.text = getRewardNum(levelInfo, 3).ToString("N0");
		Item_Num_Lavel.text = getRewardNum(levelInfo, 0).ToString("N0");
		Jewel_Lavel.text = getRewardNum(levelInfo, 2).ToString("N0");
		Item_Sprite.spriteName = "item_" + (levelInfo.Rewards[3] % 1000).ToString("000") + "_00";
		Item_Sprite.gameObject.transform.localScale = new Vector3(40f, 40f, 1f);
		contentData = new ContentsData();
		contentData.bossType = bossData.BossType;
		contentData.level = level;
	}

	public void setLockState(bool isLock)
	{
		Unlock_Trans.gameObject.SetActive(!isLock);
		Lock_Trans.gameObject.SetActive(isLock);
	}

	public void setupButton(GameObject target)
	{
		UIButtonMessage componentInChildren = Unlock_Trans.GetComponentInChildren<UIButtonMessage>();
		componentInChildren.target = target;
		componentInChildren.functionName = "OnButton";
		componentInChildren.gameObject.name = "BossButton";
	}

	private int getRewardNum(BossStageInfo.LevelInfo levelInfo, int type)
	{
		if (type == 0)
		{
			return levelInfo.RewardNums[3];
		}
		for (int i = 0; i < levelInfo.Rewards.Length; i++)
		{
			if (levelInfo.Rewards[i] == type)
			{
				return levelInfo.RewardNums[i];
			}
		}
		return 0;
	}

	public ContentsData getContentData()
	{
		return contentData;
	}
}
