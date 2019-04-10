using System;
using System.Collections.Generic;
using System.Text;

namespace GOBBlockchain.events
{
    public enum EventType
    {
        EventSaveBlock = 0,
        EventReplyTx = 1,
        EventBlockPersistCompleted = 2,
        EventNewInventory = 3,
        EventNodeDisconnect = 4,
        EventSmartCode = 5,
        EventNodeConsensusDisconnect = 6
    }

    class Event
    {
        RWMutex m;
        Dictionary<Int16, Dictionary<Subscriber, EventFunc>> subscribers;
    }

    class RWMutex
    {
        // w Mutex
        UInt32 writerSem;
        UInt32 readerSem;
        UInt32 readerCount;
        UInt32 readerWait;
    }
}
