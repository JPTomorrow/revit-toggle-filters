
using Autodesk.Revit.DB;

namespace JPMorrow.Revit.Text
{
	public static class CustomFormatValue
	{
#if REVIT2017 || REVIT2018 || REVIT2019 || REVIT2020
		private static FormatValueOptions MakeFormatValue(
			DisplayUnitType type, bool use_default,
			bool leading_zero, bool supress_spaces) {

			FormatValueOptions opts = new FormatValueOptions();
			var f_opts = opts.GetFormatOptions();
			f_opts.UseDefault = use_default;
			f_opts.DisplayUnits = type;
			f_opts.SuppressLeadingZeros = leading_zero;
			f_opts.SuppressSpaces = supress_spaces;
			opts.SetFormatOptions(f_opts);
			return opts;
		}

		public static FormatValueOptions FeetAndInches =>
			MakeFormatValue(DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES, false, true, true);

		public static FormatValueOptions Angle =>
			MakeFormatValue(DisplayUnitType.DUT_DECIMAL_DEGREES, false, false, false);

		public static FormatValueOptions Inches =>
			MakeFormatValue(DisplayUnitType.DUT_FRACTIONAL_INCHES, false, false, false);
#else

		private static FormatValueOptions MakeFormatValue(
			ForgeTypeId type, bool use_default,
			bool leading_zero, bool supress_spaces) {

			FormatValueOptions opts = new FormatValueOptions();
			var f_opts = opts.GetFormatOptions();
			f_opts.UseDefault = use_default;
			f_opts.SetUnitTypeId(type);
			f_opts.SuppressLeadingZeros = leading_zero;
			f_opts.SuppressSpaces = supress_spaces;
			opts.SetFormatOptions(f_opts);
			return opts;
		}

		public static FormatValueOptions FeetAndInches =>
			MakeFormatValue(UnitTypeId.FeetFractionalInches, false, true, true);

		public static FormatValueOptions Angle =>
			MakeFormatValue(UnitTypeId.Degrees, false, false, false);

		public static FormatValueOptions Inches =>
			MakeFormatValue(UnitTypeId.FractionalInches, false, false, false);
#endif
	}
}