using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace GOBBlockchain.merkle
{
    interface IHashStore
    {
        void Append();
        void Flush();
        void Close();
        SHA256 GetHash(UInt32 pos);
    }

    class CompactMerkleTree
    {
        uint mintree_h;
        SHA256[] hashes;

        public CompactMerkleTree(UInt32 tree_size, SHA256[] hashes, FileHashStore store)
        {
            this.mintree_h = tree_size;
            this.hashes = hashes;

        }

        public SHA256[] Hashes()
        {
            return this.hashes;
        }

        public UInt32 TreeSize()
        {
            return UInt32.MaxValue;
        }

        //public byte[] Marshal()
        //{
        //    var length = 4 + hashes.Length * 32;
            
        //}
    }

    class FileHashStore
    {
        public string fileName;

        public FileHashStore(string name, UInt32 tree)
        {
            // OpenFile

            // store.CheckConsistence(tree_size)

            // getStoredHashNum


        }

        public void checkConsistence(UInt32 tree_size)
        {
            
        }

        //public Int64 getStoredHashNum(UInt32 tree_size)
        //{
        //    var uint = countBit(tree_size);
        //}
    }

    class Treehasher
    {
        //public  hash_empty()
        //{
        //    //SHA256 sha256 = SHA256.Create();

        //    //sha256.ComputeHash();

        //    //return 
        //}

        // public Uint256 
    }

    public class Uint256
    {
        const int UINT16_SIZE = 2;
        const int UINT32_SIZE = 4;
        const int UINT64_SIZE = 8;
        const int UINT256_SIZE = 32;

        byte[] UINT256 = new byte[UINT256_SIZE];
    }
}
