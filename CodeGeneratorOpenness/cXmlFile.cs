using System;
using System.Xml;

namespace ConsoleApp1
{
    public class cXmlFile
    {
        public cXmlFile(string FileName)
        {
            try
            {
                xDoc = new XmlDocument();
                xDoc.Load(FileName);

                // get version of the file
                XmlNode n1 = xDoc.SelectSingleNode("//Engineering");
                Version = n1.Attributes["version"].Value;

                // search the document information
                n1 = xDoc.SelectSingleNode("//Document");
                foreach (XmlNode y in n1)
                {
                    if (y.Name.StartsWith("SW.Blocks."))
                    {
                        Type = y.Name.Substring(10);

                        XmlNode attr = FindChildNodeByName(y, "AttributeList");
                        XmlNode nNode = FindChildNodeByName(attr, "Name");
                        Name = nNode.InnerText;
                    }
                }

                GetLimitIDs();
                Valid = true;
            }
            catch
            {
                Valid = false;
                xDoc = null;
            }
        }

        private static XmlNode FindChildNodeByName(XmlNode Node, string ChildName)
        {
            foreach (XmlNode x in Node.ChildNodes)
            {
                if (x.Name == ChildName)
                    return x;
            }
            return null;
        }

        public void GetLimitIDs()
        {
            XmlNodeList tmp = xDoc.SelectNodes("//*"); // match every element
            foreach (XmlNode n in tmp)
            {
                int x;

                // search UID's
                if (n.Attributes["UId"] != null)
                {
                    x = Convert.ToInt32(n.Attributes["UId"].Value);
                    if (x >= 0)
                    {
                        if (x > uidHigh) uidHigh = x;
                        if (x < uidLow) uidLow = x;
                    }
                }

                // search ID's
                if (n.Attributes["ID"] != null)
                {
                    x = Convert.ToInt32(n.Attributes["ID"].Value, 16);

                    // ignore main block
                    if (n.Name != "SW.Blocks.FB")
                    {
                        if (x >= 0)
                        {
                            if (x > idHigh) idHigh = x;
                            if (x < idLow) idLow = x;
                        }
                    }
                }
            }
        }

        public void ChangeIDs(int UID_Offset, int ID_Offset)
        {
            XmlNodeList tmp = xDoc.SelectNodes("//*"); // match every element
            foreach (XmlNode n in tmp)
            {
                int x;

                if (n.Attributes["UId"] != null)
                {
                    x = Convert.ToInt32(n.Attributes["UId"].Value);
                    x = x + UID_Offset;
                    n.Attributes["UId"].Value = x.ToString();
                }

                if (n.Attributes["ID"] != null)
                {
                    // ignore main block
                    if (n.Name != "SW.Blocks.FB")
                    {
                        x = Convert.ToInt32(n.Attributes["ID"].Value, 16);
                        x = x + ID_Offset;
                        n.Attributes["ID"].Value = x.ToString("X");
                    }
                }
            }
            GetLimitIDs();
        }

        public bool Valid = false;
        public string Name = string.Empty;
        public string Type = string.Empty;
        public string Version = string.Empty;

        public int uidLow = 9999;
        public int uidHigh = -1;
        public int idLow = 9999;
        public int idHigh = -1;

        private XmlDocument xDoc;

        public XmlDocument XmlDocument { get => xDoc; }
    }
}
