using System.Collections.Generic;
using UnityEngine;

public class Editor_Arrange : MonoBehaviour
{
	private static tk2dAnimatedSprite select;

	private tk2dAnimatedSprite me;

	private UILabel label_;

	[SerializeField]
	private GameObject counterBase;

	private Editor_Counter counter;

	private int skeltonColor;

	private List<GameObject> objectList_ = new List<GameObject>();

	public GameObject[] bossObjectBase;

	private void Start()
	{
		if (select == null)
		{
			select = GameObject.Find("editor_select").GetComponent<tk2dAnimatedSprite>();
		}
		me = GetComponentInChildren<tk2dAnimatedSprite>();
		label_ = base.gameObject.transform.Find("label").GetComponent<UILabel>();
	}

	private void Update()
	{
	}

	private void button()
	{
		if (Editor_Main.editLayer == -2)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			if (Editor_Main.editLayer == 0)
			{
				if (select.CurrentClip.name.Contains("_78") || select.CurrentClip.name.Contains("_88"))
				{
					label_.text = ((!select.CurrentClip.name.Contains("_78")) ? "B" : "A");
					label_.color = Color.white;
					label_.transform.localScale = new Vector3(20f, 20f, 1f);
					label_.gameObject.SetActive(false);
					label_.gameObject.SetActive(true);
				}
				else if (select.CurrentClip.name.Contains("_87"))
				{
					if (me.IsPlaying(select.CurrentClip.name))
					{
						skeltonColor++;
						if (skeltonColor >= Editor_Main.skelltonColorName.Length)
						{
							skeltonColor = 0;
						}
					}
					int num = skeltonColor;
					if (Editor_Main.skelltonColorName.Length <= num || num < 0)
					{
						num = 0;
					}
					label_.text = Editor_Main.skelltonColorName[num];
					label_.color = Editor_Main.skelltonColor[num];
					label_.transform.localScale = new Vector3(20f, 20f, 1f);
					label_.gameObject.SetActive(false);
					label_.gameObject.SetActive(true);
				}
				else
				{
					label_.text = string.Empty;
					skeltonColor = 0;
					label_.gameObject.SetActive(false);
				}
				string s = select.CurrentClip.name.Replace("bubble_", string.Empty);
				bool flag = int.Parse(s) >= 100 && int.Parse(s) <= 107;
				if (select.CurrentClip.name == "bubble_77" || flag)
				{
					if (counter == null)
					{
						createCounter(10);
					}
					else if (Input.GetKey(KeyCode.LeftArrow))
					{
						counter.addCount(10);
					}
					else if (Input.GetKey(KeyCode.RightArrow))
					{
						counter.addCount(-10);
					}
					else if (Input.GetKey(KeyCode.UpArrow))
					{
						counter.addCount(1);
					}
					else if (Input.GetKey(KeyCode.DownArrow))
					{
						counter.addCount(-1);
					}
					else
					{
						counter.addCount(1);
					}
					me.Play(select.CurrentClip.name);
					Editor_Ivy.Instance.reSetupIvy();
				}
				else
				{
					me.Play(select.CurrentClip.name);
					Editor_Ivy.Instance.reSetupIvy();
					if (counter != null)
					{
						Object.DestroyImmediate(counter.gameObject);
						counter = null;
					}
				}
			}
			else if (Editor_Main.editLayer == -3)
			{
				if (base.transform.Find(Editor_Main.bossObjectNames[Editor_ObjectSelect.selectIndex]) == null)
				{
					createBossObject(Editor_ObjectSelect.selectIndex);
				}
			}
			else
			{
				Editor_Ivy.Instance.OnButtonEvy(me.transform.parent.gameObject, false);
			}
		}
		else if (Editor_Main.editLayer == 0)
		{
			label_.text = string.Empty;
			skeltonColor = 0;
			label_.gameObject.SetActive(false);
			if (counter != null)
			{
				Object.DestroyImmediate(counter.gameObject);
				counter = null;
			}
			me.Play("bubble_99");
			Editor_Ivy.Instance.reSetupIvy();
		}
		else if (Editor_Main.editLayer == -3)
		{
			foreach (GameObject item in objectList_)
			{
				if (item != null)
				{
					Debug.Log("delete boss object:" + item);
					Object.DestroyImmediate(item);
				}
			}
			objectList_.Clear();
		}
		else
		{
			Editor_Ivy.Instance.OnButtonEvy(me.transform.parent.gameObject, true);
		}
		me.Pause();
	}

	public void createCounter(int count)
	{
		if (counter == null)
		{
			Transform transform = base.transform.Find("count");
			GameObject gameObject = ((!(transform == null)) ? transform.gameObject : (Object.Instantiate(counterBase) as GameObject));
			gameObject.name = "count";
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, -15f);
			gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
			counter = gameObject.GetComponent<Editor_Counter>();
			counter.setCount(count);
			counter.setPosition();
			gameObject.SetActive(true);
		}
	}

	public void createBossObject(int index)
	{
		GameObject gameObject = Object.Instantiate(bossObjectBase[index]) as GameObject;
		Utility.setParent(gameObject, base.transform, false);
		gameObject.name = Editor_Main.bossObjectNames[index];
		objectList_.Add(gameObject);
	}
}
