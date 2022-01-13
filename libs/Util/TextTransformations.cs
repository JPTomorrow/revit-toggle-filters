///
/// class containing methods for simple generic text transformations

using System.Text;

namespace JPMorrow.Tools.Text {

    public static class TextTransform {
        public static string AddSpacesBetweenCaps(string text, bool preserveAcronyms = false) {
            
                if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
                StringBuilder newText = new StringBuilder(text.Length * 2);
                newText.Append(text[0]);
                for (int i = 1; i < text.Length; i++)
                {
                    if (char.IsUpper(text[i]))
                        if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                            (preserveAcronyms && char.IsUpper(text[i - 1]) && 
                            i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                            newText.Append(' ');
                    newText.Append(text[i]);
                }
                return newText.ToString();
        }
    }
}