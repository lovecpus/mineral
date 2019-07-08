﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Mineral.Core.Database2.Core;

namespace Mineral.Common.Utils
{
    public class SessionOptional
    {
        #region Field
        private static SessionOptional instance = null;
        private ISession value = null;
        #endregion


        #region Property
        public static SessionOptional Instance
        {
            get { return instance ?? new SessionOptional(); }
        }
        #endregion


        #region Contructor
        #endregion


        #region Event Method
        #endregion


        #region Internal Method
        #endregion


        #region External Method
        [MethodImpl(MethodImplOptions.Synchronized)]
        public SessionOptional SetValue(ISession value)
        {
            if (this.value == null)
            {
                this.value = value;
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsValid()
        {
            return this.value != null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reset()
        {
            this.value?.Dispose();
            this.value = null;
        }
        #endregion
    }
}
