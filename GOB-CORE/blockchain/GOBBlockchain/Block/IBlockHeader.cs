using System;
using System.Collections.Generic;
using System.Text;

namespace GOBBlockchain.Block
{
    public interface IBlockHeader
    {
        /// <summary>
        /// Block Structure 의 버전
        /// </summary>
        string Version { get; }
        /// <summary>
        /// 블록의 길이
        /// </summary>
        int BlockSize { get; }
        /// <summary>
        /// 이전 블록의 해쉬값
        /// </summary>
        byte[] PrevHash { get; set; }
        /// <summary>
        /// 블록의 생성 시각
        /// </summary>
        DateTime TimeStamp { get; }
        /// <summary>
        /// 머클 루트에 대한 해쉬값
        /// </summary>
        byte[] MerkleRootHash { get; set; }
        /// <summary>
        /// Block 생성자의 공개키
        /// </summary>
        string Sign { get; }
    }
}
