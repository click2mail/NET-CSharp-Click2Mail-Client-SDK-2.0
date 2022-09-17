using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c2mAPI;
using Product = c2mAPI.ProductOptions.Letter_8_5_x_11;
namespace testSDKCSharp
{
    class rest_SimpleSendCustomAddress_Test
    {
        static public void runSimpleSend(String un, String pw, c2mAPI.Environment.Mode mode)
        {
            Console.WriteLine("Starting REST Single Piece Test");
            int mappingId = 9999; //enter custom mapping ID.  MappingId 2 is the standard click2mail uses which is illustrated below
            /*
             <First_name>Test</First_name>
			<Last_name>Smith</Last_name>
			<organization>test org</organization>
			<Address1>6010 California Circle</Address1>
			<Address2>Apt 3</Address2>
			<City>Rockville</City>
			<State>MD</State>
			<Zip>20852</Zip>
			<Country_non-US />
            */
            Jobs r = new Jobs(new Authentication(un, pw, mode));
            
            //CREATE LIST CONTAINER
            List<List<KeyValuePair<String, String>>> addressList =new List<List<KeyValuePair<String, String>>>();
            //Create Address part container
        List<KeyValuePair<String, String>> customAddressItem = new List<KeyValuePair<String, String>>();
        customAddressItem.Add(new KeyValuePair< String, String>("name", "John"));;
        customAddressItem.Add(new KeyValuePair< String, String>("Address1", "1234 Test Street"));
        customAddressItem.Add(new KeyValuePair< String, String>("Address2", "Ste 335"));
        customAddressItem.Add(new KeyValuePair< String, String>("City", "Oak Brook"));
        customAddressItem.Add(new KeyValuePair< String, String>("State", "IL"));
        customAddressItem.Add(new KeyValuePair< String, String>("Zip", "60523"));
        addressList.Add(customAddressItem);
        customAddressItem = new List<KeyValuePair< String, String>>();
        customAddressItem.Add(new KeyValuePair< String, String>("name", "John2"));
        customAddressItem.Add(new KeyValuePair< String, String>("Address1", "1234 Test Street"));
        customAddressItem.Add(new KeyValuePair< String, String>("Address2", "Ste 335"));
        customAddressItem.Add(new KeyValuePair< String, String>("City", "Oak Brook"));
        customAddressItem.Add(new KeyValuePair< String, String>("State", "IL"));
        customAddressItem.Add(new KeyValuePair< String, String>("Zip", "60523"));
        addressList.Add(customAddressItem);

            ProductOptionsItem po = new ProductOptionsItem();
                po.DocumentClass = Product.DocumentClass;


            po.Layout = Product.Layout.C2M_Address_on_First_Page; 
            po.ProductionTime = Product.ProductionTime.C2M_Next_Day;
                po.Envelope = Product.Envelope.C2M_NUMBER10_Double_Window;
                po.PrintColor = Product.PrintColor.C2M_Black_and_White;
                po.PaperType = Product.PaperType.C2M_White_24NUMBER;
                po.PrintOption = Product.PrintOption.C2M_Printing_both_sides;
        r.runComplete(@"C:\c2m\test.pdf","PDF", r.createXMLFromCustomList(addressList, mappingId),po
                );
        Console.ReadLine();

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
