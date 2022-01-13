using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.Revit.Transactions
{
	public struct TransactionTunnel
	{
		private static Queue<TransactionTunnel> transaction_list;

		private readonly DispatcherTimer timer;
		private readonly EventHandler e;

		public DispatcherTimer Timer {get => timer;}
		public EventHandler Event {get => e;}

		public TransactionTunnel(EventHandler ev, DispatcherTimer t)
		{
			timer = t;
			e = ev;
		}

		/// <summary>
		///	Register a function to the external transaction chain
		/// <para>Action Currying function to remove parameters</para>
		/// </summary>
		public static void Register(Action action)
		{
			void callback(object sender, EventArgs e) => action();
			Register(callback);
		}

		// Actual register logic
		private static void Register(EventHandler action)
		{
			if(transaction_list == null)
				transaction_list = new Queue<TransactionTunnel>();

			DispatcherTimer new_timer = new DispatcherTimer();
			new_timer.Tick += action;
			new_timer.Interval = TimeSpan.FromMilliseconds(100);
			if(transaction_list.Count == 0)
				new_timer.Start();

			TransactionTunnel t = new TransactionTunnel(action, new_timer);
			transaction_list.Enqueue(t);
		}

		/// <summary>
		/// Get the next transaction action to proccess.
		/// <para>WARNING: Always inlclude this function inside the success condition of the functions that you register.</para>
		/// </summary>
		public static void TakeNextTransaction()
		{
			TransactionTunnel tun_to_stop = transaction_list.Dequeue();
			if(transaction_list.Any())
			{
				TransactionTunnel tun_to_start = transaction_list.Peek();
				tun_to_start.Timer.Start();
				tun_to_stop.Timer.Tick -= tun_to_stop.Event;
				tun_to_stop.Timer.Stop();
			}
			else
			{
				tun_to_stop.Timer.Tick -= tun_to_stop.Event;
				tun_to_stop.Timer.Stop();
				transaction_list.Clear();
				return;
			}
		}
	}
}