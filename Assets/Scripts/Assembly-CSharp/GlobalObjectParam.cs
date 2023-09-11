public class GlobalObjectParam
{
	public enum eObject
	{
		DataTable = 0,
		MapCamera = 1,
		MapMainUI = 2,
		MapMainUI2 = 3,
		StageIcon = 4,
		CurrentStageEffect = 5,
		Player = 6,
		TreasureBox = 7,
		HeartEffect = 8,
		FriendRoot = 9,
		Friend = 10,
		Tutorial = 11,
		Loading = 12,
		GateEffect = 13,
		HighScoreItem = 14,
		HighScoreDummyItem = 15,
		HighScoreBorderItem = 16,
		Line = 17,
		StageOpenEff = 18,
		Connecting = 19,
		EventMap = 20,
		EventStageIcon = 21,
		Cloud = 22,
		Tips = 23,
		ChallengeMap = 24,
		ChallengeStageIcon = 25,
		Wave = 26,
		CollaborationMap = 27,
		CollaborationStageIcon = 28,
		ExpEffect = 29,
		CoinEffect = 30,
		FriendParkUI = 31,
		Max = 32
	}

	private static string[] objNames_ = new string[32]
	{
		"DataTable", "DragCamera", "Main_UI_Panel", "Main_UI2_Panel", "StageIconRoot", "cerrent_eff", "Player_icon", "TreasureBoxRoot", "Heart_eff_Panel", "FriendRoot",
		"Friend_icon", "Tutorial_000_Panel", "Loading_Panel", "gate_eff", "Highscore_Item", "Highscore_dummy_Item", "line_omission_item", "line_item", "StageOpen_eff", "Connecting_Panel",
		"BG_Panel_Event", "EventStageIconRoot", "BG_Panel_Cloud", "Tips_Panel", "BG_Panel_Challenge", "ChallengeStageIconRoot", "BG_Panel_Wave", "BG_Panel_Collaboration", "CollaborationStageIconRoot", "Coin_eff_Panel",
		"Exp_eff_Panel", "ParkFriendMain_Panel"
	};

	public static string getName(eObject type)
	{
		return objNames_[(int)type];
	}
}
