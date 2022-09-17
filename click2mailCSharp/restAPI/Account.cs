using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2mAPI.restAPI
{
    public class Account
    {
        private WebPosts webPosts;
        public Authentication Auth;
        public static class AddressTypes{
            public static readonly string ReturnAddress = "Return address";
            public static readonly string BillingAddress = "Billing address";
            public static readonly string BusinessReply = "Business reply";
            public static readonly string CourtesyReplyAddress = "Courtesy reply address";
            public static readonly string EDDMMailerAddress = "EDDM Mailer address";

        }
        public Account(Authentication auth)
        {
            this.Auth = auth;
            this.webPosts = new WebPosts(auth);
        }
        public string addressAdd(NameValueCollection nvc)
        {   
            return webPosts.createRestCall("/molpro/account/addresses",nvc,Method.POST);
        }
        public string addressesGet(string addressType)
        {
           NameValueCollection nvc = new NameValueCollection();
            nvc.Add("addressType",addressType);
            return webPosts.createRestCall("/molpro/account/addresses", nvc, Method.GET);
        }
        public string authorize()
        {
            return webPosts.createRestCall("/molpro/account/authorize", null, Method.POST);
        }
        

    }
}
