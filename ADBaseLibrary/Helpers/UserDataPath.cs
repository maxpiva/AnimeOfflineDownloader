using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary.Helpers
{
    public static class UserDataPath
{
        public static string AppPath { get; set; } = "AppPath";

        public static string Get()
        {
            string basepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppPath);
            if (Directory.Exists(basepath))
            {
                return basepath;
            }
            return null;
        }
    }
}
