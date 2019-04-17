using System;
using System.Collections.Generic;
using System.Text;

using GOBCommon.Hellper;
using GOBCrypto;
using GOBBlockchain.Block;

using Org.BouncyCastle.Math;

namespace GOBBlockchain.Transaction
{
    [Serializable]
    public class Transaction : ITransaction
    {
        private static int _sequence = 0;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from">sender PublicKey</param>
        /// <param name="to">Reciepient PublicKey</param>
        /// <param name="value"></param>
        /// <param name="inputs"></param>
        public Transaction(string from, string to, float value, List<TransactionInput> inputs)
        {
            Sender = from;
            Reciepient = to;
            Value = value;
            Inputs = inputs;
        }

        #region ITransaction interface implements

        public string TransactionId { get; set; }

        public string Sender { get; }

        public string Reciepient { get; }

        public float Value { get; }

        public BigInteger[] Signature { get; set; }
        public int Sequence { get => _sequence; set => _sequence = value; }

        public List<TransactionInput> Inputs { get; set; }
        public List<TransactionOutput> Outputs { get; set; }

        public bool processTransaction()
        {
            if (!verifySignature())
            {
                Console.WriteLine("Transaction Signature failed to verify");
                return false;
            }

            //Input Transaction의 UTXO(Output Transaction)를 블록체인의 UTXO 딕셔너리로부터 셋팅한다.
            foreach (var i in Inputs)
            {
                BlockChain.UTXOs.TryGetValue(i.transactionOutputId, out i.UTOX);
            }

            //Input의 UTXO 값이 0.1f 보다 작으면 return false
            if(getInputsValue() < BlockChain.minimumTransaction)
            {
                Console.WriteLine("Transaction Inputs too small:" + getInputsValue().ToString());
                Console.WriteLine("Please enter the amount greater than" + BlockChain.minimumTransaction.ToString());
                return false;
            }

            //Transaction Output 생성
            float leftOver = getInputsValue() - Value;
            TransactionId = CalculateHash();
            Outputs.Add(new TransactionOutput(Reciepient, Value, TransactionId));
            Outputs.Add(new TransactionOutput(Sender, leftOver, TransactionId));

            foreach (var o in Outputs)
            {
                BlockChain.UTXOs.Add(o.id, o);
            }

            foreach (var i in Inputs)
            {
                if (i.UTOX == null) continue;
                BlockChain.UTXOs.Remove(i.UTOX.id);
            }

            return true;
        }

        public void generateSignature(string privateKey)
        {
            string data = Sender + Reciepient + Value.ToString();
            Signature = Crypto.GetInstance().GenerateSignature(data, privateKey);
        }

        /// <summary>
        /// Input Transaction의 UTXO(Output Transaction)의 모든 Value 값을 더한다.
        /// </summary>
        /// <returns></returns>
        public float getInputsValue()
        {
            float total = 0;
            foreach (var i in Inputs)
            {
                if (i.UTOX == null) continue;
                total += i.UTOX.value;
            }
            return total;
        }

        public float getOutputsValue()
        {
            float total = 0;
            foreach (var o in Outputs)
            {
                total += o.value;
            }
            return total;
        }

        public bool verifySignature()
        {
            string data = Sender + Reciepient + Value.ToString();
            return Crypto.GetInstance().VerifySignature(Sender, data, Signature);
        }

        #endregion

        private string CalculateHash()
        {
            _sequence++;
            return ExtSHA256.StringHash(Sender + Reciepient + Value.ToString());
        }
    }
}
