using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IteratorTasks
{
	public class Task : IDisposable, IEnumerator
	{
		private AggregateException _error;

		private List<Exception> _errors;

		private List<Action<Task>> _callback = new List<Action<Task>>();

		private static readonly Task _empty = new Task(TaskStatus.RanToCompletion);

		protected IEnumerator Routine { get; set; }

		public AggregateException Error
		{
			get
			{
				Task task = Routine as Task;
				if (task != null && task.Error != null)
				{
					AddError(task.Error);
					task.ClearError();
				}
				return _error;
			}
			private set
			{
				_error = value;
			}
		}

		public virtual object Current
		{
			get
			{
				if (IsCanceled)
				{
					return null;
				}
				return (Routine != null) ? Routine.Current : null;
			}
		}

		public TaskStatus Status { get; private set; }

		public bool IsDone
		{
			get
			{
				return IsCompleted || IsCanceled || IsFaulted;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return Status == TaskStatus.RanToCompletion;
			}
		}

		public bool IsCanceled
		{
			get
			{
				return Status == TaskStatus.Canceled;
			}
		}

		public bool IsFaulted
		{
			get
			{
				return Status == TaskStatus.Faulted;
			}
		}

		public CancellationTokenSource Cancellation { get; set; }

		protected Task()
		{
		}

		protected Task(TaskStatus status)
		{
			Status = status;
		}

		public Task(IEnumerator routine)
		{
			Routine = routine;
		}

		public Task(Func<IEnumerator> starter)
		{
			Routine = starter();
		}

		void IEnumerator.Reset()
		{
			throw new NotImplementedException();
		}

		public Task ContinueWithTask(Func<Task> continuation)
		{
			return new ContinuationTask(this, continuation);
		}

		public Task ContinueWith(Func<IEnumerator> continuation)
		{
			return ContinueWithTask(() => new Task(continuation()));
		}

		public Task<U> ContinueWithTask<U>(Func<Task<U>> continuation)
		{
			return new ContinuationTask<U>(this, continuation);
		}

		public Task<U> ContinueWith<U>(Func<Action<U>, IEnumerator> continuation)
		{
			return ContinueWithTask(() => new Task<U>(continuation));
		}

		public static Task WhenAllTask(params Task[] routines)
		{
			return new Task(WhenAll(routines));
		}

		public static IEnumerator WhenAll(params Task[] routines)
		{
			int successCount = 0;
			List<Exception> errors = new List<Exception>();
			foreach (Task r in routines)
			{
				r.OnComplete(delegate(Task t)
				{
					if (t.Error == null)
					{
						successCount++;
					}
					else
					{
						errors.AddRange(t.Error.Exceptions);
					}
				});
			}
			do
			{
				for (int i = 0; i < routines.Length; i++)
				{
					Task r2 = routines[i];
					if (r2 != null)
					{
						if (r2.MoveNext())
						{
							yield return r2.Current;
						}
						else
						{
							routines[i] = null;
						}
					}
				}
			}
			while (routines.Any((Task x) => x != null));
			if (errors.Count != 0)
			{
				throw new AggregateException(errors);
			}
		}

		protected void AddError(Exception exc)
		{
			if (_error == null)
			{
				_errors = new List<Exception>();
				_error = new AggregateException(_errors);
			}
			AggregateException ex = exc as AggregateException;
			if (ex != null)
			{
				foreach (Exception exception in ex.Exceptions)
				{
					_errors.Add(exception);
				}
				return;
			}
			_errors.Add(exc);
		}

		protected void ClearError()
		{
			if (_error != null)
			{
				_errors = null;
				_error = null;
			}
		}

		public virtual bool MoveNext()
		{
			if (Status == TaskStatus.Created)
			{
				Status = TaskStatus.Running;
			}
			if (Status != TaskStatus.Running)
			{
				return false;
			}
			if (Routine == null)
			{
				return false;
			}
			bool flag;
			try
			{
				flag = Routine.MoveNext();
			}
			catch (Exception exc)
			{
				AddError(exc);
				flag = false;
			}
			if (!flag)
			{
				Complete();
			}
			return flag;
		}

		public void Dispose()
		{
			IDisposable disposable = Routine as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			Routine = null;
		}

		public void Start(TaskScheduler scheduler)
		{
			if (Status != 0)
			{
				throw new InvalidOperationException();
			}
			scheduler.QueueTask(this);
		}

		protected void Complete()
		{
			if (Error != null)
			{
				Status = TaskStatus.Faulted;
			}
			else
			{
				Status = TaskStatus.RanToCompletion;
			}
			if (_callback.Count != 0)
			{
				foreach (Action<Task> item in _callback)
				{
					Invoke(item);
				}
			}
			_callback.Clear();
		}

		private void Invoke(Action<Task> c)
		{
			try
			{
				c(this);
			}
			catch (Exception exc)
			{
				AddError(exc);
			}
		}

		public Task OnComplete(Action<Task> callback)
		{
			if (IsDone)
			{
				Invoke(callback);
			}
			else
			{
				_callback.Add(callback);
			}
			return this;
		}

		public Task OnError<T>(Action<T> errorHandler) where T : Exception
		{
			return OnComplete(delegate(Task t)
			{
				if (t.Error != null)
				{
					foreach (Exception exception in t.Error.Exceptions)
					{
						T val = exception as T;
						if (val != null)
						{
							errorHandler(val);
						}
					}
				}
			});
		}

		public Task OnErrorAsOne(Action<IEnumerable<Exception>> errorHandler)
		{
			return OnComplete(delegate(Task t)
			{
				if (t.Error != null)
				{
					errorHandler(t.Error.Exceptions);
				}
			});
		}

		public void Cancel()
		{
			if (Cancellation == null)
			{
				throw new InvalidOperationException("Can't cancel Task.");
			}
			Cancellation.Cancel();
			MoveNext();
		}

		public void Cancel(Exception e)
		{
			if (Cancellation == null)
			{
				throw new InvalidOperationException("Can't cancel Task.");
			}
			Cancellation.Cancel(e);
			MoveNext();
		}

		public void ForceCancel()
		{
			ForceCancel(new TaskCanceledException("Task force canceled."));
		}

		public void ForceCancel(Exception e)
		{
			Status = TaskStatus.Canceled;
			AddError(e);
			Dispose();
		}

		private Task Check()
		{
			if (_error != null)
			{
				throw _error;
			}
			return this;
		}

		public static Task Empty()
		{
			return _empty;
		}

		public static Task<T> Return<T>(T value)
		{
			return new Task<T>(value);
		}

		public Task<U> Select<U>(Func<U> selector)
		{
			return ContinueWithTask(() => Return(selector()));
		}
	}
	public class Task<T> : Task
	{
		private T _result;

		public virtual T Result
		{
			get
			{
				if (base.Error != null)
				{
					throw base.Error;
				}
				return _result;
			}
		}

		internal Task()
		{
		}

		internal Task(T result)
			: base(TaskStatus.RanToCompletion)
		{
			_result = result;
		}

		public Task(Func<Action<T>, IEnumerator> starter)
		{
			base.Routine = starter(delegate(T r)
			{
				_result = r;
			});
		}

		public Task ContinueWithTask(Func<T, Task> continuation)
		{
			return new ContinuationTask(this, delegate
			{
				T result = Result;
				_result = default(T);
				GC.Collect();
				return continuation(result);
			});
		}

		public Task ContinueWith(Func<T, IEnumerator> continuation)
		{
			return ContinueWithTask(() => new Task(continuation(Result)));
		}

		public Task<U> ContinueWithTask<U>(Func<T, Task<U>> continuation)
		{
			return new ContinuationTask<U>(this, delegate
			{
				T result = Result;
				return continuation(result);
			});
		}

		public Task<U> ContinueWith<U>(Func<T, Action<U>, IEnumerator> continuation)
		{
			Func<T, Task<U>> continuation2 = (T x) => new Task<U>((Action<U> a) => continuation(x, a));
			return ContinueWithTask(continuation2);
		}

		public Task<T> OnComplete(Action<Task<T>> callback)
		{
			OnComplete((Action<Task>)delegate
			{
				callback(this);
			});
			return this;
		}

		private Task<T> Check()
		{
			if (base.Error != null)
			{
				throw base.Error;
			}
			return this;
		}

		public Task<U> Select<U>(Func<T, U> selector)
		{
			return ContinueWithTask((T x) => Task.Return(selector(x)));
		}
	}
}
