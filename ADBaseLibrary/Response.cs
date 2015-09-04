using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ADBaseLibrary
{

    public class Response : IResponse
    {
        [JsonIgnore]
        public ResponseStatus Status { get; set; }

        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
