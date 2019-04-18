using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GOBBlockchain.Transaction;
using GOBCommon;
using GOBCommon.Hellper;
using GOBCrypto;

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

        public byte[] GenerateHash()
        {
            using (MemoryStream st = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(st))
            {
                bw.Write(BlockHeader.ToByteArray());
                if(Transactions != null)
                {
                    foreach (var t in Transactions)
                    {
                        bw.Write(Common.GobSerialize(t));
                    }
                }

                return ExtSHA256.Hash(st.ToArray());
            }
        }

        #endregion
    }
}
