using System;

namespace IteratorTasks
{
	public class CancellationTokenSource
	{
		private Exception _cancelReason;

		public CancellationToken Token { get; private set; }

		public bool IsCancellationRequested { get; private set; }

		internal Exception CancelReason
		{
			get
			{
				return _cancelReason;
			}
		}

		internal event Action Canceled;

		public CancellationTokenSource()
		{
			Token = new CancellationToken(this);
		}

		public void Cancel()
		{
			Action canceled = this.Canceled;
			if (canceled != null)
			{
				canceled();
			}
			IsCancellationRequested = true;
		}

		public void Cancel(Exception cancelReason)
		{
			_cancelReason = cancelReason;
			Cancel();
		}
	}
}
