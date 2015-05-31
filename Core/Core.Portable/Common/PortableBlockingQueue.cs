using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Common
{
	public class PortableBlockingQueue<T>
	{
		readonly Queue<T> queue = new Queue<T> ();
		readonly object _lock = new object ();

		public PortableBlockingQueue ()
		{
		}

		public void Enqueue (T item)
		{
			lock (_lock) {
				queue.Enqueue (item);
			}
		}

		public T Dequeue ()
		{
			lock (_lock) {
				return queue.Dequeue ();
			}
		}

		public T Peek ()
		{
			lock (_lock) {
				return queue.Peek ();
			}
		}

		public int Count {
			get {
				lock (_lock) {
					return queue.Count;
				}
			}
		}

		public Task EnqueueAsync (T item)
		{
			Enqueue (item);
			return Task.FromResult (0);
		}

		public Task<T> DequeueAsync ()
		{
			return Task.FromResult (Dequeue ());
		}

		public Task<T> PeekAsync ()
		{
			return Task.FromResult (Peek ());
		}
	}
}

