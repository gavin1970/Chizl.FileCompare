using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Chizl.FileComparer
{
    internal static class Common
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);


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

        public static Icon GetIcon(string path)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(path);
            if (bmp.Width > 128 || bmp.Height > 128)    // max the icon size to: 128x128
                bmp = ResizeBitmap(bmp, new Size(128, 128));
            IntPtr Hicon = bmp.GetHicon();
            var ico = (Icon)Icon.FromHandle(Hicon).Clone();
            // Required cleanup or creates memory leak.
            DestroyIcon(Hicon);

            return ico;
        }

        private static Bitmap ResizeBitmap(Bitmap img, Size sz)
        {
            float width = sz.Width;
            float height = sz.Height;
            var brush = new SolidBrush(Color.Transparent);
            float scale = Math.Min(width / ((float)img.Width), height / ((float)img.Height));
            var bmp = new Bitmap(sz.Width, sz.Height);
            var graph = Graphics.FromImage(bmp);
            var scaleWidth = (int)(img.Width * scale);
            var scaleHeight = (int)(img.Height * scale);
            RectangleF rec = new RectangleF(((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);
            graph.FillRectangle(brush, rec);
            graph.DrawImage(img, rec);
            return bmp;
        }
    }
}
