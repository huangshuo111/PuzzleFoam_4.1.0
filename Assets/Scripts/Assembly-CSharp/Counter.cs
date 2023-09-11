using System.Collections;
using UnityEngine;

public class Counter : MonoBehaviour
{
	private const float OFFSE_X = 12f;

	private const int MAX_COUNT = 99;

	private int count_;

	[SerializeField]
	private UISprite sprite_00;

	[SerializeField]
	private UISprite sprite_01;

	[SerializeField]
	private bool bCountEnable = true;

	private Animation anim;

	private GameObject bg_root;

	private GameObject gas_root;

	private GameObject counter_worning;

	public Part_Stage part_;

	private Color gasColor = Color.white;

	private Bubble.eType parrentBubbleType;

	private void Awake()
	{
		anim = base.transform.GetComponent<Animation>();
		bg_root = base.transform.Find("bg_root").gameObject;
		gas_root = base.transform.Find("gas_root_00").gameObject;
		counter_worning = base.transform.Find("count_warning").gameObject;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void setupGasColor(Bubble.eType type)
	{
		int num = (int)((type - 100 >= Bubble.eType.Red) ? (type - 100) : Bubble.eType.Red);
		parrentBubbleType = type;
		gasColor = Constant.bubbleColor[num];
		UISprite[] componentsInChildren = base.transform.Find("gas_root_00").GetComponentsInChildren<UISprite>();
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			uISprite.color = gasColor;
		}
		bg_root.SetActive(false);
	}

	public bool countDown()
	{
		if (bCountEnable)
		{
			count_--;
			changeAnim(false);
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
		if (anim == null || !bCountEnable)
		{
			return false;
		}
		return anim.isPlaying;
	}

	public void setCountEnable(Bubble.eType bubbleType, bool enable, bool bGas)
	{
		bCountEnable = enable;
		anim.Stop();
		anim.enabled = enable;
		if (bubbleType == Bubble.eType.Counter)
		{
			bg_root.SetActive(enable);
		}
		else
		{
			bg_root.SetActive(false);
		}
		gas_root.SetActive(bGas);
		if (!enable)
		{
			count_ = 0;
			counter_worning.SetActive(false);
		}
	}

	private void changeSprite()
	{
		sprite_00.spriteName = "negative_bonus_number_" + (count_ % 10).ToString("00");
		sprite_00.MakePixelPerfect();
		sprite_01.spriteName = "negative_bonus_number_" + (count_ / 10).ToString("00");
		sprite_01.MakePixelPerfect();
		setPosition();
	}

	public void changeAnim(bool bTimeStop)
	{
		StartCoroutine(playCountAnimInnner(bTimeStop));
	}

	private IEnumerator playCountAnimInnner(bool bTimeStop)
	{
		if (counter_worning.activeSelf)
		{
			counter_worning.SetActive(false);
		}
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
		if (bCountEnable)
		{
			if (count_ > 3 || bTimeStop)
			{
				counter_worning.SetActive(false);
			}
			else
			{
				counter_worning.SetActive(true);
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

	public void setCount(int count, bool bTimeStop)
	{
		count_ = count;
		if (count_ > 3 || bTimeStop)
		{
			counter_worning.SetActive(false);
		}
		else
		{
			counter_worning.SetActive(true);
		}
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

	public void setSpriteEnable(bool enable, bool bGas)
	{
		sprite_00.gameObject.SetActive(enable);
		sprite_01.gameObject.SetActive(enable);
		gas_root.SetActive(bGas);
		if (!enable)
		{
			counter_worning.SetActive(false);
		}
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

	public IEnumerator playOverEffect(Bubble.eType bubbleType)
	{
		UISprite[] sprites = base.transform.Find("gas_root_01").GetComponentsInChildren<UISprite>();
		if (bubbleType >= Bubble.eType.CounterRed && bubbleType <= Bubble.eType.CounterBlack)
		{
			UISprite[] array = sprites;
			foreach (UISprite sp in array)
			{
				sp.color = gasColor;
			}
		}
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

	public void setCounterSpriteFade(bool fade, bool bWaitEffect = true)
	{
		if (bCountEnable)
		{
			StartCoroutine(setCounterSpriteFadeInner(fade));
			if (bWaitEffect && !part_.bUsingTimeStop)
			{
				gas_root.SetActive(!fade);
			}
		}
	}

	public void setGasEnable(bool enable)
	{
		gas_root.SetActive(enable);
		if (enable && count_ <= 3)
		{
			counter_worning.SetActive(true);
		}
		else
		{
			counter_worning.SetActive(false);
		}
	}

	public bool getCounterEnable()
	{
		return bCountEnable;
	}

	private IEnumerator setCounterSpriteFadeInner(bool fade)
	{
		while (anim.isPlaying)
		{
			yield return 0;
		}
		Color setColor = ((!fade) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f));
		UISprite uISprite = sprite_00;
		Color color = setColor;
		sprite_01.color = color;
		uISprite.color = color;
	}
}
