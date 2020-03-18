///
/// Sample applicatin for automated code generation for Siemens TIA Portal with Openness Interface
/// 
/// by Mark König @ 02/2020
/// 
/// cFirewall is a small helper to avoid the firewall anytime you build
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace CodeGeneratorOpenness
{
    class cFirewall
    {
        public void CalcHash()
        {
            // calc the hash for the file for the firwall settings
            string applicationPath = Application.StartupPath + "\\CodeGeneratorOpenness.exe";
            string lastWriteTimeUtcFormatted = String.Empty;
            DateTime lastWriteTimeUtc;
            HashAlgorithm hashAlgorithm = SHA256.Create();
            FileStream stream = File.OpenRead(applicationPath);
            byte[] hash = hashAlgorithm.ComputeHash(stream);
            // this is how the hash should appear in the .reg file
            string convertedHash = Convert.ToBase64String(hash);
            FileInfo fileInfo = new FileInfo(applicationPath);
            lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
            // this is how the last write time should be formatted
            lastWriteTimeUtcFormatted = lastWriteTimeUtc.ToString(@"yyyy\/MM\/dd HH:mm:ss.fff");

            // we set the key in the registry to avoid the firewall each time
            try
            {
                // first time we need to ack, then the key is present
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Siemens\Automation\Openness\" + Program.Version + @"\Whitelist\CodeGeneratorOpenness.exe\Entry", true);
                rk.SetValue("FileHash", convertedHash);
                rk.SetValue("DateModified", lastWriteTimeUtcFormatted);
            }
            catch
            { }
        }
    }
}
