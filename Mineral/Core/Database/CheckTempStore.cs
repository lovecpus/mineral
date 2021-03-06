﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mineral.Core.Database
{
    public class CheckTempStore : MineralDatabase<byte[]>
    {
        #region Field
        private static CheckTempStore instance = null;
        #endregion


        #region Property
        public static CheckTempStore Instance
        {
            get { return instance = instance ?? new CheckTempStore(); }
        }
        #endregion


        #region Constructor
        private CheckTempStore() : base("tmp") {}
        #endregion


        #region Event Method
        #endregion


        #region Internal Method
        #endregion


        #region External Method
        public override bool Contains(byte[] key)
        {
            return false;
        }

        public override void Delete(byte[] key)
        {
        }

        public override byte[] Get(byte[] key)
        {
            return null;
        }

        public override void Put(byte[] key, byte[] value)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
