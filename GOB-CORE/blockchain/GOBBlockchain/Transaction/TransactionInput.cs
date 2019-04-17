using System;
using System.Collections.Generic;
using System.Text;

namespace GOBBlockchain.Transaction
{
    [Serializable]
    public class TransactionInput
    {
        public string transactionOutputId;
        public TransactionOutput UTOX;

        public TransactionInput(string transactionOutputId)
        {
            this.transactionOutputId = transactionOutputId;
        }
    }
}
