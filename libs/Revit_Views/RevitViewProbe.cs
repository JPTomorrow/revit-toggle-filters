using JPMorrow.Revit.Documents;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System.Collections.Generic;
using JPMorrow.Tools.Diagnostics;
using System;

namespace JPMorrow.Revit.ViewHandler
{
	public readonly struct ViewInfo
	{
		public View View { get; }
	}

	public static class ViewManager
	{
		/// <summary>Retrieve the active revit view and check if it matches the provided ViewType's.</summary>
		public static bool ValidateActiveViewType(ModelInfo info, out View view, params ViewType[] types)
		{
			view = info.DOC.ActiveView;
			var type = view.ViewType;
			if(!types.Any(x => x == type))
				return false;
			return true;
		}

		public static List<View> GetProjectViewTemplates(ModelInfo info, out int vtemplate_cnt, params string[] names)
		{
			vtemplate_cnt = 0;

			FilteredElementCollector view_coll = new FilteredElementCollector(info.DOC);

			var vts = view_coll
				.OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(View))
				.Where(x => (x as View).IsTemplate).ToList();

			List<View> cvt_vts = new List<View>();
			if(vts.Any())
				cvt_vts.AddRange(vts.Cast<View>());

			cvt_vts = cvt_vts.Where(x => names.Any(y => x.Name.Equals(y))).ToList();
			vtemplate_cnt = cvt_vts.Count;
			return cvt_vts;
		}

		/// <summary> get a viewfamilytypeid for creating a ViewPlan</summary>
		public static int GetViewFamilyTypeId(ModelInfo info, ViewType type)
		{
			FilteredElementCollector type_coll = new FilteredElementCollector(info.DOC);

			var types_as_els = type_coll.OfClass(typeof(ViewFamilyType)).ToList();
			var fam_types = new List<ViewFamilyType>();
			types_as_els.ForEach(x => fam_types.Add((x as ViewFamilyType)));

			var vt_str = Enum.GetName(typeof(ViewType), type);
			string trim(ViewFamilyType t) => t.Name.Replace(" ", "");

			if(fam_types.Any(x => trim(x).Equals(vt_str)))
			{
				var ret_type = fam_types.Find((x => trim(x).Equals(vt_str)));
				return ret_type.Id.IntegerValue;
			}

			return -1; // failed to find any type ids that match
		}
	}
}