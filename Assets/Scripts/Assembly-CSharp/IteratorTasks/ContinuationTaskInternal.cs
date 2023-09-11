using System;

namespace IteratorTasks
{
	internal class ContinuationTaskInternal
	{
		private Task _task;

		private Func<Task> _continuation;

		private Action<Exception> _addError;

		private Action _complete;

		internal Task LastTask { get; private set; }

		public object Current
		{
			get
			{
				if (_task != null)
				{
					return _task.Current;
				}
				return null;
			}
		}

		internal ContinuationTaskInternal(Task firstTask, Func<Task> continuation, Action<Exception> addError, Action complete)
		{
			_task = firstTask;
			_continuation = continuation;
			_addError = addError;
			_complete = complete;
			LastTask = null;
		}

		public bool MoveNext()
		{
			if (_task != null)
			{
				if (_task.MoveNext())
				{
					return true;
				}
				if (_task.Error != null)
				{
					_addError(_task.Error);
					Complete();
					return false;
				}
				if (_continuation != null)
				{
					_task = _continuation();
					_continuation = null;
					return true;
				}
				Complete();
				return false;
			}
			return false;
		}

		private void Complete()
		{
			LastTask = _task;
			_complete();
			_task = null;
			_continuation = null;
		}
	}
}
