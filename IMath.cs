using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

namespace FileDownloader
{
    public class IMath
    {
        public static string ConvertViaMath(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB" };
            double size = bytes;
            int i = 0;

            while (size >= 1024 && i < sizes.Length - 1)
            {
               size /= 1024;
                i++;
            }
            return $"{Round(size, 2)}{sizes[i]}";
        }
    }
}
