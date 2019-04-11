using System;
using System.Collections.Generic;
using System.Text;

namespace GOBBlockchain.errors
{
    class DetailError
    {
        Int32 error;
        errcode errcoder;
        ICallStacker callstacker;
    }

    class ontError
    {
        string errmsg;
        ICallStacker callstacker;
        Int32 error;
        ErrCode code;
    }
}
