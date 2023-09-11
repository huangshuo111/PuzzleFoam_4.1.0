using UnityEngine;

public class LevelUpSprite : MonoBehaviour
{
	[SerializeField]
	private UISprite[] Numbers;

	[SerializeField]
	private float Center;

	[SerializeField]
	private float Left = -18f;

	[SerializeField]
	private float Right = 8f;

	public void setup(int lv)
	{
		for (int i = 0; i < Numbers.Length; i++)
		{
			UISprite uISprite = Numbers[i];
			if (lv > 9)
			{
				setPos(uISprite, (i != 0) ? Right : Left);
				setSprite(uISprite, getNum(lv, i + 1));
			}
			else if (i == 0)
			{
				setPos(uISprite, Center);
				setSprite(uISprite, getNum(lv, 1));
			}
			else
			{
				uISprite.gameObject.SetActive(false);
			}
		}
	}

	private int getNum(int num, int digit)
	{
		string text = num.ToString();
		if (digit > text.Length)
		{
			return -1;
		}
		return int.Parse(text.Substring(digit - 1, 1));
	}

	private void setPos(UISprite sprite, float offset)
	{
		Vector3 localPosition = sprite.transform.localPosition;
		localPosition.x = offset;
		sprite.transform.localPosition = localPosition;
	}

	private void setSprite(UISprite sprite, int lv)
	{
		sprite.spriteName = "UI_levelup_" + lv;
		sprite.MakePixelPerfect();
	}
}
