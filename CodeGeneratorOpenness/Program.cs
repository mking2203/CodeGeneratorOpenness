using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            AppDomain.CurrentDomain.AssemblyResolve += MyResolver;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMainForm());
        }

        public static string Version = string.Empty;

        private static Assembly MyResolver(object sender, ResolveEventArgs args)
        {
            int index = args.Name.IndexOf(',');
            if (index == -1)
            {
                return null;
            }
            string name = args.Name.Substring(0, index) + ".dll";

            // see what dll we can use (incomplete)
            string path = Path.Combine(@"C:\Program Files\Siemens\Automation\Portal V16\PublicAPI\V16", name);
            if (!File.Exists(path))
            {
                //path = Path.Combine(@"C:\Program Files\Siemens\Automation\Portal V14\PublicAPI\V14 SP1", name);
                //Version = "14.0";
                Version = "16.0";
            }
            else
                Version = "16.0";


            string fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                return Assembly.LoadFrom(fullPath);
            }
            return null;
        }
    }
}
