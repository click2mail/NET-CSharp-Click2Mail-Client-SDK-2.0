using System;
using System.Collections.Specialized;
using c2mAPI;
using Product = c2mAPI.ProductOptions.Letter_8_5_x_11;
namespace testSDKCSharp
{
    class rest_costEstimate
    {
        static public void runSimpleSend(String un, String pw, c2mAPI.Environment.Mode mode)
        {
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

            c2mAPI.restAPI.CostEstimate ce = new c2mAPI.restAPI.CostEstimate(new Authentication(un, pw, mode));

            Console.Write(ce.getCostEstimate(po, 2));
            Console.ReadLine();

        }
    }

}