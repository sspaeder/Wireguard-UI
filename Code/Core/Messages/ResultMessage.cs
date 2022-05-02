namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Command for the result
    /// is actual not a command, but uses similar structures
    /// </summary>
    public sealed class ResultMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ResultMessage() : base("result")
        {
        }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// Error code of the result
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        /// Message of the error
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
