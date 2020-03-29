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
using System.Xml.Serialization;

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

            frmTranslate();
        }

        private void frmTranslate()
        {
            string culture = (string)Properties.Settings.Default["Language"];
            cTranlate translate = new cTranlate(this, culture);
            translate.TranslateContext(ctxBlock, culture);
            translate.TranslateContext(ctxGroup, culture);
            translate.TranslateContext(ctxSoftware, culture);

            englishToolStripMenuItem.Checked = false;
            germanToolStripMenuItem.Checked = false;

            if (culture == "DE") germanToolStripMenuItem.Checked = true;
            if (culture == "EN") englishToolStripMenuItem.Checked = true;
        }

        private void frmMainForm_Closing(object sender, FormClosingEventArgs e)
        {
            // dispose objects
            software = null;
            project = null;

            if (tiaPortal != null)
                tiaPortal.Dispose();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(this, new EventArgs());
        }

        private void IterateThroughDevices(Project project)
        {
            // no project is open
            if (project == null)
                return;

            // shop a form to indicate work
            frmReadStructure read = new frmReadStructure();
            read.Show();
            read.BringToFront();

            //Console.WriteLine(String.Format("Iterate through {0} device(s)", project.Devices.Count));
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            Application.DoEvents();

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
                                    groups.LoadTreeView(treeView1, software);
                                }
                            }
                        }
                    }
                }
            }

            // close form
            read.Close();
            read.Dispose();

            this.BringToFront();
            Application.DoEvents();

        }

        private void btnLanguage_Click(object sender, EventArgs e)
        {
            // language test for DE/EN
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

        private void btnReload_Click(object sender, EventArgs e)
        {
            IterateThroughDevices(project);
        }

        private string GetNextFileName(string fileName)
        {
            //helper to generate unique name for the export
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (project != null)
            {
                if (project.IsModified)
                    txtSaved.Text = "MODIFIED";
                else
                    txtSaved.Text = "is saved";
            }
            else
                txtSaved.Text = "no project open";
        }

        private void SaveProject()
        {
            if (project != null)
            {
                if (project.IsModified)
                {
                    if (MessageYesNo("Do you want to save the changes?", "Save changes") == DialogResult.Yes)
                        project.Save();
                }
            }
        }

        private void CompileProject()
        {
            if (software != null)
            {
                ICompilable compileService = software.GetService<ICompilable>();
                CompilerResult result = compileService.Compile();

                this.BringToFront();
                Application.DoEvents();

                // result messages is array
                MessageOK("Result : " + result.State.ToString() + "\n" +
                                "Errors: " + result.ErrorCount.ToString() + "\n" +
                                "Warnings: " + result.WarningCount.ToString() + "\n",
                                "Compiler");

                IterateThroughDevices(project);
            }
        }

        private void TiaPortal_Confirmation(object sender, ConfirmationEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TiaPortal_Notification(object sender, NotificationEventArgs e)
        {
            if (e.Caption == "Export Completed")
                e.IsHandled = true;

            // maybe need some more work
            if (e.Caption == "Import completed with warnings")
                e.IsHandled = true;

            //throw new NotImplementedException();
        }

        #region TreeView

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            // moue click on treeview

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

        private void mnuBlockDelete_Click(object sender, EventArgs e)
        {
            // delete block / data type
            if (treeView1.SelectedNode.Tag is PlcBlock)
            {
                PlcBlock block = (PlcBlock)treeView1.SelectedNode.Tag;

                if (MessageYesNo("Do you really want to delete the block " + block.Name + "?", "Delete block") == DialogResult.Yes)
                {
                    try
                    {
                        block.Delete();
                        IterateThroughDevices(project);
                    }
                    catch (Exception ex)
                    {
                        MessageError(ex.Message, "Exception");
                    }
                }
            }
            else if (treeView1.SelectedNode.Tag is PlcStruct)
            {
                PlcStruct block = (PlcStruct)treeView1.SelectedNode.Tag;

                if (MessageYesNo("Do you really want to delete the data type " + block.Name + "?", "Delete data type") == DialogResult.Yes)
                {
                    try
                    {
                        block.Delete();
                        IterateThroughDevices(project);
                    }
                    catch (Exception ex)
                    {
                        MessageError(ex.Message, "Exception");
                    }
                }
            }
        }

        private void menuGroupDelete_Click(object sender, EventArgs e)
        {
            // delete group
            PlcBlockGroup group = (PlcBlockGroup)treeView1.SelectedNode.Tag;

            if (group.Blocks.Count > 0)
            {
                MessageOK("The group " + group.Name + " has sub blocks!\nDelete this blocks first", "Delete group");
                return;
            }

            if (group.Groups.Count > 0)
            {
                MessageOK("The group " + group.Name + " has sub groups!\nDelete this groups first", "Delete group");
                return;
            }

            if (MessageYesNo("Do you really want to delete the group " + group.Name + "?", "Delete block") == DialogResult.Yes)
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
                    if (!groups.GroupExists(name, group))
                    {
                        try
                        {
                            group.Groups.Create(name);
                            IterateThroughDevices(project);
                        }
                        catch (Exception ex)
                        {
                            MessageError(ex.Message, "Exception");
                        }
                    }
                    else
                        MessageError(name + " exist already", "Name exist already");

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
                    if (!groups.GroupExists(name, group))
                    {
                        try
                        {
                            group.Groups.Create(name);
                            IterateThroughDevices(project);
                        }
                        catch (Exception ex)
                        {
                            MessageError(ex.Message, "Exception");
                        }
                    }
                    else
                        MessageError(name + " exist already", "Name exist already");
                }
            }
        }

        #endregion

        #region Dialog messageBox

        private DialogResult MessageYesNo(string Message, string Title)
        {
            this.BringToFront();
            Application.DoEvents();

            return MessageBox.Show(Message, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private DialogResult MessageYesNoCancel(string Message, string Title)
        {
            this.BringToFront();
            Application.DoEvents();

            return MessageBox.Show(Message, Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        private DialogResult MessageOK(string Message, string Title)
        {
            this.BringToFront();
            Application.DoEvents();

            return MessageBox.Show(Message, Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private DialogResult MessageError(string Message, string Title)
        {
            this.BringToFront();
            Application.DoEvents();

            return MessageBox.Show(Message, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Menu items

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // if no project is open
            if (project == null)
            {
                // see if we have a open instance and use the first one
                foreach (TiaPortalProcess tiaPortalProcess in TiaPortal.GetProcesses())
                {
                    TiaPortalProcess p = TiaPortal.GetProcess(tiaPortalProcess.Id);

                    tiaPortal = p.Attach();
                    break;
                }

                // we don't habe an instance then open a new instance
                if (tiaPortal == null)
                {
                    tiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);
                }

                tiaPortal.Notification += TiaPortal_Notification;
                tiaPortal.Confirmation += TiaPortal_Confirmation;

                // let's get the projects
                ProjectComposition projects = tiaPortal.Projects;

                // no open project - then open file dialog
                if (projects.Count == 0)
                {
                    string p = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;


                    string filePath = (string)Properties.Settings.Default["PathOpenProject"];
                    if (filePath == string.Empty) filePath = Application.StartupPath;

                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "V16 project files (*.ap16)|*.ap16|All files (*.*)|*.*";
                        openFileDialog.FilterIndex = 1;
                        openFileDialog.InitialDirectory = filePath;

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Properties.Settings.Default["PathOpenProject"] = openFileDialog.FileName;
                            Properties.Settings.Default.Save();

                            // load projectpath
                            FileInfo projectPath = new FileInfo(openFileDialog.FileName);
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

                txtProject.Text = "Project: " + project.Name;

                // loop through the data
                IterateThroughDevices(project);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompileProject();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                SaveProject();
            }
            Application.Exit();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project != null)
            {
                SaveProject();

                listBox1.Items.Clear();
                listBox2.Items.Clear();

                treeView1.Nodes.Clear();

                txtProject.Text = "Project: ";
                txtSaved.Text = "...";

                project.Close();

                software = null;
                project = null;
            }
        }

        private void blocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (software == null)
                return;

            DialogResult res;

            if (treeView1.SelectedNode != null)
            {
                var sel = treeView1.SelectedNode.Tag;
                if (sel is PlcBlockGroup)
                {
                    try
                    {
                        string importPath = (string)Properties.Settings.Default["PathImportBlock"];
                        if (importPath == string.Empty) importPath = Application.StartupPath + "\\Import";

                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                            openFileDialog.FilterIndex = 1;
                            openFileDialog.InitialDirectory = importPath;

                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                Properties.Settings.Default["PathImportBlock"] = openFileDialog.FileName;
                                Properties.Settings.Default.Save();

                                PlcBlockGroup group = (PlcBlockGroup)sel;
                                cImportBlock f = new cImportBlock(openFileDialog.FileName);
                                if (f.BlockName != string.Empty)
                                {
                                    // check if the data type exists
                                    if (!groups.NameExists(f.BlockName, software))
                                    {
                                        // import the file
                                        group.Blocks.Import(f.XmlFileInfo, ImportOptions.None);
                                        IterateThroughDevices(project);
                                    }
                                    else
                                    {
                                        // overwrite? yes = overwrite / no = new name / cancel = just cancel
                                        res = MessageBox.Show("Data block " + f.BlockName + " exists already. Overwrite(Yes) or Rename(No) ?",
                                                              "Overwrite / Rename",
                                                              MessageBoxButtons.YesNoCancel,
                                                              MessageBoxIcon.Question);

                                        if (res == DialogResult.Yes)
                                        {
                                            // overwrite plc block
                                            group.Blocks.Import(f.XmlFileInfo, ImportOptions.Override);
                                            IterateThroughDevices(project);
                                        }
                                        else if (res == DialogResult.No)
                                        {
                                            // with a different name we need to save a copy 
                                            res = DialogResult.OK;
                                            string newName = f.BlockName;

                                            while (groups.NameExists(newName, software) && res == DialogResult.OK)
                                            {
                                                res = Input.InputBox("New block name", "Enter a new block name", ref newName);
                                            }
                                            // we don't cancel, so import with new name
                                            if (res == DialogResult.OK)
                                            {
                                                f.BlockName = newName;
                                                f.SaveXml(Application.StartupPath + "\\Temp\\temp.xml");

                                                group.Blocks.Import(f.XmlFileInfo, ImportOptions.None);
                                                IterateThroughDevices(project);
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    MessageOK("The file " + Path.GetFileName(openFileDialog.FileName) + " is not PLC block",
                                              "Not a PLC block file");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageError(ex.Message,
                                     "Exception");
                    }
                }
                else
                {
                    MessageOK("Not a group selected!",
                              "Not a group");
                }
            }
            else
            {
                MessageOK("Nothing selected!",
                          "Select a group");
            }
        }

        private void exportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // TODO : culture
            if (project != null)
            {
                try
                {
                    string filePath = (string)Properties.Settings.Default["PathLanguageText"];
                    if (filePath == string.Empty) filePath = Application.StartupPath + "\\Export";

                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "XML files (*.xlsx)|*.xlsx";
                        saveFileDialog.FilterIndex = 1;
                        saveFileDialog.InitialDirectory = filePath;
                        saveFileDialog.FileName = "TIAProjectTexts.xlsx";
                        saveFileDialog.OverwritePrompt = false;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Properties.Settings.Default["PathImportBlock"] = saveFileDialog.FileName;
                            Properties.Settings.Default.Save();

                            // the API can not overwrite
                            filePath = GetNextFileName(saveFileDialog.FileName);
                            project.ExportProjectTexts(new FileInfo(filePath), new CultureInfo("de-DE"), new CultureInfo("en-GB"));

                            MessageOK("File " + Path.GetFileName(filePath) + " has been exported",
                                      "Export language file");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageError(ex.Message,
                                 "Exception");
                }
            }
        }

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // TODO : culture
            if (project != null)
            {
                try
                {
                    string filePath = (string)Properties.Settings.Default["PathLanguageText"];
                    if (filePath == string.Empty) filePath = Application.StartupPath + "\\Export";

                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "XML files (*.xlsx)|*.xlsx";
                        openFileDialog.FilterIndex = 1;
                        openFileDialog.InitialDirectory = filePath;
                        openFileDialog.FileName = "TIAProjectTexts.xlsx";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            project.ImportProjectTexts(new FileInfo(openFileDialog.FileName), true);

                            MessageOK("File " + Path.GetFileName(openFileDialog.FileName) + " has been imported",
                                      "Import language file");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageError(ex.Message,
                                 "Exception");
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
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

                        if (block.IsConsistent)
                        {
                            string fPath = Application.StartupPath + "\\Export\\" +
                                block.ProgrammingLanguage.ToString() + "_" +
                                block.Name + "_" +
                                "V" + block.HeaderVersion.ToString() +
                                ".xml";
                            fPath = GetNextFileName(fPath);

                            FileInfo f = new FileInfo(fPath);
                            block.Export(f, ExportOptions.None);

                            MessageOK("File " + Path.GetFileName(fPath) + " has beed exported",
                                      "Export");
                        }
                        else
                            MessageError("Block " + block.Name + " is not consistent. Please compile",
                                      "Export");
                    }

                    // for data types
                    if (treeView1.SelectedNode.Tag is PlcStruct)
                    {
                        PlcStruct block = (PlcStruct)treeView1.SelectedNode.Tag;

                        if (block.IsConsistent)
                        {
                            string fPath = Application.StartupPath + "\\Export\\plcType_" +
                                block.Name + "_" +
                                block.ModifiedDate.ToShortDateString() +
                                ".xml";
                            fPath = GetNextFileName(fPath);

                            FileInfo f = new FileInfo(fPath);
                            block.Export(f, ExportOptions.None);

                            MessageOK("File " + Path.GetFileName(fPath) + " has beed exported",
                                      "Export");
                        }
                        else
                            MessageError("Data type " + block.Name + " is not consistent. Please compile",
                                      "Export");
                    }
                }
                catch (Exception ex)
                {
                    MessageError(ex.Message,
                                 "Exception");
                }
            }
        }

        private void dataTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (software == null)
                return;

            try
            {
                string fPath = string.Empty;
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
                                    MessageOK("Data type " + Path.GetFileName(fPath) + " has been imported",
                                              "Import language file");

                                    IterateThroughDevices(project);
                                }
                                else
                                {
                                    // overwrite?
                                    if (MessageYesNo("Data type " + name + " exists already. Overwrite ?", "Overwrite") == DialogResult.OK)
                                    {
                                        // overwrite data type
                                        software.TypeGroup.Types.Import(f, ImportOptions.Override);
                                        MessageOK("Data type " + Path.GetFileName(fPath) + " has been imported",
                                      "Import language file");

                                        IterateThroughDevices(project);
                                    }
                                }
                            }
                            else
                                // plc block exist
                                MessageError("PLC block with the name " + name + " exist!",
                                             "Name exits");
                        }
                        else
                        {
                            // wrong data type
                            MessageError("Wrong XML file (PlcStruct) ?",
                                         "Wrong file");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageError(ex.Message,
                             "Exception");
            }
        }

        #endregion

        private void btnAddPath_Click(object sender, EventArgs e)
        {
            if (software != null)
            {
                string[] lst = txtPath.Text.Split('\\');
                PlcBlockGroup group = software.BlockGroup;

                foreach (string p in lst)
                {
                    string name = p.Trim();
                    if (!groups.GroupExists(name, group))
                    {
                        try
                        {
                            // create new group
                            group.Groups.Create(name);

                        }
                        catch (Exception ex)
                        {
                            MessageError(ex.Message, "Exception");
                            break;
                        }
                    }
                    else
                    {
                        // grop exist already
                    }

                    // into next group
                    group = group.Groups.Find(name);
                }

                IterateThroughDevices(project);
            }
        }

        private void btnXML_Click(object sender, EventArgs e)
        {
            string fPath = Application.StartupPath + "\\Export\\LAD_Baustein_19_V0.1(3).xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fPath);

            XmlNode blocks = xmlDoc.SelectSingleNode("//SW.Blocks.FC//ObjectList");
            XmlNodeList xmlNodes = blocks.SelectNodes("MultilingualText");
            foreach (XmlNode n in xmlNodes)
            {
                MemoryStream stm = new MemoryStream();

                StreamWriter stw = new StreamWriter(stm);
                stw.Write(n.OuterXml);
                stw.Flush();
                stm.Position = 0;

                XmlSerializer ser = new XmlSerializer(typeof(MultilingualText));
                MultilingualText result = (ser.Deserialize(stm) as MultilingualText);

                Console.WriteLine(result.ObjectList[0].AttributeList.Text);
            }

            XmlNodeList comps = blocks.SelectNodes("SW.Blocks.CompileUnit");
            foreach (XmlNode c in comps)
            {
                blocks = c.SelectSingleNode("ObjectList");
                xmlNodes = blocks.SelectNodes("MultilingualText");
                foreach (XmlNode n in xmlNodes)
                {
                    MemoryStream stm = new MemoryStream();

                    StreamWriter stw = new StreamWriter(stm);
                    stw.Write(n.OuterXml);
                    stw.Flush();
                    stm.Position = 0;

                    XmlSerializer ser = new XmlSerializer(typeof(MultilingualText));
                    MultilingualText result = (ser.Deserialize(stm) as MultilingualText);

                    Console.WriteLine(result.ObjectList[0].AttributeList.Text);
                }
            }
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Language"] = "EN";
            Properties.Settings.Default.Save();

            frmTranslate();
        }

        private void germanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Language"] = "DE";
            Properties.Settings.Default.Save();

            frmTranslate();
        }
    }
}

