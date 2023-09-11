namespace Network
{
	public class GachaDrawList
	{
		public class AvatarInfo
		{
			public int index;

			public int rank;

			public int throwCharacter;

			public int supportCharacter;

			public int specialSkill;
		}

		public class RatioList
		{
			public string ratio_ss;

			public string ratio_s;

			public string ratio_a;

			public string ratio_b;
		}

		public int resultCode;

		public AvatarInfo[] gachaList;

		public RatioList totalRatioList;

		public AvatarInfo[] gachaList2;

		public RatioList totalRatioList2;
	}
}
