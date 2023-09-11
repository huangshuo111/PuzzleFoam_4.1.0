public class BossSoundSkullDragon : BossSoundBase
{
	private int se_count;

	public void playMovingSE()
	{
		if (bPlay)
		{
			se_count++;
			if (se_count > 10)
			{
				Sound.Instance.playSe(Sound.eSe.SE_549_skelton_glowl);
				se_count = 0;
			}
		}
	}
}
