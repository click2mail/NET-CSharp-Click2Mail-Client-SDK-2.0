using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2mAPI.restAPI
{
    public class Document
    {
        private WebPosts webPosts;
        public Authentication Auth;
         private Tools tools;
        public int DocumentId { get; set; }  
        public Document(Authentication auth)
        {
            this.Auth = auth;
            this.webPosts = new WebPosts(auth);
            this.tools = new Tools();
        }
        public int createDocument(string location, ProductOptionsItem po, string documentName = "", string format = "PDF")
        {
            System.Collections.Specialized.NameValueCollection x = new System.Collections.Specialized.NameValueCollection();
            if (documentName == "")
            {
                documentName = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            }
            bool url = false;
            if(location.StartsWith("http"))
            {
                url = true;
                x.Add("url", location);
            }
            x.Add("documentName", documentName);
            x.Add("documentClass", po.DocumentClass);
            x.Add("documentFormat", format);
            string results = "";
            if (!url)
            {
                 results= webPosts.callAPIWithFileParam(Auth.getRestUrl() + "/molpro/documents/", location, "file", x);
            }
            else
            {
                results = webPosts.createRestCall(Auth.getRestUrl() + "/molpro/documents/", x,Method.POST);
            }
            int documentId = Int32.Parse(tools.parseReturnxml(results, "id"));
            this.DocumentId = documentId;
            return documentId;
        }
        public string getDocument(int numberOfDocuments,int offset, string documentClass,string key)
        {
            System.Collections.Specialized.NameValueCollection x = new System.Collections.Specialized.NameValueCollection();
            x.Add("numberOfDocuments", numberOfDocuments.ToString());
            x.Add("offset", offset.ToString());
            x.Add("documentClass", documentClass);
            x.Add("key", documentClass);
            string results = webPosts.createRestCall("/molpro/documents", x, Method.GET);
            return results;
        }

    }
}
