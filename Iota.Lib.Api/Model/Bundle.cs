﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Iota.Lib.Exception;
using Iota.Lib.Utils;

namespace Iota.Lib.Model
{
    /// <summary>
    /// Represents a bundle
    /// </summary>
    public class Bundle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bundle"/>
        /// </summary>
        public Bundle(List<Transaction> transactions = null)
        {
            if(transactions != null)
            {
                Transactions = transactions;
            }
            else
            {
                Transactions = new List<Transaction>();
            }
        }
        
        /// <summary>
        /// The transactions of the bundle (input-, output- and meta transactions)
        /// </summary>
        public List<Transaction> Transactions { get; set; }

        /// <summary>
        /// A 81-tryte string that respresents the bundle hash
        /// </summary>
        public string BundleHash { get; set; }

        /// <summary>Adds a bundle entry</summary>
        /// <param name="transaction">The transaction</param>
        public void AddEntry(Transaction transaction)
        {
            if(!InputValidator.IsValidTransaction(transaction))
            {
                throw new InvalidTransactionException();
            }

            Transactions.Add(transaction);
        }

        /// <summary>Adds multiple bundle entries</summary>
        /// <param name="transactions">The transactions</param>
        public void AddEntries(List<Transaction> transactions)
        {
            if (!InputValidator.IsArrayOfValidTransactions(transactions))
            {
                throw new InvalidTransactionException();
            }

            Transactions.AddRange(transactions);
        }

        /// <summary>
        /// Adds the trytes.
        /// </summary>
        /// <param name="signatureFragments">The signature fragments.</param>
        public void AddTrytes(List<string> signatureFragments)
        {
            string emptySignatureFragment = String.Empty;

            for (int j = 0; emptySignatureFragment.Length < Constants.SIGNATURE_MESSAGE_LENGTH; j++)
            {
                emptySignatureFragment += "9";
            }

            for (int i = 0; i < Transactions.Count; i++)
            {
                Transaction transaction = Transactions[i];

                // Fill empty signatureMessageFragment
                transaction.SignatureMessageFragment = ((signatureFragments.Count <= i || string.IsNullOrEmpty(signatureFragments[i]))
                    ? emptySignatureFragment
                    : signatureFragments[i]);
                // Fill empty trunkTransaction
                transaction.TrunkTransaction = Constants.EMPTY_HASH;

                // Fill empty branchTransaction
                transaction.BranchTransaction = Constants.EMPTY_HASH;

                // Fill empty nonce
                transaction.Nonce = Constants.EMPTY_HASH;
            }
        }

        /// <summary>
        /// Normalizes the bundle
        /// </summary>
        /// <param name="bundleHash">The bundle hash</param>
        /// <returns></returns>
        public int[] NormalizeBundle(string bundleHash)
        {
            int[] normalizedBundle = new int[81];

            for (int i = 0; i < 3; i++)
            {
                long sum = 0;
                for (int j = 0; j < 27; j++)
                {
                    sum += (normalizedBundle[i*27 + j] = Converter.ConvertTritsToInteger(Converter.ConvertTrytesToTrits("" + bundleHash[i*27 + j])));
                }

                if (sum >= 0)
                {
                    while (sum-- > 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i*27 + j] > -13)
                            {
                                normalizedBundle[i*27 + j]--;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    while (sum++ < 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i*27 + j] < 13)
                            {
                                normalizedBundle[i*27 + j]++;
                                break;
                            }
                        }
                    }
                }
            }

            return normalizedBundle;
        }

        /// <summary>
        /// Calculates the bundle hash using <see cref="Kerl"/> and fills it into all transactions
        /// </summary>
        public void FinalizeBundle()
        {
            //SetIndexes();
            //CreateAndAssignBundleHash();
            int[] normalizedBundleValue;
            int[] obsoleteTagTrits = new int[81];
            String hashInTrytes;
            bool valid = true;
            Kerl kerl = new Kerl();
            do
            {
                kerl.Reset();

                for (int i = 0; i < Transactions.Count; i++)
                {

                    int[] valueTrits = ArrayUtils.PadArrayWithZeros(Converter.ConvertBigIntToTrits(Transactions[i].Value), 81);

                    int[] timestampTrits = ArrayUtils.PadArrayWithZeros(Converter.ConvertBigIntToTrits(Transactions[i].Timestamp), 27);

                    Transactions[i].CurrentIndex = i;

                    int[] currentIndexTrits = ArrayUtils.PadArrayWithZeros(Converter.ConvertBigIntToTrits(Transactions[i].CurrentIndex), 27);

                    Transactions[i].LastIndex = Transactions.Count - 1;

                    int[] lastIndexTrits = ArrayUtils.PadArrayWithZeros(Converter.ConvertBigIntToTrits(Transactions[i].LastIndex), 27);

                    int[] t = Converter.ConvertTrytesToTrits(Transactions[i].Address + Converter.ConvertTritsToTrytes(valueTrits) + Transactions[i].ObsoleteTag + Converter.ConvertTritsToTrytes(timestampTrits) + Converter.ConvertTritsToTrytes(currentIndexTrits) + Converter.ConvertTritsToTrytes(lastIndexTrits));

                    kerl.Absorb(t);
                }

                int[] hashInTrits = new int[Kerl.HASH_LENGTH];
                kerl.Squeeze(ref hashInTrits, 0, hashInTrits.Length);

                hashInTrytes = Converter.ConvertTritsToTrytes(hashInTrits);
                normalizedBundleValue = NormalizeBundle(hashInTrytes);

                bool foundValue = false;
                foreach(int aNormalizedBundleValue in normalizedBundleValue)
                {
                    if (aNormalizedBundleValue == 13)
                    {
                        foundValue = true;
                        obsoleteTagTrits = Converter.ConvertTrytesToTrits(Transactions[0].ObsoleteTag);
                        obsoleteTagTrits = Converter.Increment(obsoleteTagTrits, 81);
                        Transactions[0].ObsoleteTag = Converter.ConvertTritsToTrytes(obsoleteTagTrits);
                    }
                }
                valid = !foundValue;

            } while (!valid);

            BundleHash = hashInTrytes;

            foreach(Transaction transaction in Transactions)
            {
                transaction.Bundle = hashInTrytes;
            }
        }

        /// <summary>
        /// Loops through each transaction and adds a metatransaction if the length of the signature exceeds <see cref="Constants.SIGNATURE_MESSAGE_LENGTH"/>
        /// </summary>
        public void SliceSignatures(int securityLevel)
        {
            for (int c = 0; c < Transactions.Count; c++)
            {
                if(Transactions[c].Value >= 0)
                {
                    continue;
                }

                if (Transactions[c].SignatureMessageFragment.Length % Constants.SIGNATURE_MESSAGE_LENGTH != 0)
                {
                    throw new InvalidBundleException("Invalid signature message length");
                }

                switch (securityLevel)
                {
                    case 1:
                        break;
                    case 2:
                        {
                            Transaction metaTransaction = (Transaction)Transactions[c].Clone();
                            Transactions.Insert(c + 1, metaTransaction);
                            c++;
                            break;
                        }
                    case 3:
                        {
                            Transaction metaTransaction_01 = (Transaction)Transactions[c].Clone();
                            Transaction metaTransaction_02 = (Transaction)Transactions[c].Clone();
                            Transactions.Insert(c + 1, metaTransaction_01);
                            c++;
                            Transactions.Insert(c + 1, metaTransaction_02);
                            c++;
                            break;
                        }
                    default:
                        throw new InvalidBundleException("Invalid signature message length");
                }
            }
        }

        /// <summary>
        /// Returns all transactions as raw transactions
        /// </summary>
        /// <returns>The raw transactions as tryte encoded strings</returns>
        public IEnumerable<string> GetRawTransactions()
        {
            foreach(Transaction transaction in Transactions)
            {
                var dummy = transaction.ToTransactionTrytes().Length;
                yield return transaction.ToTransactionTrytes();
            }
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Bundle";
        }

        /// <summary>
        /// Calculates the bundle hash and assigns it to each transaction
        /// </summary>
        private void CreateAndAssignBundleHash()
        {
            Kerl kerl = new Kerl();

            foreach (Transaction transaction in Transactions)
            {
                kerl.Absorb(Converter.ConvertTrytesToTrits(transaction.Address));
                kerl.Absorb(Converter.ConvertBigIntToTrits(transaction.Value));
                kerl.Absorb(Converter.ConvertTrytesToTrits(transaction.ObsoleteTag));
                kerl.Absorb(Converter.ConvertBigIntToTrits(transaction.Timestamp));
                kerl.Absorb(Converter.ConvertIntegerToTrits(transaction.CurrentIndex));
                kerl.Absorb(Converter.ConvertIntegerToTrits(transaction.LastIndex));
            }

            int[] hashInTrits = new int[Kerl.HASH_LENGTH];

            BundleHash = Converter.ConvertTritsToTrytes(hashInTrits);
            Transactions.ForEach(tx => tx.Bundle = BundleHash);
        }
        
        /// <summary>
        /// Sets the Indexes of all transactions contained in this bundle
        /// </summary>
        private void SetIndexes()
        {
            for(int c = 0; c < Transactions.Count; c++)
            {
                Transactions[c].CurrentIndex = c;
                Transactions[c].LastIndex = Transactions.Count - 1;
            }
        }
        
    }
}