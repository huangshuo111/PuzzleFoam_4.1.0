using UnityEngine;

public class MenuParkStageListItem : MonoBehaviour
{
	private static readonly string BLANK_SPRITE_NAME = "setup_plate_star";

	private static readonly string STAR_SPRITE_NAME = "stage_flower_small_00";

	[SerializeField]
	private UILabel _stage_name_label;

	[SerializeField]
	private UISprite[] _star_sprites;

	[SerializeField]
	private GameObject _button_obj;

	public void Setup(ParkStageInfo.Info stage_info, int score)
	{
		_stage_name_label.text = MessageResource.Instance.getMessage(9102) + " " + stage_info.Common.StageNo % 10000;
		_button_obj.name = "Item_Button" + stage_info.Common.StageNo;
		for (int i = 0; i < stage_info.Common.StarScores.Length && score >= stage_info.Common.StarScores[i]; i++)
		{
			_star_sprites[i].spriteName = i.ToString(STAR_SPRITE_NAME);
		}
	}
}
