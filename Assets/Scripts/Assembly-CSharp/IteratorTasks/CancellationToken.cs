using System;

namespace IteratorTasks
{
	public struct CancellationToken
	{
		private CancellationTokenSource _source;

		public static CancellationToken None = default(CancellationToken);

		public bool IsCancellationRequested
		{
			get
			{
				if (_source == null)
				{
					return false;
				}
				return _source.IsCancellationRequested;
			}
		}

		internal CancellationToken(CancellationTokenSource source)
		{
			_source = source;
		}

		public void Register(Action onCanceled)
		{
			if (_source != null)
			{
				_source.Canceled += onCanceled;
			}
		}

		public void ThrowIfCancellationRequested()
		{
			if (IsCancellationRequested)
			{
				Exception ex = ((_source.CancelReason == null) ? new TaskCanceledException() : _source.CancelReason);
				throw ex;
			}
		}
	}
}
