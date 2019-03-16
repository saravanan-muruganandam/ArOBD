using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace obd2NET.OBDJobSchedular
{

	class OBDJobService //: MonoBehaviour
	{
		

		private static  BlockingCollection<OBDCommand> messageRequestQueue;
		private static BlockingCollection<OBDCommand> messageResponseQueue;
		private static List<CommandJob> runningJobsList = new List<CommandJob> { };
		private readonly CancellationTokenSource m_cancelTokenSrc;
		public static ObdAdapter ObdAdapter;

		private OBDJobService()
		{
			messageRequestQueue = new BlockingCollection<OBDCommand>(new ConcurrentQueue<OBDCommand>(), 100);
			messageResponseQueue = new BlockingCollection<OBDCommand>(new ConcurrentQueue<OBDCommand>());
			m_cancelTokenSrc = new CancellationTokenSource();

			ObdAdapter = new ObdAdapter();
			

			StartRequestQueueProcessor();
		}
		private static  OBDJobService instance = null;

		public static OBDJobService Instance()
		{
			//Debug.Log(instance);

			if (instance==null) {
				instance = new OBDJobService();
			}
				return instance;
			

		}
		
		public BlockingCollection<OBDCommand> _getObdResponseQueue()
		{
			return messageResponseQueue;
		}

		public bool GetVehicleConnectionStatus()
		{
			return ObdAdapter.ObdAdapterConnectionStatus();
		}
		
		public void ExecuteCommandJob(String commandName) {
			OBDCommand command = new OBDCommand(commandName);
			try
			{
				if (!runningJobsList.Any(c => (c.getCommand()._obdCommandName == commandName)))
				{
					CommandJob obdCommandJob = new CommandJob();
					CancellationTokenSource cancelToken = new CancellationTokenSource();
					obdCommandJob.setCancellationToken(cancelToken);
					obdCommandJob._command = command;
	
					obdCommandJob._commandTask = Task.Run(async () =>  // <- marked async
					{
						while (!cancelToken.Token.IsCancellationRequested)
						{
							if (OBDJobService.Instance().GetVehicleConnectionStatus())
							{
								messageRequestQueue.Add(command);
								Debug.Log("new command added, commandqueue count " + messageRequestQueue.Count);
							}
							
							await Task.Delay(200); // <- await with cancellation
						}
					});
					Debug.Log("NEW TASK ADDED FOR : " + obdCommandJob.getCommand()._obdCommandName);
					runningJobsList.Add(obdCommandJob);
					obdCommandJob._commandTask.Start();
					//obdCommandJob.setState(CommandJob.ObdCommandJobState.RUNNING);
				}
				else if (runningJobsList.Any(c => (c.getCommand()._obdCommandName == commandName) && (c._commandTask.Status != TaskStatus.Running)))
				{
					CommandJob obdCommandJob = runningJobsList.Find(c => (c.getCommand()._obdCommandName == command._obdCommandName));
					Debug.Log("RESTARTING TASK ADDED FOR : " + command._obdCommandName);
					obdCommandJob._commandTask.Start();
				}
			}
			catch
			{
			}
			finally
			{

			}

		}

		private void StartRequestQueueProcessor()
		{
			Task requestProcessorTask = Task.Run(async () => {
				try
				{
					while (!m_cancelTokenSrc.Token.IsCancellationRequested)
					{
						if ((messageRequestQueue.Count > 0) )
						{
							OBDCommand command = messageRequestQueue.Take();
							if (Instance().GetVehicleConnectionStatus())
							{
								//Debug.Log(vehicle.GetCurrentData(command._obdCommandName));
								command._responseValue = ObdAdapter.GetCurrentData(command._obdCommandName).ToString();
								//Debug.Log("REQUEST QUEUE PROCESSING " + messageRequestQueue.Count);
							}
							else
							{
								command._responseValue = "NO DATA";
							}
							messageResponseQueue.Add(command);
						}
						await Task.Delay(200);
					}
				}

				catch(Exception e) {
					Debug.Log("Exception: {0}"+ e);
				}
			});

		}

		public void StopAndRemoveTaskFromJobList(String commandName)
		{
			foreach (CommandJob job in runningJobsList)
			{
				if (job.getCommand()._obdCommandName == commandName)
				{
					job._cancellationToken.Cancel();
					job._cancellationToken.Dispose();

				}
			}
			runningJobsList.RemoveAll(c => c.getCommand()._obdCommandName == commandName);

		}

		public bool ConnectToVehicleOBD(String IpAddress, Int32 Port)
		{
			return ObdAdapter.ConnectToObdServer(IpAddress, Port);
		}

		public bool CloseConnectionToVehicle()
		{
			return ObdAdapter.CloseObdConnection();
		}

		#region IDisposable

		private bool m_isDisposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!m_isDisposed)
			{
				if (disposing)
				{
					messageRequestQueue.Dispose();
					//messageResponseQueue.Dispose();

					m_cancelTokenSrc.Cancel();
					m_cancelTokenSrc.Dispose();

				}

				m_isDisposed = true;
			}
		}

		public void _dispose()
		{
			//Debug.Log(vehicle.CloseObdConnection().ToString());
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion



	}
}
