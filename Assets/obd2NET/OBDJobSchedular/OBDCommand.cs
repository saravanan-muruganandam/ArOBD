using System;

namespace obd2NET.OBDJobSchedular
{
	/*
	* This class hold the object for the OBD command
	*/
	class OBDCommand
	{
		public  String _obdCommandName { get; set; }
		public String _responseValue { get; set; }

		public  OBDCommand(){}

		public OBDCommand(String obdCommandName)
		{
			_obdCommandName = obdCommandName;
			
		}
	}
}
