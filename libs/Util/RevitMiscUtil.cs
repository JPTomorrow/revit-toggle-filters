using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace JPMorrow.Revit.RvtMiscUtil
{
	public static class RvtUtil
	{
		/// <summary>
		/// Get Connectors in List
		/// </summary>
		public static Connector[] GetNonSetConnectors(Element el, Func<Element, ConnectorSet> get_connectors)
		{

			List<Connector> end_connectors = new List<Connector>();
			foreach(Connector c in get_connectors(el))
				end_connectors.Add(c);
			return end_connectors.ToArray();
		}

		/// <summary>
		/// Return the given element's connector set.
		/// </summary>
		public static ConnectorSet GetConnectors(Element e)
		{
			// var bad_connector_types = new List<ConnectorType>() {
			// 	ConnectorType.End
			// 	bad_connector_types.Any(x => c.ConnectorType == x)
			// };

			if (e is MEPCurve)
				return ((MEPCurve)e)?
				.ConnectorManager?
					.Connectors;

			if (e is FamilyInstance)
				return ((FamilyInstance)e)?
				.MEPModel?
					.ConnectorManager?
					.Connectors;

			return null;
		}
	}
}