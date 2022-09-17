using c2mAPI;
using c2mAPI.restAPI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testSDKCSharp
{
    internal class MiscTests
    {
        public Authentication Auth { get; set; }    

        void accountAddressAdd()
        {
            Account a = new Account(Auth);
            NameValueCollection nvc1 = new NameValueCollection();
            nvc1.Add("type", Account.AddressTypes.ReturnAddress);
            nvc1.Add("description", "My Return");
            nvc1.Add("address1", "6010 California Cir Apt 100");
            nvc1.Add("city", "Rockville");
            nvc1.Add("state", "MD");
            nvc1.Add("zip", "20852");
            nvc1.Add("firstName", "John");
            nvc1.Add("middleName", "E");
            nvc1.Add("lastName", "Doe");
            Console.Write(a.addressAdd(nvc1));
         
            NameValueCollection nvc2 = new NameValueCollection();
            nvc2.Add("type", Account.AddressTypes.BillingAddress);
            nvc2.Add("description", "My Billing");
            nvc2.Add("address1", "6010 California Cir Apt 100");
            nvc2.Add("city", "Rockville");
            nvc2.Add("state", "MD");
            nvc2.Add("zip", "20852");
            nvc2.Add("firstName", "John");
            nvc2.Add("middleName", "E");
            nvc2.Add("lastName", "Doe");
            nvc2.Add("makeDefault", "yes");
            Console.Write(a.addressAdd(nvc2));

            NameValueCollection nvc3 = new NameValueCollection();
            nvc3.Add("type", Account.AddressTypes.EDDMMailerAddress);
            nvc3.Add("description", "My Billing");
            nvc3.Add("address1", "6010 California Cir Apt 100");
            nvc3.Add("city", "Rockville");
            nvc3.Add("state", "MD");
            nvc3.Add("zip", "20852");
            nvc3.Add("firstName", "John");
            nvc3.Add("middleName", "E");
            nvc3.Add("lastName", "Doe");
            nvc3.Add("makeDefault", "yes");
            nvc3.Add("organization", "C2M LLC");
            nvc3.Add("phone", "301-529-0603");
            Console.Write(a.addressAdd(nvc3));

            NameValueCollection nvc4 = new NameValueCollection();
            nvc4.Add("type", Account.AddressTypes.BusinessReply);
            nvc4.Add("description", "My Billing");
            nvc4.Add("address1", "6010 California Cir Apt 100");
            nvc4.Add("city", "Rockville");
            nvc4.Add("state", "MD");
            nvc4.Add("zip", "20852");
            nvc4.Add("firstName", "John");
            nvc4.Add("middleName", "E");
            nvc4.Add("lastName", "Doe");
            nvc4.Add("makeDefault", "yes");
            nvc4.Add("organization", "C2M LLC");
            nvc4.Add("permitNumber", "98765");
            nvc4.Add("replyRegionId", "12345");
            nvc4.Add("replyCity", "Rockville");
            Console.Write(a.addressAdd(nvc4));

            NameValueCollection nvc5 = new NameValueCollection();
            nvc4.Add("type", Account.AddressTypes.CourtesyReplyAddress);
            nvc4.Add("description", "My Billing");
            nvc4.Add("address1", "6010 California Cir Apt 100");
            nvc4.Add("city", "Rockville");
            nvc4.Add("state", "MD");
            nvc4.Add("zip", "20852");
            nvc4.Add("firstName", "John");
            nvc4.Add("middleName", "E");
            nvc4.Add("lastName", "Doe");
            nvc4.Add("makeDefault", "yes");
            nvc4.Add("organization", "C2M LLC");
            Console.Write(a.addressAdd(nvc4));


            Console.Write(a.addressesGet(Account.AddressTypes.ReturnAddress));
            Console.Write(a.addressesGet(Account.AddressTypes.BillingAddress));
            Console.Write(a.addressesGet(Account.AddressTypes.EDDMMailerAddress));
            Console.Write(a.addressesGet(Account.AddressTypes.CourtesyReplyAddress));
            Console.Write(a.addressesGet(Account.AddressTypes.BusinessReply));

        }
      
    }
}
