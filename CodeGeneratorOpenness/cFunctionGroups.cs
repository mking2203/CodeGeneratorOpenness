///
/// Sample applicatin for automated code generation for Siemens TIA Portal with Openness Interface
/// 
/// by Mark König @ 02/2020
/// 
/// cFunctionGroup contains some functions for blocks (recursive)
///

using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Types;

namespace CodeGeneratorOpenness
{
    class cFunctionGroups
    {
        public void LoadTreeView(TreeView Tree, PlcSoftware Software)
        {
            Tree.Nodes.Clear();

            // start update treeview
            Tree.BeginUpdate();

            // add root node
            TreeNode root = new TreeNode(Software.Name);
            root.Tag = Software.BlockGroup;
            Tree.Nodes.Add(root);

            AddPlcBlocks(Software.BlockGroup, root);

            // add data types
            TreeNode dataTypes = new TreeNode("Data types");
            dataTypes.Tag = Software.TypeGroup;
            Tree.Nodes.Add(dataTypes);

            AddPlcTypes(Software.TypeGroup, dataTypes);
            dataTypes.Expand();

            // end update
            Tree.EndUpdate();

            root.Expand();
        }
        public void AddPlcBlocks(PlcBlockGroup plcGroup, TreeNode node)
        {
            // first add all plc blocks
            foreach (PlcBlock plcBlock in plcGroup.Blocks)
            {
                TreeNode n = null;

                if (plcBlock is OB)
                {
                    n = new TreeNode(plcBlock.Name + " [OB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    //n.ToolTipText = "Version " + plcBlock.HeaderVersion.ToString();
                    n.ImageIndex = 2;

                    OB ob = (OB)plcBlock;
                    if (ob.SecondaryType.Contains("Safe"))
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 7;
                    }
                }
                else if (plcBlock is FB)
                {
                    n = new TreeNode(plcBlock.Name + " [FB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 3;

                    FB fb = (FB)plcBlock;
                    if ((fb.ProgrammingLanguage == ProgrammingLanguage.F_LAD) ||
                        (fb.ProgrammingLanguage == ProgrammingLanguage.F_FBD))
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 8;
                    }
                }
                else if (plcBlock is FC)
                {
                    n = new TreeNode(plcBlock.Name + " [FC" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 4;
                }
                else if (plcBlock is InstanceDB)
                {
                    n = new TreeNode(plcBlock.Name + " [DB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 5;

                    InstanceDB db = (InstanceDB)plcBlock;
                    n.Name = db.Name + "[DB" + db.Number.ToString() + "]";
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 6;
                    }
                }
                else if (plcBlock is GlobalDB)
                {
                    n = new TreeNode(plcBlock.Name + " [DB" + plcBlock.Number.ToString() + "]");
                    n.Tag = plcBlock;
                    n.ImageIndex = 5;

                    GlobalDB db = (GlobalDB)plcBlock;
                    n.Name = db.Name + "[FB" + db.Number.ToString() + "]";
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 6;
                    }
                }

                n.SelectedImageIndex = n.ImageIndex;
                node.Nodes.Add(n);
            }

            // then add groups and search recursive
            foreach (PlcBlockGroup group in plcGroup.Groups)
            {
                TreeNode n = new TreeNode(group.Name);
                n.Tag = group;
                n.ImageIndex = 1;
                n.SelectedImageIndex = 1;

                AddPlcBlocks(group, n);
                node.Nodes.Add(n);
            }
        }
        public void AddPlcTypes(PlcTypeGroup plcTypeGroup, TreeNode node)
        {
            foreach (PlcType ty in plcTypeGroup.Types)
            {
                TreeNode n = new TreeNode(ty.Name);
                n.Tag = ty;
                n.ImageIndex = 9;
                n.SelectedImageIndex = n.ImageIndex;

                node.Nodes.Add(n);
            }

            // then add groups and search recursive
            foreach (PlcTypeGroup tGroup in plcTypeGroup.Groups)
            {
                TreeNode n = new TreeNode(tGroup.Name);
                n.Tag = tGroup;
                n.ImageIndex = 1;
                n.SelectedImageIndex = 1;

                node.Nodes.Add(n);
                AddPlcTypes(tGroup, n);
            }
        }

        public List<PlcBlock> GetAllBlocks(PlcBlockGroup SearchGroup, List<PlcBlock> result)
        {
            foreach (PlcBlock block in SearchGroup.Blocks)
            {
                result.Add(block);
            }
            foreach (PlcBlockGroup group in SearchGroup.Groups)
            {
                result = GetAllBlocks(group, result);
            }
            return result;
        }
        public List<PlcType> GetAllDataTypes(PlcTypeGroup SearchGroup, List<PlcType> result)
        {
            foreach (PlcType ty in SearchGroup.Types)
            {
                result.Add(ty);
            }
            foreach (PlcTypeGroup group in SearchGroup.Groups)
            {
                result = GetAllDataTypes(group, result);
            }
            return result;
        }
        public List<string> GetAllBlocksNames(PlcBlockGroup SearchGroup, List<string> result)
        {
            foreach (PlcBlock block in SearchGroup.Blocks)
            {
                result.Add(block.Name);
            }
            foreach (PlcBlockGroup group in SearchGroup.Groups)
            {
                result = GetAllBlocksNames(group, result);
            }
            return result;
        }
        public List<string> GetAllDataTypesNames(PlcTypeGroup SearchGroup, List<string> result)
        {
            foreach (PlcType ty in SearchGroup.Types)
            {
                result.Add(ty.Name);
            }
            foreach (PlcTypeGroup group in SearchGroup.Groups)
            {
                result = GetAllDataTypesNames(group, result);
            }
            return result;
        }

        // we need to enumerate through the tree since there is no function in the API?
        public bool NameExists(string Name, PlcSoftware Software)
        {
            List<string> list = new List<string>();
            list = GetAllBlocksNames(Software.BlockGroup, list);
            if (list.Contains(Name)) return true;

            list = new List<string>();
            list = GetAllDataTypesNames(Software.TypeGroup, list);
            if (list.Contains(Name)) return true;

            return false;
        }

        public bool GroupExists(string Name, PlcBlockGroup Group)
        {
            foreach (PlcBlockGroup g in Group.Groups)
            {
                if (g.Name.ToLower() == Name.ToLower()) return true;
            }

            return false;
        }
    }
}
