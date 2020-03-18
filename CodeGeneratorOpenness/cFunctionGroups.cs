///
/// Sample applicatin for automated code generation for Siemens TIA Portal with Openness Interface
/// 
/// by Mark König @ 02/2020
/// 
/// cFunctionGroup contains some functions for blocks (recursive)
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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

namespace CodeGeneratorOpenness
{
    class cFunctionGroups
    {
        public void AddPlcBlocks(PlcBlockGroup plcGroup, TreeNode node)
        {
            // first add all plc blocks
            foreach (PlcBlock plcBlock in plcGroup.Blocks)
            {
                TreeNode n = new TreeNode(plcBlock.Name);
                n.Tag = plcBlock;

                if (plcBlock is OB)
                {
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
                    n.ImageIndex = 4;
                }
                else if (plcBlock is InstanceDB)
                {
                    n.ImageIndex = 5;
                    InstanceDB db = (InstanceDB)plcBlock;
                    if (db.ProgrammingLanguage == ProgrammingLanguage.F_DB)
                    {
                        n.BackColor = Color.Yellow;
                        n.ImageIndex = 6;
                    }
                }
                else if (plcBlock is GlobalDB)
                {
                    n.ImageIndex = 5;
                    GlobalDB db = (GlobalDB)plcBlock;
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
    }
}
