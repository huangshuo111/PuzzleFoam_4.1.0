using System;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class EntangledInt
{
	private class Manager
	{
		public LinkedList<EntangledInt> list;

		public EntangledInt m_ie;

		public int m_hashOfRegisteredItems;

		private static Manager s_manager;

		public static Manager instance
		{
			get
			{
				if (s_manager == null)
				{
					s_manager = new Manager();
					s_manager.Initialize();
				}
				return s_manager;
			}
		}

		public void Initialize()
		{
			list = new LinkedList<EntangledInt>();
			m_ie = new EntangledInt(UnityEngine.Random.Range(1073741823, int.MaxValue));
		}

		public void Register(EntangledInt i)
		{
			list.AddLast(i);
			UpdateHash();
		}

		public void Unregister(EntangledInt i)
		{
			list.Remove(i);
			UpdateHash();
		}

		private int _ComputeHashOfAllItems()
		{
			int num = 65599;
			foreach (EntangledInt item in list)
			{
				byte[] bytes = BitConverter.GetBytes(item._value);
				byte[] array = bytes;
				foreach (byte b in array)
				{
					num = b + (num << 6) + (num << 16) - num;
				}
			}
			return num;
		}

		public bool Validate()
		{
			return m_hashOfRegisteredItems == _ComputeHashOfAllItems();
		}

		public void UpdateHash()
		{
			m_hashOfRegisteredItems = _ComputeHashOfAllItems();
		}
	}

	public int m_value;

	public int m_mask;

	public int Value
	{
		get
		{
			if (!Manager.instance.Validate())
			{
				throw new SecurityException("unexpected value change detected");
			}
			return _value;
		}
		set
		{
			m_value = value ^ m_mask;
			Manager.instance.UpdateHash();
		}
	}

	private int _value
	{
		get
		{
			return m_value ^ m_mask;
		}
	}

	public EntangledInt()
	{
		_Initialize(0);
	}

	public EntangledInt(int i)
	{
		_Initialize(i);
	}

	~EntangledInt()
	{
		Manager.instance.Unregister(this);
	}

	private void _Initialize(int i)
	{
		m_mask = UnityEngine.Random.Range(0, int.MaxValue);
		m_value = i ^ m_mask;
		Manager.instance.Register(this);
	}
}
