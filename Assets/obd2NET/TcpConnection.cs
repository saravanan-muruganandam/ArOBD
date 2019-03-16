using System;
using System.Net.Sockets;
namespace obd2NET
{
	public class TcpConnection : IOBDConnection
	{

		private String IpAddress { get; set; }
		private Int32 Port { get; set; }

		private static TcpClient TcpClientConnection =null;

		public TcpConnection()
		{

			TcpClientConnection = new TcpClient();

		}


		public bool GetTcpConnectionStatus()
		{
			try
			{
				return (TcpClientConnection.Connected && TcpClientConnection.GetStream().CanRead);
			}
			catch (Exception)
			{
				return false;
			}
		
		}

		public void Open(String IpAddress, Int32 Port) {
			
			try
			{
				if (!(TcpClientConnection==null))
				{
					TcpClientConnection = new TcpClient();
					if (!TcpClientConnection.Connected)
					{
						TcpClientConnection.Connect(IpAddress, Port);
						Console.Write("connected");
					}
					else
					{
						Console.WriteLine("client is already connected");
					}
					return ;
				}

			}
			catch (SocketException e)
			{
				Close();
				Console.WriteLine("SocketException: {0}", e);

			}

		}
		public void Close()
		{
			if (TcpClientConnection.Connected){
				TcpClientConnection.Client.Shutdown(SocketShutdown.Both);
				TcpClientConnection.Client.Close();
		}
		}
		public ControllerResponse Query(ObdAdapter.Mode parameterMode, ObdAdapter.PID parameterID) {
			String responseData = String.Empty;
			try
			{
				Byte[] data = System.Text.Encoding.ASCII.GetBytes(Convert.ToUInt32(parameterMode).ToString("X2") + Convert.ToUInt32(parameterID).ToString("X2") + "\r");
				TcpClientConnection.GetStream().Write(data, 0, data.Length);

				Console.WriteLine("Sent: {0}", data.ToString());

				data = new Byte[256];
				Int32 bytes = TcpClientConnection.GetStream().Read(data, 0, data.Length);
				responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
				Console.WriteLine("Received: {0}", responseData);
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("ArgumentNullException: {0}", e);
			}
			catch (SocketException e)
			{
				TcpClientConnection.Client.Shutdown(SocketShutdown.Both);
				TcpClientConnection.Client.Disconnect(true);
				Console.WriteLine("SocketException: {0}", e);
			}

			return new ControllerResponse(responseData, parameterMode, parameterID);

		}
		public ControllerResponse Query(ObdAdapter.Mode parameterMode) {
			String responseData = String.Empty;
			try
			{
				Byte[] data = System.Text.Encoding.ASCII.GetBytes(Convert.ToUInt32(parameterMode).ToString("X2") + "\r");
				TcpClientConnection.GetStream().Write(data, 0, data.Length);

				Console.WriteLine("Sent: {0}", data.ToString());

				data = new Byte[256];
				Int32 bytes = TcpClientConnection.GetStream().Read(data, 0, data.Length);
				responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
				Console.WriteLine("Received: {0}", responseData);
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("ArgumentNullException: {0}", e);
			}
			catch (SocketException e)
			{
				Console.WriteLine("SocketException: {0}", e);
				TcpClientConnection.Client.Shutdown(SocketShutdown.Both);
			}

			return new ControllerResponse(responseData, parameterMode);

		}
	}
}
