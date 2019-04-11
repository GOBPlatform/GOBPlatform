using System;
using System.Collections.Generic;
using System.Text;

namespace GOBBlockchain.Errors
{
    public enum Errcode
    {
        ErrNoCode = -2,
        ErrNoError = 0,
        ErrUnknown = -1,
        ErrDuplicatedTx = 45002,
        ErrDuplicateInput = 45003,
        ErrAssetPrecision = 45004,
        ErrTransactionBalance = 45005,
        ErrAttributeProgram = 45006,
        ErrTransactionContracts = 45007,
        ErrTransactionPayload = 45008,
        ErrDoubleSpend = 45009,
        ErrTxHashDuplicate = 45010,
        ErrStateUpdaterVaild = 45011,
        ErrSummaryAsset = 45012,
        ErrXmitFail = 45013,
        ErrNoAccount = 45014,
        ErrRetryExhausted = 45015,
        ErrTxPoolFull = 45016,
        ErrNetPackFail = 45017,
        ErrNetUnPackFail = 45018,
        ErrNetVerifyFail = 45019,
        ErrGasPrice = 45020,
        ErrVerifySignature = 45021
    }

    class Errcoder
    {
        Int32 ErrCode;

        public Errcoder(Int32 errCode)
        {
            this.ErrCode = errCode;
        }

        public Int32 GetErrCode()
        {
            return Int32.MaxValue;
        }

        public String GetError(Int32 err)
        {
            switch (err)
            {
                case (Int32)Errcode.ErrNoCode:
                    return "no error code";
                case (Int32)Errcode.ErrNoError:
                    return "not an error";
                case (Int32)Errcode.ErrUnknown:
                    return "unknown error";
                case (Int32)Errcode.ErrDuplicatedTx:
                    return "duplicated transaction detected";
                case (Int32)Errcode.ErrDuplicateInput:
                    return "duplicated transaction input detected";
                case (Int32)Errcode.ErrAssetPrecision:
                    return "invalid asset precision";
                case (Int32)Errcode.ErrTransactionBalance:
                    return "transaction balance unmatched";
                case (Int32)Errcode.ErrAttributeProgram:
                    return "attribute program error";
                case (Int32)Errcode.ErrTransactionContracts:
                    return "invalid transaction contract";
                case (Int32)Errcode.ErrTransactionPayload:
                    return "invalid transaction payload";
                case (Int32)Errcode.ErrDoubleSpend:
                    return "double spent transaction detected";
                case (Int32)Errcode.ErrTxHashDuplicate:
                    return "duplicated transaction hash detected";
                case (Int32)Errcode.ErrStateUpdaterVaild:
                    return "invalid state updater";
                case (Int32)Errcode.ErrSummaryAsset:
                    return "invalid summary asset";
                case (Int32)Errcode.ErrXmitFail:
                    return "transmit error";
                case (Int32)Errcode.ErrRetryExhausted:
                    return "retry exhausted";
                case (Int32)Errcode.ErrTxPoolFull:
                    return "tx pool full";
                case (Int32)Errcode.ErrNetPackFail:
                    return "net msg pack fail";
                case (Int32)Errcode.ErrNetUnPackFail:
                    return "net msg unpack fail";
                case (Int32)Errcode.ErrNetVerifyFail:
                    return "net msg verify fail";
                case (Int32)Errcode.ErrGasPrice:
                    return "invalid gas price";
                case (Int32)Errcode.ErrVerifySignature:
                    return "transaction verify signature fail";
            }

            return "Unknown error? Error code = " + err.ToString();
        }
    }
}
