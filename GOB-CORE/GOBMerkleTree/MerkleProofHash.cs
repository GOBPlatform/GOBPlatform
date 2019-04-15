using System;
using System.Collections.Generic;
using System.Text;

namespace GOBMerkleTree
{
    public class MerkleProofHash
    {
        public enum Branch
        {
            Left,
            Right,
            OldRoot,    // 일관성 증명에서 이전 루트를 계산하기 위해 선형 목록의 해시에 사용됩니다.
        }

        public MerkleHash Hash { get; protected set; }
        public Branch Direction { get; protected set; }

        public MerkleProofHash(MerkleHash hash, Branch direction)
        {
            Hash = hash;
            Direction = direction;
        }

        public override string ToString()
        {
            return Hash.ToString();
        }
    }
}
