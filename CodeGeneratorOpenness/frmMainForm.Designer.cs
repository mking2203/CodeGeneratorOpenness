namespace CodeGeneratorOpenness
{
    partial class frmMainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainForm));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ctxBlock = new System.Windows.Forms.ContextMenu();
            this.mnuBlockDelete = new System.Windows.Forms.MenuItem();
            this.ctxGroup = new System.Windows.Forms.ContextMenu();
            this.mbuGroupAdd = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.mnuGroupDelete = new System.Windows.Forms.MenuItem();
            this.ctxSoftware = new System.Windows.Forms.ContextMenu();
            this.mnuSofwareAdd = new System.Windows.Forms.MenuItem();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(36, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 43);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open project";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(36, 88);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 43);
            this.button2.TabIndex = 1;
            this.button2.Text = "Close project";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(162, 39);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(186, 95);
            this.listBox1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(159, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Device";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(36, 165);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(97, 43);
            this.button3.TabIndex = 4;
            this.button3.Text = "Languages";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(162, 153);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(186, 95);
            this.listBox2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(159, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "CPU";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(36, 214);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(97, 43);
            this.button4.TabIndex = 7;
            this.button4.Text = "Import FC";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(362, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Blocks";
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(365, 23);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(442, 390);
            this.treeView1.TabIndex = 10;
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Document_16x.png");
            this.imageList1.Images.SetKeyName(1, "FolderClosed_16x.png");
            this.imageList1.Images.SetKeyName(2, "OB.png");
            this.imageList1.Images.SetKeyName(3, "FB.png");
            this.imageList1.Images.SetKeyName(4, "FC.png");
            this.imageList1.Images.SetKeyName(5, "DB.png");
            this.imageList1.Images.SetKeyName(6, "safeDB.png");
            // 
            // ctxBlock
            // 
            this.ctxBlock.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuBlockDelete});
            // 
            // mnuBlockDelete
            // 
            this.mnuBlockDelete.Index = 0;
            this.mnuBlockDelete.Text = "Delete block";
            this.mnuBlockDelete.Click += new System.EventHandler(this.mnuBlockDelete_Click);
            // 
            // ctxGroup
            // 
            this.ctxGroup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mbuGroupAdd,
            this.menuItem3,
            this.mnuGroupDelete});
            // 
            // mbuGroupAdd
            // 
            this.mbuGroupAdd.Index = 0;
            this.mbuGroupAdd.Text = "Add group";
            this.mbuGroupAdd.Click += new System.EventHandler(this.menuGroupAdd_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "-";
            // 
            // mnuGroupDelete
            // 
            this.mnuGroupDelete.Index = 2;
            this.mnuGroupDelete.Text = "Delete group";
            this.mnuGroupDelete.Click += new System.EventHandler(this.menuGroupDelete_Click);
            // 
            // ctxSoftware
            // 
            this.ctxSoftware.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuSofwareAdd});
            // 
            // mnuSofwareAdd
            // 
            this.mnuSofwareAdd.Index = 0;
            this.mnuSofwareAdd.Text = "Add group";
            this.mnuSofwareAdd.Click += new System.EventHandler(this.menuSofwareAdd_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(365, 419);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 11;
            this.button5.Text = "Reload";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // frmMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 450);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Code Gererator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TreeView treeView1;
        internal System.Windows.Forms.ContextMenu ctxBlock;
        internal System.Windows.Forms.MenuItem mnuBlockDelete;
        internal System.Windows.Forms.ContextMenu ctxGroup;
        internal System.Windows.Forms.MenuItem mnuGroupDelete;
        private System.Windows.Forms.MenuItem mbuGroupAdd;
        private System.Windows.Forms.MenuItem menuItem3;
        internal System.Windows.Forms.ContextMenu ctxSoftware;
        internal System.Windows.Forms.MenuItem mnuSofwareAdd;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ImageList imageList1;
    }
}

