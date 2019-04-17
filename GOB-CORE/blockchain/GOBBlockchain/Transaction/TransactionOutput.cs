using System;
using System.Collections.Generic;
using System.Text;

using GOBCommon.Hellper;

namespace GOBBlockchain.Transaction
{
    [Serializable]
    public class TransactionOutput
    {
        public string id;
        /// <summary>
        /// 수신자의 PublicKey
        /// </summary>
        public string reciepient;
        public float value;
        public string parentTransactionId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reciepient">수신자 PublicKey 값</param>
        /// <param name="value"></param>
        /// <param name="parentTransactionId"></param>
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
