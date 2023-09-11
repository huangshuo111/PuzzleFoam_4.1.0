namespace IteratorTasks
{
	public interface IProgress<T>
	{
		void Report(T value);
	}
}
