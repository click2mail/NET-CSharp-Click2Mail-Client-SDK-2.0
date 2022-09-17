using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2mAPI
{
    public class WebPosts
    {
        Authentication Auth { get; set; }
        public WebPosts(Authentication auth)
        {
            Auth = auth;
        }
        public string createXMLPost(string uri, string xml)
        {
            //    Console.WriteLine("Calling URL " + uri);
            string responseText = "";
            int attempts = 0;
            int MAX_ATTEMPTS = 5;
            bool tryAgain = true;

            while (tryAgain && attempts < MAX_ATTEMPTS)
            {
                //Console.WriteLine("Starting attempt " + attempts);
                attempts++;
                try
                {
                    var client = new RestClient();
                    client.Authenticator = new HttpBasicAuthenticator(Auth.username, Auth.password);

                    var request = new RestRequest(uri, Method.POST);
                    request.AddHeader("Content-Type", "application/xml");
                    request.AddHeader("Accept", "application/xml");
                    request.AddParameter("application/xml", xml, ParameterType.RequestBody);

                    IRestResponse response = client.Post(request);
                    int httpResponseCode = (int)response.StatusCode;
                    if (httpResponseCode == 500 || httpResponseCode == 502 || httpResponseCode == 504)
                    {
                        Console.WriteLine("Received HTTP response code " + httpResponseCode + ". Will try again in 60 sec.");
                        System.Threading.Thread.Sleep(60000);
                        tryAgain = true;
                    }
                    else
                    {
                        responseText = response.Content;
                        tryAgain = false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception encountered: " + e.Message);
                    tryAgain = false;
                }
            }

            return responseText;
        }
        public string callAPIWithFileParam(string uri, string filePath, string fileParameterName, System.Collections.Specialized.NameValueCollection otherParameters)
        {
            
            string responseText = "";
            int attempts = 0;
            int MAX_ATTEMPTS = 5;
            bool tryAgain = true;

            while (tryAgain && attempts < MAX_ATTEMPTS)
            {
                attempts++;
                try
                {
                    var client = new RestClient();
                    client.Authenticator = new HttpBasicAuthenticator(Auth.username, Auth.password);

                    var request = new RestRequest(uri, Method.POST);
                    request.AddHeader("Content-Type", "multipart/form-data");
                    request.AddHeader("Accept", "application/xml");
                    request.AddFile(fileParameterName, filePath);
                    foreach (string key in otherParameters.Keys)
                    {
                        request.AddParameter(key, otherParameters[key]);
                    }

                    IRestResponse response = client.Post(request);
                    int httpResponseCode = (int)response.StatusCode;
                    if (httpResponseCode == 500 || httpResponseCode == 502 || httpResponseCode == 504)
                    {
                        Console.WriteLine("Exception HTTP respnonse code is " + httpResponseCode + ". Will try again in 60 sec.");
                        System.Threading.Thread.Sleep(60000);
                        tryAgain = true;
                    }
                    else
                    {
                        responseText = response.Content;
                        tryAgain = false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception encountered: " + e.Message);
                    tryAgain = false;
                }
            }
            return responseText;
        }

    
    public async Task<string> createRestCallAsync(string url, System.Collections.Specialized.NameValueCollection nameValueCollection, Method method )
        {

            //Console.WriteLine("Calling URL " + url);
            string responseText = "";

            int attempts = 0;
            int MAX_ATTEMPTS = 5;
            bool tryAgain = true;
            while (tryAgain && attempts < MAX_ATTEMPTS)
            {
                //  Console.WriteLine("Starting attempt " + attempts);
                attempts++;
                try
                {
                    var client = new RestClient();
                    client.Authenticator = new HttpBasicAuthenticator(Auth.username, Auth.password);
                    var request = new RestRequest(url, method);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddHeader("Accept", "application/xml");
                    foreach (string key in nameValueCollection.Keys)
                    {
                        request.AddParameter(key, nameValueCollection[key]);
                    }

                    //IRestResponse response = (method == Method.POST ? client.Post(request) : client.Get(request));
                    var cancellationTokenSource = new System.Threading.CancellationTokenSource();
                    var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);

                    int httpResponseCode = (int)response.StatusCode;
                    if (httpResponseCode == 500 || httpResponseCode == 502 || httpResponseCode == 504)
                    {
                        Console.WriteLine("Received HTTP response code " + httpResponseCode + ". Will try again in 60 sec.");
                        System.Threading.Thread.Sleep(60000);
                        tryAgain = true;
                    }
                    else
                    {



                        responseText = response.Content;
                        tryAgain = false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception encountered: " + e.Message);
                    tryAgain = false;
                }
            }
            return responseText;
        }


        public string createRestCall(string url, System.Collections.Specialized.NameValueCollection nameValueCollection, Method method)
        {
            //Console.WriteLine("Calling URL " + url);
            string responseText = "";

            int attempts = 0;
            int MAX_ATTEMPTS = 5;
            bool tryAgain = true;
            while (tryAgain && attempts < MAX_ATTEMPTS)
            {
                // Console.WriteLine("Starting attempt " + attempts);
                attempts++;
                try
                {
                    var client = new RestClient();
                    client.Authenticator = new HttpBasicAuthenticator(Auth.username, Auth.password);
                    var request = new RestRequest(url, method);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddHeader("Accept", "application/xml");
                    foreach (string key in nameValueCollection.Keys)
                    {
                        request.AddParameter(key, nameValueCollection[key]);
                    }
                    IRestResponse response = (method == Method.POST ? client.Post(request) : client.Get(request));
                    int httpResponseCode = (int)response.StatusCode;
                    if (httpResponseCode == 500 || httpResponseCode == 502 || httpResponseCode == 504)
                    {
                        Console.WriteLine("Received HTTP response code " + httpResponseCode + ". Will try again in 60 sec.");
                        System.Threading.Thread.Sleep(60000);
                        tryAgain = true;
                    }
                    else
                    {
                        responseText = response.Content;
                        tryAgain = false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception encountered: " + e.Message);
                    tryAgain = false;
                }
            }
            return responseText;
        }
    }
}
