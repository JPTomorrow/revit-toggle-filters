using Autodesk.Revit.DB;

namespace JPMorrow.BICategories
{
	public static class BICategoryCollection
	{
		public static readonly BuiltInCategory[] NormalHangerClash = new BuiltInCategory[] {
				BuiltInCategory.OST_Ceilings,
				BuiltInCategory.OST_Floors,
				BuiltInCategory.OST_Roofs,
				BuiltInCategory.OST_Joist,
				BuiltInCategory.OST_StructuralFraming,
				BuiltInCategory.OST_RvtLinks,
		};

		public static readonly BuiltInCategory[] CeilingHangerClash = new BuiltInCategory[] {
				BuiltInCategory.OST_Ceilings,
				BuiltInCategory.OST_CurtainGrids,
				BuiltInCategory.OST_CurtainWallMullions,
				BuiltInCategory.OST_CurtainWallPanels,
				BuiltInCategory.OST_Floors,
				BuiltInCategory.OST_Roofs,
				BuiltInCategory.OST_Joist,
				BuiltInCategory.OST_StructuralFraming,
				BuiltInCategory.OST_RvtLinks,
		};

		public static readonly BuiltInCategory[] Conduit = new BuiltInCategory[] {
			BuiltInCategory.OST_Conduit,
		};

		public static readonly BuiltInCategory[] HangerView = new BuiltInCategory[] {
				BuiltInCategory.OST_Conduit,
				BuiltInCategory.OST_GenericModel,
				BuiltInCategory.OST_Walls,
				BuiltInCategory.OST_Ceilings,
				BuiltInCategory.OST_Floors,
				BuiltInCategory.OST_Roofs,
				BuiltInCategory.OST_Joist,
				BuiltInCategory.OST_StructuralFraming,
				BuiltInCategory.OST_RvtLinks,
				BuiltInCategory.OST_Ceilings,
				BuiltInCategory.OST_CurtainGrids,
				BuiltInCategory.OST_CurtainWallMullions,
				BuiltInCategory.OST_CurtainWallPanels,
		};
	}
}