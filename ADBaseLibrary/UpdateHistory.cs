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
    public class UpdateHistory
    {
        private const string updatehistory="updatehistory.json";

        public static bool IsLoaded { get; private set; } = false;
        private static Dictionary<string,string> _updates=new Dictionary<string, string>();
        private static object _updatelock=new object();
        public static void Load()
        {
            lock (_updatelock)
            {
                string cpath = Path.Combine(UserDataPath.Get(), updatehistory);
                if (File.Exists(cpath))
                {
                    string str = File.ReadAllText(cpath);
                    _updates = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
                }
                else
                    _updates = new Dictionary<string, string>();
                IsLoaded = true;
            }
        }

        public static void Save()
        {
            lock (_updatelock)
            {
                string cpath = Path.Combine(UserDataPath.Get(), updatehistory);
                File.WriteAllText(cpath, JsonConvert.SerializeObject(_updates));
            }
        }

        public static void Add(string str, string val)
        {
            lock (_updatelock)
            {
                _updates.Add(str, val);
            }
        }

        public static string Get(string str)
        {
            lock (_updatelock)
            {
                if (_updates.ContainsKey(str))
                    return _updates[str];
                return null;
            }
        }
        public static bool Exists(string str)
        {
            lock (_updatelock)
            {
                if (_updates.ContainsKey(str))
                    return true;
                return false;
            }
        }
    }
}
