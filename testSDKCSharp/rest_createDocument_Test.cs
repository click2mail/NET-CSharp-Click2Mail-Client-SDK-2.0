using System;
using c2mAPI;
using c2mAPI.restAPI;
namespace testSDKCSharp
{
    class rest_createDocument_Test
    {
        static public void runSimpleSend(String un, String pw, c2mAPI.Environment.Mode mode)
        {

            Document d = new Document(new Authentication(un, pw, c2mAPI.Environment.Mode.StageMode));
            ProductOptionsItem po = new ProductOptionsItem();
            po.DocumentClass = c2mAPI.ProductOptions.Letter_8_5_x_11.DocumentClass;
            Console.Write("DocumentID IS:" + d.createDocument(@"C:\c2m\test.pdf", po, DocumentTypes.PDF));           
            Console.Write("DocumentID IS:" + d.createDocument(@"C:\c2m\test.png", po, DocumentTypes.PNG));
            Console.Write("DocumentID IS:" + d.createDocument(@"http://www.legalandcompliance.com/wp-content/uploads/2011/12/LC-Whitepaper-NASDAQ-Listing-Requirements.pdf", po, DocumentTypes.PDF));
            Console.ReadLine();
        }
    }

}

