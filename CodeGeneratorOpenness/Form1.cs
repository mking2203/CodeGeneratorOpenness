using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.ExternalSources;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.Hmi;
using HmiTarget = Siemens.Engineering.Hmi.HmiTarget;
using Siemens.Engineering.Hmi.Tag;
using Siemens.Engineering.Hmi.Screen;
using Siemens.Engineering.Hmi.Cycle;
using Siemens.Engineering.Hmi.Communication;
using Siemens.Engineering.Hmi.Globalization;
using Siemens.Engineering.Hmi.TextGraphicList;
using Siemens.Engineering.Hmi.RuntimeScripting;
using Siemens.Engineering.Compiler;
using Siemens.Engineering.Library;
using System.IO;

namespace CodeGeneratorOpenness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            foreach(TiaPortalProcess tiaPortalProcess in TiaPortal.GetProcesses())
            {
                Console.WriteLine(tiaPortalProcess.Id);
                TiaPortalProcess p = TiaPortal.GetProcess(tiaPortalProcess.Id);
                TiaPortal tiaPortal = p.Attach();
                Console.WriteLine("Found Portal...");
                break;
            }

            using (TiaPortal tiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface))
            {
                Console.WriteLine("TIA Portal has started");
                ProjectComposition projects = tiaPortal.Projects;
                Console.WriteLine("Opening Project...");

                string pat = @"P:\Projekte\Anting\30014092_R134aNA_M5BEN2_20191108_1618_MK_V16\30014092_R134aNA_M5BEN2_20191108_1618_MK_V16.ap16";

                FileInfo projectPath = new FileInfo(pat); //edit the path according to your project
                Project project = null;
                try
                {
                    project = projects.Open(projectPath);
                }
                catch (Exception)
                {
                    Console.WriteLine(String.Format("Could not open project {0}", projectPath.FullName));
                    Console.WriteLine("Demo complete hit enter to exit");
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine(String.Format("Project {0} is open", project.Path.FullName));

                IterateThroughDevices(project);
                project.Close();
            }
        }


        private static void IterateThroughDevices(Project project)
        {
            if (project == null)
            {
                Console.WriteLine("Project cannot be null");
                return;
            }
            Console.WriteLine(String.Format("Iterate through {0} device(s)", project.Devices.Count));
            foreach (Device device in project.Devices)
            {
                Console.WriteLine(String.Format("Device: \"{0}\".", device.Name));
            }
            Console.WriteLine();
        }
    }
}
