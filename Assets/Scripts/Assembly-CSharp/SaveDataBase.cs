using UnityEngine;

public abstract class SaveDataBase
{
	private bool bSetuped_;

	private bool bLoaded_;

	protected abstract void OnSetup();

	protected abstract void OnLoad();

	protected abstract void OnReset();

	protected abstract void OnSave();

	public void setup()
	{
		if (!isSetuped())
		{
			OnSetup();
			bSetuped_ = true;
		}
	}

	public void load()
	{
		if (isSetuped())
		{
			OnLoad();
			bLoaded_ = true;
		}
	}

	public void reset()
	{
		if (isSetuped())
		{
			OnReset();
		}
	}

	public void save()
	{
		if (isSetuped())
		{
			OnSave();
			PlayerPrefs.Save();
		}
	}

	public bool isLoaded()
	{
		return bLoaded_;
	}

	private bool isSetuped()
	{
		return bSetuped_;
	}

	protected void setFlag(ref int bitmap, int bit, bool bEnable)
	{
		if (bEnable)
		{
			bitmap |= 1 << (bit & 0x1F);
		}
		else
		{
			bitmap &= ~(1 << (bit & 0x1F));
		}
	}

	protected bool getFlag(int bitmap, int bit)
	{
		if ((bitmap & (1 << bit)) != 0)
		{
			return true;
		}
		return false;
	}

	protected void setGuestFlag(ref int bitmap, int bit, bool bEnable)
	{
		if (bEnable)
		{
			bitmap |= 1 << (bit & 0x1F);
		}
		else
		{
			bitmap &= ~(1 << (bit & 0x1F));
		}
	}

	protected bool getGuestFlag(int bitmap, int bit)
	{
		if ((bitmap & (1 << bit)) != 0)
		{
			return true;
		}
		return false;
	}

	protected int getValue(int bitmap, int bit)
	{
		return getValue(bitmap, bit, 1);
	}

	protected int getValue(int bitmap, int bit, int useBit)
	{
		int num = 0;
		for (int i = 0; i < useBit; i++)
		{
			if (getFlag(bitmap, bit + i))
			{
				num += 1 << (i & 0x1F);
			}
		}
		return num;
	}

	protected void setValue(ref int bitmap, int value, int bit)
	{
		setValue(ref bitmap, value, 1);
	}

	protected void setValue(ref int bitmap, int value, int bit, int useBit)
	{
		for (int i = 0; i < useBit; i++)
		{
			setFlag(ref bitmap, bit + i, false);
		}
		if (value == 0)
		{
			return;
		}
		for (int j = 0; j != useBit; j++)
		{
			setFlag(ref bitmap, bit + j, (value % 2 != 0) ? true : false);
			value /= 2;
			if (value <= 0)
			{
				break;
			}
		}
	}
}
