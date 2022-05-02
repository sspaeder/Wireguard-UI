namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Class for the start command
    /// </summary>
    public sealed class StartMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StartMessage() : base("start")
        {
        }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// The Config-File to be started
        /// </summary>
        public string File { get; set; }
    }
}
