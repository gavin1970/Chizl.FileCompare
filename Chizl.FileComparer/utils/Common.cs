using System.Windows.Forms;

namespace Chizl.FileComparer.utils
{
    internal static class Common
    {
        public static bool GetFilePath(this Form @this, string title, out string fileName)
        {
            fileName = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = false;
                openFileDialog.Title = title;
                openFileDialog.AutoUpgradeEnabled = true;
                openFileDialog.CheckFileExists = true;
                openFileDialog.InitialDirectory = Application.StartupPath;
                if (openFileDialog.ShowDialog(@this) == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                    return true;
                }
            }
            return false;
        }
    }
}
