using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Documents;
using System.Diagnostics;
using JPMorrow.Test;
using JPMorrow.Tools.Diagnostics;
using System.Linq;

namespace MainApp
{
    /// <summary>
    /// Main Execution
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("9BBF529B-520A-4877-B63B-BEF1238B6A05")]
    public partial class ThisApplication : IExternalCommand
    {
        public bool RunTests { get; } = false;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string[] dataDirectories = new string[] { "data" };

            //set revit model info
            bool debugApp = false;
            ModelInfo revit_info = ModelInfo.StoreDocuments(commandData, dataDirectories, debugApp);
            IntPtr main_rvt_wind = Process.GetCurrentProcess().MainWindowHandle;

            // run tests
            if (RunTests)
            {
                TestBed.TestAll(revit_info);
                return Result.Succeeded;
            }

            // get all filters in the active revit view
            var filters = revit_info.UIDOC.ActiveView.GetFilters().Select(x => revit_info.DOC.GetElement(x) as ParameterFilterElement);

            // toggle visibility of all filters
            using TransactionGroup gt = new TransactionGroup(revit_info.DOC, "Toggle Filters Group");
            gt.Start();
            using Transaction tx = new Transaction(revit_info.DOC, "Toggle Filters");
            tx.Start();
            foreach (var filter in filters)
            {
                var current = revit_info.UIDOC.ActiveView.GetFilterVisibility(filter.Id);
                revit_info.UIDOC.ActiveView.SetFilterVisibility(filter.Id, !current);
            }
            tx.Commit();
            gt.Assimilate();


            // Create worksets
            // WorksetManager.CreateWorkset(revit_info.DOC, "Hangers");

            return Result.Succeeded;
        }
    }
}