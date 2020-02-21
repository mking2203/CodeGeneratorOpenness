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
using System.Security.Cryptography;
using Microsoft.Win32;

namespace CodeGeneratorOpenness
{
    public partial class MainForm : Form
    {
        // just lazzy for now
        public static TiaPortal tiaPortal = null;
        public static Project project = null;

        public MainForm()
        {
            InitializeComponent();

            CalcHash();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (project == null)
            {
                // see if we have a open instance and use the first one
                foreach (TiaPortalProcess tiaPortalProcess in TiaPortal.GetProcesses())
                {
                    TiaPortalProcess p = TiaPortal.GetProcess(tiaPortalProcess.Id);

                    tiaPortal = p.Attach();
                    Console.WriteLine("TIA Portal has been attached");
                    break;
                }

                // we dont habe a instance then open a new instance
                if (tiaPortal == null)
                {
                    tiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);
                    Console.WriteLine("TIA Portal has started");
                }

                // let's get the projects
                ProjectComposition projects = tiaPortal.Projects;

                // projects available ?
                Console.WriteLine("TIA projects " + projects.Count.ToString());

                // no open project the open dialog
                if (projects.Count == 0)
                {
                    Console.WriteLine("Opening Project...");

                    string filePath = string.Empty;
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "V16 project files (*.ap16)|*.ap16|All files (*.*)|*.*";
                        openFileDialog.FilterIndex = 1;
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            //Get the path of specified file
                            filePath = openFileDialog.FileName;

                            // load projectpath
                            FileInfo projectPath = new FileInfo(filePath);
                            try
                            {
                                project = projects.Open(projectPath);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine(String.Format("Could not open project {0}", projectPath.FullName));
                                Application.Exit();
                            }
                        }
                        else
                            Application.Exit(); // no file selected
                    }
                }
                else
                {
                    project = tiaPortal.Projects[0];
                }

                Console.WriteLine(String.Format("Project {0} is open", project.Path.FullName));

                IterateThroughDevices(project);
            }
        }


        private void IterateThroughDevices(Project project)
        {
            if (project == null)
            {
                Console.WriteLine("Project cannot be null");
                return;
            }
            Console.WriteLine(String.Format("Iterate through {0} device(s)", project.Devices.Count));

            // search through devices
            foreach (Device device in project.Devices)
            {
                if (device.TypeIdentifier != null)
                {
                    // we search only for PLCs
                    if (device.TypeIdentifier == "System:Device.S71500")
                    {
                        Console.WriteLine(String.Format("Found {0}", device.Name));
                        listBox1.Items.Add(device.Name);

                        // lets get the CPU
                        foreach (DeviceItem item in device.DeviceItems)
                        {
                            if (item.Classification.ToString() == "CPU")
                            {
                                Console.WriteLine(String.Format("Found {0}", item.Name));
                                listBox2.Items.Add(item.Name);
                            }
                        }
                    }
                }
            }
        }

        // close project
        private void button2_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                listBox1.Items.Clear();

                project.Close();
                project = null;
            }
        }

        // my test area
        private void button3_Click(object sender, EventArgs e)
        {

        }



        public void CalcHash()
        {
            string applicationPath = Application.StartupPath + "\\CodeGeneratorOpenness.exe";
            string lastWriteTimeUtcFormatted = String.Empty;
            DateTime lastWriteTimeUtc;
            HashAlgorithm hashAlgorithm = SHA256.Create();
            FileStream stream = File.OpenRead(applicationPath);
            byte[] hash = hashAlgorithm.ComputeHash(stream);
            // this is how the hash should appear in the .reg file
            string convertedHash = Convert.ToBase64String(hash);
            FileInfo fileInfo = new FileInfo(applicationPath);
            lastWriteTimeUtc = fileInfo.LastWriteTimeUtc; // this is how the last write time should be formatted
            lastWriteTimeUtcFormatted = lastWriteTimeUtc.ToString(@"yyyy\/MM\/dd HH:mm:ss.fff");

            Console.WriteLine("CRC _: " + convertedHash);
            Console.WriteLine("Date : " + lastWriteTimeUtcFormatted);

        }
    }
}
