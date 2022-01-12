using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;

namespace CodeGeneratorOpenness
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // since we work im VMware images - we have only one version / image installed
            // find the correct version in the registry

            string BASE_PATH = @"SOFTWARE\Siemens\Automation\_InstalledSW\Global\Bundles\";
            RegistryKey key = GetRegistryKey(BASE_PATH);

            if (key != null)
            {
                var names = key.GetSubKeyNames().OrderBy(x => x).ToList();
                foreach (string n in names)
                {
                    if (n.Contains("TIA Portal V17"))
                    {
                        Version = "17.0";
                        Api = @"C:\Program Files\Siemens\Automation\Portal V17\PublicAPI\V17";
                    }
                    if (n.Contains("TIA Portal V16"))
                    {
                        Version = "16.0";
                        Api = @"C:\Program Files\Siemens\Automation\Portal V16\PublicAPI\V16";
                    }
                    if (n.Contains("TIA Portal V15.1"))
                    {
                        Version = "15.1";
                        Api = @"C:\Program Files\Siemens\Automation\Portal V15_1\PublicAPI\V15.1";
                    }
                    if (n.Contains("TIA Portal V15") && !n.Contains("TIA Portal V15.1"))
                    {
                        Version = "15.0";
                        Api = @"C:\Program Files\Siemens\Automation\Portal V15\PublicAPI\V15";
                    }
                    if (n.Contains("TIA Portal V14"))
                    {
                        Version = "14.0";
                        Api = @"C:\Program Files\Siemens\Automation\Portal V14\PublicAPI\V14";
                    }
                }

                key.Dispose();
            }

            AppDomain.CurrentDomain.AssemblyResolve += MyResolver;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMainForm());
        }

        public static string Version = string.Empty;
        public static string Api = string.Empty;

        private static Assembly MyResolver(object sender, ResolveEventArgs args)
        {
            int index = args.Name.IndexOf(',');
            if (index == -1)
            {
                return null;
            }

            string name = args.Name.Substring(0, index) + ".dll";
            //System.Diagnostics.Debug.Print("Assembly: " + name);

            // for Siemens we load the correct dll (version)
            if (name == "Siemens.Engineering.dll")
            {
                string path = Path.Combine(Api, name);

                // see what dll we can use (incomplete)
                if (File.Exists(path))
                {
                    string fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        return Assembly.LoadFrom(fullPath);
                    }
                }
            }
            return null;
        }

        private static RegistryKey GetRegistryKey(string keyname)
        {
            RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey key = baseKey.OpenSubKey(keyname);
            if (key == null)
            {
                baseKey.Dispose();
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                key = baseKey.OpenSubKey(keyname);
            }
            if (key == null)
            {
                baseKey.Dispose();
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                key = baseKey.OpenSubKey(keyname);
            }
            baseKey.Dispose();

            return key;
        }


    }
}
