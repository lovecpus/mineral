﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mineral.Core
{
    public class ForkManager
    {
        public static bool IsForked(Block block)
        {
            // if same height and different hash than forked block.
            Block hasBlock = BlockChain.Instance.GetBlock(block.Height);
            if (hasBlock != null && block.Hash.Equals(hasBlock.Hash))
                return true;
            return false;
        }

        public static void MergeForked(Block block)
        {
            foreach(Transactions.Transaction tx in block.Transactions)
            {
                if (!tx.Verify()) 
                    continue;
                BlockChain.Instance.AddTransactionPool(tx);
            }
            Block branch = GetBranch(block);
            // broadcast merged blocks
        }

        private static Block GetBranch(Block block)
        {
            Block cblock = BlockChain.Instance.GetBlock(block.Height);
            Block cprev = BlockChain.Instance.GetBlock(cblock.Header.PrevHash);
            Block prev = BlockChain.Instance.GetBlock(block.Header.PrevHash);
            return block;
        }
    }
}
