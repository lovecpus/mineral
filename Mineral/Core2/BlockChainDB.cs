﻿using Mineral.Core2.State;
using Mineral.Core2.Transactions;
using Mineral.Database.BlockChain;
using Mineral.Database.LevelDB;
using Mineral.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mineral.Core2
{
    public partial class BlockChain
    {
        #region Field
        //private LevelDBBlockChain _dbManager = null;
        private Manager _manager = new Manager();
        #endregion


        #region Property
        public Manager Manager { get { return _manager; } }
        #endregion


        #region Constructor
        #endregion


        #region Event Method
        #endregion


        #region Internal Method
        #endregion


        #region External Method
        public Storage NewStorage()
        {
            _manager.Chain.NewStorage();
            return _manager.Chain.Storage;
        }

        #region Block Header
        public BlockHeader GetHeader(uint height)
        {
            Block block = _manager.CacheBlocks.GetBlock(height);
            if (block != null)
                return block.Header;

            UInt256 hash = GetBlockHash(height);
            if (hash == null)
                return null;

            BlockState blockState = _manager.Chain.Storage.Block.Get(hash);
            return blockState != null ? blockState.Header : null;
        }

        public BlockHeader GetHeader(UInt256 hash)
        {
            Block block = _manager.CacheBlocks.GetBlock(hash);
            if (block != null)
                return block.Header;

            BlockState blockState = _manager.Chain.Storage.Block.Get(hash);
            return blockState != null ? blockState.Header : null;
        }

        public BlockHeader GetNextHeader(UInt256 hash)
        {
            BlockHeader header = GetHeader(hash);
            if (header == null)
                return null;

            return GetHeader(header.Height + 1);
        }

        public bool ContainsBlock(UInt256 hash)
        {
            return GetHeader(hash)?.Height <= _currentBlockHeight;
        }
        #endregion


        #region Block
        public Block GetBlock(uint height)
        {
            Block block = _manager.CacheBlocks.GetBlock(height);
            if (block != null)
                return block;

            UInt256 hash = GetBlockHash(height);
            if (hash == null)
                return null;

            return GetBlock(hash);
        }

        public Block GetBlock(UInt256 hash)
        {
            Block block = _manager.CacheBlocks.GetBlock(hash);
            if (block != null)
                return block;

            block = _manager.Chain.Storage.Block.Get(hash)?.GetBlock(p => _manager.Chain.Storage.Transaction.Get(p));
            return block;
        }

        public UInt256 GetBlockHash(uint height)
        {
            UInt256 hash = _manager.CacheBlocks.GetBlockHash(height);
            return hash;
        }

        public Block GetNextBlock(UInt256 hash)
        {
            Block block = GetBlock(hash);
            if (block == null)
                return null;

            return GetBlock(block.Height + 1);
        }

        public List<Block> GetBlocks(uint start, uint end)
        {
            var hashes = _manager.CacheBlocks.GetBlcokHashs(start, end);
            List<Block> blocks = new List<Block>();
            foreach (var hash in hashes)
            {
                Block block = GetBlock(hash);
                if (block == null)
                    break;
                blocks.Add(block);
            }
            return blocks;
        }
        #endregion


        #region Transaction
        public TransactionState GetTransaction(UInt256 hash)
        {
            return _manager.Chain.Storage.Transaction.Get(hash);
        }
        #endregion


        #region Account
        public AccountState GetAccountState(UInt160 hash)
        {
            return _manager.Chain.Storage.Account.GetAndChange(hash);
        }
        #endregion


        #region Turn table
        public TurnTableState GetTurnTable(uint height)
        {
            List<uint> heights = _manager.Chain.GetTurnTableHeightList(height).ToList();

            heights.Sort((a, b) => { return a > b ? (-1) : (a < b ? 1 : 0); });
            return _manager.Chain.GetTurnTable(heights.Count > 0 ? heights.First() : 0);
        }
        #endregion


        #region Delegate
        public DelegateState GetDelegateState(UInt160 hash)
        {
            return _manager.Chain.Storage.Delegate.Get(hash);
        }

        public List<DelegateState> GetDelegateStateAll()
        {
            return _manager.Chain.GetDelegateStateAll().ToList();
        }
        #endregion
        #endregion

    }
}