using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2mAPI
{
    public class ReturnAddressItem
    {


        public string AncilaryEndorsement = Endorsement.NoAncillaryEndorsement;
        public string ReturnAddressName { get; set; }
        public string ReturnAddress1 { get; set; }
        public string ReturnAddress2 { get; set; }

        public string ReturnCity { get; set; }
        public string ReturnState { get; set; }
        public string ReturnZip { get; set; }
        public string ReturnOrginization { get; set; }
        public ReturnAddressItem(string ReturnAddressName = null, string ReturnAddress1 = null, string ReturnAddress2 = null, string ReturnCity = null, string ReturnState = null, string ReturnZip = null,string AncilaryEndorsement = "No Ancillary Endorsement")
        {
            this.ReturnAddressName = ReturnAddressName;
            this.ReturnAddress1 = ReturnAddress1;
            this.ReturnAddress2 = ReturnAddress2;

            this.ReturnCity = ReturnCity;
            this.ReturnState = ReturnState;
            this.ReturnZip = ReturnZip;
            this.AncilaryEndorsement = AncilaryEndorsement;
        }
    }
    public static class Endorsement
    {
        public static readonly string NoAncillaryEndorsement = "No Ancillary Endorsement";
        public static readonly string ForwardingServiceRequested = "Forwarding Service Requested";
        public static readonly string AddressServiceRequested = "Address Service Requested";
        public static readonly string ReturnServiceRequested = "Return Service Requested";
         
    }

}
