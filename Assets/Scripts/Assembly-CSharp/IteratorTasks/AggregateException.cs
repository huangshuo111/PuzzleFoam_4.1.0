using System;
using System.Collections.Generic;

namespace IteratorTasks
{
	public class AggregateException : Exception
	{
		private List<Exception> _exceptions;

		public IEnumerable<Exception> Exceptions
		{
			get
			{
				return _exceptions.ToArray();
			}
		}

		public override string Message
		{
			get
			{
				int count = _exceptions.Count;
				if (count == 1)
				{
					return _exceptions[0].Message;
				}
				if (count > 1)
				{
					return string.Format("AggregateException: {0} errors", count);
				}
				return base.Message;
			}
		}

		public AggregateException(List<Exception> exceptions)
		{
			_exceptions = exceptions;
		}
	}
}
