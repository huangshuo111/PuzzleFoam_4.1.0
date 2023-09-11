using System.Collections;

public class DebugMenuSound : DebugMenuBase
{
	private enum eItem
	{
		BGMPlay = 0,
		BGMStop = 1,
		BGMVolume = 2,
		Max = 3
	}

	private Sound sound_;

	private int index_;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(3, "Sound"));
		if (Sound.Instance != null)
		{
			sound_ = Sound.Instance;
			index_ = 0;
		}
	}

	public override void OnExecute()
	{
		if (!(sound_ == null))
		{
			index_ = (int)Vary(0, index_, 1, 0, 17);
			if (IsPressCenterButton(0))
			{
				sound_.playBgm((Sound.eBgm)index_, true);
			}
			if (IsPressCenterButton(1))
			{
				sound_.stopBgm();
			}
			float bgmVolume = sound_.getBgmVolume();
			bgmVolume = (float)Vary(2, bgmVolume, 0.1f, 0f, 1f);
			if (bgmVolume != sound_.getBgmVolume())
			{
				sound_.setBgmVolume(bgmVolume);
			}
		}
	}

	public override void OnDraw()
	{
		if (!(sound_ == null))
		{
			DrawItem(0, ((Sound.eBgm)index_).ToString());
			DrawItem(1, "Stop", eItemType.CenterOnly);
			DrawItem(2, "Volume : " + sound_.getBgmVolume());
		}
	}
}
