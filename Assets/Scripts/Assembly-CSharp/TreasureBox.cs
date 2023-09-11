using System.Collections;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
	[SerializeField]
	private UILabel NumLabel;

	[SerializeField]
	private UISprite Sprite;

	[SerializeField]
	private int ID;

	private int star_ = 20;

	private TreasureInfo.BoxInfo boxInfo_ = new TreasureInfo.BoxInfo();

	public int getNeedStar()
	{
		return star_;
	}

	public int getID()
	{
		return ID;
	}

	public void setID(int id)
	{
		ID = id;
	}

	public void init(int totalStar, TreasureInfo.BoxInfo info, BoxData data)
	{
		boxInfo_ = info;
		setID(data.ID);
		star_ = info.Star;
		base.transform.localPosition = new Vector3(data.Pos.x, data.Pos.y, base.transform.localPosition.z);
		setup(totalStar);
	}

	public void setup(int totalStar)
	{
		int num = Mathf.Min(totalStar, star_);
		string message = MessageResource.Instance.getMessage(19);
		message = MessageResource.Instance.castCtrlCode(message, 1, num.ToString());
		message = MessageResource.Instance.castCtrlCode(message, 2, star_.ToString());
		NumLabel.text = message;
	}

	public bool checkComplete(int totalStar)
	{
		if (totalStar >= star_)
		{
			return true;
		}
		return false;
	}

	public TreasureInfo.BoxInfo getTreasure()
	{
		return boxInfo_;
	}

	public IEnumerator clearEvent()
	{
		open();
		yield break;
	}

	public void open()
	{
		setSprite(true);
	}

	private void setSprite(bool bOpen)
	{
		if (bOpen)
		{
			Sprite.spriteName = "UI_box_001";
		}
		else
		{
			Sprite.spriteName = "UI_box_000";
		}
	}
}
