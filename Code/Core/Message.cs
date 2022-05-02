using System.Text.Json;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Class for the commands that can be exchanged
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comType">Type of command</param>
        public Message(string comType)
        {
            Type = comType;
        }

        /// <summary>
        /// Creates a command form a json
        /// </summary>
        /// <param name="json">JSON string to be deserialized</param>
        /// <returns><see cref="Message"/></returns>
        public static Message Create(string json)
        {
            if (json.Length == 0)
                return null;

            if (json.Contains("\"Type\":\"start\""))
                return JsonSerializer.Deserialize<StartMessage>(json);
            else if (json.Contains("\"Type\":\"stop\""))
                return JsonSerializer.Deserialize<StopMessage>(json);
            else if (json.Contains("\"Type\":\"result\""))
                return JsonSerializer.Deserialize<ResultMessage>(json);
            else if (json.Contains("\"Type\":\"status\""))
                return JsonSerializer.Deserialize<StatusMessage>(json);
            else if (json.Contains("\"Type\":\"istatus\""))
                return JsonSerializer.Deserialize<InterfaceStatus>(json);
            else if (json.Contains("\"Type\":\"import\""))
                return JsonSerializer.Deserialize<ImportMessage>(json);
            else if (json.Contains("\"Type\":\"remove\""))
                return JsonSerializer.Deserialize<RemoveMessage>(json);
            else if (json.Contains("\"Type\":\"settings\""))
                return JsonSerializer.Deserialize<SettingsMessage>(json);
            else if (json.Contains("\"Type\":\"logcontent\""))
                return JsonSerializer.Deserialize<LogContentMessage>(json);
            else if (json.Contains("\"Type\":\"plugin\""))
                return JsonSerializer.Deserialize<PlugInMessage>(json);


            //If no match is found
            return null;
        }

        /// <summary>
        /// Converts the class to a JSON represented object
        /// </summary>
        /// <returns></returns>
        public abstract string ToJSON();

        /// <summary>
        /// Gets or sets the type of command
        /// </summary>
        public string Type { get; set; }
    }
}
