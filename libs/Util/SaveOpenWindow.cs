using System.Linq;
using System.Windows.Forms;

namespace JPMorrow.Windows.IO
{
    /// <summary>
    /// represents the output from a SaveFileDialog or OpenFileDialog
    /// </summary>
    public class PromptResult
    {
        public string Filename { get; private set; } = "";
        public DialogResult Result { get; private set; }

        public PromptResult(string filename, DialogResult result)
        {
            Filename = filename;
            Result = result;
        }

        /// <summary>
        /// compare the provided Dialog Result to the one stored in this class
        /// </summary>
        public bool IsResult(params DialogResult[] r)
        {
            if (r.Any(x => x == Result)) return true;
            return false;
        }
    }

    public static class OpenFileSelection
    {
        public static PromptResult Prompt(string title = "Open File", string extension = null)
        {
            var ext = extension == null ? "*" : extension;
            if (ext.StartsWith(".")) ext = ext.Substring(1);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = title;
            ofd.Filter = (ext == "*" ? "any" : ext) + " files (*." + ext + ")|*." + ext;
            var result = ofd.ShowDialog();
            return new PromptResult(ofd.FileName, result);
        }
    }

    public static class OpenFolderSelection
    {
        public static PromptResult Prompt(string path)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = path;
            var result = fbd.ShowDialog();
            return new PromptResult(fbd.SelectedPath, result);
        }
    }

    public static class SaveFileSelection
    {
        public static PromptResult Prompt(string title = "Save File", string extension = null)
        {
            var ext = extension == null ? "*" : extension;
            if (ext.StartsWith(".")) ext = ext.Substring(1);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = title;
            sfd.Filter = (ext == "*" ? "any" : ext) + " files (*." + ext + ")|*." + ext;
            var result = sfd.ShowDialog();
            return new PromptResult(sfd.FileName, result);
        }
    }
}