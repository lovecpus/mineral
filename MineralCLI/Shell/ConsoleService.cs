﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Mineral.CommandLine.Attributes;
using Mineral.Core.Net.RpcHandler;
using MineralCLI.Commands;
using static MineralCLI.Commands.BaseCommand;

namespace MineralCLI.Shell
{
    public class ConsoleService : ConsoleServiceBase, IDisposable
    {
        #region Field
        private Dictionary<string, CommandHandler> commands = new Dictionary<string, CommandHandler>()
        {
            // WalletCommands
            { RpcCommandType.ImportWallet, new CommandHandler(WalletCommand.ImportWallet) },
            { RpcCommandType.BackupWallet, new CommandHandler(WalletCommand.BackupWallet) },
            { RpcCommandType.RegisterWallet, new CommandHandler(WalletCommand.RegisterWallet) },
            { RpcCommandType.Login, new CommandHandler(WalletCommand.Login) },
            { RpcCommandType.Logout, new CommandHandler(WalletCommand.Logout) },
            { RpcCommandType.GetAddress, new CommandHandler(WalletCommand.GetAddress) },
            { RpcCommandType.GetBalance, new CommandHandler(WalletCommand.GetBalance) },
            { RpcCommandType.GetAccount, new CommandHandler(WalletCommand.GetAccount) },

            // TransactionCommands
            { RpcCommandType.CreateAccount, new CommandHandler(TransactionCommand.CreateAccount) },
            { RpcCommandType.CreateProposal, new CommandHandler(TransactionCommand.CreateProposal) },
            { RpcCommandType.CreateWitness, new CommandHandler(TransactionCommand.CreateWitness) },
            { RpcCommandType.UpdateAccount, new CommandHandler(TransactionCommand.UpdateAccount) },
            { RpcCommandType.UpdateWitness, new CommandHandler(TransactionCommand.UpdateWitness) },
            { RpcCommandType.UpdateAsset, new CommandHandler(TransactionCommand.UpdateAsset) },
            { RpcCommandType.UpdateEnergyLimit, new CommandHandler(TransactionCommand.UpdateEnergyLimit) },
            { RpcCommandType.UpdateAccountPermission, new CommandHandler(TransactionCommand.UpdateAccountPermission) },
            { RpcCommandType.UpdateSetting, new CommandHandler(TransactionCommand.UpdateSetting) },
            { RpcCommandType.DeleteProposal, new CommandHandler(TransactionCommand.DeleteProposal) },
            { RpcCommandType.SendCoin, new CommandHandler(TransactionCommand.SendCoin) },
            { RpcCommandType.FreezeBalance, new CommandHandler(TransactionCommand.FreezeBalance) },
            { RpcCommandType.UnfreezeBalance, new CommandHandler(TransactionCommand.UnFreezeBalance) },
            { RpcCommandType.VoteWitness, new CommandHandler(TransactionCommand.VoteWitness) },
            { RpcCommandType.WithdrawBalance, new CommandHandler(TransactionCommand.WithdrawBalance) },

            // BlockCommands
            { RpcCommandType.GetBlock, new CommandHandler(BlockCommand.GetBlock) },
            { RpcCommandType.GetBlockByLatestNum, new CommandHandler(BlockCommand.GetBlockByLatestNum) },
            { RpcCommandType.GetBlockById, new CommandHandler(BlockCommand.GetBlockById) },
            { RpcCommandType.GetBlockByLimitNext, new CommandHandler(BlockCommand.GetBlockByLimitNext) },

            // AssetIssueCommands
            { RpcCommandType.AssetIssueByAccount, new CommandHandler(AssetIssueCommand.AssetIssueByAccount) },
            { RpcCommandType.AssetIssueById, new CommandHandler(AssetIssueCommand.AssetIssueById) },
            { RpcCommandType.AssetIssueByName, new CommandHandler(AssetIssueCommand.AssetIssueByName) },
            { RpcCommandType.AssetIssueListByName, new CommandHandler(AssetIssueCommand.AssetIssueListByName) },
            { RpcCommandType.TransferAsset, new CommandHandler(AssetIssueCommand.TransferAsset) },
            { RpcCommandType.UnfreezeAsset, new CommandHandler(AssetIssueCommand.UnfreezeAsset) }
        };
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
        public override bool OnCommand(string[] parameters)
        {
            string command = parameters[0].ToLower();
            return commands.ContainsKey(command) ? commands[command](parameters) : base.OnCommand(parameters);
        }

        public override void OnHelp(string[] parameters)
        {
            string message =
                Config.Instance.GetVersion()

                + "\n"
                + "\n" + "".PadLeft(0) + "COMMAND : "
                //+ "\n" + "".PadLeft(1) + "BLOCK COMMAND :"
                ;

            foreach (FieldInfo info in typeof(RpcCommandType).GetFields())
            {
                CommandLineAttribute attr = (CommandLineAttribute)info.GetCustomAttribute(typeof(CommandLineAttribute));
                if (attr != null)
                {
                    message += "\n" + "".PadLeft(4);
                    message += string.Format("{0,-25} {1}", attr.Name, attr.Description);
                }
            }

            //message += ""
            //    + "\n"
            //    + "\n" + "".PadLeft(1) + "NODE COMMAND : "
            //    ;
            //foreach (FieldInfo info in typeof(RpcCommand.Node).GetFields())
            //{
            //    CommandLineAttribute attr = (CommandLineAttribute)info.GetCustomAttribute(typeof(CommandLineAttribute));
            //    if (attr != null)
            //    {
            //        message += "\n" + "".PadLeft(4);
            //        message += string.Format("{0,-25} {1}", attr.Name, attr.Description);
            //    }
            //}

            //message += ""
            //    + "\n"
            //    + "\n" + "".PadLeft(1) + "WALLET COMMAND :"
            //    ;
            //foreach (FieldInfo info in typeof(RpcCommand.Wallet).GetFields())
            //{
            //    CommandLineAttribute attr = (CommandLineAttribute)info.GetCustomAttribute(typeof(CommandLineAttribute));
            //    if (attr != null)
            //    {
            //        message += "\n" + "".PadLeft(4);
            //        message += string.Format("{0,-25} {1}", attr.Name, attr.Description);
            //    }
            //}

            message += ""
                + "\n"
                + "\n" + "".PadLeft(0) + "MISC OPTION :"
                + "\n" + "".PadLeft(4) + BaseCommand.HelpCommandOption.Help;

            Console.WriteLine(message);
        }

        public new void Dispose()
        {
            commands.Clear();
            base.Dispose();
        }

        #endregion



    }
}
