using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Autodesk.Revit.DB;

namespace JPMorrow.Revit.Custom.Parameters
{

	/// <summary>
	/// Class for probing an element for parameters
	/// </summary>
	[DataContract]
	public abstract class ParamProbe
	{
		[DataMember]
		public int Element_Id { get; private set; }

		public ParamProbe(int id)
		{
			Element_Id = id;
		}

		/// <summary>
		/// Get the value of an elements parameter as a string
		/// </summary>
		public string GetStringParam(Document doc, string generic_param_name, Dictionary<string, string> param_names)
		{
			bool s = param_names.TryGetValue(generic_param_name, out string param_name);
			if(!s) return "BAD PARAMETER VALUE";

			Element el = doc.GetElement(new ElementId(Element_Id));

			if(el == null || el.LookupParameter(param_name) == null ||
				!el.LookupParameter(param_name).HasValue)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			return el.LookupParameter(param_name).AsString();
		}

		/// <summary>
		/// Set the value of an elements parameter as a string
		/// </summary>
		public string SetStringParam(Document doc, string generic_param_name, Dictionary<string, string> param_names, string val)
		{
			bool s = param_names.TryGetValue(generic_param_name, out string param_name);
			if(!s) return "BAD PARAMETER VALUE";

			Element el = doc.GetElement(new ElementId(Element_Id));

			if(el == null || el.LookupParameter(param_name) == null)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			el.LookupParameter(param_name).Set(val);
			return el.LookupParameter(param_name).AsString();
		}

		/// <summary>
		/// Get the value of an elements parameter as a Value String
		/// </summary>
		public string GetValStringParam(Document doc, string generic_param_name, Dictionary<string, string> param_names)
		{
			bool s = param_names.TryGetValue(generic_param_name, out string param_name);
			if(!s) return "BAD PARAMETER VALUE";

			Element el = doc.GetElement(new ElementId(Element_Id));

			if(el == null || el.LookupParameter(param_name) == null)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			return el.LookupParameter(param_name).AsValueString();
		}

		/// <summary>
		/// Set the value of an elements parameter as a Value String
		/// </summary>
		public string SetValStringParam(Document doc, string generic_param_name, Dictionary<string, string> param_names, string val)
		{
			bool s = param_names.TryGetValue(generic_param_name, out string param_name);
			if(!s) return "BAD PARAMETER VALUE";

			Element el = doc.GetElement(new ElementId(Element_Id));

			if(el == null || el.LookupParameter(param_name) == null)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			el.LookupParameter(param_name).SetValueString(val);
			return el.LookupParameter(param_name).AsValueString();
		}

		/// <summary>
		/// Get the value of an elements parameter as a Double
		/// </summary>
		public double GetDblParam(Document doc, string generic_param_name, Dictionary<string, string> param_names)
		{
			bool s = param_names.TryGetValue(generic_param_name, out string param_name);
			if(!s)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			Element el = doc.GetElement(new ElementId(Element_Id));

			if(el == null || el.LookupParameter(param_name) == null)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			return el.LookupParameter(param_name).AsDouble();
		}

		/// <summary>
		/// Set the value of an elements parameter as a Double
		/// </summary>
		public double SetDblParam(Document doc, string generic_param_name, Dictionary<string, string> param_names, double val)
		{
			bool s = param_names.TryGetValue(generic_param_name, out string param_name);
			if(!s)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			Element el = doc.GetElement(new ElementId(Element_Id));

			if(el == null || el.LookupParameter(param_name) == null)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			el.LookupParameter(param_name).Set(val);
			return el.LookupParameter(param_name).AsDouble();
		}

		/// <summary>
		/// Get the value of an elements parameter as a Int
		/// </summary>
		public double GetIntParam(Document doc, string generic_param_name, Dictionary<string, string> param_names)
		{
			bool s = param_names.TryGetValue(generic_param_name, out string param_name);
			if(!s)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			Element el = doc.GetElement(new ElementId(Element_Id));

			if(el == null || el.LookupParameter(param_name) == null)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			return el.LookupParameter(param_name).AsInteger();
		}

		/// <summary>
		/// Set the value of an elements parameter as a Double
		/// </summary>
		public double SetIntParam(Document doc, string generic_param_name, Dictionary<string, string> param_names, int val)
		{
			bool s = param_names.TryGetValue(generic_param_name, out string param_name);
			if(!s)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			Element el = doc.GetElement(new ElementId(Element_Id));

			if(el == null || el.LookupParameter(param_name) == null)
				throw new Exception("The Element or parameter doesn't exist: " + param_name);

			el.LookupParameter(param_name).Set(val);
			return el.LookupParameter(param_name).AsInteger();
		}

	}
}