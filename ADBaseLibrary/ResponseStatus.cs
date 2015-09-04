using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary
{
    public enum ResponseStatus
    {
        Ok,
        MissingRequirement,
        MissingPlugin,
        InvalidArgument,
        InvalidLogin,
        MissingAuthenticationParameters,
        NotSupported,
        LoginRequired,
        WebError,
        TransferError,
        SystemError,
        Canceled
    }
}
