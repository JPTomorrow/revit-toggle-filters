using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Documents;

namespace JPMorrow.Revit.LineStyle
{
	public struct LineStyleColor
	{
		public static Color Black { get => new Color( 0x00, 0x00, 0x00 ); }
		public static Color Red { get => new Color( 0xFF, 0x00, 0x00 ); }
		public static Color Blue { get => new Color( 0x00, 0x00, 0xFF ); }
	}

	public struct LineStyleGenerator
	{
		private static List<Category> line_styles {get; set; } = new List<Category>();

		public static GraphicsStyle GetLineStyle(string name)
		{
			GraphicsStyle style = line_styles.Find(x => x.Name == name).GetGraphicsStyle(GraphicsStyleType.Projection);
			if(style == null)
				throw new Exception("The line style doesn't exist.");
			return style;
		}

		public static void RegisterNewLineStyle(ModelInfo info, string pattern_name, Color color)
		{
			Category myCategory = info.DOC.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
			if(myCategory.SubCategories == null)
				throw new Exception("No sub categories detected to make new line styles");

			foreach (var el in myCategory.SubCategories)
			{
				Category cat = el as Category;
				if(cat.Name == pattern_name)
				{
					line_styles.Add(cat);
					using(Transaction tx = new Transaction(info.DOC, "Change Line Style Color"))
					{
						tx.Start();
						cat.LineColor = color;
						tx.Commit();
					}
					return;
				}
			}

			LinePattern pattern = new LinePattern
			{
				Name = pattern_name
			};
			//Create list of segments which define the line pattern
			List<LinePatternSegment> segs = new List<LinePatternSegment>();
			segs.Add(new LinePatternSegment(LinePatternSegmentType.Dash, .3));
			pattern.SetSegments(segs);

			ElementId line_pat_elem = LinePatternElement.GetSolidPatternId();

			// The new linestyle will be a subcategory
			// of the Lines category
			Categories categories = info.DOC.Settings.Categories;
			Category line_cat = categories.get_Item(BuiltInCategory.OST_Lines );
			Category new_line_style;

			using (Transaction tx = new Transaction(info.DOC, "Create Line Style"))
			{
				tx.Start();

				// Add the new linestyle
				new_line_style = categories.NewSubcategory( line_cat, pattern_name );

				info.DOC.Regenerate();

				new_line_style.SetLineWeight( 8,
				GraphicsStyleType.Projection );

				new_line_style.LineColor = color;

				new_line_style.SetLinePatternId(
				line_pat_elem,
				GraphicsStyleType.Projection );
				tx.Commit();
			}
			line_styles.Add(new_line_style);
			return;
		}
	}


}