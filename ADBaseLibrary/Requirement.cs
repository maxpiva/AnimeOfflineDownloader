using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public class Requirement
    {
        public string Name { get; set; }
        public RequirementType RequirementType { get; set; }
        public List<string> Options { get; set; }
    }
}
