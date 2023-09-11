public class BossSoundOwl : BossSoundBase
{
	public void playMovingSE()
	{
		if (bPlay)
		{
			Sound.Instance.playSe(Sound.eSe.SE_541_fukurou_fly);
		}
	}
}
