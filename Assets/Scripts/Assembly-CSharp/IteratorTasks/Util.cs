using System;
using System.Collections;

namespace IteratorTasks
{
	public static class Util
	{
		public static IEnumerator EmptyRoutine()
		{
			return new object[0].GetEnumerator();
		}

		public static IEnumerator Concat(Func<IEnumerator> e1, Func<IEnumerator> e2)
		{
			IEnumerator x1 = e1();
			while (x1.MoveNext())
			{
				yield return x1.Current;
			}
			Dispose(x1);
			IEnumerator x2 = e2();
			while (x2.MoveNext())
			{
				yield return x2.Current;
			}
			Dispose(x2);
		}

		public static void Dispose(object obj)
		{
			IDisposable disposable = obj as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}
}
