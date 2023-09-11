using System.Collections.Generic;
using UnityEngine;

public class StageIconDataTable : MonoBehaviour
{
	public const int ONE_MAP_STAGE_MAX = 90;

	public const int ONE_MAP_EVENT_STAGE_MAX = 59;

	public const int EventStageNoBase = 10000;

	private List<StageIconData> dataList_ = new List<StageIconData>();

	private StageIconData event_data_;

	private List<StageIconData> event_dataList_ = new List<StageIconData>();

	private StageIconData challenge_data_;

	private StageIconData collaboration_data_;

	private int stageIconNum_;

	private int collaboStageIconNum_;

	private int boxIconNum_;

	private bool bLoaded_;

	public void load()
	{
		if (bLoaded_)
		{
			return;
		}
		int num = 0;
		dataList_.Clear();
		while (true)
		{
			TextAsset textAsset = Resources.Load("Parameter/stage_icon_" + num.ToString("00"), typeof(TextAsset)) as TextAsset;
			if (textAsset == null)
			{
				break;
			}
			dataList_.Add(Xml.DeserializeObject<StageIconData>(textAsset.text) as StageIconData);
			num++;
		}
		stageIconNum_ = 0;
		foreach (StageIconData item in dataList_)
		{
			stageIconNum_ += item.IconNum;
		}
		boxIconNum_ = 0;
		foreach (StageIconData item2 in dataList_)
		{
			boxIconNum_ += item2.BoxNum;
		}
		num = 0;
		event_dataList_.Clear();
		while (true)
		{
			TextAsset textAsset2 = Resources.Load("Parameter/event_stage_icon_" + num.ToString("00"), typeof(TextAsset)) as TextAsset;
			if (textAsset2 == null)
			{
				break;
			}
			event_dataList_.Add(Xml.DeserializeObject<StageIconData>(textAsset2.text) as StageIconData);
			num++;
		}
		TextAsset textAsset3 = Resources.Load("Parameter/challenge_stage_icon", typeof(TextAsset)) as TextAsset;
		challenge_data_ = Xml.DeserializeObject<StageIconData>(textAsset3.text) as StageIconData;
		TextAsset textAsset4 = Resources.Load("Parameter/collaboration_stage_icon", typeof(TextAsset)) as TextAsset;
		collaboration_data_ = Xml.DeserializeObject<StageIconData>(textAsset4.text) as StageIconData;
		collaboStageIconNum_ += collaboration_data_.IconNum;
		bLoaded_ = true;
	}

	public StageIconData getData(int stageNo)
	{
		int mapNoByStageNo = getMapNoByStageNo(stageNo);
		if (mapNoByStageNo < dataList_.Count)
		{
			return dataList_[mapNoByStageNo];
		}
		return dataList_[0];
	}

	public StageIconData getDataByIndex(int index)
	{
		if (index > dataList_.Count)
		{
			return dataList_[0];
		}
		return dataList_[index];
	}

	public int getMapNoByStageNo(int stageNo)
	{
		int num = stageNo / 90;
		if (0 > num)
		{
			return 0;
		}
		if (dataList_.Count <= num)
		{
			return dataList_.Count - 1;
		}
		return num;
	}

	public StageIconData getEventData(int stageNo)
	{
		int eventMapNoByStageNo = getEventMapNoByStageNo(stageNo);
		if (eventMapNoByStageNo < event_dataList_.Count)
		{
			return event_dataList_[eventMapNoByStageNo];
		}
		return event_dataList_[0];
	}

	public StageIconData getEventDataByIndex(int index)
	{
		if (index > event_dataList_.Count)
		{
			return event_dataList_[0];
		}
		return event_dataList_[index];
	}

	public int getEventMapNoByStageNo(int stageNo)
	{
		stageNo %= 10000;
		stageNo--;
		int num = stageNo / 59;
		if (0 > num)
		{
			return 0;
		}
		if (event_dataList_.Count <= num)
		{
			return event_dataList_.Count - 1;
		}
		return num;
	}

	public int getMaxEventIconsNum()
	{
		int num = 0;
		foreach (StageIconData item in event_dataList_)
		{
			num += item.IconNum;
		}
		return num;
	}

	public int getMaxEventMapNum()
	{
		return event_dataList_.Count;
	}

	public int getMaxMapNum()
	{
		return dataList_.Count;
	}

	public int getMaxStageIconsNum()
	{
		return stageIconNum_;
	}

	public int getMaxCollaboStageIconsNum()
	{
		return collaboStageIconNum_;
	}

	public int getMaxBoxNum()
	{
		return boxIconNum_;
	}

	public StageIconData getChallengeData()
	{
		return challenge_data_;
	}

	public StageIconData getCollaborationData()
	{
		return collaboration_data_;
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}
}
