/*
    Author: Justin Morrow
    Date Created: 4/26/2021
    Description: A Module used for debug messages in Revit
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace JPMorrow.Tools.Diagnostics
{
	public delegate void DEBUG_DELEGATE_1(string header = "Error", string sub = "Generic", string err = "", int max_itr = -1, int max_len = -1);
    public delegate DialogResult DEBUG_DELEGATE_2(string err = "", string header = "Warning!", string continue_txt = "Do you want to continue?", int max_len = -1);
	public static class debugger
	{
		public static DEBUG_DELEGATE_1 show = RevitCustomDebugger.Show;
        public static DEBUG_DELEGATE_1 debug_show = RevitCustomDebugger.DebugShow;
        public static DEBUG_DELEGATE_2 show_yesno = RevitCustomDebugger.ShowYesNo;
	}

	/// <summary>
	/// Debugger designed to use taskdialogs in revit in order to provide error handling feedback
	/// </summary>
	public static class RevitCustomDebugger
	{
		private static List<string> MasterOutput = new List<string>();

        private static int Count = 0;
		private static bool SpamLock = false;
		public static int DebugSpamTimeout {get; set;} = 30;
		private static Stopwatch GuardTimer = new Stopwatch();

		private static string ConcatOutput(int max_chars = -1)
		{
            var o = string.Join("\n", MasterOutput);

			if(max_chars > -1 && o.Length > max_chars)
			{
				o = new string(o.Take(max_chars).ToArray());
                o += "...";
            }

            return o;
        }

		public static void SortAlphabetical()
		{
			if(MasterOutput.Any())
			{
				MasterOutput.Sort();
			}
		}

		public static void Clear() //clear all the text out of the buffer
		{
			MasterOutput.Clear();
		}

		public static void AddErr(string str) //add a single string to buffer
		{
			MasterOutput.Add(str);
		}

		public static void AddErr(List<string> str) //add a List of string to buffer
		{
			foreach (string s in str)
			{
				MasterOutput.Add(s);
			}
		}

		public static void AddErr(List<int> intList) //add a List of ints to buffer
		{
			foreach (int i in intList)
			{
				MasterOutput.Add(i.ToString());
			}
		}

		public static void AddErr(List<double> doubleList) //add a list of doubles to buffer
		{
			foreach (int i in doubleList)
			{
				MasterOutput.Add(i.ToString());
			}
		}

		public static void Show(string header = "Error", string sub = "Generic", string err = "", int max_itr = -1, int max_len = -1)
		{
			if(SpamLock == true && GuardTimer.Elapsed.Seconds >= DebugSpamTimeout)
			{
				GuardTimer.Stop();
				Count = 0;
				SpamLock = false;
			}

			AddErr(err);
			string o = ConcatOutput(max_len);
			if(max_itr > 0 && !SpamLock)
			{
				if(Count < max_itr)
				{
					MessageBox.Show(o, header + " - " + (Count + 1).ToString(), MessageBoxButtons.OK);
					Count++;
				}
				if(Count == max_itr)
				{
					GuardTimer.Reset();
					GuardTimer.Start();
					SpamLock = true;
				}
			}
			else if(max_itr == -1)
			{
				DialogResult result = MessageBox.Show(o, header, MessageBoxButtons.OK);

                
			}
			Clear();
		}

        public static DialogResult ShowYesNo(
            string err = "", string header = "Warning!",
            string continue_txt = "Do you want to continue?", int max_len = -1)
		{
			AddErr(err);
			string o = ConcatOutput(max_len);
            DialogResult result = MessageBox.Show(o + "\n\n" + continue_txt, header, MessageBoxButtons.YesNo);
			Clear();
            return result;
		}

        public static void DebugShow(string header = "Error", string sub = "Generic", string err = "", int max_itr = -1, int max_len = -1)
		{
			if(SpamLock == true && GuardTimer.Elapsed.Seconds >= DebugSpamTimeout)
			{
				GuardTimer.Stop();
				Count = 0;
				SpamLock = false;
			}

			AddErr(err);
			string o = ConcatOutput(max_len);
			if(max_itr > 0 && !SpamLock)
			{
				if(Count < max_itr)
				{
					MessageBox.Show(o, header + " - " + (Count + 1).ToString(), MessageBoxButtons.OK);
					Count++;
				}
				if(Count == max_itr)
				{
					GuardTimer.Reset();
					GuardTimer.Start();
					SpamLock = true;
				}
			}
			else if(max_itr == -1)
			{
				DialogResult result = MessageBox.Show(o + "\n\nContinue Execution?\nNOTE: This will throw an exception to break execution!", header, MessageBoxButtons.YesNo);

                if(result == DialogResult.No) {
                    throw new Exception("DEBUG BREAK!!!!!!!!");
                }
			}
			Clear();
		}

		public static void Show(List<string> errList, string header = "Error", string sub = "Generic" , int max_itr = -1, int max_len = -1)
		{
			if(SpamLock && GuardTimer.Elapsed.Seconds >= DebugSpamTimeout)
			{
				GuardTimer.Stop();
				Count = 0;
				SpamLock = false;
			}

			AddErr(errList);
			string o = ConcatOutput(max_len);
			if(max_itr > 0 && !SpamLock)
			{
				if(Count < max_itr)
				{
					MessageBox.Show(o, header + " - " + (Count + 1).ToString(), MessageBoxButtons.OK);
					Count++;
				}
				else if(Count == max_itr)
				{
					GuardTimer.Reset();
					GuardTimer.Start();
					SpamLock = true;
				}
			}
			else if(max_itr == -1)
			{
				MessageBox.Show(o, header, MessageBoxButtons.OK);
			}
			Clear();
		}
	}
}
