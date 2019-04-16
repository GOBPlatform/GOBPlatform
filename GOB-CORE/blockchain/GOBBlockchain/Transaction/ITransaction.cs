using System;
using System.Collections.Generic;
using System.Text;

namespace GOBBlockchain.Transaction
{
    public class ITransaction
    {
        string TransactionId { get; set; }
        string Sender { get; }
        string Reciepient { get; }
        float value { get; }

    }
}
