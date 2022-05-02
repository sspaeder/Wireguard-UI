using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.WireGuardUIService.Classes
{
    internal class WgLogReader
    {
        /// <summary>
        /// Path to the log file
        /// </summary>
        const string PATH = @"C:\Program Files\WireGuard\Data\log.bin";

        /// <summary>
        /// Form of the date to output
        /// </summary>
        const string DATE_FORM = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// Size of the log data entry
        /// </summary>
        const int DATASIZE = 520;

        const long NANO_TO_MS = 1000000;

        /// <summary>
        /// Converts a unix nano time to a regular datetime
        /// </summary>
        /// <param name="unixNano">Value with the unix nano time</param>
        /// <see href="https://www.unixtimestamp.com/"/>
        /// <returns></returns>
        private static DateTime ConvertToDateTime(Int64 unixNano) =>
            new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(unixNano/ NANO_TO_MS);

        /// <summary>
        /// Reads the data from the Wireguard log file
        /// </summary>
        /// <returns>Content of the log file as string</returns>
        public static string Read()
        {
            StringBuilder sb = new StringBuilder();

            BinaryReader br = new BinaryReader(new FileStream(PATH, FileMode.Open,FileAccess.Read, FileShare.ReadWrite));

            br.ReadBytes(4); //Reads the first 4 byte out of the stream, dont know what good they have

            int numOfLines = BitConverter.ToInt32(br.ReadBytes(4)); // Reads the number of entrys out of the file
            int count = 0;

            byte[] buffer = new byte[DATASIZE];

            Span<byte> span = buffer;

            while(true)
            {
                int nBytes = br.Read(buffer, 0, DATASIZE);

                if (nBytes == 0)
                    break;

                sb.Append(
                    $"{GetDate(span.Slice(0,8))} : {GetText(span.Slice(8))}\n"
                    );

                count++;

                if (count == numOfLines)
                    break;
            }

            br.Close();

            return sb.ToString();
        }

        /// <summary>
        /// Gets the date out of the byte stream
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetDate(Span<byte> data) => ConvertToDateTime(BitConverter.ToInt64(data)).ToString(DATE_FORM);

        /// <summary>
        /// Gets the text out of the byte stream
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetText(Span<byte> data) => Encoding.ASCII.GetString(data).Replace("\0", String.Empty);
    }
}