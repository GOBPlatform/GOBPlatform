using System;
using System.Collections.Generic;
using System.Text;

namespace GOBCommon
{
    class consensus
    {
    }

    #region consensus

    #region consensus/actor

    #region consensus/actor/actor.go

    class TxPoolActor
    {
        PID Pool; //actor.PID
    }

    class P2PActor
    {
        PID P2P;  //actor.PID
    }

    class LedgerActor
    {
        PID Ledger; //actor.PID
    }

    #endregion

    #region consensus/actor/message.go

    class StartConsensus { }

    class StopConsensus { }

    class TimeOut { }

    class BlockCompleted
    {
        Block Block; //types.Block
    }

    #endregion

    #endregion

    #region consensus/dbft

    #region consensus/dbft/block_signatures.go

    class BlockSignatures
    {
        ConsensusMessageData msgData;
        List<SignaturesData> Signatures;
    }

    class SignaturesData
    {
        byte[] Signature;
        UInt16 Index;
    }

    #endregion

    #region consensus/dbft/change_view.go

    class ChangeView
    {
        ConsensusMessageData msgData;
        byte NewViewNumber;
    }

    #endregion

    #region consensus/dbft/consensus_context.go

    class ConsensusContext
    {
        ConsensusState State;
        Uint256 PrevHash; //common.Uint256
        UInt32 Height;
        byte ViewNumber;
        List<PublicKey> Bookkeepers; //keypair.PublicKey
        List<PublicKey> NextBookkeepers; //keypair.PublicKey
        PublicKey Owner; //keypair.PublicKey
        int BookkeeperIndex;
        UInt32 PrimaryIndex;
        UInt32 Timestamp; //Date로 변환???
        UInt64 Nonce;
        Address NextBookkeeper; //common.Address
        List<Transaction> Transactions; //types.Transaction
        byte[][] Signatures;
        byte[] ExpectedView;
        Block header; //types.Block
        bool isBookkeeperChanged;
        UInt32 nmChangedblkHeight;
    }

    #endregion

    #region consensus/dbft/consensus_message_type.go

    enum ConsensusMessageType : byte
    {
        ChangeViewMsg = 0x00,
        PrepareRequestMsg = 0x20,
        PrepareResponseMsg = 0x21,
        BlockSignaturesMsg = 0x01
    }

    #endregion

    #region consensus/dbft/consensus_message.go

    interface ConsensusMessage
    {
        void Serialization(ZeroCopySink sink);
        void Deserialization(ZeroCopySource source);
        ConsensusMessageType Type();
        byte ViewNumber();
        ConsensusMessageData ConsensusMessageData();
    }

    class ConsensusMessageData
    {
        ConsensusMessageType Type;
        byte ViewNumber;
    }

    #endregion

    #region consensus/dbft/consensus_state.go

    enum ConsensusState : byte
    {
        Initial = 0x00,
        Primary = 0x01,
        Backup = 0x02,
        RequestSent = 0x04,
        RequestReceived = 0x08,
        SignatureSent = 0x10,
        BlockGenerated = 0x20
    }

    #endregion

    #region consensus/dbft/dbft_service.go

    class DbftService
    {
        ConsensusContext context;
        Account Account; //account.Account
        Timer timer; //time.Timer
        UInt32 timerHeight;
        byte timeView;
        Time blockReceivedTime; //time.Time
        bool started;
        Ledger legder; //ledger.Ledger
        IncrementValidator incrValidator; //increment.IncrementValidator
        TxPoolActor poolActor;
        P2PActor p2p;

        PID pid; //actor.PID
        ActorSubscriber sub; //events.ActorSubscriber
    }

    #endregion

    #region consensus/dbft/prepare_request.go

    class PrepareRequest
    {
        ConsensusMessageData msgData;
        UInt64 Nonce;
        Address NextBookkeeper; //common.Address
        List<Transaction> Transactions; //types.Transaction : core/types
        byte[] Signature;
    }

    #endregion

    #region consensus/dbft/prepare_response.go

    class PrepareResponse
    {
        ConsensusMessageData msgData;
        byte[] Signature;
    }
    #endregion

    #endregion

    #region consensus/sbft

    #region consensus/sbft/sbft_service.go

    class SbftService { }

    #endregion

    #endregion

    #region consensus/solo

    #region consensus/solo/solo.go

    class SoloService
    {
        Account Account; //account.Account;
        TxPoolActor poolActor;
        IncrementValidator incrValidator;
        //existCh chan interface{ }
        Int64 genBlockInterval; //time.Duration genBlockInterval;
        PID pid; //actor.PID
        ActorSubscriber sub; //events.ActorSubscriber
    }
    #endregion

    #endregion

    #region consensus/vbft

    #region consensus/vbft/config

    #region consensus/vbft/config/config_test.go
    #endregion

    #region consensus/vbft/config/config.go
    class PeerConfig
    {
        UInt32 Index;
        string ID;
    }
    class ChainConfig
    {
        UInt32 Version;
        UInt32 View;
        UInt32 N;
        UInt32 C;
        Int64 BlockMsgDelay; //time.Duration
        Int64 HashMsgDelay; //time.Duration
        Int64 PeerHandshakeTimeout; //time.Duration
        PeerConfig[] Peers;
        UInt32[] PosTable;
        UInt32 MaxBlockChangeView;
    }

    class VbftBlockInfo
    {
        UInt32 Proposer;
        byte[] VrfValue;
        byte[] VrfProof;
        UInt32 LaskConfigBlockNum;
        ChainConfig NewChainConfig;
    }

    class VRFValue
    {
        const byte VRF_SIZE = 64;
        const byte MAX_PROPOSER_COUNT = 32;
        const byte MAX_ENDORSER_COUNT = 240;
        const byte MAX_COMMITTER_COUNT = 240;

        byte[VRF_SIZE] value;

    }
    #endregion

    #region consensus/vbft/config/genesis_test.go
    #endregion

    #region consensus/vbft/config/genesis.go
    #endregion

    #region consensus/vbft/config/types_test.go
    #endregion

    #region consensus/vbft/config/types.go
    #endregion

    #endregion

    #region consensus/vbft/block_pool.go
    class BlockList : List<Block> { }

    class CandidateEndorseSigInfo
    {
        UInt32 EndorsedProposer;
        byte[] Signature;
        bool ForEmpty;
    }

    class CandidateInfo
    {
        // server endorsed proposals
        blockProposalMsg EndorsedProposal;
        blockProposalMsg EndorsedEmptyProposal;

        // server committed proposals (one of them must be nil)
        blockProposalMsg CommittedProposal;
        blockProposalMsg CommittedEmptyProposal;

        bool commitDone;

        // server sealed block for this round
        Block SealedBlock;

        // candidate msgs for this round
        blockProposalMsg[] Proposals;
        blockCommitMsg[] CommitMsgs;

        // indexed by endorserIndex
        Dictionary<UInt32, CandidateEndorseSigInfo[]> EndorseSigs;
    }

    class BlockPool
    {
        ReadWriteLock mutex;
        UInt32 HistoryLen;

        Server server;
        ChainStore chainStore;
        Dictionary<UInt32, CandidateInfo> candidateBlocks;
    }
    #endregion

    #region consensus/vbft/chain_store_test.go
    #endregion

    #region consensus/vbft/chain_store.go
    class PendingBlock
    {
        Block block;
        ExecuteResult execResult; //store.ExecuteResult
        bool hasSubmitted;
    }

    class ChainStore
    {
        Ledger db; //ledger.Ledger
        UInt32 chainedBlockNum;
        Dictionary<UInt32, PendingBlock> pendingBlocks;
        PID pid; //actor.PID
    }
    #endregion

    #region consensus/vbft/event_timer_test.go
    #endregion

    #region consensus/vbft/event_timer.go
    enum TimerEventType
    {
        EventProposeBlockTimeout = 0,
        EventProposalBackoff,
        EventRandomBackoff,
        EventPropose2ndBlockTimeout,
        EventEndorseBlockTimeout,
        EventEndorseEmptyBlockTimeout,
        EventCommitBlockTimeout,
        EventPeerHeartbeat,
        EventTxPool,
        EventTxBlockTimeout,
        EventMax
    }

    class SendMsgEvent
    {
        uint ToPeer;
        ConsensusMsg Msg;
    }

    class TimerEvent
    {
        TimerEventType evtType;
        UInt32 blockNum;
        ConsensusMsg Msg;
    }

    class EventTimer
    {
        ReadWriteLock _lock; //lock sync.Mutex
        Server server;
        Channel<TimerEvent> C; //C chan *TimerEvent

        // bft timers
        Dictionary<TimerEventType, Dictionary<UInt32, Timer>> eventTimers; // eventTimers map[TimerEventType]perBlockTimer

        // peer heartbeat tickers
        Dictionary<UInt32, Timer> peerTickers; // peerTickers map[uint32]*time.Timer
        // other timers
        Dictionary<UInt32, Timer> normalTimers; // normalTimers map[uint32]*time.Timer
    }

    class TimerItem
    {
        Time due; // time.Time
        TimerEvent evt;
        int index;
    }

    class TimerQueue : List<TimerItem> { }
    #endregion

    #region consensus/vbft/msg_builder_test.go
    #endregion

    #region consensus/vbft/msg_builder.go
    class ConsensusMsgPayload
    {
        MsgType Type;
        UInt32 Len;
        byte[] Payload;
    }
    #endregion

    #region consensus/vbft/msg_pool_test.go
    #endregion

    #region consensus/vbft/msg_pool.go
    // type ConsensusRoundMsgs map[MsgType][]ConsensusMsg
    class ConsensusRoundMsgs : Dictionary<MsgType, ConsensusMsg[]> { }

    class ConsensusRound
    {
        UInt32 blockNum;
        Dictionary<MsgType, ConsensusMsg[]> msgs;    // msgs     map[MsgType][]ConsensusMsg
        Dictionary<Uint256, IGOBInterface> msgHashs; // msgHashs map[common.Uint256]interface{}
    }

    class MsgPool
    {
        ReadWriteLock _lock; // lock       sync.RWMutex
        Server server;
        UInt32 historyLen;
        Dictionary<UInt32, ConsensusRound> rounds; // rounds     map[uint32]*ConsensusRound
    }
    #endregion

    #region consensus/vbft/msg_types_test.go
    #endregion

    #region consensus/vbft/msg_types.go
    enum MsgType : byte //uint8
    {
        BlockProposalMessage = 0,
        BlockEndorseMessage,
        BlockCommitMessage,

        PeerHandshakeMessage,
        PeerHeartbeatMessage,

        BlockInfoFetchMessage,
        BlockInfoFetchRespMessage,
        ProposalFetchMessage,
        BlockFetchMessage,
        BlockFetchRespMessage,
        BlockSubmitMessage
    }

    interface ConsensusMsg
    {
        MsgType Type();
        void Verify(PublicKey pubKey); //keypair.PublicKey
        UInt32 GetBlockNum();
        byte[] Serialize();
    }

    class blockProposalMsg
    {
        Block Block;
    }

    class FaultyReport
    {
        UInt32 FaultyID;
        Uint256 FaultyMsgHash; //common.Uint256
    }

    class blockEndorseMsg
    {
        UInt32 Endorser;
        UInt32 EndorsedProposer;
        UInt32 BlockNum;
        Uint256 EndorsedBlockHash; // common.Uint256
        bool EndorseForEmpty;
        FaultyReport[] FaultyProposals;
        byte[] ProposerSig;
        byte[] EndorserSig;
    }

    class blockCommitMsg
    {
        UInt32 Committer;
        UInt32 BlockProposer;
        UInt32 BlockNum;
        Uint256 CommitBlockHash; // common.Uint256
        bool CommitForEmpty;
        FaultyReport[] FaultyVerifies;
        byte[] ProposerSig;
        Dictionary<UInt32, byte[]> EndorsersSig;
        byte[] CommitterSig;
    }

    class peerHandshakeMsg
    {
        UInt32 CommittedBlockNumber;
        Uint256 CommittedBlockHash;
        UInt32 CommittedBlockLeader;
        ChainConfig ChainConfig;
    }

    class peerHeartbeatMsg
    {
        UInt32 CommittedBlockNumber;
        Uint256 CommittedBlockHash;
        UInt32 CommittedBlockLeader;
        byte[][] Endorsers;
        byte[][] EndorsersSig;
        UInt32 ChainConfigView;
    }

    class BlockInfoFetchMsg
    {
        UInt32 StartBlockNum;
    }

    class BlockInfo_
    {
        UInt32 BlockNum;
        UInt32 Proposer;
        Dictionary<UInt32, byte[]> Signatures;
    }

    class BlockInfoFetchRespMsg
    {
        BlockInfo_[] Blocks;
    }

    class blockFetchMsg
    {
        UInt32 BlockNum;
    }

    class BlockFetchRespMsg
    {
        UInt32 BlockNum;
        Uint256 BlockHash;
        Block BlockData;
    }

    class proposalFetchMsg
    {
        UInt32 ProposerID;
        UInt32 BlockNum;
    }

    class blockSubmitMsg
    {
        Uint256 BlockStateRoot;
        UInt32 BlockNum;
        byte[] SubmitMsgSig;
    }
    #endregion

    #region consensus/vbft/node_sync.go
    class SyncCheckReq
    {
        ConsensusMsg msg;
        UInt32 peerIdx;
        UInt32 blockNum;
    }

    class BlockSyncReq
    {
        UInt32[] targetPeers;
        UInt32 startBlockNum;
        UInt32 targetBlockNum;
    }

    class PeerSyncer
    {
        ReaderWirterLock _lock; //sync.Mutex
        UInt32 peerIdx;
        UInt32 nextReqBlkNum;
        UInt32 targetBlkNum;
        bool active;

        Server server;
        Channel<ConsensusMsg> msgC; // msgC   chan ConsensusMsg
    }

    class SyncMsg
    {
        UInt32 fromPeer;
        ConsensusMsg msg;
    }

    class BlockMsgFromPeer
    {
        UInt32 fromPeer;
        Block block;
    }

    class BlockFromPeers : Dictionary<UInt32, Block> { };

    class Syncer
    {
        ReaderWirterLock _lock; //sync.Mutex
        Server server;

        int maxRequestPerPeer;
        UInt32 nextReqBlkNum;
        UInt32 targetBlkNum;

        Channel<SyncCheckReq> syncCheckReqC;
        Channel<BlockSyncReq> blockSyncReqC;
        Channel<SyncMsg> syncMsgC;
        Channel<BlockMsgFromPeer> blockFromPeerC;

        Dictionary<UInt32, PeerSyncer> peers;
        Dictionary<UInt32, BlockFromPeers> pendingBlocks;
    }
    #endregion

    #region consensus/vbft/node_utils_test.go
    #endregion

    #region consensus/vbft/node_utils.go
    #endregion

    #region consensus/vbft/peer_pool_test.go
    #endregion

    #region consensus/vbft/peer_pool.go
    class Peer
    {
        UInt32 Index;
        PublicKey PubKey; //keypair.PublicKey
        peerHandshakeMsg handShake;
        peerHeartbeatMsg LatestInfo;
        DateTime LastUpdateTime; //time.Time
        bool connected;
    }

    class PeerPool
    {
        ReaderWriterLock _lock;
        int maxSize;

        Server server;
        Dictionary<UInt32, PeerConfig> configs;
        Dictionary<string, UInt32> IDMap;
        Dictionary<UInt32, UInt64> P2pMap;

        Dictionary<UInt32, Peer> peers;
        Dictionary<UInt32, Channel> peerConnectionWaitings; // peerConnectionWaitings map[uint32]chan struct{}
    }
    #endregion

    #region consensus/vbft/service.go
    enum BftActionType : byte
    {
        MakeProposal = 0,
        EndorseBlock,
        CommitBlock,
        SealBlock,
        FastForward,
        ReBroadcast,
        SubmitBlock
    }

    class BftAction
    {
        BftActionType Type;
        UInt32 BlockNum;
        blockProposalMsg Proposal;
        bool forEmpty;
    }

    class BlockParticpantConfig
    {
        UInt32 BlockNum;
        UInt32 L;
        VRFValue Vrf;
        ChainConfig ChainConfig;
        UInt32[] Proposers;
        UInt32[] Endorsers;
        UInt32[] Committers;
    }

    class p2pMsgPayload
    {
        UInt32 fromPeer;
        ConsensusPayload payload;
    }

    class Server
    {
        UInt32 Index;
        Account account; // account.Account;
        TxPoolActor poolActor;
        P2PActor p2p;
        Ledger ledger; //ledger.Ledger
        IncrementValidator incrValidator;
        PID pid; // actor.PID

        UInt32 msgHistoryDuration;

        ReaderWriterLock metaLock; // metaLock                 sync.RWMutex
        UInt32 completedBlockNum;
        UInt32 currentBlockNum;
        UInt32 LastConfigBlockNum;
        ChainConfig config;
        BlockParticpantConfig currentParticipantConfig;

        ChainStore chainStore;
        MsgPool msgPool;
        BlockPool blockPool;
        PeerPool peerPool;
        Syncer syncer;
        StateMgr stateMgr;
        EventTimer timer;

        Dictionary<UInt32, Channel<p2pMsgPayload>> msgRecvC;
        Channel<ConsensusMsg> msgC;
        Channel<BftAction> bftActionC;
        Channel<SendMsgEvent> msgSendC;
        ActorSubscriber sub;
        Channel quitC;
        bool quit;
        WaitGroup quitWg;
    }
    #endregion

    #region consensus/vbft/service_mgmt_test.go
    #endregion

    #region consensus/vbft/service_mgmt.go
    enum ServerState
    {
        Init = 0,
        LocalConfigured,
        Configured,
        Syncing,
        WaitNetworkReady,
        SyncReady,
        Synced,
        SyncingCheck
    }

    enum StateEventType
    {
        ConfigLoaded = 0,
        UpdatePeerConfig,
        UpdatePeerState,
        SyncReadyTimeout,
        SyncDone,
        LiveTick
    }

    class StateEvent
    {
        StateEventType Type;
        PeerState peerState;
        UInt32 blockNum;
    }

    class PeerState
    {
        UInt32 peerIdx;
        UInt32 chainConfigView;
        UInt32 committedBlockNum;
        bool connected;
    }

    class StateMgr
    {
        Server server;
        Int64 syncReadyTimeout; // syncReadyTimeout time.Duration
        ServerState currentState;
        Channel<StateEvent> StateEventC;
        Dictionary<UInt32, PeerState> peers;

        Timer liveTicker; //time.Timer
        UInt32 lastTickChainHeight;
        UInt32 lastBlockSyncReqHeight;
    }
    #endregion

    #region consensus/vbft/types_test.go
    #endregion

    #region consensus/vbft/types.go
    class Block
    {
        types.Block Block;
        types.Block EnmptyBlock;
        VbftBlockInfo Info;
        Uint256 PrevBlockMerkleRoot;
    }
    #endregion

    #region consensus/vbft/utils_test.go
    #endregion

    #region consensus/vbft/utils.go
    class seedData
    {
        UInt32 BlockNum;
        UInt32 PrevBlockProposer;
        Uint256 BlockRoot;
        byte[] VrfValue;
    }

    class vrfData
    {
        UInt32 BlockNum;
        byte[] PrevVrf;
    }
    #endregion

    #endregion

    #region consensus/consensus.go
    interface ConsensusService
    {
        void Start();
        void Halt();
        PID GetPID(); //actor.PID
    }
    #endregion

    #region consensus/policy_level.go
    enum PolicyLevel : byte
    {
        AllowAll = 0x00,
        DenyAll = 0x01,
        AllowList = 0x02,
        DenyList = 0x03
    }
    #endregion

    #region consensus/policy.go
    class Policy
    {
        PolicyLevel PolicyLevel;
        Address[] List; //common.Address
    }
    #endregion

    #endregion
    }
