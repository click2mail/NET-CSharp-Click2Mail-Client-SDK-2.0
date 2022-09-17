
using System;
using System.Collections.Generic;
using c2mAPI;
using Product = c2mAPI.ProductOptions.Letter_8_5_x_11;

namespace testSDKCSharp
{
    class batch_merge
    {
        
        static public  void runBatchSimpleSendMultipleFiles(String un, String pw, c2mAPI.Environment.Mode mode)
        {
            Jobs job = null;
             
            Batchc2mAPI r = new Batchc2mAPI(new Authentication(un, pw, mode), "My Special Batch Reference", new System.IO.DirectoryInfo(@"c:\intel\batchRef\"));
            r.StatusChanged += r_statusChanged;
            r.BatchCompleted += r_batchCompleted;
            r.JobDetailsAvailable += R_JobDetailsAvailable;
            List<Jobs> batchJobs = new List<Jobs>();            

            

            ProductOptionsItem productOptions = new ProductOptionsItem();
            productOptions.DocumentClass = Product.DocumentClass;
            productOptions.Layout = Product.Layout.C2M_Address_on_First_Page;
            productOptions.ProductionTime = Product.ProductionTime.C2M_Next_Day;
            productOptions.Envelope = Product.Envelope.C2M_NUMBER10_Double_Window;
            productOptions.PrintColor = Product.PrintColor.C2M_Black_and_White;
            productOptions.PaperType = Product.PaperType.C2M_White_24NUMBER;
            productOptions.PrintOption = Product.PrintOption.C2M_Printing_both_sides;
            productOptions.MailClass = Product.MailClass.C2M_First_Class;

            ///STARTING OF JOBS
            List<AddressItem> addList = new List<AddressItem>();
            addList.Add(new AddressItem("John", "Smith2", "MyOrg", "1234 Test st", "Ste 335", "", "Oak Brook", "IL", "60523"));
            job = new Jobs(new System.IO.FileInfo(@"C:\intel\test.pdf"), productOptions, addList, null, "My Ref 1");
            batchJobs.Add(job);

            
            addList = new List<AddressItem>();
            
            addList.Add(new AddressItem("John3", "Smith", "MyOrg", "1234 Test Street", "Ste 335", "", "Oak Brook", "IL", "60523"));
            job = new Jobs(new System.IO.FileInfo(@"C:\intel\TEST_VARIOUS.pdf"), productOptions, addList, null, "My Ref 2");
            batchJobs.Add(job);

            r.runCompleteMultipleFiles(batchJobs, @"C:\intel\mergedFiles\");
            
            Console.WriteLine("Completed Batch Test");
            Console.ReadLine();
            
        }

        private static void R_JobDetailsAvailable(Batchc2mAPI b)
        {
            string myRef = b.RefId != null ? b.RefId : "";
            foreach (Jobs j in b.JobList)
            {
                System.Console.WriteLine("THIS BATCH HAS JOBS NOW: " + j.JobId.ToString() + " and this jobs ref was " + j.RefId + " and this batchref is :" + myRef);
            }
        }

        static void r_statusChanged(Batchc2mAPI b,string reason)
        {
            Console.WriteLine(reason);
            //Console.WriteLine(batchId);
            //Console.WriteLine(refId);
        }
        static void r_batchCompleted(c2mAPI.Batchc2mAPI b)
        {
            Console.WriteLine("BATCH HAS COMPLETED:" + (b.RefId != null ? b.RefId : ""));

            /*
             * Console.WriteLine(b.BatchId);
            Console.WriteLine(b.CreatedAt);
            Console.WriteLine(b.Received);
            Console.WriteLine(b.ErrorMessage);
            Console.WriteLine(b.HasErrors);
            Console.WriteLine(b.Submitted);
            Console.WriteLine(b.DocumentName);
            Console.WriteLine(b.Status);
            */

        }
    }
}



