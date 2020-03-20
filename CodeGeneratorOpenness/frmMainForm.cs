///
/// Sample applicatin for automated code generation for Siemens TIA Portal with Openness Interface
/// 
/// by Mark König @ 02/2020
/// 
/// build to 64 bit, since we nedd to access the registry for the TIA firewall
///

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
using Siemens.Engineering.Compiler;
using Siemens.Engineering.Library;

using System.IO;
using System.Globalization;
using System.Xml;

namespace CodeGeneratorOpenness
{
    public partial class frmMainForm : Form
    {
        // just a little lazzy
        public static TiaPortal tiaPortal = null;
        public static Project project = null;
        public static PlcSoftware software = null;

        private cFunctionGroups groups = new cFunctionGroups();

        public frmMainForm()
        {
            InitializeComponent();

            // avoid firewall
            // HLKM\SOFTWARE\Siemens\Automation\Openness\
            // set the rights for the key => everone to everything
            cFirewall firewall = new cFirewall();
            firewall.CalcHash();
        }

        private void frmMainForm_Load(object sender, EventArgs e)
        {
            // generate default folder
            Directory.CreateDirectory(Application.StartupPath + "\\Export");
            Directory.CreateDirectory(Application.StartupPath + "\\Import");
            Directory.CreateDirectory(Application.StartupPath + "\\Temp");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // dispose objects
            software = null;
            project = null;

            if (tiaPortal != null)
                tiaPortal.Dispose();
        }

        // open project
        private void btnOpen_Click(object sender, EventArgs e)
        {
            // if no project is open
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

                // we don't habe an instance then open a new instance
                if (tiaPortal == null)
                {
                    tiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);
                    Console.WriteLine("TIA Portal has started");
                }

                //tiaPortal.Confirmation += TiaPortal_Confirmation;
                //tiaPortal.Notification += TiaPortal_Notification;

                // let's get the projects
                ProjectComposition projects = tiaPortal.Projects;

                // projects available ?
                Console.WriteLine("TIA projects " + projects.Count.ToString());

                // no open project - then open file dialog
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
                            return; // no file selected
                    }
                }
                else
                {
                    // for now we use the first project
                    project = tiaPortal.Projects[0];
                }

                //Console.WriteLine(String.Format("Project {0} is open", project.Path.FullName));
                txtProject.Text = "Project: " + project.Name;

                // loop through the data
                IterateThroughDevices(project);
            }
        }

        private void TiaPortal_Notification(object sender, NotificationEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TiaPortal_Confirmation(object sender, ConfirmationEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IterateThroughDevices(Project project)
        {
            // no project is open
            if (project == null)
                return;

            // shop a form to indicate work
            frmReadStructure read = new frmReadStructure();
            read.Show();

            Application.DoEvents();

            //Console.WriteLine(String.Format("Iterate through {0} device(s)", project.Devices.Count));
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            treeView1.Nodes.Clear();

            // search through devices
            foreach (Device device in project.Devices)
            {
                if (device.TypeIdentifier != null)
                {
                    // we search only for PLCs
                    if (device.TypeIdentifier == "System:Device.S71500")
                    {
                        //Console.WriteLine(String.Format("Found {0}", device.Name));
                        listBox1.Items.Add(device.Name);

                        // let's get the CPU
                        foreach (DeviceItem item in device.DeviceItems)
                        {
                            if (item.Classification.ToString() == "CPU")
                            {
                                //Console.WriteLine(String.Format("Found {0}", item.Name));
                                listBox2.Items.Add(item.Name);

                                // get the software container
                                SoftwareContainer softwareContainer = ((IEngineeringServiceProvider)item).GetService<SoftwareContainer>();
                                if (softwareContainer != null)
                                {
                                    software = softwareContainer.Software as PlcSoftware;
                                    //Console.WriteLine("Found : " + software.Name);

                                    // start update treeview
                                    treeView1.BeginUpdate();

                                    // add root node
                                    TreeNode root = new TreeNode(software.Name);
                                    root.Tag = software.BlockGroup;
                                    treeView1.Nodes.Add(root);

                                    cFunctionGroups func = new cFunctionGroups();
                                    func.AddPlcBlocks(software.BlockGroup, root);
                                    //AddPlcBlocks(software.BlockGroup, root);

                                    // add data types
                                    TreeNode dataTypes = new TreeNode("Data types");
                                    dataTypes.Tag = software.TypeGroup;
                                    treeView1.Nodes.Add(dataTypes);

                                    func.AddPlcTypes(software.TypeGroup, dataTypes);
                                    //AddPlcTypes(software.TypeGroup, dataTypes);
                                    dataTypes.Expand();

                                    // end update
                                    treeView1.EndUpdate();

                                    root.Expand();
                                }
                            }
                        }
                    }
                }
            }

            // close form
            read.Close();
            read.Dispose();

        }

        // close project
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                treeView1.Nodes.Clear();

                txtProject.Text = "Project: ";

                //project.Save();
                project.Close();

                software = null;
                project = null;
            }
        }

        // language test for DE/EN
        private void btnLanguage_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                LanguageSettings languageSettings = project.LanguageSettings;
                LanguageComposition supportedLanguages = languageSettings.Languages;
                LanguageAssociation activeLanguages = languageSettings.ActiveLanguages;

                Language supportedGermanLanguage = supportedLanguages.Find(CultureInfo.GetCultureInfo("de-DE"));
                Language supportedEnglishLanguage = supportedLanguages.Find(CultureInfo.GetCultureInfo("en-GB"));

                // add german if needed
                Language l = activeLanguages.Find(CultureInfo.GetCultureInfo("de-DE"));
                if (l == null)
                    activeLanguages.Add(supportedGermanLanguage);
                // add english if needed
                l = activeLanguages.Find(CultureInfo.GetCultureInfo("en-GB"));
                if (l == null)
                    activeLanguages.Add(supportedEnglishLanguage);

                // set edit languages
                languageSettings.EditingLanguage = supportedGermanLanguage;
                languageSettings.ReferenceLanguage = supportedGermanLanguage;
            }
        }

        // add a fc for testing
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (software == null)
                return;

            if (treeView1.SelectedNode != null)
            {
                var sel = treeView1.SelectedNode.Tag;
                if (sel is PlcBlockGroup)
                {
                    try
                    {
                        PlcBlockGroup group = (PlcBlockGroup)sel;
                        string fPath = Application.StartupPath + "\\TestFC1.xml";
                        //string fPath = Application.StartupPath + "\\StepData.xml";
                        FileInfo f = new FileInfo(fPath);

                        // now load the xml document
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(fPath);

                        // get version of the file
                        XmlNode bkm = xmlDoc.SelectSingleNode("//Document//Engineering");
                        string version = bkm.Attributes["version"].Value;

                        // check the correct type
                        //XmlNode dataType = xmlDoc.SelectSingleNode("//Document//SW.Types.PlcStruct");
                        //if (dataType != null)
                        {
                            string blockType = string.Empty;

                            XmlNode document = xmlDoc.SelectSingleNode("//Document");
                            foreach (XmlNode node in document.ChildNodes)
                            {
                                if(node.Name.StartsWith("SW.Blocks."))
                                {
                                    blockType = node.Name.Substring(10);
                                    Console.WriteLine(node.Name.Substring(10));
                                }
                            }

                            // get the name of the data type
                            XmlNode nameDefination = xmlDoc.SelectSingleNode("//Document//SW.Blocks." + blockType + "//AttributeList//Name");
                            string name = nameDefination.InnerText;

                            // check if the data type exists
                            bool exists = false;

                            List<string> list = new List<string>();
                            list = groups.GetAllDataTypesNames(software.TypeGroup, list);
                            if (list.Contains(name)) exists = true;

                            if (!exists)
                            {
                                list = new List<string>();
                                list = groups.GetAllBlocksNames(software.BlockGroup, list);
                                if (list.Contains(name)) exists = true;

                                if (!exists)
                                {
                                    // import the file
                                    group.Blocks.Import(f, ImportOptions.None);
                                    IterateThroughDevices(project);
                                }
                                else
                                {
                                    // since we can't import with a different name we need to save a copy 
                                    // more work needed to check if new name exist....
                                    nameDefination.InnerText = "newABC";

                                    xmlDoc.Save(Application.StartupPath + "\\Temp\\temp.xml");
                                    f = new FileInfo(Application.StartupPath + "\\Temp\\temp.xml");

                                    // overwrite?
                                    DialogResult res = MessageBox.Show("Data block " + name + " exists already. Overwrite ?",
                                                                       "Overwrite",
                                                                       MessageBoxButtons.OKCancel,
                                                                       MessageBoxIcon.Question);
                                    if (res == DialogResult.OK)
                                    {
                                        // overwrite plc block
                                        group.Blocks.Import(f, ImportOptions.Override);
                                        IterateThroughDevices(project);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,
                                "Exception",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No group selected!",
                                "No group",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Nothing is selected!",
                            "Nothing selected",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
            }
        }

        // moue click on treeview
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            // Make sure this is the right button.
            if (e.Button != MouseButtons.Right) return;

            // Select this node.
            TreeNode node_here = treeView1.GetNodeAt(e.X, e.Y);
            treeView1.SelectedNode = node_here;

            // See if we got a node.
            if (node_here == null) return;

            // reset menu
            ctxGroup.MenuItems[0].Enabled = true;
            ctxGroup.MenuItems[2].Enabled = true;
            //for the root node we can't delete the group
            if (node_here.Parent == null)
                ctxGroup.MenuItems[2].Enabled = false;

            // See what kind of object this is and
            // display the appropriate popup menu.
            if (node_here.Tag is PlcSoftware)
            {
                ctxSoftware.Show(treeView1, new Point(e.X, e.Y));
            }
            if (node_here.Tag is PlcBlockGroup)
            {
                ctxGroup.Show(treeView1, new Point(e.X, e.Y));
            }
            if (node_here.Tag is PlcBlock)
            {
                ctxBlock.Show(treeView1, new Point(e.X, e.Y));
            }
            if (node_here.Tag is PlcStruct)
            {
                ctxBlock.Show(treeView1, new Point(e.X, e.Y));
            }
        }

        // delete block / data type
        private void mnuBlockDelete_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Tag is PlcBlock)
            {
                PlcBlock block = (PlcBlock)treeView1.SelectedNode.Tag;

                DialogResult dlg = MessageBox.Show("Do you really want to delete the block " + block.Name + "?", "Delete block", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlg == DialogResult.Yes)
                {
                    block.Delete();
                    IterateThroughDevices(project);
                }
            }

            if (treeView1.SelectedNode.Tag is PlcStruct)
            {
                PlcStruct block = (PlcStruct)treeView1.SelectedNode.Tag;

                DialogResult dlg = MessageBox.Show("Do you really want to delete the data type " + block.Name + "?", "Delete data type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlg == DialogResult.Yes)
                {
                    block.Delete();
                    IterateThroughDevices(project);
                }
            }
        }

        // delete group
        private void menuGroupDelete_Click(object sender, EventArgs e)
        {
            PlcBlockGroup group = (PlcBlockGroup)treeView1.SelectedNode.Tag;

            if (group.Blocks.Count > 0)
            {
                MessageBox.Show("The group " + group.Name + " has ^sub blocks!\nDelete this blocks first", "Delete group", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (group.Groups.Count > 0)
            {
                MessageBox.Show("The group " + group.Name + " has sub groups!\nDelete this groups first", "Delete group", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DialogResult dlg = MessageBox.Show("Do you really want to delete the group " + group.Name + "?", "Delete block", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlg == DialogResult.Yes)
            {
                // not implemented yet, there is no method to delete a group?    

                // code from Tia Example is using invoke
                var selectedProjectObject = treeView1.SelectedNode.Tag;
                try
                {
                    var engineeringObject = selectedProjectObject as IEngineeringObject;
                    engineeringObject?.Invoke("Delete", new Dictionary<Type, object>());
                }
                catch (EngineeringException)
                {

                }
                IterateThroughDevices(project);
            }
        }

        private void menuGroupAdd_Click(object sender, EventArgs e)
        {
            PlcBlockGroup group = (PlcBlockGroup)treeView1.SelectedNode.Tag;

            string name = string.Empty;
            DialogResult dlg = Input.InputBox("Enter new group name", "New group", ref name);

            if (dlg == DialogResult.OK)
            {
                if (name != string.Empty)
                {
                    group.Groups.Create(name);
                    IterateThroughDevices(project);
                }
            }
        }

        private void menuSofwareAdd_Click(object sender, EventArgs e)
        {
            PlcSoftware soft = (PlcSoftware)treeView1.SelectedNode.Tag;
            PlcBlockGroup group = soft.BlockGroup;

            string name = string.Empty;
            DialogResult dlg = Input.InputBox("Enter new group name", "New group", ref name);

            if (dlg == DialogResult.OK)
            {
                if (name != string.Empty)
                {
                    group.Groups.Create(name);
                    IterateThroughDevices(project);
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            IterateThroughDevices(project);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                try
                {
                    string pa = Application.StartupPath + "\\Export.xlsx";
                    project.ExportProjectTexts(new FileInfo(pa), new CultureInfo("de-DE"), new CultureInfo("en-GB"));
                }
                catch
                { }
            }
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                try
                {
                    string pa = Application.StartupPath + "\\Export.xlsx";
                    project.ImportProjectTexts(new FileInfo(pa), true);
                }
                catch (Exception ex)
                {
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (software == null)
                return;

            try
            {
                // test file
                string fPath = string.Empty;
                //string fPath = Application.StartupPath + "\\Step.xml";

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "XML files (*.xnl)|*.xml|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        fPath = openFileDialog.FileName;
                        FileInfo f = new FileInfo(fPath);

                        // now load the xml document
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(fPath);

                        // get version of the file
                        XmlNode bkm = xmlDoc.SelectSingleNode("//Document//Engineering");
                        string version = bkm.Attributes["version"].Value;

                        // check the correct type
                        XmlNode dataType = xmlDoc.SelectSingleNode("//Document//SW.Types.PlcStruct");
                        if (dataType != null)
                        {
                            // get the name of the data type
                            XmlNode nameDefination = xmlDoc.SelectSingleNode("//Document//SW.Types.PlcStruct//AttributeList//Name");
                            string name = nameDefination.InnerText;

                            // check if the name exists
                            bool exists = false;

                            List<string> list = new List<string>();
                            list = groups.GetAllBlocksNames(software.BlockGroup, list);
                            if (list.Contains(name)) exists = true;

                            if (!exists)
                            {
                                list = new List<string>();
                                list = groups.GetAllDataTypesNames(software.TypeGroup, list);
                                if (list.Contains(name)) exists = true;

                                if (!exists)
                                {
                                    // import the file
                                    software.TypeGroup.Types.Import(f, ImportOptions.None);
                                    IterateThroughDevices(project);
                                }
                                else
                                {
                                    // overwrite?
                                    DialogResult res = MessageBox.Show("Data type " + name + " exists already. Overwrite ?",
                                                                       "Overwrite",
                                                                       MessageBoxButtons.OKCancel,
                                                                       MessageBoxIcon.Question);
                                    if (res == DialogResult.OK)
                                    {
                                        // overwrite data type
                                        software.TypeGroup.Types.Import(f, ImportOptions.Override);
                                        IterateThroughDevices(project);
                                    }
                                }
                            }
                            else
                                // plc block exist
                                MessageBox.Show("PLC block with the name " + name + " exist!",
                                                "Name exits",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                        }
                        else
                        {
                            // wrong data type
                            MessageBox.Show("Wrong XML file (PlcStruct) ?",
                                            "Wrong file",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                                "Exception",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // nothing selected            
            if (treeView1.SelectedNode == null) return;
            // only blocks and structs
            if (!(treeView1.SelectedNode.Tag is PlcBlock) && !(treeView1.SelectedNode.Tag is PlcStruct))
                return;

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select export path";
            folderDialog.SelectedPath = Application.StartupPath + "\\Export";

            DialogResult res = folderDialog.ShowDialog();
            // ok now save
            if (res == DialogResult.OK)
            {
                try
                {
                    // for plcBlocks like OB,FB,FC
                    if (treeView1.SelectedNode.Tag is PlcBlock)
                    {
                        PlcBlock block = (PlcBlock)treeView1.SelectedNode.Tag;

                        string fPath = Application.StartupPath + "\\Export\\plcBlock_" + block.ProgrammingLanguage.ToString() + "_" + block.Name + ".xml";
                        fPath = getNextFileName(fPath);

                        FileInfo f = new FileInfo(fPath);
                        block.Export(f, ExportOptions.None);

                        MessageBox.Show("File " + f + " has beed exported",
                                        "Export",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }

                    // fpr data types
                    if (treeView1.SelectedNode.Tag is PlcStruct)
                    {
                        PlcStruct block = (PlcStruct)treeView1.SelectedNode.Tag;

                        string fPath = Application.StartupPath + "\\Export\\plcBlock_" + block.Name + ".xml";
                        fPath = getNextFileName(fPath);

                        FileInfo f = new FileInfo(fPath);
                        block.Export(f, ExportOptions.None);

                        MessageBox.Show("File " + f + " has beed exported",
                                        "Export",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                            "Exception",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                }
            }
        }

        //helper to generate unique name for the export
        private string getNextFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            int i = 0;
            while (File.Exists(fileName))
            {
                if (i == 0)
                    fileName = fileName.Replace(extension, "(" + ++i + ")" + extension);
                else
                    fileName = fileName.Replace("(" + i + ")" + extension, "(" + ++i + ")" + extension);
            }
            return fileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (software != null)
            {
                cFunctionGroups func = new cFunctionGroups();
                List<PlcBlock> list = new List<PlcBlock>();
                //list = getAllBlocks(software.BlockGroup, list);

                List<PlcType> list2 = new List<PlcType>();
                //list2 = getAllDataTypes(software.TypeGroup, list2);

                List<string> list3 = new List<string>();
                list3 = func.GetAllBlocksNames(software.BlockGroup, list3);
            }
        }

    }
}
