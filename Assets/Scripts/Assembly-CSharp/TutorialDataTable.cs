using UnityEngine;

public class TutorialDataTable : MonoBehaviour
{
	public enum ePlace
	{
		Invalid = -1,
		Setup = 1,
		StageProdBegin = 2,
		StageProd = 3,
		StageProdEnd = 4,
		ReadyGoEnd = 5,
		Continue = 6
	}

	public enum eSPTutorial
	{
		RealFriend = -1,
		EventStage = -2,
		Continue = -3,
		SpecialItem = -4,
		Treasure = -5,
		NoItem = -6,
		EventMap = -7,
		CoinBubble = -8,
		ContinueChance = -9,
		Ivy = -10,
		Cloud = -11,
		KeyBubble = -12,
		KeyShortage = -13,
		FirstBossOpen = -14,
		PoworUp = -15,
		BossOpenKey = -16,
		CollaborationStage = -17,
		StageSkip = -18,
		BalloonBubble = -19,
		MinilenBubbleDrop = -500003,
		MinilenGet = -500004
	}

	public enum eValidRange
	{
		Default = -1,
		ForClear = 1,
		First = 2
	}

	private TutorialData data_;

	private bool bLoaded_;

	public void load()
	{
		if (!bLoaded_)
		{
			TextAsset textAsset = Resources.Load("Parameter/tutorial_table", typeof(TextAsset)) as TextAsset;
			data_ = Xml.DeserializeObject<TutorialData>(textAsset.text) as TutorialData;
			bLoaded_ = true;
		}
	}

	public TutorialData getData()
	{
		return data_;
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}
}
