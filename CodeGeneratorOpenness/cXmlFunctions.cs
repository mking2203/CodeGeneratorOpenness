using System;
using System.Collections.Generic;
using System.Xml;

namespace CodeGeneratorOpenness
{
    class cXmlFunctions
    {
        public Dictionary<string,int> GetLimitIDs(XmlDocument XMLDocument)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            int uidLow = 9999;
            int uidHigh = 0;
            int idLow = 9999;
            int idHigh = 0;

            XmlNodeList tmp = XMLDocument.SelectNodes("//*"); // match every element
            foreach (XmlNode n in tmp)
            {

                int x;

                if (n.Attributes["UId"] != null)
                {
                    x = Convert.ToInt32(n.Attributes["UId"].Value);

                    if (x > 0)
                    {
                        if (x > uidHigh) uidHigh = x;
                        if (x < uidLow) uidLow = x;
                    }
                }

                if (n.Attributes["ID"] != null)
                {
                    x = Convert.ToInt32(n.Attributes["ID"].Value, 16);

                    if (x > 0)
                    {
                        if (x > idHigh) idHigh = x;
                        if (x < idLow) idLow = x;
                    }
                }
            }

            result.Add("minUID", uidLow);
            result.Add("maxUID", uidHigh);
            result.Add("minID", idLow);
            result.Add("maxID", idHigh);

            return result;
        }

    }
}
