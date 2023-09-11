using System.Collections;
using UnityEngine;

public class Egg : GimmickBase
{
	private const int breakSpriteCount = 4;

	public int row;

	public int column;

	public StagePause_Boss stagePause_;

	public Part_BossStage part_;

	public BossBase bossBase_;

	private int nowState;

	private GameObject[] breakSprites;

	private UISprite firstSprite;

	public bool isExist { get; private set; }

	public IEnumerator EggInit()
	{
		breakSprites = new GameObject[4];
		for (int i = 0; i < 4; i++)
		{
			breakSprites[i] = base.transform.Find("egg_root/egg/break" + i.ToString("00")).gameObject;
		}
		firstSprite = breakSprites[0].GetComponent<UISprite>();
		isExist = true;
		yield return stagePause_.sync();
	}

	public IEnumerator HitEgg()
	{
		if (bossBase_.state == BossBase.eState.attacking)
		{
			yield break;
		}
		if (nowState < 3)
		{
			Sound.Instance.playSe(Sound.eSe.SE_514_snake_egg_break);
			base.GetComponent<Animation>().Play("Boss_02_egg_break_anm");
			breakSprites[nowState].SetActive(false);
			nowState++;
			breakSprites[nowState].SetActive(true);
		}
		else if (nowState >= 3)
		{
			Sound.Instance.playSe(Sound.eSe.SE_539_egg_broken);
			base.GetComponent<Animation>().Play("Boss_02_egg_burst_anm");
			while (base.GetComponent<Animation>().IsPlaying("Boss_02_egg_burst_anim"))
			{
				yield return stagePause_.sync();
			}
			breakSprites[nowState].SetActive(false);
			isExist = false;
			nowState = 0;
			bossBase_.state = BossBase.eState.angry;
			bossBase_.isCanHit = true;
			bossBase_.isMoving = false;
			Color color = firstSprite.color;
			color.a = 0f;
			firstSprite.color = color;
		}
	}

	public IEnumerator RegenerativeEgg()
	{
		breakSprites[nowState].SetActive(true);
		Color color = firstSprite.color;
		color.r = 1f;
		color.g = 1f;
		color.b = 1f;
		color.a = firstSprite.color.a;
		firstSprite.color = color;
		bool setAlpha = false;
		while (!setAlpha)
		{
			color.a += Time.deltaTime;
			if (color.a > 1f)
			{
				setAlpha = true;
				color.a = 1f;
			}
			firstSprite.color = color;
			yield return stagePause_.sync();
		}
		isExist = true;
	}

	public void ImmediatelyRegenerativeEgg()
	{
		if (nowState == 0)
		{
			breakSprites[nowState].SetActive(true);
			Color color = new Color(1f, 1f, 1f, 1f);
			firstSprite.color = color;
		}
		isExist = true;
	}
}
