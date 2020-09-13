using System;
using System.Collections.Generic;

namespace Functions.CosmosDB.Models
{
    /// <summary>
    /// Paged Results.
    /// </summary>
    public class PagedResults
    {
        /// <summary>
        /// Gets or sets the pagination.
        /// </summary>
        /// <value>
        /// The pagination.
        /// </value>
        public PaginationModel Pagination { get; set; } = new PaginationModel();

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public List<dynamic> Data { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public PaginationLinks Links { get; set; } = new PaginationLinks();

        /// <summary>
        /// Pagination Model.
        /// </summary>
        public class PaginationModel
        {
            /// <summary>
            /// Gets or sets the continuation token.
            /// </summary>
            /// <value>
            /// The continuation token.
            /// </value>
            public string ContinuationToken { get; set; }

            /// <summary>
            /// Gets the URL encoded continuation token.
            /// </summary>
            /// <value>
            /// The URL encoded continuation token.
            /// </value>
            public string UrlEncodedContinuationToken { get => Uri.EscapeDataString(this.ContinuationToken ?? string.Empty); }
        }

        /// <summary>
        /// Links.
        /// </summary>
        public class PaginationLinks
        {
            /// <summary>
            /// Gets or sets the self.
            /// </summary>
            /// <value>
            /// The self.
            /// </value>
            public string Self { get; set; }

            /// <summary>
            /// Gets or sets the next.
            /// </summary>
            /// <value>
            /// The next.
            /// </value>
            public string Next { get; set; }
        }
    }
}
