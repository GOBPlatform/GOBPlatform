using System;
using System.Collections.Generic;
using System.Text;

using GOBCommon.Hellper;

namespace GOBBlockchain.Transaction
{
    class TransactionOutput
    {
        public string id;
        public string reciepient;
        public float value;
        public string parentTransactionId;

        public TransactionOutput(string reciepient, float value, string parentTransactionId)
        {
            this.reciepient = reciepient;
            this.value = value;
            this.parentTransactionId = parentTransactionId;
            StringBuilder sb = new StringBuilder(reciepient);
            sb.Append(value.ToString());
            sb.Append(parentTransactionId);
            this.id = ExtSHA256.StringHash(sb.ToString());//reciepient + value + parentTransactionId 로 해쉬 256;
        }

        public bool IsMine(string publicKey)
        {
            return publicKey == reciepient;
        }
    }
}
