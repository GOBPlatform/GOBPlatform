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

        string calcuateHash();
        bool AddTransaction(ITransaction transaction);
    }
}
