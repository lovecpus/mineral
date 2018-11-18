﻿using Newtonsoft.Json.Linq;
using Mineral.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineral.Network.RPC.Command
{
    public partial class RpcProcessCommand
    {
        public static JObject ProcessTransaction(LocalNode node, byte[] transaction)
        {
            JObject json = new JObject();

            Transaction tx = Transaction.DeserializeFrom(transaction);
            if (tx != null)
            {
                if (tx.Verify() && tx.VerifyBlockchain())
                {
                    node.AddTransaction(tx);
                    json["transaction"] = tx.ToJson();
                }
                else
                {
                    json = RpcCommand.CreateErrorResult(null, (int)tx.TxResult, tx.TxResult.ToString());
                }
            }
            else
            {
                json = RpcCommand.CreateErrorResult(null, 0, "Invalid trasaction data");
            }

            return json;
        }

        public static JObject OnCadidateDelegates(object obj, JArray parameters)
        {
            JObject json = new JObject();
            json["cadidates"] = new JArray();
            List<DelegateState> list = Blockchain.Instance.storage.GetCadidateDelgates();
            foreach (DelegateState state in list)
            {
                JObject jstate = new JObject();
                jstate["address"] = state.AddressHash.ToString();
                jstate["name"] = Encoding.UTF8.GetString(state.Name); ;
                (json["cadidates"] as JArray).Add(jstate);
            }
            return json;
        }

        public static JObject OnGetTurnTable(object obj, JArray parameters)
        {
            JObject json = new JObject();
            json["TurnTable"] = new JArray();
            TurnTableState table = Blockchain.Instance.GetTurnTable(parameters[0].Value<int>());
            foreach (UInt160 hash in table.addrs)
            {
                DelegateState state = Blockchain.Instance.storage.GetDelegateState(hash);
                JObject jstate = new JObject();
                jstate["address"] = state.AddressHash.ToString();
                jstate["name"] = Encoding.UTF8.GetString(state.Name); ;
                (json["TurnTable"] as JArray).Add(jstate);
            }
            return json;
        }
    }
}