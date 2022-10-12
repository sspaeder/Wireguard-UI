using System;
using System.IO.Pipes;

namespace WireGuard.Core
{
    /// <summary>
    /// Client class for the communication
    /// </summary>
    public class Client : PipeBase
    {
        /// <summary>
        /// Namedpipe as communication
        /// </summary>
        NamedPipeClientStream pipe;

        /// <summary>
        /// Constructor 
        /// </summary>
        public Client()
        {
        }

        /// <summary>
        /// Connects to the namedpipe
        /// </summary>
        public void Connect()
        {
            if (!System.IO.File.Exists($@"\\.\\pipe\\{PIPE_NAME}"))
                throw new Exception("No server is running!");

            //Creates the client and trys to connect
            pipe = new NamedPipeClientStream(".", PIPE_NAME, PipeDirection.InOut, PipeOptions.None);
            stream = pipe;

            pipe.Connect(15*1000);
        }

        /// <summary>
        /// Closes the pipe
        /// </summary>
        public void Disconnect() => pipe?.Close();
    }
}
