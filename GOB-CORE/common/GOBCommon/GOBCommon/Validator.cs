using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GOBCommon
{
    class Validator
    {
    }

    #region validator/db/best_block.go

    class BestBlock
    {
        UInt32 Height;
        common.Uint256 Hash;
    }

    interface BestStateProvider
    {
        BestBlock GetBestBlock();

        types.Header GetBestHeader();
    }

    #endregion

    #region validator/db/key_prefix.go

    enum KeyPrefix :byte
    {
        SYS_VERSION = 0,
        SYS_GENESIS_BLOCK = 1,
        SYS_BEST_BLOCK = 2,
        SYS_BEST_BLOCK_HEADER = 3,
        DATA_TRANSACTION = 10
    }

    #endregion

    #region validator/db/store.go
    class Store
    {
        storcomm.PersistStore db;
        sync.RWMutex mutex;
        *types.Header bestBlockHeader;
        *types.Block genesisBlock;
    }
    #endregion

    #region validator/db/transaction_provider.go
    interface TransactionProvider : BestStateProvider
    {
        bool ContainTransaction(common.Uint256 hash);
        byte[] GetTransactionBytes(common.Uint256 hash);
        types.Transaction GetTransaction(common.Uint256 hash);
        void PersistBlock(types.Block block);
    }
    #endregion

    #region validator/increment/increment.go
    class IncrementValidator
    {
        ReaderWriterLock mutext; //sync.Mutex mutex;

        List<Dictionary<common.Uint256, bool>> blocks;
        UInt32 baseHeight;
        int maxBlocks;
    }
    #endregion

    #region validator/stateful/stateful_validator.go
    interface Validator
    {
        void Register(*actor.PID poolId);
        void UnRegister(*actor.PID poolId);

        vattypes.VerifyType VerifyType();
    }

    class validator
    {
        private *actor.PID pid;
        private string id;
        private BestBlock bestBlock;
    }
    #endregion

    #region validator/stateless/stateless_validator.go

    // stateful_validator에서 선언해서 생략
    //interface Validator
    //{
    //    void Register(*actor.PID poolId);
    //    void UnRegister(*actor.PID poolId);
    //    vattypes.VerifyType VerifyType();
    //}
    // stateful_validator에서 선언해서 생략
    //class validator
    //{
    //    private * actor.PID pid;
    //    private string id;
    //}
    #endregion

    #region validator/types/messages.go
    class RegisterValidator
    {
        *actor.PID Sender;
        VerifyType Type;
        string Id;
    }

    class UnRegisterValidator
    {
        string Id;
        VerifyType Type;
    }

    class UnRegisterAck
    {
        string Id;
        VerifyType Type;
    }

    class CheckTx
    {
        byte WorkerId; // golang uint8
        *types.Transaction Tx;
    }

    class CheckResponse
    {
        byte WorkerId; // golang uint8
        VerifyType Type;
        common.Uint256 Hash;
        UInt32 Height;
        errors.ErrCode ErrCode;
    }

    enum VerifyType : byte //uint8
    {
        Stateless = 0,
        Stateful
    }
    #endregion
}
