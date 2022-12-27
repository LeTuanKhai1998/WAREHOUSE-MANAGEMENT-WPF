using System;
using System.Diagnostics;
using System.IO;

namespace QuanLyKho
{
    public static class Constants
    {
        public static String basePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName).Replace("bin\\Debug", "Images\\");
        public static String objectImagesPath = basePath + "Objects\\";
        public static String CustomerImagesPath = basePath + "Customers\\";
    }
}
