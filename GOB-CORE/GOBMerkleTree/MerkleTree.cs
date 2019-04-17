using GOBCommon;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// https://github.com/cliftonm/MerkleTree/blob/master/MerkleTreeDemo/Demo.cs 참고
/// </summary>
namespace GOBMerkleTree
{
    public class MerkleTree
    {
        public MerkleNode RootNode { get; protected set; }

        protected List<MerkleNode> nodes;
        protected List<MerkleNode> leaves;

        public static void Contract(Func<bool> action, string msg)
        {
            if (!action())
            {
                throw new MerkleException(msg);
            }
        }

        public MerkleTree()
        {
            nodes = new List<MerkleNode>();
            leaves = new List<MerkleNode>();
        }

        /// <summary>
        /// 잎사귀 추가
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public MerkleNode AppendLeaf(MerkleNode node)
        {
            nodes.Add(node);
            leaves.Add(node);

            return node;
        }
        /// <summary>
        /// 해쉬로 노드를 만들어 추가하여 Node를 반환
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>

        public MerkleNode AppendLeaf(MerkleHash hash)
        {
            var node = CreateNode(hash);
            nodes.Add(node);
            leaves.Add(node);

            return node;
        }
        /// <summary>
        /// 잎사귀들 추가
        /// </summary>
        /// <param name="nodes"></param>
        public void AppendLeaves(MerkleNode[] nodes)
        {
            nodes.ForEach(n => AppendLeaf(n));
        }
        /// <summary>
        /// 입사귀들 추가하여 Node List를 반환
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public List<MerkleNode> AppendLeaves(MerkleHash[] hashes)
        {
            List<MerkleNode> nodes = new List<MerkleNode>();
            hashes.ForEach(h => nodes.Add(AppendLeaf(h)));

            return nodes;
        }
        /// <summary>
        /// 받은 트리로 입사귀들을 구성하여 Hash를 반환
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public MerkleHash AddTree(MerkleTree tree)
        {
            Contract(() => leaves.Count > 0, "Cannot add to a tree with no leaves.");
            tree.leaves.ForEach(l => AppendLeaf(l));

            return BuildTree();
        }

        public void FixOddNumberLeaves()
        {
            //Bit 연산시킨다. leaves가 1 일경우에만 아래 if문을 수행한다.
            if ((leaves.Count & 1) == 1)
            {
                var lastLeaf = leaves.Last();
                var l = AppendLeaf(lastLeaf.Hash);
                // l.Text = lastLeaf.Text;
            }
        }

        /// <summary>
        /// 머클트리를 만들고
        /// 루트 토드의 해쉬를 반환
        /// </summary>
        /// <returns></returns>
        public MerkleHash BuildTree()
        {
            Contract(() => leaves.Count > 0, "Cannot build a tree with no leaves.");
            BuildTree(leaves);

            return RootNode.Hash;
        }

        /// <summary>
        /// 루트해시를 만들어 가며 검사 증명 해시 리스트를 반환.
        /// </summary>
        /// <param name="leafHash"></param>
        /// <returns></returns>
        public List<MerkleProofHash> AuditProof(MerkleHash leafHash)
        {
            List<MerkleProofHash> auditTrail = new List<MerkleProofHash>();

            var leafNode = FindLeaf(leafHash);

            if (leafNode != null)
            {
                Contract(() => leafNode.Parent != null, "Expected leaf to have a parent.");
                var parent = leafNode.Parent;
                BuildAuditTrail(auditTrail, parent, leafNode);
            }

            return auditTrail;
        }

        /// <summary>
        /// 함수 이름이 일관적인 감사다..;;
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public List<MerkleProofHash> ConsistencyProof(int m)
        {
            // Rule 1:
            // 일관성 증명을 시작할 수 있는 트리의 가장 왼쪽 노드를 찾는다.
            // 이 노드의 입사귀 갯수를 k로 설정한다.
            List<MerkleProofHash> hashNodes = new List<MerkleProofHash>();
            int idx = (int)Math.Log(m, 2);

            // 맨 왼쪽 노드를 가져온다.
            MerkleNode node = leaves[0];

            // idx가 지정된 노드에 도달할 때까지 트리를 탐색.
            while (idx > 0)
            {
                node = node.Parent;
                --idx;
            }

            int k = node.Leaves().Count();
            hashNodes.Add(new MerkleProofHash(node.Hash, MerkleProofHash.Branch.OldRoot));

            if (m == k)
            {
                // Continue with Rule 3 -- the remainder is the audit proof
            }
            else
            {
                // Rule 2:
                // 초기 형제 노드 (SN)를 규칙 1에 의해 획득 된 노드의 형제로 설정합니다.
                // m-k == # SN의 잎의 # 경우 형제 SN의 해시를 연결하고 규칙 2를 종료하십시오. 이는 이전 루트의 해시를 나타냅니다.
                // m-k <SN의 잎수이면 SN을 왼쪽 자식 노드로 설정하고 규칙 2를 반복합니다.

                // sibling node:
                MerkleNode sn = node.Parent.RightNode;
                bool traverseTree = true;

                while (traverseTree)
                {
                    Contract(() => sn != null, "Sibling node must exist because m != k");
                    int sncount = sn.Leaves().Count();

                    if (m - k == sncount)
                    {
                        hashNodes.Add(new MerkleProofHash(sn.Hash, MerkleProofHash.Branch.OldRoot));
                        break;
                    }

                    if (m - k > sncount)
                    {
                        hashNodes.Add(new MerkleProofHash(sn.Hash, MerkleProofHash.Branch.OldRoot));
                        sn = sn.Parent.RightNode;
                        k += sncount;
                    }
                    else // (m - k < sncount)
                    {
                        sn = sn.LeftNode;
                    }
                }
            }

            // Rule 3: Apply ConsistencyAuditProof below.

            return hashNodes;
        }

        /// <summary>
        /// 일관성 증명의 마지막 노드를 사용하여 감사 증명으로 일관성 증명을 완료합니다.
        /// </summary>
        /// <param name="nodeHash"></param>
        /// <returns></returns>
        public List<MerkleProofHash> ConsistencyAuditProof(MerkleHash nodeHash)
        {
            List<MerkleProofHash> auditTrail = new List<MerkleProofHash>();

            var node = RootNode.Single(n => n.Hash == nodeHash);
            var parent = node.Parent;
            BuildAuditTrail(auditTrail, parent, node);

            return auditTrail;
        }

        public static bool VerifyAudit(MerkleHash rootHash, MerkleHash leafHash, List<MerkleProofHash> auditTrail)
        {
            Contract(() => auditTrail.Count > 0, "Audit trail cannot be empty.");
            MerkleHash testHash = leafHash;

            // TODO: Inefficient - compute hashes directly.
            foreach (MerkleProofHash auditHash in auditTrail)
            {
                testHash = auditHash.Direction == MerkleProofHash.Branch.Left ?
                    MerkleHash.Create(testHash.Value.Concat(auditHash.Hash.Value).ToArray()) :
                    MerkleHash.Create(auditHash.Hash.Value.Concat(testHash.Value).ToArray());
            }

            return rootHash == testHash;
        }

        /// <summary>
        /// For demo / debugging purposes, we return the pairs of hashes used to verify the audit proof.
        /// </summary>
        public static List<Tuple<MerkleHash, MerkleHash>> AuditHashPairs(MerkleHash leafHash, List<MerkleProofHash> auditTrail)
        {
            Contract(() => auditTrail.Count > 0, "Audit trail cannot be empty.");
            var auditPairs = new List<Tuple<MerkleHash, MerkleHash>>();
            MerkleHash testHash = leafHash;

            // TODO: Inefficient - compute hashes directly.
            foreach (MerkleProofHash auditHash in auditTrail)
            {
                switch (auditHash.Direction)
                {
                    case MerkleProofHash.Branch.Left:
                        auditPairs.Add(new Tuple<MerkleHash, MerkleHash>(testHash, auditHash.Hash));
                        testHash = MerkleHash.Create(testHash.Value.Concat(auditHash.Hash.Value).ToArray());
                        break;

                    case MerkleProofHash.Branch.Right:
                        auditPairs.Add(new Tuple<MerkleHash, MerkleHash>(auditHash.Hash, testHash));
                        testHash = MerkleHash.Create(auditHash.Hash.Value.Concat(testHash.Value).ToArray());
                        break;
                }
            }

            return auditPairs;
        }

        public static bool VerifyConsistency(MerkleHash oldRootHash, List<MerkleProofHash> proof)
        {
            MerkleHash hash, lhash, rhash;

            if (proof.Count > 1)
            {
                lhash = proof[proof.Count - 2].Hash;
                int hidx = proof.Count - 1;
                hash = rhash = MerkleTree.ComputeHash(lhash, proof[hidx].Hash);
                hidx -= 2;

                // foreach (var nextHashNode in proof.Skip(1))
                while (hidx >= 0)
                {
                    lhash = proof[hidx].Hash;
                    hash = rhash = MerkleTree.ComputeHash(lhash, rhash);

                    --hidx;
                }
            }
            else
            {
                hash = proof[0].Hash;
            }

            return hash == oldRootHash;
        }

        public static MerkleHash ComputeHash(MerkleHash left, MerkleHash right)
        {
            return MerkleHash.Create(left.Value.Concat(right.Value).ToArray());
        }

        protected void BuildAuditTrail(List<MerkleProofHash> auditTrail, MerkleNode parent, MerkleNode child)
        {
            if (parent != null)
            {
                Contract(() => child.Parent == parent, "Parent of child is not expected parent.");
                var nextChild = parent.LeftNode == child ? parent.RightNode : parent.LeftNode;
                var direction = parent.LeftNode == child ? MerkleProofHash.Branch.Left : MerkleProofHash.Branch.Right;

                // For the last leaf, the right node may not exist.  In that case, we ignore it because it's
                // the hash we are given to verify.
                if (nextChild != null)
                {
                    auditTrail.Add(new MerkleProofHash(nextChild.Hash, direction));
                }

                BuildAuditTrail(auditTrail, child.Parent.Parent, child.Parent);
            }
        }

        protected MerkleNode FindLeaf(MerkleHash leafHash)
        {
            // TODO: We can improve the search for the leaf hash by maintaining a sorted list of leaf hashes.
            // We use First because a tree with an odd number of leaves will duplicate the last leaf
            // and will therefore have the same hash.
            return leaves.FirstOrDefault(l => l.Hash == leafHash);
        }

        /// <summary>
        /// Reduce the current list of n nodes to n/2 parents.
        /// </summary>
        /// <param name="nodes"></param>
        protected void BuildTree(List<MerkleNode> nodes)
        {
            Contract(() => nodes.Count > 0, "node list not expected to be empty.");

            if (nodes.Count == 1)
            {
                RootNode = nodes[0];
            }
            else
            {
                List<MerkleNode> parents = new List<MerkleNode>();

                for (int i = 0; i < nodes.Count; i += 2)
                {
                    MerkleNode right = (i + 1 < nodes.Count) ? nodes[i + 1] : null;
                    // Constructing the MerkleNode resolves the right node being null.
                    MerkleNode parent = CreateNode(nodes[i], right);
                    parents.Add(parent);
                }

                BuildTree(parents);
            }
        }

        // Override in derived class to extend the behavior.
        // Alternatively, we could implement a factory pattern.

        protected virtual MerkleNode CreateNode(MerkleHash hash)
        {
            return new MerkleNode(hash);
        }

        protected virtual MerkleNode CreateNode(MerkleNode left, MerkleNode right)
        {
            return new MerkleNode(left, right);
        }
    }
}
