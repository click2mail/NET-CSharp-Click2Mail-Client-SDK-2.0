using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace c2mAPI.restAPI
{
    public class AddressCorrection
    {
        private WebPosts webPosts;
        public Authentication Auth;

        public AddressCorrection(Authentication auth)
        {
            this.Auth = auth;
            this.webPosts = new WebPosts(auth);
        }
        public List<AddressItem> CorrectList(List<AddressItem> al)
        {
            
            string results = webPosts.createXMLPost("/molpro/addressCorrection", createXMLFromAddressList(al));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(results);
            var list = doc.SelectNodes("//addresses/address");
            for (int i=0;i<list.Count ;i++)
            {
                al[i].Address1 = list[i].SelectSingleNode("address1").InnerText;
                al[i].Address2 = list[i].SelectSingleNode("address2").InnerText;
                al[i].City = list[i].SelectSingleNode("city").InnerText;
                al[i].State = list[i].SelectSingleNode("state").InnerText;
                al[i].Zip = list[i].SelectSingleNode("zip").InnerText;
                al[i].CountryNonUS = list[i].SelectSingleNode("country").InnerText;
                al[i].FineCorrectionMessage = list[i].SelectSingleNode("fineCorrectionMessage").InnerText;
                al[i].CoarseCorrectionMessage = list[i].SelectSingleNode("coarseCorrectionMessage").InnerText;
                try
                {
                    al[i].Standard = bool.Parse(list[i].SelectSingleNode("standard").InnerText);
                }
                catch (Exception ex) { }
                try
                {
                    al[i].International = bool.Parse(list[i].SelectSingleNode("international").InnerText);
                }
                catch(Exception ex)
                {

                }
                try
                {
                    al[i].NonMailable = bool.Parse(list[i].SelectSingleNode("nonMailable").InnerText);
                }
                catch (Exception ex)
                {

                }
                
            }
            return al;
        }

        

        private string createXMLFromAddressList(List<AddressItem> ai)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            //create nodes
            System.Xml.XmlElement root = doc.CreateElement("addresses");


            foreach (AddressItem a in ai)
            {
                System.Xml.XmlElement address = doc.CreateElement("address");
                System.Xml.XmlElement name = doc.CreateElement("First_name");
                name.InnerXml = a.FirstName + ' ' + a.LastName;
                address.AppendChild(name);
                System.Xml.XmlElement Address1 = doc.CreateElement("address1");
                Address1.InnerXml = a.Address1;
                address.AppendChild(Address1);
                System.Xml.XmlElement Address2 = doc.CreateElement("address2");
                Address2.InnerXml = a.Address2;
                address.AppendChild(Address2);
                System.Xml.XmlElement Address3 = doc.CreateElement("address3");
                Address3.InnerXml = a.Address3;
                address.AppendChild(Address3);
                System.Xml.XmlElement City = doc.CreateElement("city");
                City.InnerXml = a.City;
                address.AppendChild(City);
                System.Xml.XmlElement State = doc.CreateElement("state");
                State.InnerXml = a.State;
                address.AppendChild(State);
                System.Xml.XmlElement zip = doc.CreateElement("zip");
                zip.InnerXml = a.Zip;
                address.AppendChild(zip);
                



               root.AppendChild(address);
            }

            doc.AppendChild(root);
            string xmlString = null;
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                using (XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter, settings))
                {
                    doc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();

                    xmlString = stringWriter.GetStringBuilder().ToString();
                }
            }
            return xmlString;

        }

    }
}
