namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Class for the stop command
    /// </summary>
    public sealed class StopMessage : Message
    {
        /// <summary>
        /// Constructor of this class
        /// </summary>
        public StopMessage() : base("stop")
        {
        }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// The name of the tunnel to be closed
        /// </summary>
        public string Name { get; set; }
    }
}
