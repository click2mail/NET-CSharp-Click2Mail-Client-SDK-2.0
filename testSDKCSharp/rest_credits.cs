using System;
using System.Collections.Specialized;
using c2mAPI;
using c2mAPI.restAPI;
namespace testSDKCSharp
{
    class rest_credits
    {
        static public void runSimpleSend(String un, String pw, c2mAPI.Environment.Mode mode)
        {

            Credit d = new Credit(new Authentication(un, pw, c2mAPI.Environment.Mode.StageMode));
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("billingName", "John Doe");
            nvc.Add("billingAddress1", "6010 California Cir Apt 100");
            nvc.Add("billingCity", "Rockville");
            nvc.Add("billingState", "MD");
            nvc.Add("billingZip", "20852-4868");
            nvc.Add("billingAmount", "10");
            nvc.Add("billingNumber", "4111111111111111");
            nvc.Add("billingMonth", "01");
            nvc.Add("billingYear", "2030");
            nvc.Add("billingCvv", "123");
            nvc.Add("billingCcType", "VI");
            Console.WriteLine(d.addCredits(nvc));
            Console.ReadLine();
        }
    }

}