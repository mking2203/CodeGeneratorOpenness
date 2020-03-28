using System.IO;
using System.Xml;

namespace CodeGeneratorOpenness
{
    public class cImportBlock
    {
        private XmlDocument xmlDoc;
        private XmlNode nameDefination;

        private string blockName;
        private FileInfo xmlFileInfo;

        public cImportBlock(string Filename)
        {
            if (File.Exists(Filename))
            {
                try
                {
                    xmlFileInfo = new FileInfo(Filename);

                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(Filename);

                    // get the version of the document
                    XmlNode bkm = xmlDoc.SelectSingleNode("//Document//Engineering");
                    BlockVersion = bkm.Attributes["version"].Value;

                    // get block type
                    XmlNode document = xmlDoc.SelectSingleNode("//Document");
                    foreach (XmlNode node in document.ChildNodes)
                    {
                        if (node.Name.StartsWith("SW.Blocks."))
                        {
                            BlockType = node.Name.Substring(10);
                            break;
                        }
                    }

                    // we found the type of the software
                    if (BlockType != string.Empty)
                    {
                        nameDefination = xmlDoc.SelectSingleNode("//Document//SW.Blocks." + BlockType + "//AttributeList//Name");
                        blockName = nameDefination.InnerText;

                        XmlNodeList nodes = xmlDoc.GetElementsByTagName("Access");
                        foreach (XmlNode n in nodes)
                        {
                            
                            if(n.Attributes["Scope"].Value == "GlobalVariable")
                            {

                            }
                            else if (n.Attributes["Scope"].Value == "LocalVariable")
                            {

                            }
                        }
                    }
                    else
                    {
                        // no success
                        xmlDoc = null;
                        xmlFileInfo = null;

                        BlockType = string.Empty;
                        BlockVersion = string.Empty;
                        blockName = string.Empty;
                    }
                }
                catch
                {
                    // something went wrong
                    xmlDoc = null;
                    xmlFileInfo = null;

                    BlockType = string.Empty;
                    BlockVersion = string.Empty;
                    blockName = string.Empty;
                }
            }
        }

        public void SaveXml(string FileName)
        {
            if (xmlDoc != null)
            {
                xmlDoc.Save(FileName);
                xmlFileInfo = new FileInfo(FileName);
            }
        }

        public FileInfo XmlFileInfo
        {
            get
            {
                return xmlFileInfo;
            }
        }
        public string BlockVersion { get; } = string.Empty;
        public string BlockName
        {
            get
            {
                return blockName;
            }
            set
            {
                if(xmlDoc != null)
                {
                    blockName = value;
                    nameDefination.InnerText = value;
                }
            }
        }
        public string BlockType { get; } = string.Empty;
    }
}
