using System.Text.Json.Serialization;

namespace Functions.CosmosDB.Models
{
    /// <summary>
    /// Continuation Token.
    /// </summary>
    public class ContinuationToken
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the get range.
        /// </summary>
        /// <value>
        /// The get range.
        /// </value>
        [JsonPropertyName("range")]
        public Range GetRange { get; set; }

        /// <summary>
        /// The Range.
        /// </summary>
        public class Range
        {
            /// <summary>
            /// Gets or sets the minimum.
            /// </summary>
            /// <value>
            /// The minimum.
            /// </value>
            [JsonPropertyName("min")]
            public string Min { get; set; }

            /// <summary>
            /// Gets or sets the maximum.
            /// </summary>
            /// <value>
            /// The maximum.
            /// </value>
            [JsonPropertyName("max")]
            public string Max { get; set; }
        }
    }
}
