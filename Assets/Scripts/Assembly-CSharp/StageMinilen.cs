using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMinilen : MonoBehaviour
{
	private static readonly float Y_GRAVITY = -1600f;

	private static readonly int SIDE_MAX = 5;

	private static readonly Vector3 POS_REMOVE = new Vector3(-600f, -316f, 3f);

	private static readonly Vector3 POS_FIRST = new Vector3(-100f, -244f, 3f);

	private static readonly Vector3 POS_SIDE_DELTA = new Vector3(60f, 0f, 0.01f);

	private static readonly Vector3 POS_RANDAM = new Vector3(5f, 10f, 0f);

	private static readonly float JUMP_HEIGHT_FIRST = 250f;

	private static readonly float JUMP_HEIGHT_NORMAL = 100f;

	private static readonly float JUMP_HEIGHT_FINAL = 400f;

	private static readonly float JUMP_HEIGHT_IDLE = 30f;

	private static readonly float JUMP_WAIT_IDLE_MIN = 1f;

	private static readonly float JUMP_WAIT_IDLE_MAX = 5f;

	private static readonly float MINILEN_SCALE = 0.8f;

	private int _position_index_current;

	private int _position_index_tobe;

	private bool _is_jumping;

	private Transform _transform;

	private Part_Stage _part_stage;

	private float _idle_wait_jump = -1f;

	private static int _minilen_lost_no = 0;

	public void animStart(Part_Stage part_stage)
	{
		_part_stage = part_stage;
		_transform = base.transform;
		List<StageMinilen> minilenList = _part_stage.minilenList;
		int count = minilenList.Count;
		_position_index_current = minilenList.Count;
		_position_index_tobe = _position_index_current - 1;
		_part_stage.updateMinilenCount();
		_part_stage.MinilenFireworks(_transform.localPosition);
		StartCoroutine(Jump(_position_index_current - 1, JUMP_HEIGHT_FIRST));
		Sound.Instance.stopSe(Sound.eSe.SE_704_park_mini);
		Sound.Instance.playSe(Sound.eSe.SE_704_park_mini);
	}

	public void Escape(Part_Stage part_stage)
	{
		_transform = base.transform;
		_part_stage = part_stage;
		_part_stage._lost_minilen = this;
		StartCoroutine(EscapeRoutine());
	}

	private IEnumerator EscapeRoutine()
	{
		Vector3 erase_pos = _transform.localPosition;
		erase_pos.z = -20f;
		_transform.localPosition = erase_pos;
		erase_pos.y = -600f;
		yield return 0;
		IEnumerator jumping = Jump(_transform.localPosition, erase_pos, UnityEngine.Random.Range(100f, 300f));
		float rot_power2 = 3600f / (float)Math.PI;
		rot_power2 *= ((_minilen_lost_no++ % 2 != 0) ? (-1f) : 1f);
		while (jumping.MoveNext())
		{
			if (!_part_stage.stagePause.pause)
			{
				_transform.localScale *= 1f + 0.5f * Time.deltaTime;
				_transform.Rotate(Vector3.forward, rot_power2 * Time.deltaTime);
			}
			yield return 0;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator Jump(int fin_index, float height)
	{
		Vector3 ini_pos = _transform.localPosition;
		Vector3 fin_pos = GetFinalPosition(fin_index);
		_is_jumping = true;
		yield return StartCoroutine(Jump(ini_pos, fin_pos, height));
		_is_jumping = false;
		_position_index_current = fin_index;
		if (fin_index < 0)
		{
			_part_stage.minilenList.Remove(this);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		List<StageMinilen> minilen_list = _part_stage.minilenList;
		if (minilen_list.FindLast((StageMinilen m) => true) == this && minilen_list.Count > SIDE_MAX)
		{
			for (int i = 0; i < minilen_list.Count; i++)
			{
				minilen_list[i]._position_index_tobe = i - Mathf.Max(minilen_list.Count - SIDE_MAX, 0);
			}
		}
	}

	private IEnumerator Jump(Vector3 ini_pos, Vector3 fin_pos, float height)
	{
		if (ini_pos.x < fin_pos.x)
		{
			_transform.localScale = new Vector3(0f - MINILEN_SCALE, MINILEN_SCALE, 1f);
		}
		else
		{
			_transform.localScale = new Vector3(MINILEN_SCALE, MINILEN_SCALE, 1f);
		}
		float peak_y_from_ini = height;
		float peak_y_from_fin = height;
		if (ini_pos.y < fin_pos.y)
		{
			peak_y_from_ini = fin_pos.y - ini_pos.y + height;
		}
		else
		{
			peak_y_from_fin = ini_pos.y + height - fin_pos.y;
		}
		Vector2 vel_0 = default(Vector2);
		vel_0.y = (float)Math.Sqrt(2f * Math.Abs(Y_GRAVITY) * peak_y_from_ini);
		float time_ini_to_peak = vel_0.y / Math.Abs(Y_GRAVITY);
		float time_peak_to_fin = (float)Math.Sqrt(2f * peak_y_from_fin / Math.Abs(Y_GRAVITY));
		float time_all = time_ini_to_peak + time_peak_to_fin;
		vel_0.x = (fin_pos.x - ini_pos.x) / time_all;
		float time = 0f;
		while (time < time_all)
		{
			yield return 0;
			if (!_part_stage.stagePause.pause)
			{
				time += Time.deltaTime;
				_transform.localPosition = new Vector3(ini_pos.x + vel_0.x * time, ini_pos.y + vel_0.y * time + 0.5f * Y_GRAVITY * time * time, fin_pos.z);
			}
		}
		_transform.localPosition = fin_pos;
	}

	private Vector3 GetFinalPosition(int position_index)
	{
		if (position_index >= 0)
		{
			Vector3 vector = POS_FIRST + position_index * POS_SIDE_DELTA;
			Vector3 pOS_RANDAM = POS_RANDAM;
			float min = 0f - pOS_RANDAM.x;
			Vector3 pOS_RANDAM2 = POS_RANDAM;
			float x = UnityEngine.Random.Range(min, pOS_RANDAM2.x);
			Vector3 pOS_RANDAM3 = POS_RANDAM;
			float min2 = 0f - pOS_RANDAM3.y;
			Vector3 pOS_RANDAM4 = POS_RANDAM;
			float y = UnityEngine.Random.Range(min2, pOS_RANDAM4.y);
			Vector3 pOS_RANDAM5 = POS_RANDAM;
			float min3 = 0f - pOS_RANDAM5.z;
			Vector3 pOS_RANDAM6 = POS_RANDAM;
			return vector + new Vector3(x, y, UnityEngine.Random.Range(min3, pOS_RANDAM6.z));
		}
		return POS_REMOVE;
	}

	private void Update()
	{
		if (_is_jumping)
		{
			return;
		}
		if (_position_index_tobe < _position_index_current)
		{
			if (_position_index_current - 1 >= 0)
			{
				StartCoroutine(Jump(_position_index_current - 1, JUMP_HEIGHT_NORMAL));
			}
			else
			{
				StartCoroutine(Jump(_position_index_current - 1, JUMP_HEIGHT_FINAL));
			}
		}
		else
		{
			if (!_part_stage || _part_stage.stagePause.pause)
			{
				return;
			}
			if (_idle_wait_jump >= 0f)
			{
				_idle_wait_jump -= Time.deltaTime;
				if (_idle_wait_jump < 0f)
				{
					StartCoroutine(Jump(_position_index_current, JUMP_HEIGHT_IDLE));
				}
			}
			else
			{
				_idle_wait_jump = UnityEngine.Random.Range(JUMP_WAIT_IDLE_MIN, JUMP_WAIT_IDLE_MAX);
			}
		}
	}

	public static void SetNum(Part_Stage part_stage)
	{
		List<StageMinilen> remove_list = new List<StageMinilen>();
		List<StageMinilen> minilenList = part_stage.minilenList;
		for (int i = 0; i < minilenList.Count; i++)
		{
			StageMinilen stageMinilen = minilenList[i];
			stageMinilen.StopAllCoroutines();
			stageMinilen._is_jumping = false;
			stageMinilen._position_index_current = i;
			stageMinilen._position_index_tobe = i;
			stageMinilen._transform.localPosition = stageMinilen.GetFinalPosition(i);
			if (i >= Mathf.Min(part_stage.minilenCountCurrent, SIDE_MAX))
			{
				remove_list.Add(stageMinilen);
			}
		}
		minilenList.RemoveAll((StageMinilen m) => remove_list.Contains(m));
		remove_list.ForEach(delegate(StageMinilen r)
		{
			UnityEngine.Object.Destroy(r.gameObject);
		});
	}
}
