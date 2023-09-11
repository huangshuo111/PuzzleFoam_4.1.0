using UnityEngine;

public class MenuParkRoadListItem : MonoBehaviour
{
	private enum eRoadItemMessage
	{
		Main = 9114,
		Sub = 9115,
		Swiching = 9116
	}

	private enum eSplitMessageIndex
	{
		RoadName = 0,
		MainName = 1,
		SubName = 2,
		Max = 3
	}

	private const char BOUNDARY_ROAD_AND_PERIOD = '（';

	private const char BOUNDARY_PERIOD_MAIN_AND_SUB = '、';

	[SerializeField]
	private UILabel _road_name;

	[SerializeField]
	private UILabel _main_obj_item;

	[SerializeField]
	private UILabel _main_obj_name;

	[SerializeField]
	private UILabel _sub_obj_item;

	[SerializeField]
	private UILabel _sub_obj_name;

	[SerializeField]
	private UILabel _swiching_name;

	[SerializeField]
	private UISprite _road_image;

	[SerializeField]
	private GameObject _swiching_button;

	private int _road_id;

	public int roadId
	{
		get
		{
			return _road_id;
		}
	}

	public void Setup(DialogParkBuildingList.BuildingListData road_info, bool can_build, GameObject data_object)
	{
		_road_id = road_info.info.ID;
		MessageResource instance = MessageResource.Instance;
		_swiching_button.SetActive(false);
		_main_obj_item.text = instance.getMessage(9114);
		_sub_obj_item.text = instance.getMessage(9115);
		_swiching_name.text = instance.getMessage(9116);
		int nameID = road_info.info.NameID;
		string message = instance.getMessage(nameID);
		string[] array = message.Split(',');
		if (array.Length >= 3)
		{
			_road_name.text = array[0];
			_main_obj_name.text = array[1];
			_sub_obj_name.text = array[2];
		}
		_road_image.spriteName = road_info.iconSpriteName;
		_road_image.MakePixelPerfect();
	}
}
