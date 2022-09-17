using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c2mAPI;
using Product = c2mAPI.ProductOptions.Letter_8_5_x_11;
namespace testSDKCSharp
{
    class batch_simpleSend
    {
        static public void runBatchSimpleSend(String un,String pw, c2mAPI.Environment.Mode mode)
        {
            Console.WriteLine("Starting Batch Test");
            Batchc2mAPI r = new Batchc2mAPI(new Authentication(un, pw, mode));
            r.StatusChanged += R_StatusChanged;
            r.BatchCompleted += R_BatchCompleted;

            List<Jobs> batchJobs = new List<Jobs>();
            List<AddressItem> addList = new List<AddressItem>();
           Jobs job;

            addList.Add(new AddressItem("John", "Smith", "MyOrg", "1234 Test st", "Ste 335", "", "Oak Brook", "IL", "60523", "United States"));
            addList.Add(new AddressItem("John", "Smith2", "MyOrg", "1234 Test st", "Ste 335", "", "Oak Brook", "IL", "60523", ""));

            //will send pages 1-6 of pdf to above 2 addresses using below settings
             
            job = new Jobs(1, 6
              , Product.DocumentClass
                , Product.Layout.C2M_Address_on_First_Page
                , Product.ProductionTime.C2M_Next_Day
                , Product.Envelope.C2M_NUMBER10_Double_Window
                , Product.PrintColor.C2M_Black_and_White
                , Product.PaperType.C2M_White_24NUMBER
                , Product.PrintOption.C2M_Printing_both_sides
                , Product.MailClass.C2M_First_Class
                , addList);
            batchJobs.Add(job);            
            
            //Second Job in batch                       
            addList = new List<AddressItem>();
            AddressItem address3 = new AddressItem("John3", "Smith", "MyOrg", "1234 Test Street", "Ste 335","", "Oak Brook", "IL", "60523", "United States");
            addList.Add(address3);

            //will send pages 7-10 of pdf to above address using below settings
            job = new Jobs(7, 10
                      , Product.DocumentClass
                , Product.Layout.C2M_Address_on_First_Page
                , Product.ProductionTime.C2M_Next_Day
                , Product.Envelope.C2M_NUMBER10_Double_Window
                , Product.PrintColor.C2M_Black_and_White
                , Product.PaperType.C2M_White_24NUMBER
                , Product.PrintOption.C2M_Printing_both_sides
                , Product.MailClass.C2M_First_Class
                , addList);
            batchJobs.Add(job);

            r.runComplete(@"C:\intel\test.pdf", batchJobs,"My Reference");
            Console.WriteLine("ALL ITEM ARE UPDATED, HERE IS THE ID:" + r.BatchId.ToString());
            Console.ReadLine();
            Console.WriteLine("Completed Batch Test");

        }
        static void R_StatusChanged(Batchc2mAPI b, string reason)
        {
            Console.WriteLine(reason);
            //Console.WriteLine(b.BatchId);
           // Console.WriteLine(b.RefId);
        }
        static void R_BatchCompleted(c2mAPI.Batchc2mAPI b)
        {
            Console.WriteLine("BATCH HAS COMPLETED");

            Console.WriteLine(b.BatchId);
            Console.WriteLine(b.CreatedAt);
            Console.WriteLine(b.Received);
            Console.WriteLine(b.ErrorMessage);
            Console.WriteLine(b.HasErrors);
            Console.WriteLine(b.Submitted);
            Console.WriteLine(b.DocumentName);
            Console.WriteLine(b.Status);

        }
    }
}

