using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Documents;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.BetterFasterStrongerLinq
{
    public static class BFSLinq
    {

        /// <summary>
        /// Cut the filename off of a split array
        /// </summary>
        public static string PathNoFilename(this string filename)
        {
            string[] split_str = filename.Split('\\');
            string[] ret_arr = new string[split_str.Length - 1];
            split_str.CopyTo(ret_arr, 0);
            return String.Join("\\", ret_arr);
        }

        public static string FilenameNoPath(this string filename)
        {
            string[] split_str = filename.Split('\\');
            return split_str.Last();
        }

        public static string RemoveFileExts(this string filename)
        {
            if (!filename.Contains(".")) return filename;
            int idx = filename.IndexOf(filename.Where(x => x == '.').First());
            return filename.Remove(idx, filename.Length - idx);
        }

        private static IEnumerable<IEnumerable<int>> SplitIdList(this IEnumerable<int> source, int delem)
        {

            if (source.Count() == 0) return new List<List<int>>();

            List<List<int>> ret_list = new List<List<int>>();
            List<int> buffer_list = new List<int>();
            foreach (int i in source)
            {
                if (i != delem)
                    buffer_list.Add(i);
                else
                {
                    ret_list.Add(buffer_list);
                    buffer_list.Clear();
                }
            }
            return ret_list;
        }

        public static IEnumerable<ElementId> GetSimilar(this ElementId first_id, ModelInfo info, BuiltInCategory cat, string parameter = null)
        {

            string from(Element elem) => elem.LookupParameter("From").AsString();
            string to(Element elem) => elem.LookupParameter("To").AsString();
            string param_find(Element elem) => elem.LookupParameter(parameter).AsString();

            List<ElementId> ret_list = new List<ElementId>();
            Element el = info.DOC.GetElement(first_id);

            FilteredElementCollector elcoll = new FilteredElementCollector(info.DOC, info.UIDOC.ActiveView.Id);

            if (parameter == null)
            {
                ret_list = elcoll
                    .OfCategory(cat)
                    .Where(x => x.LookupParameter("From") != null &&
                    !String.IsNullOrWhiteSpace(from(x)) &&
                    x.LookupParameter("To") != null &&
                    !String.IsNullOrWhiteSpace(to(x)))
                    .Select(x => x.Id).ToList();
            }
            else
            {
                ret_list = elcoll
                    .OfCategory(cat)
                    .Where(x => x.LookupParameter("From") != null &&
                    !String.IsNullOrWhiteSpace(from(x)) &&
                    x.LookupParameter("To") != null &&
                    !String.IsNullOrWhiteSpace(to(x)) &&
                    x.LookupParameter(parameter) != null &&
                    param_find(x) == param_find(el))
                    .Select(x => x.Id).ToList();
            }


            return ret_list;
        }

        /// <summary>
        ///	order revit elements by their x position
        /// </summary>
        /// <param name="source">source list</param>
        /// <param name="order_str">  -> or <-  </param>
        /// <returns></returns>
        public static IEnumerable<ElementId> OrderByPosition(this IEnumerable<ElementId> source, ModelInfo info, string order_str, string axis_str = "X")
        {
            if (order_str != "->" && order_str != "<-")
                throw new Exception("No order specifioed.");

            double x_loc(Element x) => (x.Location as LocationPoint).Point.X;
            double y_loc(Element y) => (y.Location as LocationPoint).Point.Y;
            double z_loc(Element z) => (z.Location as LocationPoint).Point.Z;

            if (source == null) return new List<ElementId>();
            if (axis_str.ToUpper() != "X" &&
                axis_str.ToUpper() != "Y" &&
                axis_str.ToUpper() != "Z") return new List<ElementId>();

            Element get_el(ElementId x) => info.DOC.GetElement(x);
            List<Element> els_to_sort = new List<Element>();
            source.ToList().ForEach(x =>
            {
                Element el = get_el(x);

                if (el.Category.Name == "Conduits" || el.Category.Name == "Conduit Fittings")
                    throw new Exception("Cannot use conduit with this orderby function.");

                els_to_sort.Add(el);
            });

            if (order_str == "->") // ascending
            {
                switch (axis_str.ToUpper())
                {
                    case "X":
                        return els_to_sort.OrderBy(x => x_loc(x)).Select(y => y.Id);
                    case "Y":
                        return els_to_sort.OrderBy(x => y_loc(x)).Select(y => y.Id);
                    case "Z":
                        return els_to_sort.OrderBy(x => z_loc(x)).Select(y => y.Id);
                    default:
                        return source;
                }
            }
            else
            {
                switch (axis_str.ToUpper())
                {
                    case "X":
                        return els_to_sort.OrderByDescending(x => x_loc(x)).Select(y => y.Id);
                    case "Y":
                        return els_to_sort.OrderByDescending(x => y_loc(x)).Select(y => y.Id);
                    case "Z":
                        return els_to_sort.OrderByDescending(x => z_loc(x)).Select(y => y.Id);
                    default:
                        return source;
                }
            }
        }

        /// <summary>
        ///	order revit elements by their x position
        /// </summary>
        /// <param name="source">source list</param>
        /// <param name="order_str">  -> or <-  </param>
        /// <returns></returns>
        public static IEnumerable<ElementId> OrderConduitByPosition(this IEnumerable<ElementId> source, ModelInfo info, string order_str, string axis_str = "X")
        {
            if (order_str != "->" && order_str != "<-")
                throw new Exception("No order specifioed.");

            double x_loc(Element x) => (x.Location as LocationCurve).Curve.GetEndPoint(0).X;
            double y_loc(Element y) => (y.Location as LocationCurve).Curve.GetEndPoint(0).Y;
            double z_loc(Element z) => (z.Location as LocationCurve).Curve.GetEndPoint(0).Z;

            if (source == null) return new List<ElementId>();
            if (axis_str.ToUpper() != "X" &&
                axis_str.ToUpper() != "Y" &&
                axis_str.ToUpper() != "Z") return new List<ElementId>();

            Element get_el(ElementId x) => info.DOC.GetElement(x);
            List<Element> els_to_sort = new List<Element>();
            source.ToList().ForEach(x =>
            {
                Element el = get_el(x);

                if (el.Category.Name != "Conduits")
                    throw new Exception("Cannot use none conduit elements with this orderby function.");

                els_to_sort.Add(el);
            });

            if (order_str == "->") // ascending
            {
                switch (axis_str.ToUpper())
                {
                    case "X":
                        return els_to_sort.OrderBy(x => x_loc(x)).Select(y => y.Id);
                    case "Y":
                        return els_to_sort.OrderBy(x => y_loc(x)).Select(y => y.Id);
                    case "Z":
                        return els_to_sort.OrderBy(x => z_loc(x)).Select(y => y.Id);
                    default:
                        return source;
                }
            }
            else
            {
                switch (axis_str.ToUpper())
                {
                    case "X":
                        return els_to_sort.OrderByDescending(x => x_loc(x)).Select(y => y.Id);
                    case "Y":
                        return els_to_sort.OrderByDescending(x => y_loc(x)).Select(y => y.Id);
                    case "Z":
                        return els_to_sort.OrderByDescending(x => z_loc(x)).Select(y => y.Id);
                    default:
                        return source;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="info"></param>
        /// <param name="origin_id"></param>
        /// <returns></returns>
        public static IEnumerable<ElementId> OrderElementsFromPt(this IEnumerable<ElementId> source, ModelInfo info, ElementId origin_id)
        {
            XYZ pt(ElementId id) => (info.DOC.GetElement(id).Location as LocationPoint).Point;
            List<ElementId> ret_ids = new List<ElementId>(source.Except(new[] { origin_id }));
            XYZ o_pt = pt(origin_id);

            var sort_list = new List<Tuple<ElementId, XYZ>>()
            .Select(x => new { Id = x.Item1, Pt = x.Item2 }).ToList();

            foreach (var id in ret_ids)
            {
                try
                {
                    sort_list.Add(new { Id = id, Pt = pt(id) });
                }
                catch
                {
                    var need_bb = info.DOC.GetElement(id).get_BoundingBox(null);
                    var center_pt = need_bb.Min + (need_bb.Max - need_bb.Min) / 2;
                    sort_list.Add(new { Id = id, Pt = center_pt });
                }
            }

            ret_ids.Clear();
            ret_ids = sort_list.OrderBy(x => x.Pt.DistanceTo(o_pt)).Select(x => x.Id).ToList();

            //string o = "";
            //ret_ids.ForEach(x => o += pt(x).DistanceTo(o_pt).ToString() + "\n");
            //debugger.show(err:o);

            return ret_ids;
        }

        public static bool UnorderedEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        public static bool IsAlmostEqual(this double dbl1, double dbl2, double tolerance = 0.05)
        {
            return dbl1 > dbl2 - tolerance && dbl1 < dbl2 + tolerance;
        }

        public static bool IsAlmostEqual(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.000001;
        }

        // split a list into smaller equal sized lists
        public static IEnumerable<List<T>> SplitList<T>(this List<T> locations, int nSize = 30)
        {

            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}