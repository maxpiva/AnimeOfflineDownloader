using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public interface IResponse
    {
        ResponseStatus Status { get; set; }

        string ErrorMessage { get; set; }
    }
}
