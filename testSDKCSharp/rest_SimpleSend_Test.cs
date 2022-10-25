using System;
using c2mAPI;
using Product = c2mAPI.ProductOptions.Letter_8_5_x_11;
namespace testSDKCSharp
{
    class rest_SimpleSend
    {
        static public void runSimpleSend(String un, String pw, c2mAPI.Environment.Mode mode)
        {


            Console.WriteLine("Starting REST TEST");

            Jobs r = new Jobs(new Authentication(un, pw, mode));
            r.StatusChanged += r_statusChanged;
            r.JobStatusCheck += r_jobStatusCheck;

            r.AddressList.Clear(); //Not needed just good habit to clear before you add any addresses
            AddressItem x;
            x = new AddressItem("John", "Smith", "TestOrg", "1234 Test Street", "Ste 335", "", "Oak Brook", "IL", "60523");
            r.AddressList.Add(x);
            x = new AddressItem("John", "Smith2", "TestOrg", "1234 Test Street", "Ste 335", "", "Oak Brook", "IL", "60523");
            r.AddressList.Add(x);

            ProductOptionsItem po = new ProductOptionsItem(
               Product.DocumentClass
             , Product.Layout.C2M_Address_on_First_Page
             , Product.ProductionTime.C2M_Next_Day
             , Product.Envelope.C2M_NUMBER10_Double_Window
             , Product.PrintColor.C2M_Black_and_White
             , Product.PaperType.C2M_White_24NUMBER
             , Product.PrintOption.C2M_Printing_both_sides
             , Product.MailClass.C2M_First_Class
             );

            r.runComplete(@"C:\intel\test.pdf", DocumentTypes.PDF, r.createXMLFromAddressList(), po
                , new ReturnAddressItem("Return Address Name", "1234 Test Street", "", "Oak Brook", "IL", "60523")
                );
            Console.ReadLine();
            Console.WriteLine("Completed REST TEST");
        }

        static void r_jobStatusCheck(string id, string status, string description)
        {
            Console.WriteLine("jobId is:" + id);
            Console.WriteLine("job Status is:" + status);
            Console.WriteLine("job Description is:" + description);
        }

        static void r_statusChanged(string reason)
        {
            Console.WriteLine(reason);
        }
    }
}
