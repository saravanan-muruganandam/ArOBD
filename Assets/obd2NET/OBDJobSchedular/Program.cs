using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace obd2NET.OBDJobSchedular
{

	class PrintThreadNameToProducer
	{


		public async Task PrintThreadNameAsync()
		{
			await Task.Delay(2000);

			//ObdMessageManager.Instance.AddJobToTheQueue("TASKA");

		}
	}
	class Program
	{
		static void Main(string[] args)
		{
			OBDJobService.Instance().ExecuteCommandJob("RPM");



		}
	}
}
