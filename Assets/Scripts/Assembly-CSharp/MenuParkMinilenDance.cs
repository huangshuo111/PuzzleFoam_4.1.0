using System.Collections;
using UnityEngine;

public class MenuParkMinilenDance : MonoBehaviour
{
	private static readonly int ANIMATOR_HASH_DANCE = Animator.StringToHash("dance");

	private static readonly int ANIMATOR_HASH_SLEEP = Animator.StringToHash("sleep");

	private static MenuParkMinilenDance _instance = null;

	private Animator[] _minilen_array = new Animator[5];

	private bool _is_initialized;

	public static MenuParkMinilenDance instance
	{
		get
		{
			if (!_instance)
			{
				GameObject gameObject = ResourceLoader.Instance.loadGameObject("Prefabs/", "UI_MinilenDance");
				if (!gameObject)
				{
					Debug.LogError("Cannot Load prefab Prefabs/  UI_MinilenDance");
					return null;
				}
				GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
				if (!gameObject2)
				{
					Debug.LogError("Cannot Instance prefab Prefabs/  UI_MinilenDance");
					return null;
				}
				_instance = gameObject2.AddComponent<MenuParkMinilenDance>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Initializze();
	}

	private void Initializze()
	{
		if (_is_initialized)
		{
			return;
		}
		Transform transform = base.transform;
		transform.parent = null;
		transform.localPosition = new Vector3(0f, 0f, 0f);
		Vector3 localScale = GameObject.Find("UI Root").transform.localScale;
		localScale.z = 1f;
		transform.localScale = localScale;
		ParkObjectManager parkObjectManager = ParkObjectManager.Instance;
		Minilen minilen = parkObjectManager.createMinilen(30000);
		parkObjectManager.Remove(minilen);
		minilen.transform.parent = transform;
		minilen.transform.localPosition = new Vector3(-1000f, -2000f, 0f);
		minilen.transform.localScale = new Vector3(3f, 3f, 1f);
		SetLayer(minilen.gameObject, LayerMask.NameToLayer("ParkOver"));
		minilen.PlayAnimation(Minilen.eAnimationState.Dance);
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject = Object.Instantiate(minilen.gameObject) as GameObject;
			if (!gameObject)
			{
				Debug.LogError("Cannot Instance prefab Prefabs/  UI_MinilenDance");
				continue;
			}
			Transform transform2 = gameObject.transform;
			transform2.parent = minilen.transform.parent;
			transform2.localPosition = new Vector3(-1000f + (float)i * 500f, -2000f, 0f);
			transform2.localScale = new Vector3(3f, 3f, 1f);
			_minilen_array[i] = gameObject.GetComponentInChildren<Animator>();
		}
		minilen.gameObject.SetActive(false);
		_is_initialized = true;
	}

	private void SetLayer(GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform item in obj.transform)
		{
			SetLayer(item.gameObject, layer);
		}
	}

	public void Play()
	{
		Initializze();
		base.gameObject.SetActive(true);
		int num = 5;
		for (int i = 0; i < _minilen_array.Length; i++)
		{
			_minilen_array[i].SetBool(ANIMATOR_HASH_DANCE, false);
			_minilen_array[i].SetBool(ANIMATOR_HASH_SLEEP, false);
			int num2 = Random.Range(0, 100);
			if (num2 < num)
			{
				Sleep(_minilen_array[i]);
				num /= 2;
			}
			else if (num2 < 3 * num)
			{
				num /= 2;
				StartCoroutine(WaitDance(_minilen_array[i]));
			}
			else
			{
				_minilen_array[i].SetBool(ANIMATOR_HASH_DANCE, true);
			}
		}
	}

	public void Stop()
	{
		ParkEffect[] componentsInChildren = GetComponentsInChildren<ParkEffect>();
		foreach (ParkEffect parkEffect in componentsInChildren)
		{
			Object.Destroy(parkEffect.gameObject);
		}
		base.gameObject.SetActive(false);
	}

	private void Sleep(Animator animator)
	{
		animator.SetBool(ANIMATOR_HASH_SLEEP, true);
		ParkEffect parkEffect = ParkObjectManager.Instance.createEffect(ParkEffect.eEffectType.Sleep);
		parkEffect.transform.parent = animator.transform.parent.parent;
		parkEffect.transform.localPosition = new Vector3(0f, 0f, 0f);
		parkEffect.transform.localScale = new Vector3(1f, 1f, 1f);
		SetLayer(parkEffect.gameObject, LayerMask.NameToLayer("ParkOver"));
		parkEffect.gameObject.SetActive(true);
		parkEffect.Play();
	}

	private IEnumerator WaitDance(Animator animator)
	{
		yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
		animator.SetBool(ANIMATOR_HASH_DANCE, true);
	}
}
