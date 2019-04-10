using System;
using System.Collections.Generic;
using System.Text;

namespace GOBBlockchain.errors
{
    interface ICallStacker
    {
        CallStack GetCallStack(int skip, int depth);
    }

    class CallStack
    {
        UInt32[] Stacks;
    }
}
