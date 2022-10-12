using System;

using System.Text;


namespace c2mAPI
{
    public class Authentication
    {
      
        public string username { get; set; }
        public string password { get; set; }
        public Environment.Mode mode { get; set; }
        private readonly string batchURL = "batch.click2mail.com";
        private readonly string restURL = "rest.click2mail.com";
        
        public string getBatchUrl()
        {
            if (this.mode == Environment.Mode.StageMode)
            {
                return "https://stage-" + batchURL;
            }
            return "https://" + batchURL;
        }
        public string getBasicAuthentication()
        {
            
           string authinfo = Convert.ToBase64String(Encoding.Default.GetBytes(this.username + ":" + this.password));
            return "Basic " + authinfo;
            // Create the web request  

            
        }
        public string getRestUrl()
        {
            if (this.mode == Environment.Mode.StageMode)
            {
                return "https://stage-" + restURL;
            }
            return "https://" + restURL;
        }
        public Authentication(string username,string pw, Environment.Mode mode)
        {
            this.username = username;
            this.password = pw;
            this.mode = mode;
        }
    }
}
