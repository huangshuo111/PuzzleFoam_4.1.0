using System;
using System.Text;
using UnityEngine;

public class MinilenThanksDataTable : MonoBehaviour
{
	private MinilenThanks thanksInfo_;

	public void load()
	{
		byte[] bytes = Aes.DecryptFromFile(StageDataTable.getMinilenThanksDataPath());
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		string @string = encoding.GetString(bytes);
		thanksInfo_ = Xml.DeserializeObject<MinilenThanks>(@string) as MinilenThanks;
		int num = 5;
	}

	public MinilenThanks.MinilenThanksInfo getInfo(int ID)
	{
		if (thanksInfo_ == null)
		{
			return null;
		}
		return Array.Find(thanksInfo_.MinilenThanksInfoList, (MinilenThanks.MinilenThanksInfo m) => m.ID == ID);
	}

	public MinilenThanks.MinilenThanksInfo[] getAllInfo()
	{
		if (thanksInfo_ == null)
		{
			return null;
		}
		return thanksInfo_.MinilenThanksInfoList;
	}
}
