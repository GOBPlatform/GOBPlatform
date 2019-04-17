using System;
using System.Collections.Generic;
using System.Text;
using GOBBlockchain.Transaction;

namespace GOBBlockchain.Block
{
    public class Block : IBlock
    {
        public Block(IBlockHeader header, List<ITransaction> transactions)
        {
            this.BlockHeader = header;
            this.Transactions = transactions;
        }

        #region implement IBlock interface 

        public IBlockHeader BlockHeader { get; }

        public List<ITransaction> Transactions { get; set; }

        public bool AddTransaction(ITransaction transaction)
        {
            if (transaction == null) return false;
            if (!BlockHeader.PrevHash.Equals(0x00)) //Genesis 블록이 아니라면...
            {
                if((transaction.processTransaction() != true))
                {
                    Console.WriteLine("Transaction failed to process. Discarded.");
                    return false;
                }
            }

            Transactions.Add(transaction);
            Console.WriteLine("Transaction Successfully added to Block");
            return true;
        }

        public string calcuateHash()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
