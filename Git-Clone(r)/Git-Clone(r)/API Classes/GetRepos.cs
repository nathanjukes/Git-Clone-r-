using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Git_Clone_r_.API_Classes
{
    static class GetRepos
    {
        private static Dictionary<string, string> _gitLinks;
        private static Dictionary<string, string> _userSettings;

        public static void CloneRepos(string type, Dictionary<string, string> userSettings)
        {
            _userSettings = userSettings;

            if(type == "public")
            {
                GetPublicRepos();
            }
            else if(type == "private")
            {
                //Get private repos
            }
           

            foreach(var i in _gitLinks)
            {
                //Console.WriteLine(i.Key + $" : ({i.Value})");
                Clone(i.Value);
            }
        }

        public static void GetPublicRepos() //From a user
        {
            //Prompt for username to clone from
            Console.WriteLine("Please input the user you wish to clone from: ");
            string username = Console.ReadLine();
            _gitLinks = GetRepoLinks(username);
        }

        private static Dictionary<string, string> GetRepoLinks(string username)
        {
            _gitLinks = new Dictionary<string, string>();

            WebClient wc = new WebClient();
            wc.Headers.Add("user-agent", "*"); //Github was throwing 403 forbidden due to no user-agent header so I had to add this - it means nothing
            string data = wc.DownloadString($"https://api.github.com/users/{username}/repos");

            dynamic dataDeserialized = JsonConvert.DeserializeObject<dynamic>(data);

            foreach(var i in dataDeserialized)
            {
                _gitLinks.Add(i.name.ToString(), i.clone_url.ToString());
            }

            return _gitLinks;
        }

        private static void Clone(string cloneLink)
        {
            Process process = Process.Start("CMD.EXE", $@"/C {_userSettings["defaultDirectory"].Substring(0, 1)}:&&cd {_userSettings["defaultDirectory"]}&&git clone https://github.com/nathanjukes/nathanjukes.git&&echo Repo Cloned to {_userSettings["defaultDirectory"]}");
            process.WaitForExit();
        }
    }
}
