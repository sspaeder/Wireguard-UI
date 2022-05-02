using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.WireGuardUIService
{
    /// <summary>
    /// Class for encyprting a config file
    /// </summary>
    static class DPAPI
    {
        #region Extern Structures, Enumerations & Methods

        /// <summary>
        /// 
        /// </summary>
        /// <see href="http://pinvoke.net/default.aspx/Enums/CryptProtectPromptFlags.html"/>
        /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/dpapi/ns-dpapi-cryptprotect_promptstruct#members"/>
        [Flags]
        private enum CryptProtectPromptFlags
        {
            CRYPTPROTECT_PROMPT_ON_UNPROTECT = 0x1,
            CRYPTPROTECT_PROMPT_ON_PROTECT = 0x2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see href="http://pinvoke.net/default.aspx/Enums/CryptProtectFlags.html"/>
        /// <seealso href="https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/aa381414(v=vs.85)"/>
        [Flags]
        private enum CryptProtectFlags
        {
            CRYPTPROTECT_UI_FORBIDDEN = 0x1,
            CRYPTPROTECT_LOCAL_MACHINE = 0x4,
            CRYPTPROTECT_CRED_SYNC = 0x8,
            CRYPTPROTECT_AUDIT = 0x10,
            CRYPTPROTECT_NO_RECOVERY = 0x20,
            CRYPTPROTECT_VERIFY_PROTECTION = 0x40
        }

        /// <summary>
        /// Structure to descript the command prompt
        /// </summary>
        /// <see href="http://pinvoke.net/default.aspx/Structures/CRYPTPROTECT_PROMPTSTRUCT.html"/>
        /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/dpapi/ns-dpapi-cryptprotect_promptstruct"/>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CRYPTPROTECT_PROMPTSTRUCT
        {
            public int cbSize;
            public CryptProtectPromptFlags dwPromptFlags;
            public IntPtr hwndApp;
            public String szPrompt;
        }

        /// <summary>
        /// Structure with the data
        /// </summary>
        /// <see href="http://pinvoke.net/default.aspx/Structures/DATA_BLOB.html"/>
        /// <seealso href="https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/aa381414(v=vs.85)"/>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        /// <summary>
        /// Method to encypt data
        /// </summary>
        /// <see href="http://pinvoke.net/default.aspx/crypt32/CryptProtectData.html"/>
        /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/dpapi/nf-dpapi-cryptprotectdata"/>
        [DllImport("Crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptProtectData(
            ref DATA_BLOB pDataIn,
            string szDataDescr,
            IntPtr pOptionalEntropy,
            IntPtr pvReserved,
            IntPtr pPromptStruct,
            CryptProtectFlags dwFlags,
            ref DATA_BLOB pDataOut);

        #endregion

        /// <summary>
        /// Encrypts the Data of a file
        /// </summary>
        /// <param name="source">FileInfo to encypt</param>
        /// <param name="targetPath">Target to write the encrypted data to</param>
        /// <see href="https://github.com/WireGuard/wireguard-windows/blob/ba4edc55c4712016921bea54dbd7c0408a69ae7b/conf/store.go#L99">Wireguard Save config</see>
        /// <see href="https://github.com/WireGuard/wireguard-windows/blob/ba4edc55c4712016921bea54dbd7c0408a69ae7b/conf/writer.go#L13">Wireguard convert to string</see>
        /// <see href="https://github.com/WireGuard/wireguard-windows/blob/ba4edc55c4712016921bea54dbd7c0408a69ae7b/conf/dpapi/dpapi_windows.go#L25">CryptProtectData function call</see>
        /// <remarks>Test has shown that there is problem with this function. Furhter investigations are needed.</remarks>
        public static void EncryptFile(FileInfo source, string targetPath)
        {
            //Gets the bytes of the file
            byte[] inData = File.ReadAllBytes(source.FullName);

            //Copys the data to unmanged area
            IntPtr datPtr = Marshal.AllocHGlobal(inData.Length);
            Marshal.Copy(inData, 0, datPtr, inData.Length);

            //Create blobs for input
            DATA_BLOB inBlob = new DATA_BLOB() { cbData = inData.Length, pbData = datPtr };
            DATA_BLOB outBlob = new DATA_BLOB();

            //Generate filename for datadescripton
            string fileName = source.Name.Remove(source.Name.IndexOf('.'));

            Core.LogManager.Debug($"Filename: {fileName} / No. bytes to encrypt: {inBlob.cbData}", nameof(DPAPI));

            if (CryptProtectData(ref inBlob, fileName, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, CryptProtectFlags.CRYPTPROTECT_UI_FORBIDDEN, ref outBlob))
            {
                byte[] outData = new byte[outBlob.cbData];
                Marshal.Copy(outBlob.pbData, outData, 0, outBlob.cbData);

                Core.LogManager.Debug($"Outputbuffer: {outBlob.cbData}", nameof(DPAPI));

                File.WriteAllBytes($@"{targetPath}\{source.Name}.dpapi", outData);

                Core.LogManager.Debug($"File encrypted successfully", nameof(DPAPI));

                File.Delete(source.FullName);
            }
            else
                throw new Exception("Error encrypting file");

            //Free the allocated space
            Marshal.FreeHGlobal(datPtr);
            Marshal.FreeHGlobal(outBlob.pbData);
        }

    }
}
