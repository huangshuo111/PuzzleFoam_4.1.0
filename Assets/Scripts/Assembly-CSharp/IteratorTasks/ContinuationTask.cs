using System;

namespace IteratorTasks
{
	internal class ContinuationTask : Task
	{
		private ContinuationTaskInternal _inner;

		public override object Current
		{
			get
			{
				return _inner.Current;
			}
		}

		internal ContinuationTask(Task firstTask, Func<Task> continuation)
		{
			_inner = new ContinuationTaskInternal(firstTask, continuation, base.AddError, base.Complete);
		}

		public override bool MoveNext()
		{
			return _inner.MoveNext();
		}
	}
	internal class ContinuationTask<U> : Task<U>
	{
		private ContinuationTaskInternal _inner;

		public override object Current
		{
			get
			{
				return _inner.Current;
			}
		}

		public override U Result
		{
			get
			{
				Task<U> task = _inner.LastTask as Task<U>;
				if (task != null)
				{
					return task.Result;
				}
				return default(U);
			}
		}

		internal ContinuationTask(Task firstTask, Func<Task<U>> continuation)
		{
			_inner = new ContinuationTaskInternal(firstTask, () => continuation(), base.AddError, base.Complete);
		}

		public override bool MoveNext()
		{
			return _inner.MoveNext();
		}
	}
}
