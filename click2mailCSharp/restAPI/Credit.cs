using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2mAPI.restAPI
{
    public class Credit
    {
        private WebPosts webPosts;
        public Authentication Auth;
       
        public Credit(Authentication auth)
        {
            this.Auth = auth;
            this.webPosts = new WebPosts(auth);
        }
        public string getCredits()
        {   
            
            return webPosts.createRestCall("/molpro/credit", new NameValueCollection(), Method.GET);
        }
        public string addCredits(NameValueCollection nvc)
        {
           
            return webPosts.createRestCall("/molpro/credit/purchase", nvc, Method.POST);
        }
    }
}
