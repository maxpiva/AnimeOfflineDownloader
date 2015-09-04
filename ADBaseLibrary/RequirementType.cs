using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    [Flags]
    public enum RequirementType
    {
        String = 1,
        Password = 2,
        Integer = 3,
        Bool = 4,
        FilePath = 10,
        FolderPath= 11,
        DropDownList=12,
        Link=20,
        Required=256,
    }
}
