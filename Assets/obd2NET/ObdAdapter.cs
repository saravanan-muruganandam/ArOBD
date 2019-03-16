﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



namespace obd2NET
{
    /// <summary>
    /// Vehicle that can be accessed through an <c>IOBDConnection</c>
    /// </summary>
    public  class ObdAdapter
    {
        /// <summary>
        /// List of supported PIDs
        /// <see cref="http://en.wikipedia.org/wiki/OBD-II_PIDs#Mode_1_PID_03"/>
        /// </summary>
        public enum PID
        {
            Unknown = 0x0,
            MIL = 0x01,
            DTCCount = 0x01,
            Speed = 0x0D,
            EngineTemperature = 0x05,
            RPM = 0x0C,
            ThrottlePosition = 0x11,
            CalculatedEngineLoadValue = 0x04,
            FuelPressure = 0x0A
        };

		private String IpAddress="127.0.0.1";
		private Int32 Port=13000;
		public static IOBDConnection obdConnection = new TcpConnection();
		/// <summary>
		/// List of supported modes
		/// <see cref="http://en.wikipedia.org/wiki/OBD-II_PIDs#Mode_1_PID_03"/>
		/// </summary>
		public enum Mode
        {
            Unknown = 0x00,
            CurrentData = 0x01,
            FreezeFrameData = 0x02,
            DiagnosticTroubleCodes = 0x03
        }

		public bool ConnectToObdServer(String IpAddress, Int32 Port)
		{
			this.IpAddress = IpAddress;
			this.Port = Port;
			obdConnection.Open(IpAddress, Port);
			return obdConnection.GetTcpConnectionStatus();
		}

		public bool  CloseObdConnection()
		{
			obdConnection.Close();
			return obdConnection.GetTcpConnectionStatus();
		}

		public bool ObdAdapterConnectionStatus()
		{
			return obdConnection.GetTcpConnectionStatus();
		}

		public uint TestCommand()
		{
			ControllerResponse response = obdConnection.Query(Mode.CurrentData, PID.EngineTemperature);
			if (response.HasValidData()) throw new QueryException("Engine temperature couldn't be queried, the controller returned no data");

			return Convert.ToUInt32(response.Raw);
		}
		public  uint CurrentSpeed()
        {
            ControllerResponse response = obdConnection.Query(Mode.CurrentData, PID.Speed);
            if (response.HasValidData()) throw new QueryException("Vehicle speed couldn't be queried, the controller returned no data");

            // the first value byte represents the speed in km/h
            return (response.Value.Length >= 1) ? Convert.ToUInt32(response.Value.First()) : 0;
        }

        /// <summary>
        /// Queries the current engine temperature
        /// </summary>
        /// <param name="obdConnection"> Connection to the vehicle interface used to query the data </param>
        /// <returns> Temperature given in celsius </returns>
        public uint CurrentEngineTemperature()
        {
			 ControllerResponse response = obdConnection.Query(Mode.CurrentData, PID.EngineTemperature);
			 if (response.HasValidData()) throw new QueryException("Engine temperature couldn't be queried, the controller returned no data");

			//return response.Raw;

			// the first value byte minus 40 represents the engine temperature in celsius
			 return (response.Value.Length >= 1) ? Convert.ToUInt32(response.Value.First()) - 40 : 0;
			
			//return new System.Random().Next(200, 210).ToString();
		}

        /// <summary>
        /// Queries the current RPM
        /// </summary>
        /// <param name="obdConnection"> Connection to the vehicle interface used to query the data </param>
        /// <returns> The retrieved RPM </returns>
        public  uint CurrentRPM()
        {
            ControllerResponse response = obdConnection.Query(Mode.CurrentData, PID.RPM);
            if (response.HasValidData()) throw new QueryException("RPM couldn't be queried, the controller returned no data");

            // RPM is given in 2 bytes
            // Formula: ((A*256)+B)/4 
            if (response.Value.Length < 2) throw new QueryException("RPM couldn't be queried, retrieved data was uncomplete");
            
            uint rpm1 = Convert.ToUInt32(response.Value.First());
            uint rpm2 = Convert.ToUInt32(response.Value.ElementAt(1));
            return ((rpm1 * 256) + rpm2) / 4;  
        }

        /// <summary>
        /// Queries the current throttle position
        /// </summary>
        /// <param name="obdConnection"> Connection to the vehicle interface used to query the data </param>
        /// <returns> The retrieved throttle position in percentage (0-100) </returns>
        public  uint CurrentThrottlePosition()
        {
            ControllerResponse response = obdConnection.Query(Mode.CurrentData, PID.ThrottlePosition);
            if (response.HasValidData()) throw new QueryException("Throttle position couldn't be queried, the controller returned no data");

            // given in percentage, first value byte *100/255
            return (response.Value.Length >= 1) ? (Convert.ToUInt32(response.Value.First()) * 100) / 255 : 0;
        }

        /// <summary>
        /// Queries the current calculated engine load value
        /// </summary>
        /// <param name="obdConnection"> Connection to the vehicle interface used to query the data </param>
        /// <returns> The retrieved calculated engine load value in percentage (0-100) </returns>
        public  uint CurrentEngineLoad()
        {
            ControllerResponse response = obdConnection.Query(Mode.CurrentData, PID.CalculatedEngineLoadValue);
            if (response.HasValidData()) throw new QueryException("Calculated engine load value couldn't be queried, the controller returned no data");

            // given in percentage, first value byte *100/255
            return (response.Value.Length >= 1) ? (Convert.ToUInt32(response.Value.First()) * 100) / 255 : 0;
        }

        /// <summary>
        /// Queries the current fuel pressure
        /// </summary>
        /// <param name="obdConnection"> Connection to the vehicle interface used to query the data </param>
        /// <returns> The retrieved fuel pressure given in kPa </returns>
        public  uint CurrentFuelPressure()
        {
            ControllerResponse response = obdConnection.Query(Mode.CurrentData, PID.FuelPressure);
            if (response.HasValidData()) throw new QueryException("Fuel pressure couldn't be queried, the controller returned no data");

            // given in kPa, first value byte * 3
            return (response.Value.Length >= 1) ? Convert.ToUInt32(response.Value.First()) * 3 : 0;
        }

        /// <summary>
        /// Queries the status of the malfunction indicator lamp (MIL)
        /// </summary>
        /// <param name="obdConnection"> Connection to the vehicle interface used to query the data </param>
        /// <returns> true if the MIL is illuminated, false if not </returns>
        public  bool CurrentMalfunctionIndicatorLamp()
        {
            ControllerResponse response = obdConnection.Query(Mode.CurrentData, PID.MIL);
            if (response.HasValidData()) throw new QueryException("Malfunction indicator lamp couldn't be queried, the controller returned no data");
            if (response.Value.Length == 0) return false;

            return Convert.ToBoolean((response.Value.First() >> 7) & 1);
        }

        /// <summary>
        /// Queries the available diagnostic trouble codes (DTCs)
        /// </summary>
        /// <param name="obdConnection"> Connection to the vehicle interface used to query the data </param>
        /// <returns> List containing all DTCs that could be retrieved </returns>
        public  List<DiagnosticTroubleCode> DiagnosticTroubleCodes()
        {
            // issue the request for the actual DTCs
            ControllerResponse response = obdConnection.Query(Mode.DiagnosticTroubleCodes);
            if (response.HasValidData()) throw new QueryException("Diagnostic trouble codes couldn't be queried, the controller returned no data");
            if (response.Value.Length < 2) throw new QueryException("Diagnostic trouble codes couldn't be queried, received data was not complete");

            var fetchedCodes = new List<DiagnosticTroubleCode>();
            for (int i = 1; i < response.Value.Length; i += 3) // each error code got a size of 3 bytes
            {
                byte[] troubleCode = new byte[3];
                Array.Copy(response.Value, i, troubleCode, 0, 3);

                if(!troubleCode.All(b => b == default(Byte))) // if the byte array is not entirely empty, add the error code to the parsed list
                    fetchedCodes.Add(new DiagnosticTroubleCode(troubleCode));
            }

            return fetchedCodes;
        }

		public uint GetCurrentData(String CommandName)
		{
			uint value = 0;
			switch (CommandName)
			{
				case "RPM":
					value= CurrentRPM();
					break;
				case "ENGINETEMPERATURE":
					value= CurrentEngineTemperature();
					break;
				case "FUELPREASURE":
					value = CurrentFuelPressure();
					break;
				case "ENGINELOAD":
					value = CurrentEngineLoad();
					break;
				case "THROTTLEPOSITION":
					value = CurrentThrottlePosition();
					break;
				case "TESTCOMMAND":
					value = TestCommand();
					break;
				default:
					value = TestCommand();
					break;
					//throw new InvalidOperationException("unknown command type");
			}

			return value;
		}
    }
}
