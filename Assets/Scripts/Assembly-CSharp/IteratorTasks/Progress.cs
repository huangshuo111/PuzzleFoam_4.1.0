using System;

namespace IteratorTasks
{
	public class Progress<T> : IProgress<T>
	{
		public event Action<T> ProgressChanged;

		public Progress()
		{
		}

		public Progress(Action<T> onProgressChanged)
		{
			this.ProgressChanged = (Action<T>)Delegate.Combine(this.ProgressChanged, onProgressChanged);
		}

		void IProgress<T>.Report(T value)
		{
			Action<T> progressChanged = this.ProgressChanged;
			if (progressChanged != null)
			{
				progressChanged(value);
			}
		}
	}
}
