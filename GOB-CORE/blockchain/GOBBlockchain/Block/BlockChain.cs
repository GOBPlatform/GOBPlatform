using System;
using System.Collections.Generic;
using System.Text;

using GOBBlockchain.Transaction;

namespace GOBBlockchain.Block
{
    public class BlockChain
    {
        public static Dictionary<string, TransactionOutput> UTXOs = new Dictionary<string, TransactionOutput>();
        public static float minimumTransaction = 0.1f;
    }
}
