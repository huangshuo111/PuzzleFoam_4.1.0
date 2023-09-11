public class SaveKeys
{
	private static string optionDataRootKey_ = "OptionData";

	private static string otherDataRootKey_ = "OtherData";

	private static string networkDataRootKey_ = "NetworkData";

	private static string informationDataRootKey_ = "Information";

	private static string informationDataIDKey_ = "ID";

	private static string informationDataDateKey_ = "Date";

	public static string getOptionFlgDataKey()
	{
		return optionDataRootKey_ + "Flg";
	}

	public static string getOptionLanguageKey()
	{
		return optionDataRootKey_ + "Lang";
	}

	public static string getOptionCharacterAvatarKey()
	{
		return optionDataRootKey_ + "Avatar";
	}

	public static string getOptionDataVersionKey()
	{
		return optionDataRootKey_ + "OptionDataVersion";
	}

	public static string getOtherFlgDataKey()
	{
		return otherDataRootKey_ + "Flg";
	}

	public static string getOtherGuestFlgDataKey()
	{
		return otherDataRootKey_ + "Flg_Guest";
	}

	public static string getOtherStageNoKey()
	{
		return optionDataRootKey_ + "StageNo";
	}

	public static string getOtherEventStageNoKey()
	{
		return optionDataRootKey_ + "EventStageNo";
	}

	public static string getOtherChallengeStageNoKey()
	{
		return optionDataRootKey_ + "ChallengeStageNo";
	}

	public static string getOtherCollaborationStageNoKey()
	{
		return optionDataRootKey_ + "CollaborationStageNo";
	}

	public static string getOtherInformationDateKey()
	{
		return optionDataRootKey_ + "InfoDate";
	}

	public static string getOtherEventKeyStageNoKey()
	{
		return optionDataRootKey_ + "EventStageKeyNo";
	}

	public static string getOtherChallengeKeyStageNoKey()
	{
		return optionDataRootKey_ + "ChallengeStageKeyNo";
	}

	public static string getOtherCollaborationKeyStageNoKey()
	{
		return optionDataRootKey_ + "CollaborationStageKeyNo";
	}

	public static string getNetworkEventNoKey()
	{
		return networkDataRootKey_ + "EventNo";
	}

	public static string getNetworkEventStageDataNoKey()
	{
		return networkDataRootKey_ + "EventStageNo";
	}

	public static string getNetworkResourceVersionKey()
	{
		return networkDataRootKey_ + "ResourceVersion";
	}

	public static string getNetworkPlacementDataVersionKey()
	{
		return networkDataRootKey_ + "PlacementDataVersion";
	}

	public static string getNetworkRecoverySaveDateKey()
	{
		return networkDataRootKey_ + "RecoverySaveDate";
	}

	public static string getNetworkRankingDateKey()
	{
		return networkDataRootKey_ + "RankingDate";
	}

	public static string getNetworkRankingUniqueIDKey()
	{
		return networkDataRootKey_ + "RankingUniqueID";
	}

	public static string getNetworkSessionIDKey()
	{
		return networkDataRootKey_ + "SessionID";
	}

	public static string getNetworkParkStageDataVersionKey()
	{
		return networkDataRootKey_ + "ParkDataVersion";
	}

	public static string getNetworkParkPlacementVersionKey()
	{
		return networkDataRootKey_ + "ParkPlacementVersion";
	}

	public static string getNetworkMinilenThanksVersionKey()
	{
		return networkDataRootKey_ + "MinilenThanksVersion";
	}

	public static string getNetworkBuildingVersionKey()
	{
		return networkDataRootKey_ + "BuildingVersion";
	}

	public static string getInformationDataIDKey(int id)
	{
		return informationDataRootKey_ + informationDataIDKey_ + id;
	}

	public static string getInformationDataDateKey(int id)
	{
		return informationDataRootKey_ + informationDataDateKey_ + id;
	}
}
