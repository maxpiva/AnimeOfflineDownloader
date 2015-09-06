using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADBaseLibrary.Helpers;
using Newtonsoft.Json;

namespace ADBaseLibrary
{
    public class LibSettings : Dictionary<string,string>
    {
        public void Load(string settings)
        {
            string cpath = Path.Combine(UserDataPath.Get(), settings);
            if (File.Exists(cpath))
            {
                string str = File.ReadAllText(cpath);
                JsonConvert.PopulateObject(str, this);
            }
            Save(settings);
        }

        public void Save(string settings)
        {
            string cpath = Path.Combine(UserDataPath.Get(), settings);
            File.WriteAllText(cpath, JsonConvert.SerializeObject(this));
        }

    }
}
