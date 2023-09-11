using System.Collections;
using UnityEngine;

public class Editor_Counter : MonoBehaviour
{
	private const float OFFSE_X = 12f;

	private const int MAX_COUNT = 99;

	private int count_;

	[SerializeField]
	private UISprite sprite_00;

	[SerializeField]
	private UISprite sprite_01;

	private Animation anim;

	private bool bCountEnable = true;

	private void Awake()
	{
		anim = base.transform.GetComponent<Animation>();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public bool countDown()
	{
		if (bCountEnable)
		{
			count_--;
			changeAnim();
			return true;
		}
		return false;
	}

	public bool isCountOver()
	{
		return bCountEnable && count_ <= 0;
	}

	public bool isPlayingCountdown()
	{
		if (anim == null)
		{
			return false;
		}
		return anim.isPlaying;
	}

	public void setCountEnable(bool enable)
	{
		bCountEnable = enable;
	}

	private void changeSprite()
	{
		sprite_00.spriteName = "negative_bonus_number_" + (count_ % 10).ToString("00");
		sprite_00.MakePixelPerfect();
		sprite_01.spriteName = "negative_bonus_number_" + (count_ / 10).ToString("00");
		sprite_01.MakePixelPerfect();
		setPosition();
	}

	public void changeAnim()
	{
		StartCoroutine(playCountAnimInnner());
	}

	private IEnumerator playCountAnimInnner()
	{
		if (anim != null)
		{
			if (count_ >= 10)
			{
				anim.Play("Counter_count_anm_01");
			}
			else
			{
				anim.Play("Counter_count_anm_00");
			}
			while (anim.isPlaying)
			{
				yield return 0;
			}
		}
	}

	public void setPosition()
	{
		if (count_ >= 10)
		{
			if (!sprite_01.gameObject.activeSelf)
			{
				sprite_01.gameObject.SetActive(true);
			}
			float num = (sprite_01.transform.localScale.x - sprite_00.transform.localScale.x) / 2f;
			float x = num - (0.5f + sprite_00.pivotOffset.x) * sprite_00.transform.localScale.x + sprite_00.transform.localScale.x * 0.5f;
			float x2 = num - (0.5f + sprite_01.pivotOffset.x) * sprite_01.transform.localScale.x - sprite_01.transform.localScale.x * 0.5f;
			sprite_00.transform.localPosition = new Vector3(x, 0f, -0.5f);
			sprite_01.transform.localPosition = new Vector3(x2, 0f, -0.5f);
		}
		else
		{
			if (sprite_01.gameObject.activeSelf)
			{
				sprite_01.gameObject.SetActive(false);
			}
			float x3 = (0f - (0.5f + sprite_00.pivotOffset.x)) * sprite_00.transform.localScale.x;
			sprite_00.transform.localPosition = new Vector3(x3, 0f, -0.5f);
		}
	}

	public void setCount(int count)
	{
		count_ = count;
		changeSprite();
	}

	public int getCount()
	{
		return count_;
	}

	public void addCount(int addition)
	{
		count_ += addition;
		if (count_ > 99)
		{
			count_ = 99;
		}
		else if (count_ < 0)
		{
			count_ = 0;
		}
		changeSprite();
		updateImmediate();
	}

	public void setSpriteEnable(bool enable)
	{
		sprite_00.gameObject.SetActive(enable);
		sprite_01.gameObject.SetActive(enable);
	}

	private void updateImmediate()
	{
		if (sprite_00.gameObject.activeSelf)
		{
			sprite_00.gameObject.SetActive(false);
			sprite_00.gameObject.SetActive(true);
		}
		if (sprite_01.gameObject.activeSelf)
		{
			sprite_01.gameObject.SetActive(false);
			sprite_01.gameObject.SetActive(true);
		}
	}

	public IEnumerator playOverEffect()
	{
		anim.enabled = true;
		anim.Play("Counter_count_anm_02");
		while (anim.isPlaying)
		{
			yield return 0;
		}
		anim.enabled = false;
	}

	public bool isPlayingOverEffect()
	{
		return anim.IsPlaying("Counter_count_anm_02");
	}
}
