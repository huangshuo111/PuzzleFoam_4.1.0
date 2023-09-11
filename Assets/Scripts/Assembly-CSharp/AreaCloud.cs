using System.Collections;
using UnityEngine;

public class AreaCloud : MonoBehaviour
{
	public enum eAreaCloudType
	{
		Up = 0,
		Low = 1,
		Max = 2
	}

	public enum eAreaCloudSpriteType
	{
		CloudArea = 0,
		CloudSpaceArea = 1,
		SpaceArea = 2
	}

	private const int SpaceChangeMapNo = 4;

	private MapCamera mapCamera_;

	private UIDragObjectEx dragObject_;

	private float base_y;

	private GameObject arrowObj_;

	private GameObject cloudObj_;

	private GameObject cloudSpaceObj_;

	private GameObject spaceObj_;

	[SerializeField]
	public Vector2[] Base_Pos_Y;

	private void Awake()
	{
		arrowObj_ = base.transform.Find("Arrow").gameObject;
		cloudObj_ = base.transform.Find("Cloud").gameObject;
		cloudSpaceObj_ = base.transform.Find("Cloud_ground_space").gameObject;
		spaceObj_ = base.transform.Find("Cloud_space").gameObject;
	}

	public void setup(GameObject caller, MapCamera camera, int type, int mapNo)
	{
		base_y = getBasePosY(mapNo, type);
		mapCamera_ = camera;
		dragObject_ = camera.transform.parent.GetComponent<UIDragObjectEx>();
		UIButtonMessage[] componentsInChildren = base.transform.GetComponentsInChildren<UIButtonMessage>();
		UIButtonMessage[] array = componentsInChildren;
		foreach (UIButtonMessage uIButtonMessage in array)
		{
			uIButtonMessage.target = caller;
			uIButtonMessage.functionName = "OnButton";
		}
		base.transform.localPosition = new Vector3(0f, base_y, -30f);
		updateCloudSpriteType(mapNo, type);
	}

	public IEnumerator moveToCenter()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", mapCamera_.transform.localPosition.y, "easetype", iTween.EaseType.easeInSine, "time", 1f, "islocal", true));
		while (base.gameObject.GetComponent<iTween>() != null)
		{
			yield return 0;
		}
	}

	private void LateUpdate()
	{
		if (Part_Map.bTransMap && mapCamera_ != null && dragObject_ != null)
		{
			base.transform.localPosition = new Vector3(mapCamera_.transform.localPosition.x, base_y, -30f);
		}
	}

	public void setArrowEndable(bool enable)
	{
		arrowObj_.SetActive(enable);
	}

	public float getBasePosY(int mapNo, int direction)
	{
		if (direction == 0)
		{
			return Base_Pos_Y[mapNo].x;
		}
		return Base_Pos_Y[mapNo].y;
	}

	public void updateCloudSpriteType(int mapNo, int type)
	{
		if ((mapNo == 3 && type == 0) || (mapNo == 4 && type == 1))
		{
			cloudObj_.SetActive(false);
			cloudSpaceObj_.SetActive(true);
			spaceObj_.SetActive(false);
		}
		else if (mapNo >= 4)
		{
			cloudObj_.SetActive(false);
			cloudSpaceObj_.SetActive(false);
			spaceObj_.SetActive(true);
		}
		else
		{
			cloudObj_.SetActive(true);
			cloudSpaceObj_.SetActive(false);
			spaceObj_.SetActive(false);
		}
	}
}
