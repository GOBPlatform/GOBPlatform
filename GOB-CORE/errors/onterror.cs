using System;

namespace GOBPlatform.errors
{
    public class ontError
    {
        string errmsg;
        CallStack callstack;
        UInt32 root;

    }

    public class CallStack
    {
        uint[] Stacks;
    }
}