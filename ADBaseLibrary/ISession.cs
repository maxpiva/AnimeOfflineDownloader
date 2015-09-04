using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public interface ISession : IResponse
    {
        Dictionary<string,string> Serialize();

    }
}
