﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GOBCommon
{
    public class p2pServer
    {
        #region p2pserver/p2pserver.go
        /// <summary>
        /// p2pServer/p2pserver.go/P2PServer
        /// </summary>
        class P2PServer
        {
            P2P network;
            MessageRouter msgRouter;
            //pid       *evtActor.PID
            //blockSync *BlockSyncMgr
            //ledger    *ledger.Ledger
            //ReconnectAddrs
            Dictionary<UInt32, string> recentPeers;
            bool quitSyncRecent;    //TODO...Chanel
            bool quitOnline;        //TODO...Chanel
            bool quitHeartBeat;     //TODO...Chanel
        }
        /// <summary>
        /// p2pServer/p2pserver.go/ReconnectAddrs
        /// </summary>
        class ReconnectAddrs
        {
            //sync.RWMutex
            Dictionary<string, int> RetryAddrs;
        }
        #endregion

        #region p2pserver/block_sync.go
        const int SYNC_MAX_HEADER_FORWARD_SIZE = 5000;       //keep CurrentHeaderHeight - CurrentBlockHeight <= SYNC_MAX_HEADER_FORWARD_SIZE
        const int SYNC_MAX_FLIGHT_HEADER_SIZE = 1;          //Number of headers on flight
        const int SYNC_MAX_FLIGHT_BLOCK_SIZE = 50;          //Number of blocks on flight
        const int SYNC_MAX_BLOCK_CACHE_SIZE = 500;          //Cache size of block wait to commit to ledger
        const int SYNC_HEADER_REQUEST_TIMEOUT = 2;          //s, Request header timeout time. If header haven't receive after SYNC_HEADER_REQUEST_TIMEOUT second, retry
        const int SYNC_BLOCK_REQUEST_TIMEOUT = 2;          //s, Request block timeout time. If block haven't received after SYNC_BLOCK_REQUEST_TIMEOUT second, retry
        const int SYNC_NEXT_BLOCK_TIMES = 3;          //Request times of next height block
        const int SYNC_NEXT_BLOCKS_HEIGHT = 2;          //for current block height plus next
        const int SYNC_NODE_RECORD_SPEED_CNT = 3;          //Record speed count for accuracy
        const int SYNC_NODE_RECORD_TIME_CNT = 3;          //Record request time  for accuracy
        const int SYNC_NODE_SPEED_INIT = 100 * 1024;   //Init a big speed (100MB/s) for every node in first round
        const int SYNC_MAX_ERROR_RESP_TIMES = 5;          //Max error headers/blocks response times, if reaches, delete it
        const int SYNC_MAX_HEIGHT_OFFSET = 5;          //Offset of the max height and current height

        /// <summary>
        /// NodeWeight record some params of node, using for sort
        /// p2pserver/block_sync.go
        /// </summary>
        class NodeWeight
        {
            UInt64 id;          //NodeID
            float[] speed;      //Record node request-response speed, using for calc the avg speed, unit kB/s
            int timeoutCnt;     //Node response timeout count
            int errorRespCnt;   //Node response error data count
            Int64 reqTime;      //Record request time, using for calc the avg req time interval, unit millisecond
        }

        /// <summary>
        /// SyncFlightInfo record the info of fight object(header or block)
        /// p2pserver/block_sync.go
        /// </summary>
        class SyncFlightInfo
        {
            UInt32 Height;
            UInt64 nodeId;
            //startTime time.Time      //Request start time
            Dictionary<UInt64, int> failedNodes;
            int totalFailed;
            //lock        sync.RWMutex
        }

        /// <summary>
        /// BlockInfo is used for saving block information in cache
        /// p2pserver/block_sync.go
        /// </summary>
        class BlockInfo
        {
            UInt64 nodeID;
            //block      *types.Block
            //merkleRoot common.Uint256
        }

        /// <summary>
        /// BlockSyncMgr is the manager class to deal with block sync
        /// p2pserver/block_sync.go
        /// </summary>
        class BlockSyncMgr
        {
            //flightBlocks   map[common.Uint256][]*SyncFlightInfo
            //flightHeaders  map[uint32]*SyncFlightInfo
            //blocksCache    map[uint32]*BlockInfo
            //server         *P2PServer
            bool syncBlockLock;
            bool syncHeaderLock;
            bool saveBlockLock;
            //exitCh         chan interface{}
            //ledger         *ledger.Ledger
            //lock           sync.RWMutex
            //nodeWeights    map[uint64]*NodeWeight
        }
        #endregion

        #region net
        #region p2pserver/net/protocol/server.go
        /// <summary>
        /// p2pserver/net/protocol/server.go/P2P
        /// </summary>
        interface P2P
        {
            void Start();
            void Halt();

            UInt64 GetID();
            UInt32 GetVersion();
            UInt16 GetSyncPort();

            UInt16 GetConsPort();
            UInt16 GetHttpInfoPort();

            bool GetRelay();

            UInt64 GetHeight();

            Int64 GetTime();

            UInt64 GetServices();

            //GetNeighbors() []*peer.Peer
            //GetNeighborAddrs() []common.PeerAddr

            UInt32 GetConnectionCnt();
            //GetNp() *peer.NbrPeers
            //GetPeer(uint64) *peer.Peer
            //SetHeight(uint64)
            //IsPeerEstablished(p *peer.Peer) bool
            //Send(p *peer.Peer, msg types.Message, isConsensus bool) error
            //GetMsgChan(isConsensus bool) chan *types.MsgPayload
            //GetPeerFromAddr(addr string) *peer.Peer
            //AddOutConnectingList(addr string) (added bool)
            int GetOutConnRecordLen();
            void RemoveFromConnectingList(string addr);
            void RemoveFromOutConnRecord(string addr);
            void RemoveFromInConnRecord(string addr);
            //AddPeerSyncAddress(addr string, p* peer.Peer)
            //AddPeerConsAddress(addr string, p *peer.Peer)
            void GetOutConnectingListLen(uint count);
            void RemovePeerSyncAddress(string addr);
            void RemovePeerConsAddress(string addr);
            //AddNbrNode(*peer.Peer)
            //DelNbrNode(id uint64) (*peer.Peer, bool)
            //NodeEstablished(uint64) bool
            //Xmit(msg types.Message, isCons bool)
            void SetOwnAddress(string addr);
            bool IsOwnAddress(string addr);
            bool IsAddrFromConnecting(string addr);
        }
        #endregion

        #region p2pserver/net/netserver/netserver.go
        /// <summary>
        /// p2pserver/net/netserver/netserver.go/NetServer
        /// </summary>
        class NetServer
        {
            //base         peer.PeerCom
            //synclistener net.Listener
            //conslistener net.Listener
            //SyncChan     chan *types.MsgPayload
            //ConsChan     chan *types.MsgPayload
            //ConnectingNodes
            //PeerAddrMap
            //Np            *peer.NbrPeers
            //connectLock   sync.Mutex
            //inConnRecord  InConnectionRecord
            //outConnRecord OutConnectionRecord
            string OwnAddress;
        }
        /// <summary>
        /// p2pserver/net/netserver/netserver.go/InConnectionRecord
        /// </summary>
        class InConnectionRecord
        {
            //sync.RWMutex;
            string[] InConnectingAddrs;
        }
        /// <summary>
        /// p2pserver/net/netserver/netserver.go/OutConnectionRecord
        /// </summary>
        class OutConnectionRecord
        {
            //sync.RWMutex
            string[] OutConnectingAddrs;
        }
        /// <summary>
        /// p2pserver/net/netserver/netserver.go/ConnectingNodes
        /// </summary>
        class ConnectingNodes
        {
            //sync.RWMutex
            string[] ConnectingAddrs;
        }
        /// <summary>
        /// p2pserver/net/netserver/netserver.go/PeerAddrMap
        /// </summary>
        class PeerAddrMap
        {
            //sync.RWMutex
            //PeerSyncAddress map[string]*peer.Peer
            //PeerConsAddress map[string]*peer.Peer
        }
        #endregion

        #region p2pserver/net/netserver/net_utils.go

        #endregion
        #endregion

        #region peer
        #region p2pserver/peer/peer.go
        /// <summary>
        /// p2pserver/peer/peer.go/PeerCom
        /// </summary>
        class Peer
        {
            PeerCom obase;
            byte[] cap;
            //SyncLink* conn.Link
            //ConsLink* conn.Link
            UInt32 syncState;
            UInt32 consState;
            UInt64 txnCnt;
            UInt64 rxTxnCnt;
            //connLock sync.RWMutex
        }

        /// <summary>
        /// p2pserver/peer/peer.go/PeerCom
        /// </summary>
        class PeerCom
        {
            UInt64 id;
            UInt32 version;
            UInt64 services;
            bool relay;
            UInt16 httpInfoPort;
            UInt16 syncPort;
            UInt16 consPort;
            UInt64 height;
            string softVersion;
        }
        #endregion

        #region p2pserver/peer/nbr_peers.go
        /// <summary>
        /// p2pserver/peer/nbr_peers.go/NbrPeers
        /// </summary>
        class NbrPeers
        {
            //sync.RWMutex
            //List map[uint64]*Peer
        }

        #endregion
        #endregion

        #region link
        #region p2pserver/link/link.go
        /// <summary>
        /// p2pserver/link/link.go/Link
        /// </summary>
        class Link
        {
            UInt64 id;
            string addr;                            //The address of the node
            //conn      net.Conn               // Connect socket with the peer node
            UInt16 port;                            //The server port of the node
            //time      time.Time              // The latest time the node activity
            //recvChan  chan *types.MsgPayload //msgpayload channel
            Dictionary<string, Int64> reqRecord;    //Map RequestId to Timestamp, using for rejecting duplicate request in specific time
        }
        #endregion
        #endregion

        #region Common        
        #region p2pserver/common/p2p_common.go
        /// <summary>
        /// p2pserver/common/p2p_common.go/PeerAddr
        /// </summary>
        class PeerAddr
        {
            Int64 Time;
            UInt64 Services;
            byte[] IpAddr;
            UInt16 Port;
            UInt16 ConsensusPort;
            UInt64 ID;
        }
        /// <summary>
        /// p2pserver/common/p2p_common.go/PeerAddr
        /// </summary>
        class AppendPeerID
        {
            UInt64 ID;
        }

        /// <summary>
        /// p2pserver/common/p2p_common.go/PeerAddr
        /// </summary>
        class RemovePeerID
        {
            UInt64 ID;
        }

        /// <summary>
        /// p2pserver/common/p2p_common.go/PeerAddr
        /// </summary>
        class AppendHeaders
        {
            UInt64 FromID;
            //Headers[]*types.Header
        }
        
        class AppendBlock
        {
            UInt64 FromID;
            UInt32 BlockSize;
            //Block* types.Block
            //MerkleRoot com.Uint256
        }

        #endregion

        #region p2pserver/common/checksum.go
        /// <summary>
        /// p2pserver/common/checksum.go/checksum
        /// </summary>
        class checksum
        {
            //hash.Hash
        }
        #endregion
        #endregion

        #region actor

        #region p2pserver/actor/server/common.go
        /// <summary>
        /// p2pserver/actor/server/common.go/StopServerReq
        /// </summary>
        class StopServerReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/StopServerRsp
        /// </summary>
        class StopServerRsp
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetVersionReq
        /// </summary>
        class GetVersionReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetVersionRsp
        /// </summary>
        class GetVersionRsp
        {
            UInt32 Version;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetConnectionCntReq
        /// </summary>
        class GetConnectionCntReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetConnectionCntRsp
        /// </summary>
        class GetConnectionCntRsp
        {
            UInt32 Cnt;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetIdReq
        /// </summary>
        class GetIdReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetIdRsp
        /// </summary>
        class GetIdRsp
        {
            UInt64 Id;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetSyncPortReq
        /// </summary>
        class GetSyncPortReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetSyncPortRsp
        /// </summary>
        class GetSyncPortRsp
        {
            UInt16 SyncPort;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetConsPortReq
        /// </summary>
        class GetConsPortReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetConsPortRsp
        /// </summary>
        class GetConsPortRsp
        {
            UInt16 ConsPort;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetPortReq
        /// </summary>
        class GetPortReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetPortRsp
        /// </summary>
        class GetPortRsp
        {
            UInt16 SyncPort;
            UInt16 ConsPort;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetConnectionStateReq
        /// </summary>
        class GetConnectionStateReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetConnectionStateRsp
        /// </summary>
        class GetConnectionStateRsp
        {
            UInt32 State;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetTimeReq
        /// </summary>
        class GetTimeReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetTimeRsp
        /// </summary>
        class GetTimeRsp
        {
            Int64 Time;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetNodeTypeReq
        /// </summary>
        class GetNodeTypeReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetNodeTypeRsp
        /// </summary>
        class GetNodeTypeRsp
        {
            UInt64 NodeType;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetRelayStateReq
        /// </summary>
        class GetRelayStateReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetRelayStateRsp
        /// </summary>
        class GetRelayStateRsp
        {
            bool Relay;
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetNeighborAddrsReq
        /// </summary>
        class GetNeighborAddrsReq
        {

        }

        /// <summary>
        /// p2pserver/actor/server/common.go/GetNeighborAddrsRsp
        /// </summary>
        class GetNeighborAddrsRsp
        {
            //Addrs []types.PeerAddr
        }

        /// <summary>
        /// p2pserver/actor/server/common.go/TransmitConsensusMsgReq
        /// </summary>
        class TransmitConsensusMsgReq
        {
            UInt64 Target;
            //Msg ptypes.Message;
        }
        #endregion

        #region p2pserver/actor/server/actor.go
        class P2PActor
        {
            //props  *actor.Props
            //server *p2pserver.P2PServer
        }
        #endregion

        #endregion

        #region message

        #region p2pserver/message/utils/msg_router.go
        class MessageRouter
        {
            //msgHandlers  map[string]MessageHandler
            //RecvSyncChan chan *types.MsgPayload
            //RecvConsChan chan *types.MsgPayload
            //stopSyncCh   chan bool
            //stopConsCh   chan bool
            //p2p          p2p.P2P
            //pid          *actor.PID
        }
        #endregion

        #region p2pserver/message/types/address_req.go
        class AddrReq
        {

        }
        #endregion

        #region p2pserver/message/types/address.go
        class Addr
        {
            //NodeAddrs[] comm.PeerAddr
        }
        #endregion

        #region p2pserver/message/types/block_header.go
        class BlkHeader
        {
            //BlkHdr[]*ct.Header
        }
        #endregion

        #region p2pserver/message/types/block_headers_req.go
        class HeadersReq
        {
            uint Len;
            //HashStart common.Uint256
            //HashEnd   common.Uint256
        }
        #endregion

        #region p2pserver/message/types/block.go
        class Block
        {
            //Blk        *ct.Block
            //MerkleRoot common.Uint256
        }
        #endregion

        #region p2pserver/message/types/blocks_req.go
        class BlocksReq
        {
            uint HeaderHashCount;
            //HashStart comm.Uint256
            //HashStop comm.Uint256
        }
        #endregion

        #region p2pserver/message/types/consensus_payload.go
        class ConsensusPayload
        {
            UInt32 Version;
            //PrevHash common.Uint256
            UInt32 Height;
            UInt16 BookkeeperIndex;
            UInt32 Timestamp;
            byte[] Data;
            //Owner keypair.PublicKey
            byte[] Signature;
            UInt64 PeerId;
            //hash common.Uint256
        }
        #endregion

        #region p2pserver/message/types/consensus.go
        class Consensus
        {
            //Cons ConsensusPayload
        }
        #endregion

        #region p2pserver/message/types/data_req.go

        class DataReq
        {
            //DataType common.InventoryType
            //Hash     common.Uint256
        }
        #endregion

        #region p2pserver/message/types/disconnected.go

        class Disconnected
        {

        }
        #endregion

        #region p2pserver/message/types/inventory.go
        class InvPayload
        {
            //InvType common.InventoryType
            //Blk     []common.Uint256
        }
        #endregion

        #region p2pserver/message/types/message.go
        interface Message
        {
            //Serialization(sink *comm.ZeroCopySink) error
            //Deserialization(source *comm.ZeroCopySource) error
            string CmdType();
        }

        class MsgPayload
        {
            UInt64 Id;
            string Addr;
            UInt32 PayloadSize;
            //Payload Message
        }

        class messageHeader
        {
            UInt32 Magin;
            //CMD      [common.MSG_CMD_LEN]byte
            UInt32 Length;
            //Checksum [common.CHECKSUM_LEN]byte
        }
        #endregion

        #region p2pserver/message/types/notfound.go
        class NotFound
        {
            //Hash common.Uint256
        }
        #endregion

        #region p2pserver/message/types/ping.go
        class Ping
        {
            UInt64 Height;
        }
        #endregion

        #region p2pserver/message/types/pong.go
        class Pong
        {
            UInt64 Height;
        }
        #endregion

        #region p2pserver/message/types/transaction.go
        class Trn
        {
            //Txn* types.Transaction
        }
        #endregion

        #region p2pserver/message/types/verack.go
        class VerACK
        {
            bool IsConsensus;
        }
        #endregion

        #region p2pserver/message/types/
        class VersionPayload
        {
            UInt32 Version;
            UInt64 Services;
            Int64 TimeStamp;
            UInt16 SyncPort;
            UInt16 HttpInfoPort;
            UInt16 ConsPort;
            byte[] Cap;
            UInt64 Nonce;
            UInt64 StartHeight;
            uint Relay;
            bool IsConsensus;
            string SoftVersion;
        }
        class Version
        {
            //P VersionPayload
        }
        #endregion

        #endregion
    }
}
