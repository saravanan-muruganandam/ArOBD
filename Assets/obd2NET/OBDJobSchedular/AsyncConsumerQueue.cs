using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace obd2NET.OBDJobSchedular
{
	public class AsyncConsumerQueue<T> : IDisposable
	{
		private readonly Action<T> m_consumer;
		private readonly BlockingCollection<T> m_queue;
		private readonly CancellationTokenSource m_cancelTokenSrc;

		public AsyncConsumerQueue(BlockingCollection<T> blockingQueue, Action<T> consumer)
		{
			if (consumer == null)
			{
				throw new ArgumentNullException(nameof(consumer));
			}

			m_consumer = consumer;
			m_queue = blockingQueue; // new BlockingCollection<T>(new ConcurrentQueue<T>());
			m_cancelTokenSrc = new CancellationTokenSource();

			new Task(() => ConsumeLoop(m_cancelTokenSrc.Token)).Start();
		}


		private void ConsumeLoop(CancellationToken cancelToken)
		{
			Debug.Log("CONSUMER STARTED");
			while (!cancelToken.IsCancellationRequested)
			{
				try
				{
					var item = m_queue.Take(cancelToken);
					
					m_consumer(item);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine(ex);
				}
			}
		}

		#region IDisposable

		private bool m_isDisposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!m_isDisposed)
			{
				if (disposing)
				{
					m_cancelTokenSrc.Cancel();
					m_cancelTokenSrc.Dispose();
					m_queue.Dispose();
				}

				m_isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
