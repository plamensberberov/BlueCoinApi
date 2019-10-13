using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace BlueCoin.Blockchain
{
    public class Blockchain
    {
        public Blockchain()
        {
            this.PendingTransactions = new List<Transaction>();
            this.Chain = new List<Block>();
            this.Chain.Add(this.GenerateGenesisBlock());
        }
        public List<Transaction> PendingTransactions { get; private set; }
        public decimal MiningReward => 10m;
        public List<Block> Chain { get; private set; }
        public int Difficulty => 4;

        public Transaction AddTransaction(int sender, int receiver, decimal amount)
        {
            if (sender == receiver) throw new Exception ("Invalid Transaction");
            if (amount == 0m) throw new Exception ("Invalid Transaction");

            Transaction transaction = new Transaction(sender, receiver, amount);
            if (!transaction.IsValid()) throw new Exception("Invalid Transaction");

            this.PendingTransactions.Add(transaction);
            return transaction;
        }

        public bool MineFirstPendingTransaction(int minerId)
        {
            if (this.PendingTransactions.Count < 1) throw new Exception("No transactions to mine!");

            List<Transaction> minedTransactions = new List<Transaction>();
            minedTransactions.Add(PendingTransactions[0]);

            Block newBlock = new Block(DateTime.Now, minedTransactions, this.Chain.Last().Hash);
            newBlock.MineBlock(this.Difficulty);
            this.Chain.Add(newBlock);

            Transaction reward = new Transaction(0, minerId, MiningReward);
            PendingTransactions.Add(reward);

            return true;
        }

        private Block GenerateGenesisBlock()
        {
            List<Transaction> genesisTransactions = new List<Transaction>();
            Transaction transaction = new Transaction(-1, 0, 100);
            genesisTransactions.Add(transaction);

            Block genesis = new Block(DateTime.Now, genesisTransactions, new byte[] { 0 });

            return genesis;
        }
    }

    public class Block
    {
        public Block(DateTime time, List<Transaction> transactions, byte[] previous)
        {
            this.Time = time;
            this.Transactions = transactions;
            this.Previous = previous;

            this.Hash = GenerateHash();
        }

        public List<Transaction> Transactions { get; private set; } 
        public DateTime Time { get; private set; }
        public byte[] Hash { get; private set; }
        public byte[] Previous { get; private set; }

        public int Nonce { get; private set; }

        private byte[] GenerateHash()
        {
            List<byte> transactionSeed = new List<byte>();
            foreach(var transaction in this.Transactions)
            {
                transactionSeed.AddRange(transaction.Hash.AsEnumerable());
            }

            byte[] hashSeed = BitConverter.GetBytes(this.Time.ToBinary())
                .Concat(this.Previous.AsEnumerable())
                .Concat(transactionSeed)
                .ToArray();

            SHA256 sha = new SHA256Managed();

            return sha.ComputeHash(hashSeed);
        }

        public void MineBlock(int difficulty)
        {
            SHA256 sha = new SHA256Managed();
            string minedCheck = string.Empty;
            for (int k = 0; k < difficulty; k++)
                minedCheck = minedCheck.Insert(k ,"0");

            string hashString = string.Empty;
            for (int k = 0; k <= difficulty; k++)
                hashString = hashString.Insert(k, "1");

            for (int i = 1; hashString.Substring(0, difficulty) != minedCheck; i++)
            {
                this.Nonce = i;

                List<byte> transactionSeed = new List<byte>();
                foreach (var transaction in this.Transactions)
                {
                    transactionSeed.AddRange(transaction.Hash.AsEnumerable());
                }

                byte[] hashSeed = BitConverter.GetBytes(this.Time.ToBinary())
                .Concat(this.Previous.AsEnumerable())
                .Concat(transactionSeed)
                .Append((byte)Nonce)
                .ToArray();

                this.Hash = sha.ComputeHash(hashSeed);

                hashString = string.Empty;
                foreach (var currByte in this.Hash)
                {
                    hashString = hashString.Insert(hashString.Length, ((int)currByte).ToString("X2"));
                }
                Console.WriteLine("Mining block current hash:");
                Console.WriteLine(hashString);
            }
        }

        private bool AreTransactionsValid()
        {
            foreach(var transaction in this.Transactions)
            {
                if (!transaction.IsValid()) return false;
            }
            return true;
        }
    }
}
