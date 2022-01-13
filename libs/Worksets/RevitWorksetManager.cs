using Autodesk.Revit.DB;
using System.Linq;
using System;
using System.Collections.Generic;

namespace JPMorrow.Revit.Worksets
{
	public static class WorksetManager
	{
		public static void CreateWorkset(Document doc, string set_name = "New Workset")
		{
			// Worksets can only be created in a document with worksharing enabled
			if (doc.IsWorkshared)
			{
				// Workset name must not be in use by another workset
				if (WorksetTable.IsWorksetNameUnique(doc, set_name))
				{
					using (Transaction tx = new Transaction(doc, "Creating " + set_name + "workset"))
					{
						tx.Start();
						Workset.Create(doc, set_name);
						tx.Commit();
					}
				}
			}
		}

		public static WorksetId GetWorksetId(Document doc, string set_name)
		{
			if (!doc.IsWorkshared)
				throw new Exception("This document is not workshared. Please make it a workshared document and restart this application to create the appropriate worksets.");

			FilteredWorksetCollector ws_coll = new FilteredWorksetCollector(doc);
			var ws_id = ws_coll.Where(x => x.Name.Equals(set_name)).FirstOrDefault().Id;

			if (ws_id == null)
				throw new ArgumentNullException("The specified workset does not exist. The program should create this workset in a workshared model when you launch it.");

			return ws_id;
		}
	}
}