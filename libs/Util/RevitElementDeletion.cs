using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JPMorrow.Revit.Documents;

namespace JPMorrow.Revit.ElementDeletion {
	public static class RvtElementDeletion {
		private static DeleteRvtElements handler_delete_elements = null;
		private static ExternalEvent exEvent_delete_elements = null;

		public static void DeleteRevitElementsSignUp()
		{
			handler_delete_elements = new DeleteRvtElements();
			exEvent_delete_elements = ExternalEvent.Create(handler_delete_elements);
		}

		public static void DeleteRevitElements(ModelInfo info, IEnumerable<ElementId> ids)
		{

			handler_delete_elements.Info = info;
			handler_delete_elements.IdsToRemove = ids.ToList();

			exEvent_delete_elements.Raise();
		}

		public class DeleteRvtElements : IExternalEventHandler
		{
			public ModelInfo Info { get; set; }
			public List<ElementId> IdsToRemove { get; set; }


			public void Execute(UIApplication app)
			{
				using var tx = new Transaction(Info.DOC, "deleting element");

				tx.Start();
				Info.DOC.Delete(IdsToRemove);
				tx.Commit();

			}

			public string GetName()
			{
				return "Delete Elements from Revit Model";
			}
		}
	}
}