using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace Git_Clone_r_.API_Classes
{
    static class AuthorizationCodeFlow
    {
        public static string ReturnOAuthToken(Dictionary<string, string> userSettings)
        {
            const string redirectURI = "http://localhost:29522/";
            string authURL = $"https://github.com/login/oauth/authorize?client_id={userSettings["clientID"]}&redirect_uri={redirectURI}&login=nathanjukes&scope=repo";

            Console.WriteLine("\nPlease check your web browser to authorize");

            string code = ListenForCode(redirectURI, authURL);

            if(!string.IsNullOrWhiteSpace(code))
            {
                string accessToken = GetAccessToken(userSettings, code);

                Console.Clear();

                return accessToken;
            }

            Console.WriteLine("Error: Not authenticated");
            return "";
        }

        private static string ListenForCode(string redirectURI, string authURL) //http://localhost:29522/?code=X - Example 
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) //Launches the Auth webpage
            {
                ProcessStartInfo proc = new ProcessStartInfo() { CreateNoWindow = true, Arguments = $@"/c start {authURL}", FileName = "CMD.exe" };
                Process.Start(proc);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
            {
                Process.Start("xdg-open", authURL);
            }
            else
            {
                Process.Start("open", authURL); //OSX 
            }

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(redirectURI);

            try
            {
                listener.Start();
                
                HttpListenerContext context = listener.GetContext();
                NameValueCollection queryStrings = context.Request.QueryString;
                
                return queryStrings["code"];
            }
            catch(Exception e)
            {
                Console.WriteLine("Too many OAuth token requests");
                return "";
            }
        }

        private static string GetAccessToken(Dictionary<string, string> userSettings, string code)
        {
            string getAccessTokenURI = $"https://github.com/login/oauth/access_token?client_id={userSettings["clientID"]}&client_secret={userSettings["clientSecret"]}&code={code}";
            string OAuthToken = "";

            using (WebClient wc = new WebClient())
            {
                string data = wc.UploadString(getAccessTokenURI, "POST", "");
                OAuthToken = HttpUtility.ParseQueryString(data).Get("access_token");
            }
            
            return OAuthToken;
        }
    }
}
