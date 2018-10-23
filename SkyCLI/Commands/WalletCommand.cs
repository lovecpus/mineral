﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Sky;
using Sky.Core;
using Sky.Cryptography;
using Sky.Network.RPC.Command;
using Sky.Wallets;
using SkyCLI.Network;

namespace SkyCLI.Commands
{
    public class WalletCommand : BaseCommand
    {
        public static bool OnCreateAccount(string[] parameters)
        {
            string[] usage = new string[] { string.Format(
                "{0} [command option] <path>\n"
                , RpcCommand.Wallet.CreateAccount) };
            string[] command_option = new string[] { HelpCommandOption.Help };

            if (parameters.Length == 1 || parameters.Length > 3)
            {
                OutputHelpMessage(usage, null, command_option, null);
                return true;
            }

            int index = 1;
            if (parameters.Length > index)
            {
                string option = parameters[index];
                if (option.ToLower().Equals("-help") || option.ToLower().Equals("-h"))
                {
                    OutputHelpMessage(usage, null, command_option, null);
                    index++;
                    return true;
                }
            }

            WalletAccount account = WalletAccount.CreateAccount();

            JObject json = new JObject();
            json["address"] = account.Address;
            json["addresshash"] = account.AddressHash.ToArray();
            json["privatekey"] = account.Key.PrivateKey.D.ToByteArray();
            json["publickey"] = account.Key.PublicKey.ToByteArray();

            string path = parameters[1].Contains(".json") ? parameters[1] : parameters[1] + ".json";
            using (var file = File.CreateText(path))
            {
                file.Write(json);
                file.Flush();
            }

            Program.Wallet = account;
            Console.WriteLine(json.ToString());

            return true;
        }

        public static bool OnOpenAccount(string[] parameters)
        {
            string[] usage = new string[] { string.Format(
                "{0} [command option] <path>\n"
                , RpcCommand.Wallet.OpenAccount) };
            string[] command_option = new string[] { HelpCommandOption.Help };;

            if (parameters.Length == 1 || parameters.Length > 3)
            {
                OutputHelpMessage(usage, null, command_option, null);
                return true;
            }

            int index = 1;
            if (parameters.Length > index)
            {
                string option = parameters[index];
                if (option.ToLower().Equals("-help") || option.ToLower().Equals("-h"))
                {
                    OutputHelpMessage(usage, null, command_option, null);
                    index++;
                    return true;
                }
            }

            string path = parameters[1].Contains(".json") ? parameters[1] : parameters[1] + ".json";
            if (!File.Exists(path))
            {
                Console.WriteLine(string.Format("Not found file : [0]", path));
                return true;
            }

            JObject json;
            using (var file = File.OpenText(path))
            {
                string data = file.ReadToEnd();
                json = JObject.Parse(data);
            }

            JToken key;
            if (json.TryGetValue("privatekey", out key))
            {
                Program.Wallet = new Sky.Wallets.WalletAccount(key.ToObject<byte[]>());
            }

            string message = Program.Wallet != null ?
                                string.Format("Address : {0}", Program.Wallet.Address.ToString()) : "Load fail to wallet account";
            Console.WriteLine(message);

            return true;
        }

        public static bool OnCloseAccount(string[] parameters)
        {
            Program.Wallet = null;
            return true;
        }

        public static bool OnGetAccount(string[] parameters)
        {
            JObject obj = MakeCommand(Config.BlockVersion, RpcCommand.Wallet.GetAccount, new JArray());
            obj = RcpClient.RequestPostAnsyc(Program.url, obj.ToString()).Result;

            return true;
        }

        public static bool OnGetAddress(string[] parameters)
        {
            //JObject obj = MakeCommand(Config.BlockVersion, RpcCommand.Wallet.GetAddress, new JArray());
            //obj = RcpClient.RequestPostAnsyc(Program.url, obj.ToString()).Result;

            //WalletAccount account = new WalletAccount(Sky.Cryptography.Helper.SHA256(Encoding.Default.GetBytes(parameters[1])));
            //JObject json = new JObject();
            //json["address"] = account.Address;
            //json["addresshash"] = account.AddressHash.ToArray();
            //json["privatekey"] = account.Key.PrivateKey.D.ToByteArray();
            //json["publickey"] = account.Key.PublicKey.ToByteArray();


            //string path = parameters[1].Contains(".json") ? parameters[1] : parameters[1] + ".json";
            //using (var file = File.CreateText(path))
            //{
            //    file.Write(json);
            //    file.Flush();
            //}

            //Console.WriteLine(json.ToString());

            return true;
        }

        public static bool OnGetBalance(string[] parameters)
        {
            if (Program.Wallet == null)
            {
                Console.WriteLine("Not loaded wallet account");
                return true;
            }

            string[] usage = new string[] { string.Format(
                "{0} [command option]\n"
                , RpcCommand.Wallet.GetBalance) };
            string[] command_option = new string[] { HelpCommandOption.Help };;

            if (parameters.Length > 2)
            {
                OutputHelpMessage(usage, null, command_option, null);
                return true;
            }

            int index = 1;
            if (parameters.Length > index)
            {
                string option = parameters[index];
                if (option.ToLower().Equals("-help") || option.ToLower().Equals("-h"))
                {
                    OutputHelpMessage(usage, null, command_option, null);
                    index++;
                    return true;
                }
            }

            JArray param = new JArray() { Program.Wallet.AddressHash.ToString() };
            SendCommand(Config.BlockVersion, RpcCommand.Wallet.GetBalance, param);

            return true;
        }

        public static bool OnSendTo(string[] parameters)
        {
            if (Program.Wallet == null)
            {
                Console.WriteLine("Not loaded wallet account");
                return true;
            }

            string[] usage = new string[] { string.Format(
                    "{0} [command option] <to address> <balance>\n"
                    , RpcCommand.Wallet.SendTo) };
            string[] command_option = new string[] { HelpCommandOption.Help };

            if (parameters.Length == 1 || parameters.Length > 4)
            {
                OutputHelpMessage(usage, null, command_option, null);
                return true;
            }

            int index = 1;
            if (parameters.Length > index)
            {
                string option = parameters[index];
                if (option.ToLower().Equals("-help") || option.ToLower().Equals("-h"))
                {
                    OutputHelpMessage(usage, null, command_option, null);
                    index++;
                    return true;
                }
            }

            UInt160 to_address = WalletAccount.ToAddressHash(parameters[1]);
            Fixed8 value = Fixed8.Parse(parameters[2]);

            TransferTransaction trans = new TransferTransaction()
            {
                From = Program.Wallet.AddressHash,
                To = new Dictionary<UInt160, Fixed8> { { to_address, value } }
            };

            Transaction tx = new Transaction(eTransactionType.TransferTransaction, DateTime.UtcNow.ToTimestamp(), trans);
            tx.Sign(Program.Wallet);

            JArray param = new JArray(tx.ToArray());
            SendCommand(Config.BlockVersion, RpcCommand.Wallet.SendTo, param);

            return true;
        }

        public static bool OnLockBalance(string[] parameters)
        {
            if (Program.Wallet == null)
            {
                Console.WriteLine("Not loaded wallet account");
                return true;
            }

            string[] usage = new string[] { string.Format(
                    "{0} [command option] <balance>\n"
                    , RpcCommand.Wallet.LockBalance) };
            string[] command_option = new string[] { HelpCommandOption.Help };

            if (parameters.Length == 1 || parameters.Length > 3)
            {
                OutputHelpMessage(usage, null, command_option, null);
                return true;
            }

            int index = 1;
            if (parameters.Length > index)
            {
                string option = parameters[index];
                if (option.ToLower().Equals("-help") || option.ToLower().Equals("-h"))
                {
                    OutputHelpMessage(usage, null, command_option, null);
                    index++;
                    return true;
                }
            }

            Fixed8 value = Fixed8.Parse(parameters[1]);

            LockTransaction trans = new LockTransaction()
            {
                From = Program.Wallet.AddressHash,
                LockValue = value
            };

            Transaction tx = new Transaction(eTransactionType.LockTransaction, DateTime.UtcNow.ToTimestamp(), trans);
            tx.Sign(Program.Wallet);

            JArray param = new JArray(tx.ToArray());
            SendCommand(Config.BlockVersion, RpcCommand.Wallet.LockBalance, param);

            return true;
        }

        public static bool OnUnlockBalance(string[] parameters)
        {
            if (Program.Wallet == null)
            {
                Console.WriteLine("Not loaded wallet account");
                return true;
            }

            string[] usage = new string[] { string.Format(
                    "{0} [command option]\n"
                    , RpcCommand.Wallet.UnlockBalance) };
            string[] command_option = new string[] { HelpCommandOption.Help };

            if (parameters.Length > 2)
            {
                OutputHelpMessage(usage, null, command_option, null);
                return true;
            }

            int index = 1;
            if (parameters.Length > index)
            {
                string option = parameters[index];
                if (option.ToLower().Equals("-help") || option.ToLower().Equals("-h"))
                {
                    OutputHelpMessage(usage, null, command_option, null);
                    index++;
                    return true;
                }
            }

            UnlockTransaction trans = new UnlockTransaction()
            {
                From = Program.Wallet.AddressHash
            };

            Transaction tx = new Transaction(eTransactionType.UnlockTransaction, DateTime.UtcNow.ToTimestamp(), trans);
            tx.Sign(Program.Wallet);

            JArray param = new JArray(tx.ToArray());
            SendCommand(Config.BlockVersion, RpcCommand.Wallet.UnlockBalance, param);

            return true;
        }

        public static bool OnVoteWitness(string[] parameters)
        {
            if (Program.Wallet == null)
            {
                Console.WriteLine("Not loaded wallet account");
                return true;
            }

            string[] usage = new string[] { string.Format(
                    "{0} [command option] <to address> <vote balance> <to address> <vote balance> ...\n"
                    , RpcCommand.Wallet.VoteWitness) };
            string[] command_option = new string[] { HelpCommandOption.Help };

            if (parameters.Length == 1)
            {
                OutputHelpMessage(usage, null, command_option, null);
                return true;
            }

            int index = 1;
            if (parameters.Length > index)
            {
                string option = parameters[index];
                if (option.ToLower().Equals("-help") || option.ToLower().Equals("-h"))
                {
                    OutputHelpMessage(usage, null, command_option, null);
                    index++;
                    return true;
                }
            }

            Dictionary<UInt160, Fixed8> votes = new Dictionary<UInt160, Fixed8>();
            for (int i = 0; i < parameters.Length - 1; i+=2)
                votes.Add(WalletAccount.ToAddressHash(parameters[i]), Fixed8.Parse(parameters[i]));

            VoteTransaction trans = new VoteTransaction()
            {
                From = Program.Wallet.AddressHash,
                Votes = votes
            };

            Transaction tx = new Transaction(eTransactionType.VoteTransaction, DateTime.UtcNow.ToTimestamp(), trans);
            tx.Sign(Program.Wallet);

            JArray param = new JArray(tx.ToArray());
            SendCommand(Config.BlockVersion, RpcCommand.Wallet.VoteWitness, param);

            return true;
        }
    }
}