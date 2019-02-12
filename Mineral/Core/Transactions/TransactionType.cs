﻿namespace Mineral.Core.Transactions
{
    public enum TransactionType : short
    {
        Supply = 1,
        Transfer,
        Vote,
        RegisterDelegate,
        OtherSign,
        Sign,
        Lock,
        Unlock,
    }
}
