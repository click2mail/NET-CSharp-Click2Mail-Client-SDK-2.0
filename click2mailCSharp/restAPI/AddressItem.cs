using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2mAPI
{
    public class AddressItem
    {
        public string FirstName { get; set; }= "";
        public string LastName { get; set; } = "";
        public string Organization { get; set; } = "";
        public string Address1  { get; set; }= "";
        public string Address2  { get; set; }= "";
        public string Address3  { get; set; }= "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Zip { get; set; } = "";
        public string FineCorrectionMessage { get; set; } = "";
        public string CoarseCorrectionMessage { get; set; } = "";
        public bool Standard { get; set; } = true;

        public bool International { get; set; } = false;
        public bool NonMailable { get; set; } = false;
        public string CountryNonUS { get; set; } = "";

        public AddressItem(string FirstName,string LastName,string Organization,string Address1,string Address2,string Address3, string City, string State, string Zip, string CountryNonUS = "")
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Organization = Organization;
            this.Address1 = Address1;
            this.Address2 = Address2;
            this.Address3 = Address3;
            this.City = City;
            this.State = State;
            this.Zip = Zip;
            this.CountryNonUS = CountryNonUS;
            
        }
    }
}
