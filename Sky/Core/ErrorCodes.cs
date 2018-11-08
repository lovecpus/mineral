﻿namespace Sky.Core
{
    public enum ERROR_CODES
    {
        E_NO_ERROR = 0,
        E_SYS_ERROR = 1,
        E_SYS_BLOCK_HEIGHT    = 2,

        E_SYS_EXIST_TRANSACTION = 100,

        E_TX_FROM_ADDRESS_INVALID = 1000,
        E_TX_FROM_ACCOUNT_INVALID,
        E_TX_SIGNATURE_INVALID,
        E_TX_NOT_ENOUGH_BALANCE,
        E_TX_SELF_TRANSFER_NOT_ALLOWED,
        E_TX_TOO_SMALL_TRANSFER_BALANCE,

        // lock
        E_TX_NOT_ENOUGH_LOCKBALANCE = 2000,
        E_TX_NO_LOCK_BALANCE,
        E_TX_LOCK_VALUE_CANNOT_NEGATIVE,
        E_TX_LOCK_TTL_NOT_ARRIVED,

        // vote
        E_TX_VOTE_TTL_NOT_ARRIVED = 2100,
        E_TX_VOTE_OVERCOUNT,

        // delegate
        E_TX_DELEGATE_NAME_INVALID = 2200,
        E_TX_DELEGATE_ALREADY_REGISTER,
    };
}
