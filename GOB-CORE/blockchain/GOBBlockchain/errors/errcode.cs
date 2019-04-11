using System;
using System.Collections.Generic;
using System.Text;

namespace GOBBlockchain.errors
{
    public enum ErrCode
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

    interface IErrCode
    {
        string GetErrorString(Int32 err);
    }

    class errcode : IErrCode
    {
        Int32 code;

        string IErrCode.GetErrorString(Int32 err)
        {
            switch (err)
            {
                case (Int32)ErrCode.ErrNoCode:
                    return "no error code";
                case (Int32)ErrCode.ErrNoError:
                    return "not an error";
                case (Int32)ErrCode.ErrUnknown:
                    return "unknown error";
                case (Int32)ErrCode.ErrDuplicatedTx:
                    return "duplicated transaction detected";
                case (Int32)ErrCode.ErrDuplicateInput:
                    return "duplicated transaction input detected";
                case (Int32)ErrCode.ErrAssetPrecision:
                    return "invalid asset precision";
                case (Int32)ErrCode.ErrTransactionBalance:
                    return "transaction balance unmatched";
                case (Int32)ErrCode.ErrAttributeProgram:
                    return "attribute program error";
                case (Int32)ErrCode.ErrTransactionContracts:
                    return "invalid transaction contract";
                case (Int32)ErrCode.ErrTransactionPayload:
                    return "invalid transaction payload";
                case (Int32)ErrCode.ErrDoubleSpend:
                    return "double spent transaction detected";
                case (Int32)ErrCode.ErrTxHashDuplicate:
                    return "duplicated transaction hash detected";
                case (Int32)ErrCode.ErrStateUpdaterVaild:
                    return "invalid state updater";
                case (Int32)ErrCode.ErrSummaryAsset:
                    return "invalid summary asset";
                case (Int32)ErrCode.ErrXmitFail:
                    return "transmit error";
                case (Int32)ErrCode.ErrRetryExhausted:
                    return "retry exhausted";
                case (Int32)ErrCode.ErrTxPoolFull:
                    return "tx pool full";
                case (Int32)ErrCode.ErrNetPackFail:
                    return "net msg pack fail";
                case (Int32)ErrCode.ErrNetUnPackFail:
                    return "net msg unpack fail";
                case (Int32)ErrCode.ErrNetVerifyFail:
                    return "net msg verify fail";
                case (Int32)ErrCode.ErrGasPrice:
                    return "invalid gas price";
                case (Int32)ErrCode.ErrVerifySignature:
                    return "transaction verify signature fail";
            }

            return "Unknown error? Error code = " + err.ToString();
        }
    }
}
