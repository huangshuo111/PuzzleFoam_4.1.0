using System;
using System.Collections.Generic;

[Serializable]
public class ReferenceCounter
{
	public class CallBackData
	{
		public eCB Type = eCB.ChangeDisable;

		public FunctionCallBack CallBack;
	}

	public enum eCB
	{
		ChangeEnable = 0,
		ChangeDisable = 1,
		SetEnable = 2,
		SetDisable = 3
	}

	public delegate void FunctionCallBack();

	private bool bReference_;

	private bool bDefaultReference_;

	private int count_;

	private List<CallBackData> CBList_;

	public int Count
	{
		get
		{
			return count_;
		}
	}

	public bool Reference
	{
		get
		{
			return bReference_;
		}
		set
		{
			count_ += (value ? 1 : (-1));
			if (!value)
			{
				callCB(eCB.SetDisable);
			}
			else
			{
				callCB(eCB.SetEnable);
			}
			if (count_ <= 0)
			{
				if (!value && bReference_ != value)
				{
					callCB(eCB.ChangeDisable);
					bReference_ = false;
				}
			}
			else if (value && bReference_ != value)
			{
				callCB(eCB.ChangeEnable);
				bReference_ = true;
			}
		}
	}

	public ReferenceCounter(bool bReference, bool bUseCallBack)
	{
		if (bUseCallBack)
		{
			CBList_ = new List<CallBackData>();
		}
		bDefaultReference_ = bReference;
		if (bDefaultReference_)
		{
			Reference = true;
		}
	}

	public void reset()
	{
		count_ = 0;
		bReference_ = false;
		if (bDefaultReference_)
		{
			Reference = true;
		}
	}

	public void releaseCB()
	{
		clearCB();
	}

	public void addCB(CallBackData data)
	{
		if (CBList_ != null)
		{
			CBList_.Add(data);
		}
	}

	public void removeCB(CallBackData data)
	{
		if (CBList_ != null)
		{
			CBList_.Remove(data);
		}
	}

	public void clearCB()
	{
		if (CBList_ != null)
		{
			CBList_.Clear();
		}
	}

	private void callCB(eCB type)
	{
		if (CBList_ == null)
		{
			return;
		}
		foreach (CallBackData item in CBList_)
		{
			if (item.Type == type && item.CallBack != null)
			{
				item.CallBack();
			}
		}
	}
}
