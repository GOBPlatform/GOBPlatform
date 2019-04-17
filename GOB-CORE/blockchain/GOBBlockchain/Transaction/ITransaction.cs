using System;
using System.Collections.Generic;
using System.Text;

using Org.BouncyCastle.Math;

namespace GOBBlockchain.Transaction
{
    public interface ITransaction
    {
        /// <summary>
        /// Contains a hash of transaction*
        /// </summary>
        string TransactionId { get; set; }
        /// <summary>
        /// 송신자의 PublicKey
        /// </summary>
        string Sender { get; }
        /// <summary>
        /// 수신자의 PublicKey
        /// </summary>
        string Reciepient { get; }
        /// <summary>
        /// Contains the amount we wish to send to the recipient.
        /// </summary>
        float Value { get; }
        /// <summary>
        /// This is to prevent anybody else from spending funds in our wallet.
        /// </summary>
        BigInteger[] Signature { get; set; }
        /// <summary>
        /// A rough count of how many transactions have been generated 
        /// </summary>
        int Sequence { get; set; }
        List<TransactionInput> Inputs { get; set; }
        List<TransactionOutput> Outputs { get; set; }

        bool processTransaction();
        float getInputsValue();
        void generateSignature(string privateKey);
        bool verifySignature();
        float getOutputsValue();

    }
}
