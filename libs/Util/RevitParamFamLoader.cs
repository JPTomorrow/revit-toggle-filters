
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Documents;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.Revit.Loader
{
	public struct FamilyLoader
	{
		public static void LoadFamilies(ModelInfo info, string fam_dir, params string[] filenames)
		{
			List<string> added = new List<string>();
			bool debug = false;

			FilteredElementCollector fam_coll = new FilteredElementCollector(info.DOC);
			var fams = fam_coll.OfClass(typeof(FamilySymbol)).Where(x => filenames.Any(y => (x as FamilySymbol).FamilyName.Contains(y.Split('.').First())));

			using(Transaction tx = new Transaction(info.DOC, "Load Families"))
			{
				tx.Start();
				foreach(var name in filenames)
				{
					bool is_loadable = !fams.Any(x => (x as FamilySymbol).FamilyName.Equals(name.Split('.').First()));
					if(!is_loadable) continue;

                    bool s = info.DOC.LoadFamily(fam_dir + name);
					added.Add(s ? "Loaded: " + name : "Failed: " + name);
				}
				tx.Commit();
			}

			if(added.Any() && debug)
			{
				added.Sort();
				debugger.show(err:string.Join("\n", added), header:"Load Famulies");
			}
		}

		public static FamilySymbol GetFamilySymbol(Document doc, string partial_family_name)
		{
			if(partial_family_name.Contains(".rfa"))
				partial_family_name = partial_family_name.Split('.').First();

			FilteredElementCollector fam_coll = new FilteredElementCollector(doc);
			var sym = fam_coll.OfClass(typeof(FamilySymbol))
				.Where(x => x.Name.Contains(partial_family_name))
				.FirstOrDefault() as FamilySymbol;

			ActivateSymbol(sym);
			return sym;
		}

		/// <summary>
		/// Activate the familySymbol in the Revit Model
		/// </summary>
		/// <param name="sym">the FamilySymbol to activate</param>
		public static void ActivateSymbol(FamilySymbol sym)
		{
			if (sym == null)
				return;
			if (!sym.IsActive)
				sym.Activate();
		}
	}

	public struct ParamLoader
	{
		public static bool ParameterExist(ModelInfo info, BuiltInCategory cat, params string[] param_name)
		{
			bool param_loaded = true;
			foreach(var name in param_name)
			{
				if(!param_loaded) continue;
				FilteredElementCollector p_coll = new FilteredElementCollector(info.DOC);
				List<Element> els = p_coll.OfCategory(cat)
					.Where(x => x.LookupParameter(name) != null).ToList();

				if(!els.Any()) param_loaded = false;
			}
			return param_loaded;
		}
	}
}