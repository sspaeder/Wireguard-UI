using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using WireGuard.Core.Messages;

namespace WireGuard.Core
{
    /// <summary>
    /// Base class for the usage of the pipe
    /// </summary>
    public abstract class PipeBase
    {
        /// <summary>
        /// Gets the name of the pipe
        /// </summary>
        protected const string PIPE_NAME = "wg.{80DBE9C5-E1CD-4A80-86CF-C8C4D49AFD4D}";

        /// <summary>
        /// Stream to read from and write to
        /// </summary>
        protected Stream stream;

        /// <summary>
        /// Sends a command to the endpoint
        /// </summary>
        /// <param name="command">Command to be send</param>
        public void Send(Message command) => WriteString(command.ToJSON());

        /// <summary>
        /// Recieves a command from the endpoint
        /// </summary>
        /// <returns><see cref="Message"/></returns>
        public Message Recive() => Message.Create(ReadData());

        /// <summary>
        /// Creates the security descriptor of the pipe
        /// </summary>
        protected PipeSecurity CreatePipeSecurity()
        {
            PipeSecurity security = new PipeSecurity();

            //Allow system user to access
            security.AddAccessRule(new PipeAccessRule(UAC.SystemUser, PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));

            //Allow authenticated user to access
            security.AddAccessRule(new PipeAccessRule(UAC.AuthenticatedUserGroup, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));

            //Deny networkservices to access
            security.AddAccessRule(new PipeAccessRule(UAC.NetworkServiceGroup, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Deny));

            //Deny networkusers to access
            security.AddAccessRule(new PipeAccessRule(UAC.NetworkUserGroup, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Deny));

            return security;
        }

        /// <summary>
        /// Method to read the string from the input
        /// </summary>
        /// <returns></returns>
        private string ReadData()
        {
            string result = Encoding.Unicode.GetString(ReadAllByte());

            LogManager.Debug($"Recieved message: {result}", nameof(PipeBase));

            return result;
        }

        /// <summary>
        /// Method to read all they bytes form the input
        /// </summary>
        /// <returns></returns>
        private byte[] ReadAllByte()
        {
            byte[] numOfBytes = new byte[sizeof(int)];
            stream.Read(numOfBytes, 0, sizeof(int));

            int numOfBytesToRead = BitConverter.ToInt32(numOfBytes, 0);

            byte[] buffer = new byte[numOfBytesToRead];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;

                read = stream.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, read);

                LogManager.Debug($"Number of read bytes {read}", nameof(PipeBase));

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Writes a string to the endpoint
        /// </summary>
        /// <param name="json">JSON string to submit</param>
        private void WriteString(string json)
        {
            MemoryStream ms = new MemoryStream();

            byte[] buffer = Encoding.Unicode.GetBytes(json);

            ms.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
            ms.Write(buffer, 0 , buffer.Length);
            ms.Position = 0;

            stream.Write(ms.ToArray(), 0, (int)ms.Length);
            stream.Flush();
        }
    }
}