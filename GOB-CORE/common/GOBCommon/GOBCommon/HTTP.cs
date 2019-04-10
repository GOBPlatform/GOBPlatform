using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GOBCommon
{
    class HTTP
    {
    }

    #region http/base/actor/event.go
    
    class EventActor
    {
        delegate void BlockPersistCompleted(IGOBInterface v);
        delegate void SmartCodeEvt(IGOBInterface v);
    }

    #endregion


    #region http/base/common/common.go

    class BalanceOfRsp {
        string Ont; //'json:"ont"'
        string Ong; //'json:"ong"'
    }

    class MerkleProof
    {
        string Type;
        string TransactionsRoot;
        UInt32 BlockHeight;
        string CurBlockRoot;
        UInt32 CurBlockHeight;
        string[] TargetHashes;
    }

    class LogEventArgs
    {
        string TxHash;
        string ContractAddress;
        string Message;
    }

    class ExecuteNotify
    {
        string TxHash;
        byte State;
        UInt64 GasConsumed;
        NotifyEventInfo[] Notify;
    }

    class PreExecuteResult
    {
        byte State;
        UInt64 Gas;
        IGOBInterface Result;
        NotifyEventInfo[] Notify;
    }

    class NotifyEventInfo
    {
        string ContractAddress;
        IGOBInterface States;
    }

    class TxAttributeInfo
    {
        types.TransactionAttributeUsage Usage;
        string Data;
    }

    class AmountMap
    {
        common.Unit256 Key;
        common.Fixed64 Value;
    }

    class Fee
    {
        common.Fixed64 Amount;
        string Payer;
    }

    class Sig
    {
        string[] PubKeys;
        UInt64 M;
        string[] SigData;
    }

    class Transactions
    {
        byte Version;
        UInt32 Nonce;
        UInt64 GasPrice;
        UInt64 GasLimit;
        string Payer;
        types.TransactionType TxType;
        PayloadInfo Payload;
        TxAttributeInfo[] Attributes;
        Sig[] Sigs;
        string Hash;
        UInt32 Height;
    }

    class BlockHead
    {
        UInt32 Version;
        string PrevBlockHash;
        string TransactionsRoot;
        string BlockRoot;
        UInt32 Timestamp;
        UInt32 Height;
        UInt64 ConsensusData;
        string ConsensusPayload;
        string NextBookkeeper;

        string[] Bookkeepers;
        string[] SigData;

        string Hash;
    }

    class BlockInfo
    {
        string Hash;
        int Size;
        BlockHead Header;
        Transactions[] Transactions;
    }

    class NodeInfo
    {
        uint NodeState;
        UInt16 NodePort;
        UInt64 ID;
        Int64 NodeTime;
        UInt32 NodeVersion;
        UInt64 NodeType;
        bool Relay;
        UInt32 Height;
        UInt32[] TxnCnt;
        //UInt64 RxTxnCnt;
    }

    class ConsensusInfo
    {
        // TODO
    }

    class TXNAttrInfo
    {
        UInt32 Height;
        int Type;
        int ErrCode;
    }

    class TXNEntryInfo
    {
        TXNAttrInfo[] State;
    }

    /// <summary>
    /// GetContractAllowance() 안에서 생성
    /// </summary>
    class allowanceStruct
    {
        common.Address From;
        common.Address To;
    }

    /// <summary>
    /// GetBlockTransactions() 안에서 생성
    /// </summary>
    class BlockTransactions
    {
        string Hash;
        UInt32 Height;
        string[] Transactions;
    }

    #endregion


    #region http/base/common/payload_to_hex.go

    interface PayloadInfo {}
    
    class BookKeepingInfo
    {
        UInt64 Nonce;
    }

    class InvokeCodeInfo
    {
        string Code;
    }

    class DeployCodeInfo
    {
        string Code;
        bool NeedStorage;
        string Name;
        string CodeVersion;
        string Author;
        string Email;
        string Description;
    }

    class RecordInfo
    {
        string RecordType;
        string RecordData;
    }


    class BookkeeperInfo
    {
        string PubKey;
        string Action;
        string Issuer;
        string Controller;
    }

    class DataFileInfo
    {
        string IPFSPath;
        string Filename;
        string Note;
        string Issuer;
    }

    class PrivacyPayloadInfo
    {
        uint PayloadType; //golang -> uint8
        string Payload;
        uint EncryptType; //golang -> uint8
        string EncryptAttr;
    }

    class VoteInfo
    {
        string[] PubKeys;
        string Voter;
    }

    #endregion


    #region http/base/rest/interfaces.go

    interface ApiServer
    {
        void Start();
        void Stop();
    }

    #endregion


    #region http/base/rpc/rpc.go

    class ServeMux
    {
        sync.RWMutex;
        Dictionary<string, object> m; // map[string]func([]interface{}) map[string]interface{}
        delegate void defaultFunction(ResponseWriter res, Request req);
    }

    #endregion


    #region http/nodeinfo/ngbnodeinfo.go

    class NgbNodeInfo
    {
        string NgbId;
        string NgbType;
        string NgbAddr;
        string HttpInfoAddr;
        UInt16 HttpInfoPort;
        bool HttpInfoStart;
        string NgbVersion;
    }

    class NgbNodeInfoSlice : List<NgbNodeInfo> { } // type NgbNodeInfoSlice []NgbNodeInfo

    #endregion


    #region http/nodeinfo/server.go

    class Info
    {
        string NodeVersion;
        UInt32 BlockHeight;
        int NeighborCnt;
        NgbNodeInfo[] Neighbors; // <-- NgbNodeInfoSlice 들어갈것 같음.
        //NgbNodeInfoSlice Neighbors;
        int HttpRestPort;
        int HttpWsPort;
        int HttpJsonPort;
        int HttpLocalPort;
        UInt16 NodePort;
        string NodeId;
        string NodeType;
    }

    #endregion


    #region http/restful/restful/router.go

    class Route
    {
        string Method;
        Regex Path;
        string[] Params;
        delegate void Handler(ResponseWriter res, Request req);
    }

    class Router
    {
        Route[] routes;
    }

    #endregion


    #region http/restful/restful/server.go

    delegate Dictionary<string, IGOBInterface> handler(Dictionary<string, IGOBInterface> paramDic);

    class Action
    {
        sync.RWMutex;
        string name;
        handler handler;
    }

    class restServer
    {
        Router router;
        net.Listener listener;
        http.Server server;
        Dictionary<string, Action> postMap;
        Dictionary<string, Action> getMap;
    }

    #endregion


    #region http/test/func_test.go

    /// <summary>
    /// TestMerkleVerifier() 안에서 선언
    /// </summary>
    class merkleProof
    {
        string Type;
        string TransactionsRoot;
        UInt32 BlockHeight;
        string CurBlockRoot;
        UInt32 CurBlockHeight;
        string[] TargetHashes;
    }

    #endregion


    #region http/websocket/session/session.go

    class TxHashInfo
    {
        string TxHash;
        Int64 StartTime;
    }

    class Session
    {
        sync.Mutex;
        *websocket.Conn mConnection;
        Int64 nLastActive; //last active time
        string sessionId;
        TxHashInfo[] TxHashArr;
    }

    #endregion


    #region http/websocket/session/sessionlist.go

    class SessionList
    {
        //sync.Mutex;
        Dictionary<string, Session> mapOnlineList; // mapOnlineList map[string]*Session //key is SessionId
    }

    #endregion


    #region http/websocket/websocket/server.go

    //  http/restful/restful/server.go 에서 선언되어서 주석 처리.
    // delegate Dictionary<string, IGOBInterface> handler(Dictionary<string, IGOBInterface> paramDic);

    class Handler
    {
        handler handler;
        bool pushFlag;
    }

    class subscribe
    {
        string[] ContractsFilter;
        bool SubscribeEvent;
        bool SubscribeJsonBlock;
        bool SubscribeRawBlock;
        bool SubscribeBlockTxHashs;
    }


    class WsServer
    {
        sync.RWMutex;
        websocket.Upgrader Upgrader;
        net.Listener listener;
        *http.Server server;
        SessionList SessionList;
        Dictionary<string, Handler> ActionMap;
        Dictionary<string, string> TxHashMap;
        Dictionary<string, subscribe> SubscribeMap;
    }

    #endregion

}
