using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Documents;

namespace JPMorrow.Revit.Custom.View
{
	public static class ViewGen
	{
		public static View3D CreateView(ModelInfo info, string view_name, BuiltInCategory[] view_cats)
		{
			View3D view = null;
			using(Transaction tx = new Transaction(info.DOC, "Making View"))
			{
				tx.Start();
				string viewName = view_name;

				//try to find view already named
				FilteredElementCollector view_coll = new FilteredElementCollector(info.DOC);
				View3D[] views = view_coll.OfClass(typeof(View3D)).Cast<View3D>().Where(x => x.Name == viewName).ToArray();
				if (views.Count() > 0)
					return views.First();

				//if it doesnt already exist. create it.
				FilteredElementCollector view_type_coll = new FilteredElementCollector(info.DOC);
				ViewFamilyType vft = view_type_coll
					.OfClass(typeof(ViewFamilyType))
					.Cast<ViewFamilyType>()
					.FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);
				view = View3D.CreateIsometric(info.DOC, vft.Id);
				view.Name = viewName;

				//set bounding box
				view.SetSectionBox(GetModelExtents(info, view_cats));

				//Set Detail
				view.DetailLevel = ViewDetailLevel.Fine;
				view.DisplayStyle = DisplayStyle.Wireframe;

				//Hide unneeded elements
				Categories cats = info.DOC.Settings.Categories;
				foreach (Category cat in cats)
				{
					try
					{
						if (!view_cats.Any(x => new ElementId(x) == cat.Id))
							view.SetCategoryHidden(cat.Id, true);
					}
					catch
					{
						continue;
					}
				}

				tx.Commit();
			}

			if(view == null)
				throw new Exception("View failed to be generated.");

			return view;
		}

		/// <summary>
		/// Return a bounding box enclosing all model
		/// elements using only quick filters.
		/// </summary>
		private static BoundingBoxXYZ GetModelExtents(ModelInfo info, BuiltInCategory[] view_cats)
		{
			FilteredElementCollector model_coll
			  = new FilteredElementCollector(info.DOC)
				.WherePasses(new ElementMulticategoryFilter(view_cats))
				.WhereElementIsNotElementType()
				.WhereElementIsViewIndependent();

			BoundingBoxXYZ ret_bb = new BoundingBoxXYZ();
			List<Element> bb_els = model_coll.Cast<Element>().Where(x => x.get_BoundingBox(info.DOC.ActiveView) != null).ToList();

			foreach (Element el in bb_els)
			{
				BoundingBoxXYZ bb = el.get_BoundingBox(null);
				if (bb != null)
					ret_bb.ExpandToContain(bb);
			}
			return ret_bb;
		}

		/// <summary>
		/// Expand the given bounding box to include
		/// and contain the given point.
		/// </summary>
		public static void ExpandToContain(
		  this BoundingBoxXYZ bb,
		  XYZ p)
		{
			bb.Min = new XYZ(Math.Min(bb.Min.X, p.X),
			  Math.Min(bb.Min.Y, p.Y),
			  Math.Min(bb.Min.Z, p.Z));

			bb.Max = new XYZ(Math.Max(bb.Max.X, p.X),
			  Math.Max(bb.Max.Y, p.Y),
			  Math.Max(bb.Max.Z, p.Z));
		}

		/// <summary>
		/// Expand the given bounding box to include
		/// and contain the given other one.
		/// </summary>
		public static void ExpandToContain(
		  this BoundingBoxXYZ bb,
		  BoundingBoxXYZ other)
		{
			bb.ExpandToContain(other.Min);
			bb.ExpandToContain(other.Max);
		}
	}
}