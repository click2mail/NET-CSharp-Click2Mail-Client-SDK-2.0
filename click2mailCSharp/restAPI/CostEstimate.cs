using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2mAPI.restAPI
{
    public class CostEstimate
    {
        private WebPosts webPosts;
        public Authentication Auth;

        public CostEstimate(Authentication auth)
        {
            this.Auth = auth;
            this.webPosts = new WebPosts(auth);
        }
        public string getCostEstimate(ProductOptionsItem po, int qty, int numberOfPages = 1, string paymentType = "User Credit", int nonStandardQty = 0, int internationQty = 0)
        {
            System.Collections.Specialized.NameValueCollection y = new System.Collections.Specialized.NameValueCollection();
            y.Add("documentClass", po.DocumentClass);
            y.Add("layout", po.Layout);
            y.Add("productionTime", po.ProductionTime);
            y.Add("envelope", po.Envelope);
            y.Add("mailClass", po.MailClass);
            y.Add("color", po.PrintColor);
            y.Add("paperType", po.PaperType);
            y.Add("printOption", po.PrintOption);
            y.Add("quantity", qty.ToString());
            y.Add("nonStandardQuantity", nonStandardQty.ToString());
            y.Add("internationalQuantity", internationQty.ToString());
            y.Add("numberOfPages", numberOfPages.ToString());
            y.Add("paymentType", paymentType);
            return webPosts.createRestCall("/molpro/costEstimate", y, Method.GET);
        }

    }
}
