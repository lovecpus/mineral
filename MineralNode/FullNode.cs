﻿using System;
using System.Collections.Generic;
using System.Text;
using Mineral;
using Mineral.Core;
using Mineral.Core.Config.Arguments;

namespace MineralNode
{
    public class FullNode
    {
        #region Field
        #endregion


        #region Property
        #endregion


        #region Constructor
        #endregion


        #region Event Method
        #endregion


        #region Internal Method
        #endregion


        #region External Method
        public void Run(string[] args)
        {
            Logger.Info("Full node start.");
            Args.Instance.SetParam(args, DefineParameter.CONF_FILE);


        }
        #endregion
    }
}
