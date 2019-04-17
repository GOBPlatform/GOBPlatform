using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.IO;

namespace GOBBlock
{
    public interface IBlockHeader
    {
        string Version { get; }
        byte[] PrevHash { get; set; }
    }

    public class BlockHeader
    {
        
    }
    /// <summary>
    /// IBlock v.1.9.4.16
    /// </summary>
    public interface IBlock
    {
        string Version { get; }
        /// <summary>
        /// 블록의 길이
        /// </summary>
        int BlockSize { get; }
        /// <summary>
        /// 블록 해쉬(필요 없는거 같다.ㅋㅋㅋ)
        /// </summary>
        byte[] Hash { get; set; }
        /// <summary>
        /// 머클트리의 루트에 대한 해쉬
        /// </summary>
        byte[] MerkleRootHash { get; set; }
        /// <summary>
        /// 작업 증명에서 사용되는 카운터(사용안함)
        /// </summary>
        int Nonce { get; set; }
        /// <summary>
        /// 이전 블록의 해쉬
        /// </summary>
        byte[] PrevHash { get; set; }
        /// <summary>
        /// 블록의 생성 시각
        /// </summary>
        DateTime TimeStamp { get; }
        /// <summary>
        /// Transaction Data
        /// </summary>
        Object[] Transactions { get; }
        /// <summary>
        /// Block 생성자의 공개키
        /// </summary>
        string Sign { get; }
    }
    
    public class Block : IBlock
    {
        #region Member Fields

        /// <summary>
        /// Block Structure 의 버전
        /// </summary>
        private const string BLOCK_VER_1 = "v.1.9.4.16";

        public string Version { get; }
        public Object[] Transactions { get; }
        /// <summary>
        /// 블록 해쉬
        /// </summary>
        public byte[] Hash { get; set; }
        /// <summary>
        /// 요거 필요 없음.
        /// </summary>
        public int Nonce { get; set; }
        /// <summary>
        /// 이전 블록의 해쉬
        /// </summary>
        public byte[] PrevHash { get; set; }
        /// <summary>
        /// 블록 생성 시간
        /// </summary>
        public DateTime TimeStamp { get; }
        /// <summary>
        /// 머클 루트의 해쉬(블록에 쓰여지는 거래로 만든 머클루트 해쉬값)
        /// </summary>
        public byte[] MerkleRootHash { get; set; }
        /// <summary>
        /// 블록을 생성한 자의 공개키
        /// </summary>
        public string Sign { get; }

        public int BlockSize { get; }

        #endregion


        public Block(byte[] Transactions, string sign)
        {
            Version = BLOCK_VER_1;
            BlockSize = 1;
            Transactions = Transactions ?? throw new ArgumentNullException(nameof(Transactions));
            Nonce = 0;
            PrevHash = new byte[] { 0x00 };
            TimeStamp = DateTime.Now;
            Sign = sign;
        }

        public override string ToString()
        {
            return $"{BitConverter.ToString(Hash).Replace("-", "")}:\n{BitConverter.ToString(PrevHash).Replace("-", "")}\n {TimeStamp}";
        }

    }

    public class BlockChain : IEnumerable<IBlock>
    {
        private List<IBlock> _items = new List<IBlock>();

        public BlockChain(byte[] difficulty, IBlock genesis)
        {
            Diffiiculty = difficulty;
            genesis.Hash = genesis.MineHash(difficulty);
            Items.Add(genesis);
        }

        public void Add(IBlock item)
        {
            if (Items.LastOrDefault() != null)
            {
                item.PrevHash = Items.LastOrDefault()?.Hash;
            }
            item.Hash = item.MineHash(Diffiiculty);
            Items.Add(item);
        }

        public int Count => Items.Count;
        public IBlock this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }
        public List<IBlock> Items
        {
            get => _items;
            set => _items = value;
        }
        public byte[] Diffiiculty { get; }

        public IEnumerator<IBlock> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }

    public static class BlockChainExtension
    {
        public static byte[] GenerateHash(this IBlock block)
        {
            using (SHA512 sha = new SHA512Managed())
            using (MemoryStream st = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(st))
            {
                bw.Write(block.Version);
                bw.Write(block.BlockSize);
                bw.Write(block.MerkleRootHash);
                bw.Write(block.TimeStamp.ToBinary());
                bw.Write(block.PrevHash);
                bw.Write(block.Sign);
                var starr = st.ToArray();
                return sha.ComputeHash(starr);
            }
        }

        public static byte[] MineHash(this IBlock block, byte[] difficulty)
        {
            if (difficulty == null) throw new ArgumentNullException(nameof(difficulty));

            byte[] hash = new byte[0];
            int d = difficulty.Length;
            while (!hash.Take(2).SequenceEqual(difficulty))
            {
                block.Nonce++;
                hash = block.GenerateHash();
            }
            return hash;
        }


        public static bool IsValid(this IBlock block)
        {
            var bk = block.GenerateHash();
            return block.Hash.SequenceEqual(bk);
        }

        public static bool IsValidPrevBlock(this IBlock block, IBlock prevBlock)
        {
            if (prevBlock == null) throw new ArgumentNullException(nameof(prevBlock));

            var prev = prevBlock.GenerateHash();
            return prevBlock.IsValid() && block.PrevHash.SequenceEqual(prev);
        }
        public static bool IsValid(this IEnumerable<IBlock> items)
        {
            var enmerable = items.ToList();
            return enmerable.Zip(enmerable.Skip(1), Tuple.Create).All(block => block.Item2.IsValid()); // && block.Item2.Hash.)
        }
    }

    class test
    {
        public static void Run()
        {
            //Random rnd = new Random(DateTime.UtcNow.Millisecond);
            //IBlock genesis = new Block(new byte[] {0x00, 0x00, 0x00, 0x00, 0x00 });
            //byte[] difficulty = new byte[] { 0x00, 0x00 };

            //BlockChain chain = new BlockChain(difficulty, genesis);

            //for (int i = 0; i < 200; i++)
            //{
            //    var data = Enumerable.Range(0, 2256).Select(p => (byte)rnd.Next());
            //    chain.Add(new Block(data.ToArray()));
            //    Console.WriteLine(chain.LastOrDefault()?.ToString());

            //    Console.WriteLine($"Chain is Valid: {chain.IsValid()}");
            //}

            //Console.ReadLine();
        }
    }
}

    
