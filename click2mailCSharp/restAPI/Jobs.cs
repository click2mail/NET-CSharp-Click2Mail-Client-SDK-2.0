
//using Microsoft.VisualBasic;


using System.Collections.Generic;

using System.Text;
using System.Xml;

using System.IO;
using System;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;
using System.Net.Http;
using c2mAPI.restAPI;
using System.Net;

namespace c2mAPI
{
    public class Jobs
    {


        private System.Threading.CancellationTokenSource tokenSource;

        private Tools tools;
        private WebPosts webPosts;
        private string _addressListName = "";
        private List<AddressItem> _al = new List<AddressItem>();
        public bool CostRan { get; set; } = false;
        public bool PendingCostRun { get; set; } = false;
        public int AddressListId { get; set; }
        public int DocumentId { get; set; }
        public int JobId { get; set; }
        public int ProofId { get; set; }
        public string CompletedDate { get; set; }
        public string ErrorMessage { get; set; }
        public string Invoice { get; set; }
        public string JobIndex { get; set; }
        public string JobStatus { get; set; }
        public string MailingDate { get; set; }
        public string SubmittedDate { get; set; }
        public float Cost { get; set; } = 0;
        public float TotalTax { get; set; } = 0;
        public int StartingPage { get; set; }
        public int EndingPage { get; set; }
        public string DocumentClass { get; set; }
        public string Layout { get; set; }
        public string ProductionTime { get; set; }
        public string Envelope { get; set; }
        public string Color { get; set; }
        public string PaperType { get; set; }
        public string PrintOption { get; set; }
        public string MailClass { get; set; }
        public string RefId { get; set; }
        public FileInfo MergeFile { get; set; }
        public Environment.Mode Mode { get; set; }
        public string StatusDescription { get; set; }
        public List<AddressItem> AddressList { get; set; }
        public ReturnAddressItem ReturnAddress { get; set; }

        public event StatusChangedEventHandler StatusChanged;
        public delegate void StatusChangedEventHandler(string Reason);
        public event JobStatusCheckEventHandler JobStatusCheck;
        public delegate void JobStatusCheckEventHandler(string id, string status, string description);

        public event JobCostCheckEventHandler JobCostCheck;
        public delegate void JobCostCheckEventHandler(int JobId, float cost, float taxCost, string refId);
        public Authentication Auth;
        
        public Jobs(Authentication auth)
        {
            this.Auth = auth;
            this.webPosts = new WebPosts(auth);
            this.tools = new Tools();
        }



        public Jobs(int startingPage, int endingPage, string documentClass, string layout, string productionTime, string envelope, string color, string paperType, string printOption, string mailClass,
        List<AddressItem> addressList, ReturnAddressItem returnAddress = null, string refId = null)
        {
            this.StartingPage = startingPage;
            this.EndingPage = endingPage;
            this.DocumentClass = documentClass;
            this.Layout = layout;
            this.ProductionTime = productionTime;
            this.Envelope = envelope;
            this.Color = color;
            this.PaperType = paperType;
            this.PrintOption = printOption;
            this.MailClass = mailClass;
            this.AddressList = addressList;
            this.ReturnAddress = returnAddress;
            this.RefId = refId;
        }
        public Jobs(int startingPage, int endingPage, ProductOptionsItem po, List<AddressItem> addressList, ReturnAddressItem returnAddress = null, string refId = null)
        {
            this.StartingPage = startingPage;
            this.EndingPage = endingPage;
            this.DocumentClass = po.DocumentClass;
            this.Layout = po.Layout;
            this.ProductionTime = po.ProductionTime;
            this.Envelope = po.Envelope;
            this.Color = po.PrintColor;
            this.PaperType = po.PaperType;
            this.PrintOption = po.PrintOption;
            this.MailClass = po.MailClass;
            this.AddressList = addressList;
            this.ReturnAddress = returnAddress;
            this.RefId = refId;
        }
        public Jobs(FileInfo mergeFile, string documentClass, string layout, string productionTime, string envelope, string color, string paperType, string printOption, string mailClass,
        List<AddressItem> addressList, ReturnAddressItem returnAddress = null, string refId = null)
        {
            this.MergeFile = mergeFile;

            this.DocumentClass = documentClass;
            this.Layout = layout;
            this.ProductionTime = productionTime;
            this.Envelope = envelope;
            this.Color = color;
            this.PaperType = paperType;
            this.PrintOption = printOption;
            this.MailClass = mailClass;
            this.AddressList = addressList;
            this.ReturnAddress = returnAddress;
            this.RefId = refId;
        }
        public Jobs(FileInfo mergeFile, ProductOptionsItem po, List<AddressItem> addressList, ReturnAddressItem returnAddress = null, string refId = null)
        {
            this.MergeFile = mergeFile;

            this.DocumentClass = po.DocumentClass;
            this.Layout = po.Layout;
            this.ProductionTime = po.ProductionTime;
            this.Envelope = po.Envelope;
            this.Color = po.PrintColor;
            this.PaperType = po.PaperType;
            this.PrintOption = po.PrintOption;
            this.MailClass = po.MailClass;
            this.AddressList = addressList;
            this.ReturnAddress = returnAddress;
            this.RefId = refId;
        }

        //TOOLS
      
        public string runComplete(string file, string format,string addressList,ProductOptionsItem po, ReturnAddressItem ra = null, string appSignature = "")
        {
            Document d = new Document(this.Auth);
            this.DocumentId = d.createDocument(file,po,format);
            if (StatusChanged != null)
            {
                StatusChanged("DocumentID:" + DocumentId);
            }
            Console.WriteLine("Creating addresslist:" + addressList);
            createAddressListSimple(addressList);
            if (StatusChanged != null)
            {
                StatusChanged("AddressID:" + AddressListId);
            }
            waitForCompletedAddressList();
            
            createJobSimple(po, appSignature, ra);

            if (StatusChanged != null)
            {
                StatusChanged("JobId:" + JobId);
            }
            Console.WriteLine("Submitting Job:" + JobId);
            submitJobSimple();
            checkJobStatus();

            //RaiseEvent statusChanged(checkJobStatus())
            if (StatusChanged != null)
            {
                StatusChanged("Completed");
            }
            return "";
        }
        public string checkJobStatus()
        {
            String url = Auth.getRestUrl() + "/molpro/jobs/" + JobId;
            Console.WriteLine("Calling URL " + url);
            System.Collections.Specialized.NameValueCollection y = new System.Collections.Specialized.NameValueCollection();
            y.Clear();
            string results = webPosts.createRestCall(url, y, Method.GET);
            this.JobStatus = this.tools.parseReturnxml(results, "status");
            this.StatusDescription = tools.parseReturnxml(results, "description");
            if (JobStatusCheck != null)
            {
                JobStatusCheck(tools.parseReturnxml(results, "id"), tools.parseReturnxml(results, "status"), tools.parseReturnxml(results, "description"));
            }
            return results;
        }
        public void cancelToken()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
        }
        public async Task getJobCostAsync()
        {


            HttpClient client = new HttpClient();
            try
            {


                var byteArray = Encoding.ASCII.GetBytes(this.Auth.username + ":" + this.Auth.password);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Add("Accept", "application/xml");


                //var requestTimeout = TimeSpan.FromSeconds(15);
                var httpTimeout = TimeSpan.FromSeconds(100);
                client.Timeout = httpTimeout;

                tokenSource = new System.Threading.CancellationTokenSource();

                tokenSource.CancelAfter(30000);
                //System.Threading.Thread.Sleep(100);
                HttpResponseMessage response = await client.GetAsync(new Uri(Auth.getRestUrl() + "/molpro/jobs/" + JobId + "/cost"), tokenSource.Token);

                XmlDocument x = new XmlDocument();
                var success = response.IsSuccessStatusCode;
                var results = await response.Content.ReadAsStringAsync();

                //  System.Diagnostics.Debug.WriteLine(results);
                response.Dispose();
                //                    tokenSource.Dispose();


                if (success)
                {
                    bool loading = true;
                    try
                    {
                        x.LoadXml(results);
                        if (x.SelectSingleNode("//job/cost") != null)
                        {
                            loading = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Load Failed");
                        //System.Threading.Thread.Sleep(10000);
                        //    getJobCostAsync();
                        return;
                    }
                    float cost = 0;
                    float.TryParse(x.SelectSingleNode("//job/cost").InnerText, out cost);
                    this.Cost = cost;

                    float totalTax = 0;
                    float.TryParse(x.SelectSingleNode("//job/totalTax").InnerText, out totalTax);
                    this.TotalTax = totalTax;
                    this.CostRan = true;
                    if (this.JobCostCheck != null)
                    {
                        this.JobCostCheck(this.JobId, cost, totalTax, this.RefId);
                        this.PendingCostRun = false;
                    }

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("TRYING AGAIN");
                    //                    System.Threading.Thread.Sleep(10000);
                    //  getJobCostAsync();
                    //      return;
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                //System.Threading.Thread.Sleep(10000);
                // getJobCostAsync();
                //return;
            }

        }
        public async void getJobCostAsyncOLD()
        {
            String url = Auth.getRestUrl() + "/molpro/jobs/" + JobId + "/cost";

            //Console.WriteLine("Calling URL " + url);
            System.Collections.Specialized.NameValueCollection y = new System.Collections.Specialized.NameValueCollection();
            y.Clear();
            string results = await webPosts.createRestCallAsync(url, y, Method.GET);

            if (results != null)
            {
                XmlDocument x = new XmlDocument();
                x.LoadXml(results);
                float cost = 0;
                float.TryParse(x.SelectSingleNode("//job/cost").InnerText, out cost);
                this.Cost = cost;

                float totalTax = 0;
                float.TryParse(x.SelectSingleNode("//job/totalTax").InnerText, out totalTax);
                this.TotalTax = totalTax;
                this.CostRan = true;
                if (JobCostCheck != null)
                {
                    JobCostCheck(this.JobId, cost, totalTax, this.RefId);
                }

            }
            //System.Threading.Thread.Sleep(10000);
            //return results;
        }
        public string getJobCost()
        {
            String url = Auth.getRestUrl() + "/molpro/jobs/" + JobId + "/cost";

            //Console.WriteLine("Calling URL " + url);
            System.Collections.Specialized.NameValueCollection y = new System.Collections.Specialized.NameValueCollection();
            y.Clear();
            string results = webPosts.createRestCall(url, y, Method.GET);
            if (results != null)
            {
                XmlDocument x = new XmlDocument();
                x.LoadXml(results);
                float cost = 0;
                float.TryParse(x.SelectSingleNode("//job/cost").InnerText, out cost);
                this.Cost = cost;

                float totalTax = 0;
                float.TryParse(x.SelectSingleNode("//job/totalTax").InnerText, out totalTax);
                this.TotalTax = totalTax;
                this.CostRan = true;
            }
            return results;

        }

        public string submitJobSimple(System.Collections.Specialized.NameValueCollection y = null)
        {
            string results = null;
            if (y == null)
            {
                y = new System.Collections.Specialized.NameValueCollection();
                y.Add("billingType", "User Credit");
            }
            results = webPosts.createRestCall(Auth.getRestUrl() + "/molpro/jobs/" + JobId + "/submit", y, Method.POST);
            return results;
        }
        public int createJobSinglePiece(string templateName, string pdf, string addressXML, string billingType = "User Credit")
        {
           // Console.WriteLine("Creating single piece job with template: " + templateName + " and address: " + addressXML);
            System.Collections.Specialized.NameValueCollection y = new System.Collections.Specialized.NameValueCollection();
            y.Add("templateName", templateName);
            y.Add("address", addressXML);
            y.Add("billingType", billingType);
            string results = webPosts.callAPIWithFileParam(Auth.getRestUrl() + "/molpro/jobs/jobTemplate/submitonepiece/", pdf, "document", y);
            JobId = Int32.Parse(this.tools.parseReturnxml(results, "id"));
            return JobId;
        }
        public int createJobSimple(ProductOptionsItem po, string appSignature = "", ReturnAddressItem ra = null)
        {
            System.Collections.Specialized.NameValueCollection y = new System.Collections.Specialized.NameValueCollection();
            y.Add("documentClass", po.DocumentClass);
            y.Add("layout", po.Layout);
            y.Add("productionTime", po.ProductionTime);
            y.Add("envelope", po.Envelope);
            y.Add("mailClass", po.MailClass);
            y.Add("color", po.PrintColor);
            y.Add("paperType", po.PaperType);
            y.Add("printOption", po.PrintOption);
            y.Add("documentId", DocumentId.ToString());
            y.Add("addressId", AddressListId.ToString());

            y.Add("appSignature", appSignature);
            if (ra != null)
            {
                y.Add("rtnName", ra.ReturnAddressName);
                y.Add("rtnOrganization", ra.ReturnOrginization);
                y.Add("rtnaddress1", ra.ReturnAddress1);
                y.Add("rtnaddress2", ra.ReturnAddress2);
                y.Add("rtnCity", ra.ReturnCity);
                y.Add("rtnState", ra.ReturnState);
                y.Add("rtnZip", ra.ReturnZip);
                if (ra.AncilaryEndorsement != Endorsement.NoAncillaryEndorsement && ra.AncilaryEndorsement != null && ra.AncilaryEndorsement != "")
                {
                    y.Add("endorsement", ra.AncilaryEndorsement);
                }

            }
            string results = null;
            results = webPosts.createRestCall(Auth.getRestUrl() + "/molpro/jobs", y, Method.POST);
            JobId = Int32.Parse(this.tools.parseReturnxml(results, "id"));
            return JobId;
        }
        public String waitForCompletedAddressList()
        {
            string status = "0";
            String url = Auth.getRestUrl() + "/molpro/addressLists/" + AddressListId;
            Console.WriteLine("Calling URL " + url);

            var client = new RestClient();
            client.Authenticator = new HttpBasicAuthenticator(Auth.username, Auth.password);

            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-Type", "application/xml");
            request.AddHeader("Accept", "application/xml");

            var response = client.Get(request);
            string results = response.Content;
            status = this.tools.parseReturnxml(results, "status");

            int MAX_ATTEMPTS = 5;
            int attempts = 0;
            while (status != "3" && attempts < MAX_ATTEMPTS)
            {
                attempts++;
                if (StatusChanged != null)
                {
                    StatusChanged("Waiting Address List to processes.  Current Status is: " + status + " (Attempt# " + attempts + ")");
                }
                System.Threading.Thread.Sleep(5000);
                status = waitForCompletedAddressList();
            }
            if (attempts < MAX_ATTEMPTS)
            {
                StatusChanged("The status received is 3, which means we can proceed");
            }
            else
            {
                StatusChanged("Timeout waiting for Address List to process. Will stop trying now.");
            }

            return status;

        }
  

        public object createAddressListSimple(string Xml)
        {
            string results = webPosts.createXMLPost(Auth.getRestUrl() + "/molpro/addressLists/", Xml);
            AddressListId = Int32.Parse(this.tools.parseReturnxml(results, "id"));
            return AddressListId;
        }



        public string cancelJob()
        {
          
                String url = Auth.getRestUrl() + "/molpro/jobs/" + this.JobId + "/cancel";
                System.Collections.Specialized.NameValueCollection y = new System.Collections.Specialized.NameValueCollection();
                y.Clear();
                string results = webPosts.createRestCall(url, y, Method.POST);
                return results;
        }
        public string createProof()
        {

            String url = Auth.getRestUrl() + "/molpro/jobs/" + this.JobId + "/proof";
            
            System.Collections.Specialized.NameValueCollection y = new System.Collections.Specialized.NameValueCollection();
            y.Clear();
            string results = webPosts.createRestCall(url, y, Method.POST);
            ProofId = Int32.Parse(this.tools.parseReturnxml(results, "id"));
            return results;
        }
        public void getProof(string destination)
        {
            
            String url = Auth.getRestUrl() + "/molpro/jobs/" + this.JobId + "/proof" + ProofId;

            WebClient Client = new WebClient();
            Client.Headers.Add("Authentication", Auth.getBasicAuthentication());
            Client.DownloadFile(url, destination);
            
        }
        public byte[] getProof()
        {

            String url = Auth.getRestUrl() + "/molpro/jobs/" + this.JobId + "/proof" + ProofId;

            WebClient Client = new WebClient();
            Client.Headers.Add("Authentication", Auth.getBasicAuthentication());
            return Client.DownloadData(url);

        }
        public System.IO.Stream GenerateStreamFromString(string s)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;

        }

        

        public string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null)
            {
                return null;
            }

            StringBuilder newString = new StringBuilder();
            char ch = '\0';


            for (int i = 0; i <= inString.Length - 1; i++)
            {
                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                //if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                //if using .NET version prior to 4, use above logic
                if (XmlConvert.IsXmlChar(ch))
                {
                    //this method is new in .NET 4
                    newString.Append(ch);
                }
            }
            return newString.ToString();

        }


        public string createXMLFromAddressList()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            //create nodes
            System.Xml.XmlElement root = doc.CreateElement("addressList");


            System.Xml.XmlElement addressListName = doc.CreateElement("addressListName");
            _addressListName = Guid.NewGuid().ToString("N");
            addressListName.InnerXml = _addressListName;

            root.AppendChild(addressListName);

            System.Xml.XmlElement addressMappingId = doc.CreateElement("addressMappingId");
            addressMappingId.InnerXml = "2";
            root.AppendChild(addressMappingId);

            System.Xml.XmlElement addresses = doc.CreateElement("addresses");
            root.AppendChild(addresses);

            foreach (AddressItem a in AddressList)
            {
                System.Xml.XmlElement address = doc.CreateElement("address");
                System.Xml.XmlElement fname = doc.CreateElement("First_name");
                fname.InnerXml = a.FirstName;
                address.AppendChild(fname);
                System.Xml.XmlElement lname = doc.CreateElement("Last_name");
                lname.InnerXml = a.LastName;
                address.AppendChild(lname);
                System.Xml.XmlElement Organization = doc.CreateElement("Organization");
                Organization.InnerXml = a.Organization;
                address.AppendChild(Organization);
                System.Xml.XmlElement Address1 = doc.CreateElement("Address1");
                Address1.InnerXml = a.Address1;
                address.AppendChild(Address1);
                System.Xml.XmlElement Address2 = doc.CreateElement("Address2");
                Address2.InnerXml = a.Address2;
                address.AppendChild(Address2);
                System.Xml.XmlElement Address3 = doc.CreateElement("Address3");
                Address3.InnerXml = a.Address3;
                address.AppendChild(Address3);
                System.Xml.XmlElement City = doc.CreateElement("City");
                City.InnerXml = a.City;
                address.AppendChild(City);
                System.Xml.XmlElement State = doc.CreateElement("State");
                State.InnerXml = a.State;
                address.AppendChild(State);
                System.Xml.XmlElement zip = doc.CreateElement("zip");
                zip.InnerXml = a.Zip;
                address.AppendChild(zip);
                System.Xml.XmlElement country = doc.CreateElement("Country_non-US");
                country.InnerXml = a.CountryNonUS;
                address.AppendChild(country);


                /* if(a.ReturnZip != null && a.ReturnZip.Length>1)
                 {
                     {
                         System.Xml.XmlNode returnAddress = doc.CreateElement("returnAddress");
                         address.AppendChild(returnAddress);

                         System.Xml.XmlNode raname = doc.CreateElement("name");
                         raname.InnerText = a.ReturnAddressName;
                         returnAddress.AppendChild(raname);

                         System.Xml.XmlNode raorg = doc.CreateElement("organization");
                         raorg.InnerText = a.ReturnOrginization;
                         returnAddress.AppendChild(raorg);


                         System.Xml.XmlNode raaddress1 = doc.CreateElement("address1");
                         raaddress1.InnerText = a.ReturnAddress1;
                         returnAddress.AppendChild(raaddress1);

                         System.Xml.XmlNode raaddress2 = doc.CreateElement("address2");
                         raaddress2.InnerText = a.ReturnAddress2;
                         returnAddress.AppendChild(raaddress2);

                         System.Xml.XmlNode racity = doc.CreateElement("city");
                         racity.InnerText = a.ReturnCity;
                         returnAddress.AppendChild(racity);

                         System.Xml.XmlNode rastate = doc.CreateElement("state");
                         rastate.InnerText = a.ReturnState;
                         returnAddress.AppendChild(rastate);

                         System.Xml.XmlNode rapost = doc.CreateElement("postalCode");
                         rapost.InnerText = a.ReturnZip;
                         returnAddress.AppendChild(rapost);
                     }

                 }*/


                addresses.AppendChild(address);
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

        public string createXMLFromCustomList(List<List<KeyValuePair<string, string>>> myList, int mappingId)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            //create nodes
            System.Xml.XmlElement root = doc.CreateElement("addressList");


            System.Xml.XmlElement addressListName = doc.CreateElement("addressListName");
            _addressListName = Guid.NewGuid().ToString("N");
            addressListName.InnerXml = _addressListName;

            root.AppendChild(addressListName);

            System.Xml.XmlElement addressMappingId = doc.CreateElement("addressMappingId");
            addressMappingId.InnerXml = mappingId.ToString();

            root.AppendChild(addressMappingId);

            System.Xml.XmlElement addresses = doc.CreateElement("addresses");
            root.AppendChild(addresses);
            System.Xml.XmlElement address = null;
            foreach (List<KeyValuePair<string, string>> a in myList)
            {
                address = doc.CreateElement("address");
                foreach (KeyValuePair<string, string> aa in a)
                {
                    System.Xml.XmlElement i = doc.CreateElement(aa.Key);
                    i.InnerXml = aa.Value;
                    address.AppendChild(i);
                }
                addresses.AppendChild(address);
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