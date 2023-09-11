using UnityEngine;

namespace TnkAd
{
	public class EventHandler : MonoBehaviour
	{
		public const int PUB_STAT_NO = 0;

		public const int PUB_STAT_YES = 1;

		public const int PUB_STAT_TEST = 2;

		public const int CLOSE_SIMPLE = 0;

		public const int CLOSE_CLICK = 1;

		public const int CLOSE_EXIT = 2;

		public const int FAIL_NO_AD = -1;

		public const int FAIL_NO_IMAGE = -2;

		public const int FAIL_TIMEOUT = -3;

		public const int FAIL_CANCELED = -4;

		public const int FAIL_NOT_PREPARED = -5;

		public const int FAIL_SYSTEM = -9;

		public string handlerName;

		private void Awake()
		{
			base.gameObject.name = handlerName;
			Object.DontDestroyOnLoad(base.gameObject);
		}

		public void onReturnQueryPointBinding(string point)
		{
			int point2 = int.Parse(point);
			onReturnQueryPoint(point2);
		}

		public void onReturnWithdrawPointsBinding(string point)
		{
			int point2 = int.Parse(point);
			onReturnWithdrawPoints(point2);
		}

		public void onReturnPurchaseItemBinding(string point)
		{
			char[] separator = new char[1] { ',' };
			string[] array = point.Split(separator);
			long curPoint = long.Parse(array[0]);
			long seqId = long.Parse(array[1]);
			onReturnPurchaseItem(curPoint, seqId);
		}

		public void onReturnQueryPublishStateBinding(string state)
		{
			int state2 = int.Parse(state);
			onReturnQueryPublishState(state2);
		}

		public void onCloseBinding(string type)
		{
			int type2 = int.Parse(type);
			onClose(type2);
		}

		public void onFailureBinding(string err)
		{
			int errCode = int.Parse(err);
			onFailure(errCode);
		}

		public void onLoadBinding(string dummy)
		{
			onLoad();
		}

		public void onShowBinding(string dummy)
		{
			onShow();
		}

		public void onVideoCompletedBinding(string skipped)
		{
			if (string.Compare(skipped, "Y") == 0)
			{
				onVideoCompleted(true);
			}
			else
			{
				onVideoCompleted(false);
			}
		}

		public virtual void onClose(int type)
		{
		}

		public virtual void onFailure(int errCode)
		{
		}

		public virtual void onLoad()
		{
		}

		public virtual void onShow()
		{
		}

		public virtual void onVideoCompleted(bool skipped)
		{
		}

		public virtual void onReturnQueryPoint(int point)
		{
		}

		public virtual void onReturnWithdrawPoints(int point)
		{
		}

		public virtual void onReturnPurchaseItem(long curPoint, long seqId)
		{
		}

		public virtual void onReturnQueryPublishState(int state)
		{
		}
	}
}
