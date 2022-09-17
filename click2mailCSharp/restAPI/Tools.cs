using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace c2mAPI

{
 public class Tools
    {
        public string parseReturnxml(string strxml, string lookfor)
        {
            Console.WriteLine(strxml + ", lookfor: " + lookfor);

            string s = "0";

            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(strxml)))
            {

                //            reader.ReadToFollowing(lookfor)
                //reader.MoveToFirstAttribute()
                //Dim genre As String = reader.Value
                //output.AppendLine("The genre value: " + genre)

                reader.ReadToFollowing(lookfor);
                s = reader.ReadElementContentAsString();
                reader.Close();
            }
            return s;
        }
    }
}
