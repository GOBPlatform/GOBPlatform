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
    #endregion

    #region consensus/vbft/chain_store_test.go
    #endregion

    #region consensus/vbft/chain_store.go
    #endregion

    #region consensus/vbft/event_timer_test.go
    #endregion

    #region consensus/vbft/event_timer.go
    #endregion

    #region consensus/vbft/msg_builder_test.go
    #endregion

    #region consensus/vbft/msg_builder.go
    #endregion

    #region consensus/vbft/msg_pool_test.go
    #endregion

    #region consensus/vbft/msg_pool.go
    #endregion

    #region consensus/vbft/msg_types_test.go
    #endregion

    #region consensus/vbft/msg_types.go
    #endregion

    #region consensus/vbft/node_tync.go
    #endregion

    #region consensus/vbft/node_utils_test.go
    #endregion

    #region consensus/vbft/node_utils.go
    #endregion

    #region consensus/vbft/peer_pool_test.go
    #endregion

    #region consensus/vbft/peer_pool.go
    #endregion

    #region consensus/vbft/service.go
    #endregion

    #region consensus/vbft/service_mgmt_test.go
    #endregion

    #region consensus/vbft/service_mgmt.go
    #endregion

    #region consensus/vbft/types_test.go
    #endregion

    #region consensus/vbft/types.go
    #endregion

    #region consensus/vbft/utils_test.go
    #endregion

    #region consensus/vbft/utils.go
    #endregion

    #endregion

    #region consensus/consensus.go
    #endregion

    #region consensus/policy_level.go
    #endregion

    #region consensus/policy.go
    #endregion

    #endregion
}
