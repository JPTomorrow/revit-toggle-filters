using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.Revit.Tools
{

	public static class REP
	{

		public static void ConvertIdsToAll(
			IEnumerable<ElementId> els_to_cvt,
			Action<ElementId, Window> proc_func, Action end_func,
			int ms_interval = 100, Window parent_window = null)
		{
			if(!els_to_cvt.Any()) return;
			Queue<ElementId> input_elements = new Queue<ElementId>(els_to_cvt);
			input_elements.Enqueue(new ElementId(-1));

			DispatcherTimer process_timer = new DispatcherTimer();
			process_timer.Tick += Tick;

			void Tick(object sender, EventArgs e)
			{
				if(input_elements.Any() && input_elements.Peek().IntegerValue != -1)
				{
					proc_func(input_elements.Dequeue(), parent_window);
				}
				else
				{
					process_timer.Stop();
					process_timer.Tick -= Tick;
					end_func();
				}
			}

			process_timer.Interval = new TimeSpan(ms_interval);
			process_timer.Start();
		}


	}
}