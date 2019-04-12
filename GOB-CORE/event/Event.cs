using System;
using System.Collections.Generic;

namespace GOBBlockchain.Events
{
    enum EventType
    {
        EventSaveBlock = 0,
        EventReplyTx = 1,
        EventBlockPersistCompleted = 2,
        EventNewInventory = 3,
        EventNodeDisconnect = 4,
        EventSmartCode = 5,
        EventNodeConsensusDisconnect = 6
    }

    public class Event
    {
        public class RWMutex
        {
            uint writerSem;
            uint readerSem;
            uint readerCount;
            uint readerWait;

            Dictionary<Int16, Dictionary<Subscriber, EventFunc>> subscribers;
        }
    }

    // type Subscriber chan interface{}
    public class Subscriber
    {

    }

    // type EventFunc func(v interface{})
    public class EventFunc
    {

    }

    public class ActorEvent
    {
        //	"github.com/ontio/ontology-eventbus/actor"
        //	"github.com/ontio/ontology-eventbus/eventhub"

        // https://github.com/AsynkronIT/protoactor-dotnet

        // 온톨로지와 같은 라이브러리를 사용할 수 있지 않을까 싶습니다.
        // 닷넷 코어가 아닌 것 같아요.

        // var DefEvtHub *eventhub.EventHub
        // var DefPublisherPID* actor.PID
        // var DefActorPublisher *ActorPublisher
        // var defPublisherProps* actor.Props

        public class ActorPublisher
        {
            // type ActorPublisher struct {
            // EvtHub* eventhub.EventHub
            // Publisher* actor.PID
            // }
        }

        public class ActorSubscriber
        {
            // type ActorSubscriber struct {
            // EvtHub* eventhub.EventHub
            // Subscriber* actor.PID
            // }
        }
    }
}
