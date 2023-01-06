
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Threading.Tasks;


namespace c2mAPI
{
    public class Batchc2mAPI
    {
        private readonly object _Locker = new object();


        string _pdfFile = "";
        string _batchPDFFile = "";
        
        private string _addressListName;

        private const string _Smainurl = "https://stage-batch.click2mail.com";
        private const string _lmainurl = "https://batch.click2mail.com";

        public string PDF { get; set; }
        public string RefId { get; set; }
        public int BatchId { get; set; }
        public bool StopAllChecks { get; set; } = false;
        public bool JobsReturned { get; set; } = false;
        public Environment.Mode Mode { get; set; }
        public List<Jobs> JobList { get; set; }

        public event StatusChangedEventHandler StatusChanged;
        public delegate void StatusChangedEventHandler(Batchc2mAPI b, string Reason);
        public event BatchCompletedEventHandler BatchCompleted;
        public delegate void BatchCompletedEventHandler(Batchc2mAPI b);
        public event CostCheckEventHandler CostCheck;
        public delegate void CostCheckEventHandler(int batchId, int jobId, float cost, float costTax, float costTotal, int remaining, string batchRefId = null, string jobRefId = null);
        public event CostCheckCompleteEventHandler CostCheckComplete;
        public delegate void CostCheckCompleteEventHandler(int batchId, float totalCost, float totalTax, float grandTotal, string refId = null);
        public event JobDetailsAvailableEventHandler JobDetailsAvailable;
        public delegate void JobDetailsAvailableEventHandler(Batchc2mAPI batch);
        public DirectoryInfo StoreReferencePathDirectory { get; set; }
        public string CreatedAt { get; set; }
        public string ErrorMessage { get; set; }
        public string HasErrors { get; set; }
        public string Received { get; set; }
        public string Submitted { get; set; }
        public string NoOfPieces { get; set; }
        public string Status { get; set; }
        public string DocumentName { get; set; }
        private volatile int JobCostCheckRemaining = 0;
        private volatile int JobCostCheckTotal = 0;
        public Authentication Auth { get; set; }


        public Batchc2mAPI(Authentication auth, string refId = null, DirectoryInfo StoreReferencePathDirectory = null)
        {
            this.Auth = auth;
            
            this.RefId = refId;
            this.StoreReferencePathDirectory = StoreReferencePathDirectory;
        }

        public int createBatchSimple()
        {
            string results = createbatch();
            BatchId = Int32.Parse(parseReturnxml(results, "id"));

            return BatchId;
        }
        //TOOLS
        private void loadBatchObject(string rawXML)
        {
            XmlDocument results = new XmlDocument();
            results.LoadXml(rawXML);
            XmlNode createdAt = results.SelectSingleNode("//batchjob/createdAt");
            XmlNode errorMessage = results.SelectSingleNode("//batchjob/errorMessage");
            XmlNode hasErrors = results.SelectSingleNode("//batchjob/hasErrors");
            XmlNode batchId = results.SelectSingleNode("//batchjob/id");
            XmlNode received = results.SelectSingleNode("//batchjob/received");
            XmlNode submitted = results.SelectSingleNode("//batchjob/submitted");
            XmlNode status = results.SelectSingleNode("//batchjob/status");

            this.CreatedAt = createdAt.InnerText;
            this.ErrorMessage = errorMessage.InnerText;
            this.HasErrors = hasErrors.InnerText;
            this.BatchId = int.Parse(batchId.InnerText);
            this.Received = received.InnerText;
            this.Submitted = submitted.InnerText;




        }
        private void loadBatchObjectStatusDetails(string rawXML)
        {
            XmlDocument results = new XmlDocument();
            results.LoadXml(rawXML);
            XmlNode createdAt = results.SelectSingleNode("//batch/createdAt");
            XmlNode errorMessage = results.SelectSingleNode("//batch/errorMessage");

            XmlNode batchId = results.SelectSingleNode("//batch/batchNumber");
            XmlNode noOfPieces = results.SelectSingleNode("//batch/noOfPieces");
            XmlNode status = results.SelectSingleNode("//batch/status");
            XmlNode documentName = results.SelectSingleNode("//batch/documentName");

            this.CreatedAt = createdAt.InnerText;
            this.ErrorMessage = errorMessage.InnerText;

            this.BatchId = int.Parse(batchId.InnerText);
            this.NoOfPieces = noOfPieces.InnerText;
            this.Status = status.InnerText;
            this.DocumentName = documentName.InnerText;




        }
        public void loadBatchObjectAll(bool includeCost, bool skipDetail = false)
        {
            XmlDocument results = new XmlDocument();
            if (skipDetail == false)
            {
                results.LoadXml(getBatchStatusDetails());
                XmlNode createdAt = results.SelectSingleNode("//batch/createdAt");
                XmlNode errorMessage = results.SelectSingleNode("//batch/errorMessage");

                XmlNode batchId = results.SelectSingleNode("//batch/batchNumber");
                XmlNode noOfPieces = results.SelectSingleNode("//batch/noOfPieces");
                XmlNode status = results.SelectSingleNode("//batch/status");
                XmlNode documentName = results.SelectSingleNode("//batch/documentName");

                this.CreatedAt = createdAt.InnerText;
                this.ErrorMessage = errorMessage.InnerText;

                this.BatchId = int.Parse(batchId.InnerText);
                this.NoOfPieces = noOfPieces.InnerText;
                this.Status = status.InnerText;
                this.DocumentName = documentName.InnerText;
                this.CreatedAt = createdAt.InnerText;
                this.ErrorMessage = errorMessage.InnerText;
            }
            XmlDocument results1 = new XmlDocument();
            results1.LoadXml(getBatchStatus());
            XmlNode hasErrors = results1.SelectSingleNode("//batchjob/hasErrors");
            XmlNode received = results1.SelectSingleNode("//batchjob/received");
            XmlNode submitted = results1.SelectSingleNode("//batchjob/submitted");

            this.HasErrors = hasErrors.InnerText;

            this.Received = received.InnerText;
            this.Submitted = submitted.InnerText;
            XmlNodeList selectedNodeList = results1.SelectNodes("//batchjob/jobs");
            float totalCost = 0;
            float totalTax = 0;
            if (this.JobList == null || JobList.Count != selectedNodeList.Count)
            {
                this.JobList = new List<Jobs>();
            }
            if (selectedNodeList.Count > 0)
            {



                int current = 0;
                int total = selectedNodeList.Count;
                StreamReader readtext = null;
                if (getStoreRefPath().Exists)
                {
                    readtext = new StreamReader(getStoreRefPath().FullName);
                }



                foreach (XmlNode x in selectedNodeList)
                {

                    Jobs job;

                    if (JobList.Count == selectedNodeList.Count)
                    {
                        job = JobList[current];
                    }
                    else
                    {
                        job = new Jobs(this.Auth);
                    }
                    current += 1;
                    if (readtext != null)
                    {
                        string[] row = readtext.ReadLine().Split('|');
                        job.RefId = row[0];
                        job.MergeFile = new FileInfo(row[1]);
                        job.StartingPage = (row.Length > 2 ? int.Parse(row[2]) : 0);
                        job.EndingPage = (row.Length > 3 ? int.Parse(row[3]) : 0);
                        this.RefId = (row.Length > 4 ? row[4] : "");
                    }

                    job.JobId = int.Parse(x.SelectSingleNode("jobId").InnerText.ToString());
                    job.JobIndex = x.SelectSingleNode("jobIndex")?.InnerText;
                    job.JobStatus = x.SelectSingleNode("jobStatus")?.InnerText;
                    job.ErrorMessage = x.SelectSingleNode("errorMessage")?.InnerText;
                    job.Invoice = x.SelectSingleNode("invoice")?.InnerText;
                    job.MailingDate = x.SelectSingleNode("mailingDate")?.InnerText;
                    job.SubmittedDate = x.SelectSingleNode("submittedDate")?.InnerText;

                    if (job.JobStatus == "MAILED" && includeCost)
                    {
                        job.getJobCost();
                        totalTax += job.TotalTax;
                        totalCost += job.Cost;
                    }
                    if (CostCheck != null && includeCost)
                    {
                        CostCheck(this.BatchId, job.JobId, job.Cost, job.TotalTax, job.Cost + job.TotalTax, total - current, this.RefId, job.RefId);
                    }

                    this.JobList.Add(job);

                }
                this.JobsReturned = true;
                if (JobDetailsAvailable != null)
                {
                    JobDetailsAvailable(this);
                }

            }
            if (CostCheckComplete != null && includeCost)
            {
                CostCheckComplete(this.BatchId, totalCost, totalTax, totalCost + totalTax, this.RefId);
            }

        }
        public async Task loadBatchObjectAllAsync(bool includeCost, bool skipDetail = false)
        {
            lock (_Locker)
            {

                this.JobCostCheckRemaining = 0;
            }
            if (skipDetail == false)
            {
                XmlDocument results = new XmlDocument();
                results.LoadXml(getBatchStatusDetails());
                XmlNode createdAt = results.SelectSingleNode("//batch/createdAt");
                XmlNode errorMessage = results.SelectSingleNode("//batch/errorMessage");

                XmlNode batchId = results.SelectSingleNode("//batch/batchNumber");
                XmlNode noOfPieces = results.SelectSingleNode("//batch/noOfPieces");
                XmlNode status = results.SelectSingleNode("//batch/status");
                XmlNode documentName = results.SelectSingleNode("//batch/documentName");

                this.CreatedAt = createdAt.InnerText;
                this.ErrorMessage = errorMessage.InnerText;


                this.NoOfPieces = noOfPieces.InnerText;
                this.Status = status.InnerText;
                this.DocumentName = documentName.InnerText;
                this.BatchId = int.Parse(batchId.InnerText);
                this.CreatedAt = createdAt.InnerText;
                this.ErrorMessage = errorMessage.InnerText;
            }
            XmlDocument results1 = new XmlDocument();
            results1.LoadXml(getBatchStatus());
            XmlNode hasErrors = results1.SelectSingleNode("//batchjob/hasErrors");
            XmlNode received = results1.SelectSingleNode("//batchjob/received");
            XmlNode submitted = results1.SelectSingleNode("//batchjob/submitted");

            this.HasErrors = hasErrors.InnerText;

            this.Received = received.InnerText;
            this.Submitted = submitted.InnerText;
            XmlNodeList selectedNodeList = results1.SelectNodes("//batchjob/jobs");
            if (this.JobList == null || JobList.Count != selectedNodeList.Count)
            {
                this.JobList = new List<Jobs>();
            }
            int current = 0;
            if (selectedNodeList.Count > 0)
            {
                StreamReader readtext = null;
                if (getStoreRefPath().Exists)
                {
                    readtext = new StreamReader(getStoreRefPath().FullName);
                }

                foreach (XmlNode x in selectedNodeList)
                {

                    Jobs job;

                    if (JobList.Count == selectedNodeList.Count)
                    {
                        job = JobList[current];
                    }
                    else
                    {
                        job = new Jobs(this.Auth);
                    }
                    if (readtext != null)
                    {
                        string[] row = readtext.ReadLine().Split('|');
                        job.RefId = row[0];
                        job.MergeFile = new FileInfo(row[1]);
                        job.StartingPage = (row.Length > 2 ? int.Parse(row[2]) : 0);
                        job.EndingPage = (row.Length > 3 ? int.Parse(row[3]) : 0);
                        this.RefId = (row.Length > 4 ? row[4] : "");
                    }

                    current += 1;

                    job.JobId = int.Parse(x.SelectSingleNode("jobId").InnerText.ToString());
                    job.JobIndex = x.SelectSingleNode("jobIndex")?.InnerText;
                    job.JobStatus = x.SelectSingleNode("jobStatus")?.InnerText;
                    job.ErrorMessage = x.SelectSingleNode("errorMessage")?.InnerText;
                    job.Invoice = x.SelectSingleNode("invoice")?.InnerText;
                    job.MailingDate = x.SelectSingleNode("mailingDate")?.InnerText;
                    job.SubmittedDate = x.SelectSingleNode("submittedDate")?.InnerText;


                    if (job.JobStatus == "MAILED" && includeCost)
                    {

                        job.JobCostCheck += Job_JobCostCheck;
                        this.JobCostCheckRemaining += 1;

                        job.PendingCostRun = true;
                        //Console.WriteLine("submitted cost request JobId:" + job.jobId.ToString());
                    }


                    this.JobList.Add(job);

                }
                this.JobCostCheckTotal = this.JobCostCheckRemaining;
                runJobCostBatch();
                if (JobDetailsAvailable != null)
                {
                    JobDetailsAvailable(this);
                }
                this.JobsReturned = true;
            }


        }
        private void runJobCostBatch(int triggered = 0)
        {


            for (int i = 0; i < this.JobList.Count; i++)
            {
                if (this.JobList[i].PendingCostRun == true)
                {
                    triggered += 1;
                    _ = this.JobList[i].getJobCostAsync();

                }
                if (triggered % 700 == 0)
                {
                    System.Threading.Thread.Sleep(1000);
                    int counter = 0;
                    int lastCount = this.JobCostCheckRemaining;

                    while (this.JobCostCheckRemaining > (this.JobCostCheckTotal - triggered))
                    {
                        if (lastCount != this.JobCostCheckRemaining)
                        {
                            lastCount = this.JobCostCheckRemaining;
                            counter = 0;
                        }
                        System.Threading.Thread.Sleep(1500);
                        System.Diagnostics.Debug.WriteLine("Waiting on batch of 2000 to finish " + (this.JobCostCheckTotal - triggered).ToString() + " should match " + this.JobCostCheckRemaining.ToString());
                        counter++;

                        if (counter > 25)
                        {
                            System.Diagnostics.Debug.WriteLine("Counter has had same count for 15 seconds, running cleanupW");
                            bool noMatches = true;
                            for (int ii = 0; ii < triggered; ii++)
                            {
                                if (this.JobList[ii].PendingCostRun == true)
                                {
                                    System.Diagnostics.Debug.WriteLine("Found Bad Item");
                                    this.JobList[ii].cancelToken();
                                    this.JobList[ii].getJobCostAsync();
                                    noMatches = false;
                                }

                            }
                            if (noMatches)
                            {
                                this.JobCostCheckRemaining = this.JobCostCheckTotal - triggered;
                            }
                            counter = 0;

                        }
                    }
                }


            }


        }

        private void Job_JobCostCheck(int jobId, float cost, float taxCost, string jobRefId)
        {

            float totalTax = 0;
            float totalCost = 0;
            lock (_Locker)
            {
                JobCostCheckRemaining = JobCostCheckRemaining - 1;

                if (CostCheck != null)
                {
                    CostCheck(this.BatchId, jobId, cost, taxCost, taxCost + cost, this.JobCostCheckRemaining, this.RefId, jobRefId);
                }
                if (CostCheckComplete != null && this.JobCostCheckRemaining == 0)
                {
                    for (int i = 0; i < this.JobList.Count; i++)
                    {
                        totalTax += this.JobList[i].TotalTax;
                        totalCost += this.JobList[i].Cost;
                    }
                    System.Diagnostics.Debug.WriteLine("We have completed");
                    CostCheckComplete(this.BatchId, totalCost, totalTax, totalCost + totalTax, this.RefId);
                    System.Threading.Thread.Sleep(1000);

                }
            }

        }

        private string createbatch()
        {

            HttpWebResponse response = null;
            StreamReader reader = default(StreamReader);
            Uri address = default(Uri);
            StringBuilder data = null;
            byte[] byteData = null;
            Stream postStream = null;

            address = new Uri(this.Auth.getBatchUrl() + "/v1/batches");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(address);
            string authinfo = null;
            

            // Create the web request  
            request = (HttpWebRequest)WebRequest.Create(address);
            request.Headers["Authorization"] = this.Auth.getBasicAuthentication();
            request.Method = "POST";
            request.ContentType = "text/plain";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException wex)
            {
                // This exception will be raised if the server didn't return 200 - OK  
                // Try to retrieve more information about the network error  
                if ((wex.Response != null))
                {
                    HttpWebResponse errorResponse = null;
                    try
                    {
                        errorResponse = (HttpWebResponse)wex.Response;
                        Console.WriteLine("The server returned '{0}' with the status code {1} ({2:d}).", errorResponse.StatusDescription, errorResponse.StatusCode, errorResponse.StatusCode);
                        return "";
                    }
                    finally
                    {
                        if ((errorResponse != null))
                            errorResponse.Close();
                    }
                }
            }
            finally
            {
                if ((postStream != null))
                    postStream.Close();

            }
            //

            try
            {
                reader = new StreamReader(response.GetResponseStream());

                // Console application output  
                string s = reader.ReadToEnd();
                reader.Close();
                // Console.Write(s)
                return s;
                //    c2m.StatusPick.jobStatus = parsexml(s, "status")
                //MsgBox(s)

            }
            finally
            {
                // If c2m.jobid = 0 Then
                //            c2m.StatusPick.jobStatus = 99
                //End If
                //If Not response Is Nothing Then response.Close()
            }
        }
        public string createXMLBatchPost()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
            //create nodes
            System.Xml.XmlElement root = doc.CreateElement("batch");

            //"    <username>" & username & "</username>" &
            //"    <password>" & password & "</password>" &
            //"    <filename>" & fileName & "</filename>" &
            //"    <appSignature>MyTest App</appSignature>" &
            System.Xml.XmlElement attr = doc.CreateElement("username");
            attr.InnerXml = this.Auth.username;
            root.AppendChild(attr);

            attr = doc.CreateElement("password");
            attr.InnerXml = this.Auth.password;
            root.AppendChild(attr);

            attr = doc.CreateElement("filename");
            attr.InnerXml = this.PDF;
            root.AppendChild(attr);


            attr = doc.CreateElement("appSignature");
            attr.InnerXml = ".NET SDK API";
            root.AppendChild(attr);
            foreach (Jobs b in JobList)
            {
                dynamic job = doc.CreateElement("job");

                attr = doc.CreateElement("startingPage");
                attr.InnerXml = b.StartingPage.ToString();
                job.AppendChild(attr);

                attr = doc.CreateElement("endingPage");
                attr.InnerXml = b.EndingPage.ToString();
                job.AppendChild(attr);


                dynamic printProductIOptions = doc.CreateElement("printProductionOptions");
                job.AppendChild(printProductIOptions);
                //<documentClass>Letter 8.5 x 11</documentClass>" &
                //"            <layout>Address on First Page</layout>" &
                //"            <productionTime>Next Day</productionTime>" &
                //"            <envelope>#10 Double Window</envelope>" &
                //"            <color>Full Color</color>" &
                //"            <paperType>White 24#</paperType>" &
                //"            <printOption>Printing One side</printOption>" &
                //"            <mailClass>First Class</mailClass>" &
                attr = doc.CreateElement("documentClass");
                attr.InnerXml = b.DocumentClass;
                printProductIOptions.AppendChild(attr);

                attr = doc.CreateElement("layout");
                attr.InnerXml = b.Layout;
                printProductIOptions.AppendChild(attr);

                attr = doc.CreateElement("productionTime");
                attr.InnerXml = b.ProductionTime;
                printProductIOptions.AppendChild(attr);
                attr = doc.CreateElement("envelope");
                attr.InnerXml = b.Envelope;
                printProductIOptions.AppendChild(attr);
                attr = doc.CreateElement("color");
                attr.InnerXml = b.Color;
                printProductIOptions.AppendChild(attr);

                attr = doc.CreateElement("paperType");
                attr.InnerXml = b.PaperType;
                printProductIOptions.AppendChild(attr);
                attr = doc.CreateElement("printOption");
                attr.InnerXml = b.PrintOption;
                printProductIOptions.AppendChild(attr);
                attr = doc.CreateElement("mailClass");
                attr.InnerXml = b.MailClass;
                printProductIOptions.AppendChild(attr);
                if (b.ReturnAddress != null)
                {

                    System.Xml.XmlNode returnAddress = doc.CreateElement("returnAddress");
                    job.AppendChild(returnAddress);

                    System.Xml.XmlNode raname = doc.CreateElement("name");
                    raname.InnerText = b.ReturnAddress.ReturnAddressName;
                    returnAddress.AppendChild(raname);

                    System.Xml.XmlNode raorg = doc.CreateElement("organization");
                    raorg.InnerText = b.ReturnAddress.ReturnOrginization;
                    returnAddress.AppendChild(raorg);


                    System.Xml.XmlNode raaddress1 = doc.CreateElement("address1");
                    raaddress1.InnerText = b.ReturnAddress.ReturnAddress1;
                    returnAddress.AppendChild(raaddress1);

                    System.Xml.XmlNode raaddress2 = doc.CreateElement("address2");
                    raaddress2.InnerText = b.ReturnAddress.ReturnAddress2;
                    returnAddress.AppendChild(raaddress2);

                    System.Xml.XmlNode racity = doc.CreateElement("city");
                    racity.InnerText = b.ReturnAddress.ReturnCity;
                    returnAddress.AppendChild(racity);

                    System.Xml.XmlNode rastate = doc.CreateElement("state");
                    rastate.InnerText = b.ReturnAddress.ReturnState;
                    returnAddress.AppendChild(rastate);

                    System.Xml.XmlNode rapost = doc.CreateElement("postalCode");
                    rapost.InnerText = b.ReturnAddress.ReturnZip;
                    returnAddress.AppendChild(rapost);


                }
                XmlElement addressList = doc.CreateElement("recipients");
                job.AppendChild(addressList);
                foreach (AddressItem ai in b.AddressList)
                {

                    XmlElement address = doc.CreateElement("address");
                    addressList.AppendChild(address);
                    attr = doc.CreateElement("name");
                    if ((ai.FirstName.Length > 0 & ai.LastName.Length > 0))
                    {
                        attr.InnerText = ai.LastName + ", " + ai.FirstName;
                    }
                    else
                    {
                        attr.InnerText = (ai.FirstName + " " + ai.LastName).Trim();
                    }
                    address.AppendChild(attr);

                    attr = doc.CreateElement("organization");
                    attr.InnerText = ai.Organization.Trim();
                    address.AppendChild(attr);

                    attr = doc.CreateElement("address1");
                    attr.InnerText = ai.Address1.Trim();
                    address.AppendChild(attr);

                    attr = doc.CreateElement("address2");
                    attr.InnerText = ai.Address2.Trim();
                    address.AppendChild(attr);

                    attr = doc.CreateElement("address3");
                    attr.InnerText = ai.Address3.Trim(); ;
                    address.AppendChild(attr);

                    attr = doc.CreateElement("city");
                    attr.InnerText = ai.City.Trim();
                    address.AppendChild(attr);

                    attr = doc.CreateElement("state");
                    attr.InnerText = ai.State.Trim();
                    address.AppendChild(attr);

                    attr = doc.CreateElement("postalCode");
                    attr.InnerText = ai.Zip.Trim(); ;
                    address.AppendChild(attr);

                    attr = doc.CreateElement("country");
                    attr.InnerText = ai.CountryNonUS.Trim();
                    address.AppendChild(attr);


                }
                root.AppendChild(job);

            }

            doc.AppendChild(root);

            //doc.Declaration = New XDeclaration("1.0", "utf-8", Nothing)
            string xmlString = null;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    doc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();

                    xmlString = stringWriter.GetStringBuilder().ToString();
                }
            }




            return xmlString;

        }


        private void uploadBatchxml()
        {
            XmlDocument _XMLDOC = new XmlDocument();
            //Console.Write(createXMLBatchPost())
            //Return
            _XMLDOC.LoadXml(createXMLBatchPost());
            string strURI = string.Empty;
            strURI = this.Auth.getBatchUrl() + "/v1/batches/" + BatchId;
            PutObject(strURI, _XMLDOC);
            return;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURI);
            string authinfo = null;
            
            request.Headers["Authorization"] = this.Auth.getBasicAuthentication();
            request.Accept = "text/xml";
            request.Method = "PUT";
            using (MemoryStream ms = new MemoryStream())
            {
                _XMLDOC.Save(ms);
                request.ContentLength = ms.Length;
                ms.WriteTo(request.GetRequestStream());
            }
            string result = null;

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            return;


            //Console.WriteLine(result)
        }

        public String PutObject(string postUrl, XmlDocument xmlDoc)
        {
            NetworkCredential myCreds = new NetworkCredential(Auth.username, Auth.password);

            MemoryStream xmlStream = new MemoryStream();
            xmlDoc.Save(xmlStream);

            string result = "";
            xmlStream.Flush();
            //Adjust this if you want read your data 
            xmlStream.Position = 0;

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Credentials = myCreds;
                client.Headers.Add("Content-Type", "application/xml");
                byte[] b = client.UploadData(postUrl, "PUT", xmlStream.ToArray());
                //Dim b As Byte() = client.UploadFile(postUrl, "PUT", "C:\test\test.xml")

                result = client.Encoding.GetString(b);
            }

            return result;
        }
        private string parseReturnxml(string strxml, string lookfor)
        {

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

        public void uploadBatchPDF()
        {
            WebClient client = new WebClient();

            string strURI = string.Empty;
            strURI = this.Auth.getBatchUrl() + "/v1/batches/" + BatchId;
            string authinfo = null;
            
            client.Headers["Authorization"] = this.Auth.getBasicAuthentication();
            client.Headers.Add("Content-Type", "application/pdf");
            //Dim sentXml As Byte() = System.Text.Encoding.ASCII.GetBytes(_XMLDOC.OuterXml)

            FileInfo fInfo = new FileInfo(PDF);

            long numBytes = fInfo.Length;

            FileStream fStream = new FileStream(PDF, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fStream);

            byte[] data = br.ReadBytes(Convert.ToInt32(numBytes));

            // Show the number of bytes in the array.


            br.Close();

            fStream.Close();




            byte[] response = client.UploadData(strURI, "PUT", data);

            // Console.WriteLine(System.Text.Encoding.Default.GetString(response));


            //Console.WriteLine(response.ToString())
        }
        public string getBatchStatus()
        {
            string strURI = string.Empty;
            strURI = this.Auth.getBatchUrl() + "/v1/batches/" + BatchId;
            //   Console.WriteLine(strURI);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURI);
            
            
            request.Headers["Authorization"] = this.Auth.getBasicAuthentication();
            request.Method = System.Net.WebRequestMethods.Http.Get;
            string result = null;
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();

                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
                //Console.Write(ex.Message)
            }

        }
        public string getBatchStatusDetails()
        {
            string strURI = string.Empty;
            strURI = this.Auth.getBatchUrl() + "/v1/batches/" + BatchId + "/details";
            //   Console.WriteLine(strURI);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURI);
            
            
            request.Headers["Authorization"] = this.Auth.getBasicAuthentication();
            request.Method = System.Net.WebRequestMethods.Http.Get;
            string result = null;
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();

                    }
                }
                return result;


            }
            catch (Exception ex)
            {
                return ex.Message;
                //Console.Write(ex.Message)
            }

        }
        public string getBatchTracking(string trackingType)
        {
            string strURI = string.Empty;
            strURI = this.Auth.getBatchUrl() + "/v1/batches/" + BatchId + "/tracking?trackingType=" + trackingType;
            //    Console.WriteLine(strURI);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURI);
            request.Headers["Authorization"] = this.Auth.getBasicAuthentication();
            request.Method = System.Net.WebRequestMethods.Http.Get;
            string result = null;
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();

                    }
                }
                return result;


            }
            catch (Exception ex)
            {
                return ex.Message;
                //Console.Write(ex.Message)
            }

        }
        private void submitbatch()
        {

            HttpWebResponse response = null;
            StreamReader reader = default(StreamReader);
            Uri address = default(Uri);

            Stream postStream = null;

            address = new Uri(this.Auth.getBatchUrl() + "/v1/batches/" + BatchId);
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(address);

            // Create the web request  
            request = (HttpWebRequest)WebRequest.Create(address);
            request.Headers["Authorization"] = this.Auth.getBasicAuthentication();
            request.Method = "POST";

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException wex)
            {
                // This exception will be raised if the server didn't return 200 - OK  
                // Try to retrieve more information about the network error  
                if ((wex.Response != null))
                {
                    HttpWebResponse errorResponse = null;
                    try
                    {
                        errorResponse = (HttpWebResponse)wex.Response;
                        Console.WriteLine("The server returned '{0}' with the status code {1} ({2:d}).", errorResponse.StatusDescription, errorResponse.StatusCode, errorResponse.StatusCode);
                    }
                    finally
                    {
                        if ((errorResponse != null))
                            errorResponse.Close();
                    }
                }
            }
            finally
            {
                if ((postStream != null))
                    postStream.Close();
            }
            //

            try
            {
                reader = new StreamReader(response.GetResponseStream());
            }
            finally
            {
            }

        }
        private FileInfo getStoreRefPath()
        {
            string pre = "";
            if (Mode == Environment.Mode.StageMode)
            {
                pre = "stage_";
            }
            string refFile = pre + "ref_" + BatchId.ToString() + ".ref";
            return new FileInfo(this.StoreReferencePathDirectory + refFile);
        }
        public void runComplete(string PDF, List<Jobs> jobList, string refId = null)
        {
            BatchId = createBatchSimple();
            this.PDF = PDF;
            this.JobList = jobList;


            refCreate();





            if (StatusChanged != null)
            {
                StatusChanged(this, "BatchID Created:" + BatchId);
            }
            //RaiseEvent statusChanged(createXMLBatchPost())
            uploadBatchxml();
            if (StatusChanged != null)
            {
                StatusChanged(this, "XML UPLOAD Completed");
            }
            uploadBatchPDF();
            if (StatusChanged != null)
            {
                StatusChanged(this, "PDF UPLOAD Completed");
            }
            submitbatch();
            if (StatusChanged != null)
            {
                StatusChanged(this, "Batch UPLOAD Completed");
            }

            loadBatchObjectAll(false);


            if (BatchCompleted != null)
            {
                BatchCompleted(this);
            }
            if (JobDetailsAvailable != null)
            {
                ActivateJobCheck();
            }


        }
        public async Task ActivateJobCheck()
        {

            while (StopAllChecks == false && this.HasErrors != "true" && JobsReturned == false)
            {
                System.Diagnostics.Debug.WriteLine("Still no jobs");
                await loadBatchObjectAllAsync(false, true);
                await Task.Delay(5000);

            }



        }
        public void runCompleteMultipleFiles(List<Jobs> jobList, string MergedFileOutputDirectory)
        {


            if (!Directory.Exists(MergedFileOutputDirectory))
            {
                Directory.CreateDirectory(MergedFileOutputDirectory);
            }
            BatchId = createBatchSimple();
            string pre = "";
            if (Mode == Environment.Mode.StageMode)
            {
                pre = "stage_";
            }
            var PDF = MergedFileOutputDirectory + pre + "MergedBatch_BatchID_" + BatchId.ToString() + ".pdf";
            MergePDF.MergeMultiplePDFJobs(jobList, new FileInfo(PDF));
            this.PDF = PDF;
            this.JobList = jobList;

            refCreate();

            if (StatusChanged != null)
            {
                StatusChanged(this, "BatchID Created:" + BatchId);
            }
            //RaiseEvent statusChanged(createXMLBatchPost())
            uploadBatchxml();
            if (StatusChanged != null)
            {
                StatusChanged(this, "XML UPLOAD Completed");
            }
            uploadBatchPDF();
            if (StatusChanged != null)
            {
                StatusChanged(this, "PDF UPLOAD Completed");
            }
            submitbatch();
            if (StatusChanged != null)
            {
                StatusChanged(this, "Batch UPLOAD Completed");
            }


            loadBatchObjectAll(false);
            if (BatchCompleted != null)
            {
                //   getBatchStatus();
                BatchCompleted(this);

            }
            if (JobDetailsAvailable != null)
            {
                ActivateJobCheck();
            }

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
        private void refCreate()
        {

            bool canCreate = false;
            if (this.StoreReferencePathDirectory != null)
            {
                if (!Directory.Exists(this.StoreReferencePathDirectory.FullName))
                {
                    Directory.CreateDirectory(this.StoreReferencePathDirectory.FullName);
                }

                foreach (Jobs job in JobList)
                {
                    if (job.MergeFile != null || (job.RefId != null && job.RefId.Length > 0))
                    {
                        canCreate = true;
                        break;
                    }
                }

            }

            if (canCreate)
            {
                using (StreamWriter writetext = new StreamWriter(getStoreRefPath().FullName))
                {

                    foreach (Jobs job in JobList)
                    {
                        var jobRefId = (job.RefId == null ? " " : job.RefId);
                        var mergeFile = (job.MergeFile.FullName == null ? " " : job.MergeFile.FullName);
                        writetext.WriteLine(jobRefId + "|" + mergeFile + "|" + job.StartingPage.ToString() + "|" + job.EndingPage.ToString() + "|" + (this.RefId != null ? this.RefId : ""));
                    }

                }

            }



        }

    }
}
