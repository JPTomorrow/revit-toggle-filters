using System.Collections.Generic;
using forms = System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using JPMorrow.Revit.Documents;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.Revit.RevitPicker
{
	public static class RvtPicker {
        
		public static IEnumerable<XYZ> PickPoints(
            ModelInfo info, ObjectSnapTypes snap,
            int itr = -1, string prompt = "Pick Points") {
            
			List<XYZ> ret_pts = new List<XYZ>();
			XYZ pt;
            View view = info.UIDOC.ActiveView;

            if(itr == -1) {
                while(true) {
                    try {
                        pt = info.SEL.PickPoint(snap, prompt);
                        ret_pts.Add(pt);
                    }
                    catch {
                        
                        var result = debugger.show_yesno(
                            err:"You cancelled the selection.",
                            header:"Pick Points",
                            continue_txt:"Is Your Selection Complete?");

                        if(result == forms.DialogResult.No) {
                            
                            if(view.Id.IntegerValue != info.UIDOC.ActiveView.Id.IntegerValue)
                                info.UIDOC.ActiveView = view;
                        }
                        else
                            return ret_pts;
                    }
                }
            }
            else {
                for(var i = 0; i < itr; i++) {
                    try {
                        
                        pt = info.SEL.PickPoint(snap, prompt);
                        ret_pts.Add(pt);
                    }
                    catch {
                        
                        var result = debugger.show_yesno(
                            err:"You cancelled the selection.",
                            header:"Pick Points",
                            continue_txt:"Is Your Selection Complete?");

                        if(result == forms.DialogResult.No) {
                            
                            if(view.Id.IntegerValue != info.UIDOC.ActiveView.Id.IntegerValue)
                                info.UIDOC.ActiveView = view;
                        }
                        else
                            return ret_pts;
                    }
                }
            }
            
			return ret_pts;
		}

        public static IEnumerable<ElementId> PickObjectsSafe(
            ModelInfo info, ObjectType type,  ISelectionFilter filter, string prompt, int itr = -1) {

            List<ElementId> ids = new List<ElementId>();
            View view = info.UIDOC.ActiveView;

            if(itr == -1) {
                while(true) {
                    try {
                        
                        var id = info.SEL.PickObject(type, filter, prompt).ElementId;
                        ids.Add(id);
                    }
                    catch {
                        
                        var result = debugger.show_yesno(
                            err:"You cancelled the selection.",
                            header:"Pick Objects",
                            continue_txt:"Is Your Selection Complete?");

                        if(result == forms.DialogResult.No) {
                            
                            if(view.Id.IntegerValue != info.UIDOC.ActiveView.Id.IntegerValue)
                                info.UIDOC.ActiveView = view;
                        }
                        else
                            return ids;
                    }
                }
            }
            else {
                for(var i = 0; i < itr; i++) {
                    try {
                        
                        var id = info.SEL.PickObject(type, filter, prompt).ElementId;
                        ids.Add(id);
                    }
                    catch {
                        
                        var result = debugger.show_yesno(
                            err:"You cancelled the selection.",
                            header:"Pick Objects",
                            continue_txt:"Is Your Selection Complete?");

                        if(result == forms.DialogResult.No) {
                            
                            if(view.Id.IntegerValue != info.UIDOC.ActiveView.Id.IntegerValue)
                                info.UIDOC.ActiveView = view;
                        }
                        else
                            return ids;
                    }
                }
            }

            return ids;
        }
	}
}
