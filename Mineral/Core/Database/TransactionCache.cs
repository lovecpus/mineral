﻿using System;
using System.Collections.Generic;
using System.Text;
using Mineral.Core.Capsule;
using Mineral.Core.Database2.Common;

namespace Mineral.Core.Database
{
    public class TransactionCache : MineralStoreWithRevoking<BytesCapsule, object>
    {
        #region Field
        #endregion


        #region Property
        #endregion


        #region Contructor
        public TransactionCache(IRevokingDatabase revoking_database, string db_name = "trans-cache")
            : base(revoking_database, db_name, typeof(TxCacheDB))
        {
        }
        #endregion


        #region Event Method
        #endregion


        #region Internal Method
        #endregion


        #region External Method
        #endregion
    }
}
