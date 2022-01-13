using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JPMorrow.Revit.Documents;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.Schedules {

	public static class ScheduleCreation {

		// Handle Creation of HangerModel
		private static CreateSchedule handler_create_schedule = null;
		private static ExternalEvent exEvent_create_schedule = null;

		public static void ScheduleCreationSignUp()
		{
			handler_create_schedule = new CreateSchedule();
			exEvent_create_schedule = ExternalEvent.Create(handler_create_schedule.Clone() as IExternalEventHandler);
		}

		public static async Task<ViewSchedule> GetNewSchedule(ModelInfo info, string name, ElementId cat_id)
		{
			handler_create_schedule.Info = info;
			handler_create_schedule.Name = name;
			handler_create_schedule.CategoryId = cat_id;
			exEvent_create_schedule.Raise();

			while(exEvent_create_schedule.IsPending) {
				await Task.Delay(200);
			}

			return handler_create_schedule.Schedule;
		}

		/// <summary>
		/// Revit Event for placing a single hanger
		/// </summary>
		public class CreateSchedule : IExternalEventHandler, ICloneable
		{
			public ModelInfo Info { get; set; }
			public ViewSchedule Schedule { get; set; } = null;
			public ElementId CategoryId { get; set; }
			public string Name { get; set; }

			public object Clone()
			{
				return this;
			}

			public void Execute(UIApplication app)
			{
				try {
					using var tx = new Transaction(Info.DOC, "Creating Schedule");
					tx.Start();

					//make schedule
					var schedule = ViewSchedule.CreateSchedule(Info.DOC, ElementId.InvalidElementId, ElementId.InvalidElementId);
					schedule.Name = Name;
					var field = schedule.Definition.AddField(ScheduleFieldType.Count);
					field.SheetColumnWidth = 1.0;

					// ScheduleFilter filter = new ScheduleFilter(field.FieldId, ScheduleFilterType.Equal, 99);
					// schedule.Definition.AddFilter(filter);
					tx.Commit();

					Schedule = schedule;
				}
				catch(Exception ex) {
					debugger.show(err:ex.ToString());
				}
			}

			public string GetName()
			{
				return "Creating Schedule";
			}
		}
	}

	/// <summary>
	/// External event system for updating schedules
	/// </summary>
	public static class ScheduleUpdate {

		public static UpdateSchedule handler_update_schedule = null;
		public static ExternalEvent exEvent_update_schedule = null;

		public static void ScheduleUpdateSignUp()
		{
			handler_update_schedule = new UpdateSchedule();
			exEvent_update_schedule = ExternalEvent.Create(handler_update_schedule.Clone() as IExternalEventHandler);
		}

		public static void ClearHandler() {
			handler_update_schedule.CellModifyTextValue = null;
			handler_update_schedule.Info = null;
			handler_update_schedule.Dimensions = null;
			handler_update_schedule.CellPos = null;
			handler_update_schedule.Schedule = null;
			handler_update_schedule.Name = null;
			handler_update_schedule.CellTextColor = null;
			handler_update_schedule.CellBackgroundColor = null;
		}

		/// <summary>
		/// Revit Event for placing a single hanger
		/// </summary>
		public class UpdateSchedule : IExternalEventHandler, ICloneable
		{
			public ModelInfo Info { get; set; }
			public ViewSchedule Schedule { get; set; }
			public string Name { get; set; } = null;
			public int[] CellPos { get; set; } = null;
			public Color CellTextColor { get; set; } = null;
			public Color CellBackgroundColor { get; set; } = null;
			public string CellModifyTextValue { get; set; } = null;
			public int[] Dimensions { get; set; } = null;

			public object Clone() {
				return this;
			}

			public void Execute(UIApplication app) {
				//update schedule
				using var tx = new Transaction(Info.DOC, "Updating Schedule");
				var header = Schedule.GetTableData().GetSectionData(SectionType.Header);

				if(Name != null) {

					tx.Start();
					Schedule.Name = Name;
					tx.Commit();
				}

				if(Dimensions != null && Dimensions.Length == 2) {
					tx.Start();



					for(var x = 1; x < Dimensions[0] + 1; x++) {
						header.InsertColumn(x);
						header.SetColumnWidth(x, 1.0);
					}

					for(var y = 1; y < Dimensions[1]; y++) {
						header.InsertRow(y);
						header.SetRowHeight(y, 1.0 / 24.0);
					}

					TableMergedCell t = new TableMergedCell();
					t.Top = 0;
					t.Bottom = 0;
					t.Left = 0;
					t.Right = header.LastColumnNumber;
					header.MergeCells(t);

					tx.Commit();
				}

				if(CellPos != null && CellModifyTextValue != null) {
					tx.Start();
					header.SetCellText(CellPos[1], CellPos[0], CellModifyTextValue);
					tx.Commit();
				}

				if(CellPos != null && CellTextColor != null && CellBackgroundColor != null) {
					tx.Start();
					TableCellStyle style = new TableCellStyle();
					style.TextColor = CellTextColor;
					style.BackgroundColor = CellBackgroundColor;
					var ov = style.GetCellStyleOverrideOptions();
					ov.FontColor = true;
					ov.BackgroundColor = true;
					style.SetCellStyleOverrideOptions(ov);

					if(!header.AllowOverrideCellStyle(CellPos[1], CellPos[0]))
						throw new Exception("The cell text style cannot be overwritten.");

					header.SetCellStyle(CellPos[1], CellPos[0], style);
					tx.Commit();
				}
			}

			public string GetName()
			{
				return "Creating Schedule";
			}
		}
	}
}