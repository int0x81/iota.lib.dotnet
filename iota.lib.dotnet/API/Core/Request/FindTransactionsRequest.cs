﻿using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Represents the core api request 'FindTransactions'
    /// </summary>
    public class FindTransactionsRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindTransactionsRequest"/> class.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <param name="addresses">The addresses.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="approvees">The approvees.</param>
        public FindTransactionsRequest(List<string> bundles, List<string> addresses, List<string> tags, List<string> approves) : base(Core.Command.FindTransactions)
        {
            Bundles = bundles;
            Addresses = addresses;
            Tags = tags;
            Approves = approves;

            if (Bundles == null)
                Bundles = new List<string>();
            if (Addresses == null)
                Addresses = new List<string>();
            if (Tags == null)
                Tags = new List<string>();
            if (Approves == null)
                Approves = new List<string>();
        }

        /// <summary>
        /// Gets or sets the bundles.
        /// </summary>
        /// <value>
        /// The bundles.
        /// </value>
        public List<string> Bundles { get; set; }

        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        public List<string> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the approvees.
        /// </summary>
        /// <value>
        /// The approvees.
        /// </value>
        public List<string> Approves { get; set; }
    }
}