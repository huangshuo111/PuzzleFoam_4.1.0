using System.Collections.Generic;

namespace IteratorTasks
{
	public class TaskRunner : TaskScheduler
	{
		private List<Task> _runningTasks = new List<Task>();

		private List<Task> _toBeRemoved = new List<Task>();

		public override void QueueTask(Task task)
		{
			_runningTasks.Add(task);
		}

		public int GetTaskNum()
		{
			return _runningTasks.Count;
		}

		public void Update()
		{
			foreach (Task runningTask in _runningTasks)
			{
				if (!runningTask.MoveNext())
				{
					_toBeRemoved.Add(runningTask);
				}
			}
			foreach (Task item in _toBeRemoved)
			{
				_runningTasks.Remove(item);
			}
			_toBeRemoved.Clear();
		}

		public void Update(int numFrames)
		{
			for (int i = 0; i < numFrames; i++)
			{
				Update();
			}
		}
	}
}
