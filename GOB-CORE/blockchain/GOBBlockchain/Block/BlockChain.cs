using System;
using System.Collections.Generic;
using System.Text;

using GOBBlockchain.Transaction;

namespace GOBBlockchain.Block
{
    public class BlockChain
    {
        private List<IBlock> _Blocks;

        public static Dictionary<string, TransactionOutput> UTXOs = new Dictionary<string, TransactionOutput>();
        public static float minimumTransaction = 0.1f;

        public List<IBlock> Blocks { get => _Blocks; set => _Blocks = value; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BlockChain()
        {
            //블록체인의 시작~! 제네시스 블록을 만들어 Start 한다.
            IBlockHeader header = new BlockHeader("AAA");
            IBlock genesis;
        }
    }
}
