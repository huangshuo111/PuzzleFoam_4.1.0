using System.Collections;
using UnityEngine;

public class InstantateAsyncManager : MonoBehaviour
{
	public static InstantateAsyncManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	public static Object InstanceObject(Object original, bool bPool = false)
	{
		if (!original)
		{
			return null;
		}
		Object @object = Object.Instantiate(original);
		if (@object == null)
		{
			Debug.Log("##### Instance Error!! name = " + original.name);
		}
		return @object;
	}

	public static void InstanceAsync(GameObject[] objects, Transform self)
	{
		Instance.StartCoroutine(Instance.InstanceObjects(objects, self));
	}

	public IEnumerator InstanceObjects(GameObject[] objects, Transform self)
	{
		self.gameObject.SetActive(false);
		foreach (GameObject obj in objects)
		{
			GameObject item = (GameObject)Object.Instantiate(obj);
			item.transform.parent = self;
			yield return null;
		}
		self.gameObject.SetActive(true);
	}
}
