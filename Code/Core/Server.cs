using System.IO.Pipes;

namespace WireGuard.Core
{
    /// <summary>
    /// Server class for the communication
    /// </summary>
    public class Server : PipeBase
    {
        /// <summary>
        /// Namedpipe as communication
        /// </summary>
        NamedPipeServerStream pipe;

        /// <summary>
        /// Constructor
        /// </summary>
        public Server()
        {
            //Create the pipex
            pipe = NamedPipeServerStreamConstructors.New(
                PIPE_NAME, 
                PipeDirection.InOut, 
                1, 
                PipeTransmissionMode.Byte, 
                pipeSecurity:CreatePipeSecurity());

            stream = pipe;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect() => pipe.Disconnect();

        /// <summary>
        /// Starts the server synchronus
        /// </summary>
        public void Start()
        {
            pipe.WaitForConnection();
        }

        /// <summary>
        /// Closes the server instance
        /// </summary>
        public void Close() => pipe.Close();

        /// <summary>
        /// Disposes the server and its ressources
        /// </summary>
        public void Dispose() => pipe.Dispose();
    }
}
