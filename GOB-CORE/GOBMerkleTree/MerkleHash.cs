using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Cryptography;
using System.Linq;

namespace GOBMerkleTree
{
    public class MerkleHash
    {
        public const int HASH_LENGTH = 32;

        /// <summary>
        /// Hash 값
        /// </summary>
        public byte[] Value { get; set; }

        protected MerkleHash() { }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// buffer byte array로 Hash 값을 만든다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static MerkleHash Create(byte[] buffer)
        {
            MerkleHash hash = new MerkleHash();
            hash.ComputeHash(buffer);

            return hash;
        }

        /// <summary>
        /// buffer string으로 Hash 값을 만든다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static MerkleHash Create(string buffer)
        {
            return Create(Encoding.UTF8.GetBytes(buffer));
        }

        /// <summary>
        /// 왼쪽 Hash와 오른쪽 Hash를 연결하여 Hash값을 만든다.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static MerkleHash Create(MerkleHash left, MerkleHash right)
        {
            return Create(left.Value.Concat(right.Value).ToArray());
        }

        /// <summary>
        /// 같음 비교 연산자(==) 재정의
        /// </summary>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        public static bool operator ==(MerkleHash h1, MerkleHash h2)
        {
            return h1.Equals(h2);
        }

        /// <summary>
        /// 다름 연산자(!=) 재정의
        /// </summary>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        public static bool operator !=(MerkleHash h1, MerkleHash h2)
        {
            return !h1.Equals(h2);
        }

        public override bool Equals(object obj)
        {
            MerkleTree.Contract(() => obj is MerkleHash, "rvalue is not a MerkleHash");
            return Equals((MerkleHash)obj);
        }

        public override string ToString()
        {
            return BitConverter.ToString(Value).Replace("-", "");
        }

        public void ComputeHash(byte[] buffer)
        {
            SHA256 sha = SHA256.Create();
            SetHash(sha.ComputeHash(buffer));
        }

        /// <summary>
        /// Hash의 길이를 검증하고 전역변수 Value에 Hash값을 넣는다.
        /// </summary>
        /// <param name="hash"></param>
        public void SetHash(byte[] hash)
        {
            MerkleTree.Contract(() => hash.Length == HASH_LENGTH, "Unexpected hash length.");
            Value = hash;
        }

        public bool Equals(byte[] hash)
        {
            return Value.SequenceEqual(hash);
        }

        public bool Equals(MerkleHash hash)
        {
            bool ret = false;

            if (((object)hash) != null)
            {
                ret = Value.SequenceEqual(hash.Value);
            }

            return ret;
        }
    }
}
