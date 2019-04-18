using System;
using System.Collections.Generic;
using System.Text;

using GOBBlockchain.Transaction;

namespace GOBBlockchain.Block
{
    public interface IBlock
    {
        IBlockHeader BlockHeader { get; }
        List<ITransaction> Transactions { get; set; }

        byte[] GenerateHash();
        bool AddTransaction(ITransaction transaction);
    }
}
