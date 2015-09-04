using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class Episodes : Response
    {
        public List<Episode> Items { get; set; }
        public Uri ImageUri { get; set; }
    }
}
