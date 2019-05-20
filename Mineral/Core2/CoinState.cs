﻿using System;

namespace Mineral.Core2
{
    [Flags]
    public enum CoinState : byte
    {
        Unconfirmed = 0,
        Confirmed = 1 << 0,
        Spent = 1 << 1,
        Claimed = 1 << 3,
        Frozen = 1 << 5,
    }
}