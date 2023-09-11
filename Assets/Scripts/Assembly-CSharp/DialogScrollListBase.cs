using System.Collections.Generic;
using UnityEngine;

public class DialogScrollListBase : DialogBase
{
	protected List<GameObject> itemList_ = new List<GameObject>();

	protected List<GameObject> reservedClearItemList_ = new List<GameObject>();

	protected UIGrid grid_;

	protected UIDraggablePanel dragPanel_;

	protected UIPanel panel_;

	protected GameObject item_;

	protected GameObject line_;

	private Vector3 panelPos_ = Vector3.zero;

	public override void OnCreate()
	{
		findObject();
		panelPos_ = dragPanel_.transform.localPosition;
		panel_ = dragPanel_.GetComponent<UIPanel>();
	}

	protected virtual void findObject()
	{
		dragPanel_ = base.transform.Find("DragPanel").GetComponent<UIDraggablePanel>();
		grid_ = dragPanel_.transform.Find("contents").GetComponent<UIGrid>();
	}

	public virtual void init(GameObject item)
	{
		item_ = item;
		NGUIUtility.setupButton(item_, base.gameObject, true);
	}

	public void createLine()
	{
		createLine(-70f);
	}

	public void createLine(float y)
	{
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Line);
		line_ = Object.Instantiate(@object) as GameObject;
		Utility.setParent(line_, base.transform, false);
		GameObject gameObject = line_.transform.Find("line").gameObject;
		Vector3 localPosition = gameObject.transform.localPosition;
		localPosition.y = y;
		gameObject.transform.localPosition = localPosition;
	}

	protected void addLine()
	{
		_addItem(line_, 0);
	}

	protected void addItem(GameObject item, int index)
	{
		_addItem(item, index + 1);
	}

	private void _addItem(GameObject item, int index)
	{
		item.gameObject.SetActive(true);
		item.name = index.ToString();
		Utility.setParent(item.gameObject, grid_.transform, false);
	}

	protected void repositionItem()
	{
		grid_.repositionNow = true;
	}

	public void clear()
	{
		for (int num = itemList_.Count - 1; num >= 0; num--)
		{
			GameObject obj = itemList_[num];
			Object.Destroy(obj);
		}
		itemList_.Clear();
	}

	public void clearItem()
	{
		for (int num = itemList_.Count - 1; num >= 0; num--)
		{
			itemList_[num].SetActive(false);
			reservedClearItemList_.Add(itemList_[num]);
		}
		itemList_.Clear();
	}

	protected GameObject createItem(UserData data)
	{
		GameObject gameObject = _createItem();
		if (data == null)
		{
			return gameObject;
		}
		Texture texture = data.Texture;
		PlayerIcon icon = gameObject.gameObject.GetComponent<PlayerItemBase>().getIcon();
		icon.createMaterial();
		if (texture != null)
		{
			icon.setTexture(texture);
		}
		else
		{
			dialogManager_.StartCoroutine(icon.loadTexture(data.URL, true, data));
		}
		return gameObject;
	}

	protected GameObject createItem(UserData data, UserData[] datas, int index)
	{
		GameObject gameObject = _createItem();
		Texture texture = data.Texture;
		PlayerIcon icon = gameObject.gameObject.GetComponent<PlayerItemBase>().getIcon();
		icon.createMaterial();
		if (texture != null)
		{
			icon.setTexture(texture);
		}
		else
		{
			dialogManager_.StartCoroutine(icon.loadTexture(data.URL, true, datas, index));
		}
		return gameObject;
	}

	private GameObject _createItem()
	{
		GameObject gameObject = Object.Instantiate(item_) as GameObject;
		Utility.setParent(gameObject, base.transform, false);
		return gameObject;
	}

	protected void addDummyFriend(UserData data, GameObject dummyFriendItem)
	{
		GameObject gameObject = Object.Instantiate(dummyFriendItem) as GameObject;
		Utility.setParent(gameObject, base.transform, false);
		UserDataObject userDataObject = gameObject.AddComponent<UserDataObject>();
		userDataObject.setData(data);
		itemList_.Add(gameObject);
	}

	protected virtual void OnDisable()
	{
		if (dragPanel_ != null)
		{
			dragPanel_.MoveRelative(new Vector3(0f - dragPanel_.transform.localPosition.x, 0f - dragPanel_.transform.localPosition.y, 0f));
			dragPanel_.MoveRelative(new Vector3(panelPos_.x, panelPos_.y, 0f));
		}
	}

	private void Update()
	{
		foreach (GameObject item in itemList_)
		{
			Transform transform = item.transform;
			if (item.activeSelf && (transform.position.y > 2f || transform.position.y < -2f))
			{
				item.SetActive(false);
			}
			else if (!item.activeSelf && !(transform.position.y > 2f) && !(transform.position.y < -2f))
			{
				item.SetActive(true);
				Utility.updateUIWidget(item);
			}
		}
	}
}
