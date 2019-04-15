using GOBCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOBMerkleTree
{
    public class MerkleNode : IEnumerable<MerkleNode>
    {
        /// <summary>
        /// 이 노드가 갖고 있는 hash 값
        /// </summary>
        public MerkleHash Hash { get; protected set; }
        /// <summary>
        /// 이 노드의 왼쪽 하위 노드
        /// </summary>
        public MerkleNode LeftNode { get; protected set; }
        /// <summary>
        /// 이 노드의 오른쪽 하위 노드
        /// </summary>
        public MerkleNode RightNode { get; protected set; }
        /// <summary>
        /// 부모 노드
        /// </summary>
        public MerkleNode Parent { get; protected set; }
        /// <summary>
        /// 이 노드가 잎사귀 노드냐 여부를 판단한다.(좌,우 노드가 없다면 잎사귀 노드이다.)
        /// </summary>
        public bool IsLeaf { get { return LeftNode == null && RightNode == null; } }

        #region Constructor

        /// <summary>
        /// 생성자1
        /// </summary>
        public MerkleNode() { }
        /// <summary>
        /// 생성자2
        /// </summary>
        /// <param name="hash"></param>
        public MerkleNode(MerkleHash hash) { Hash = hash; }
        /// <summary>
        /// 생성자3
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public MerkleNode(MerkleNode left, MerkleNode right = null)
        {
            LeftNode = left; //왼쪽 노드 설정
            RightNode = right; //오른쪽 노드 설정
            LeftNode.Parent = this; //왼쪽노드의 부모를 이 노드로 설정
            RightNode.IfNotNull(r => r.Parent = this); //오른쪽노드의 부모를 이 노드로 설정
            ComputeHash();
        }

        #endregion

        public override string ToString()
        {
            return Hash.ToString();
        }

        public MerkleHash ComputeHash(byte[] buffer)
        {
            Hash = MerkleHash.Create(buffer);

            return Hash;
        }

        /// <summary>
        /// 입사귀들을 반환...
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MerkleNode> Leaves()
        {
            return this.Where(n => n.LeftNode == null && n.RightNode == null);
        }

        /// <summary>
        /// 왼쪽 노드를 설정한다.
        /// </summary>
        /// <param name="node"></param>
        public void SetLeftNode(MerkleNode node)
        {
            MerkleTree.Contract(() => node.Hash != null, "Node hash must be initialized.");
            LeftNode = node;
            LeftNode.Parent = this;
            ComputeHash();
        }

        /// <summary>
        /// 오른쪽 노드를 설정한다.
        /// </summary>
        /// <param name="node"></param>
        public void SetRightNode(MerkleNode node)
        {
            MerkleTree.Contract(() => node.Hash != null, "Node hash must be initialized.");
            RightNode = node;
            RightNode.Parent = this;

            // Can't compute hash if the left node isn't set yet.
            if (LeftNode != null)
            {
                ComputeHash();
            }
        }

        /// <summary>
        /// 검증할 수 있는 상태인지 확인
        /// </summary>
        /// <returns></returns>
        public bool CanVerifyHash()
        {
            //왼쪽, 오른쪽 노드 둘다 Null이 아니거나 왼쪽 노드만이라도 Null이 아니면 True
            return (LeftNode != null && RightNode != null) || (LeftNode != null);
        }

        /// <summary>
        /// 검증
        /// </summary>
        /// <returns></returns>
        public bool VerifyHash()
        {
            //왼쪽, 오른쪽 노드 둘다 Null이면 true
            if (LeftNode == null && RightNode == null)
            {
                return true;
            }
            //오른쪽 노드가 널이라면
            if (RightNode == null)
            {
                //부모의 해쉬와 왼쪽노드의 해쉬가 같다면 true
                return Hash.Equals(LeftNode.Hash);
            }

            MerkleTree.Contract(() => LeftNode != null, "Left branch must be a node if right branch is a node.");
            MerkleHash leftRightHash = MerkleHash.Create(LeftNode.Hash, RightNode.Hash);
            //왼쪽, 오른쪽 노드의 해쉬 Sum 시킨값이 이 노드의 Hash와 같다면 true.
            return Hash.Equals(leftRightHash);
        }

        /// <summary>
        /// 파라미터로 들어온 노드의 Hash와 이 노드의 hash를 비교한다.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Equals(MerkleNode node)
        {
            return Hash.Equals(node.Hash);
        }

        protected void ComputeHash()
        {
            Hash = RightNode == null ?
                LeftNode.Hash : //MerkleHash.Create(LeftNode.Hash.Value.Concat(LeftNode.Hash.Value).ToArray()) : 
                MerkleHash.Create(LeftNode.Hash.Value.Concat(RightNode.Hash.Value).ToArray());
            Parent?.ComputeHash();      // 부모가 있다면 부모노드로 가서 또 ComputeHash를 수행.
        }

        protected IEnumerable<MerkleNode> Iterate(MerkleNode node)
        {
            if (node.LeftNode != null)
            {
                foreach (var n in Iterate(node.LeftNode)) yield return n;
            }

            if (node.RightNode != null)
            {
                foreach (var n in Iterate(node.RightNode)) yield return n;
            }

            yield return node;
        }

        #region IEnumerable 인터페이스 구현

        public IEnumerator<MerkleNode> GetEnumerator()
        {
            foreach (var n in Iterate(this)) yield return n;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
