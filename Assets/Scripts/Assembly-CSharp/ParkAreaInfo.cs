public class ParkAreaInfo
{
	public int area_id = -1;

	public ParkStageInfo.Info first_info;

	public ParkStageInfo.Info[] stage_infos;

	public int[] gettable_minilens;

	public ParkAreaInfo(int area_id)
	{
		this.area_id = area_id;
	}
}
