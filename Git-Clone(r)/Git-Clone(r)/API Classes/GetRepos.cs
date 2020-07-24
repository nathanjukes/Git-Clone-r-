using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

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
                _gitLinks = GetPublicRepos();

                if(CheckForNullRepos())
                {
                    return;
                }

                PromptForUserChoice();
            }
            else if(type == "private")
            {
                //Get private repos
            }
            else if(type == "self")
            {
                _gitLinks = GetRepoLinks(_userSettings["defaultUsername"]);

                if (CheckForNullRepos())
                {
                    return;
                }

                PromptForUserChoice();
            }
            
        }

        private static Dictionary<string, string> GetPublicRepos() //From a user
        {
            //Prompt for username to clone from
            Console.WriteLine("Please input the user you wish to clone from: ");
            string username = Console.ReadLine();
            return GetRepoLinks(username);
        }

        private static Dictionary<string, string> GetRepoLinks(string username)
        {
            _gitLinks = new Dictionary<string, string>();

            WebClient wc = new WebClient();
            wc.Headers.Add("User-Agent", "*"); //Github was throwing 403 forbidden due to no user-agent header so I had to add this - it means nothing

            string data;
            int currentPage = 1;
            int repoCount = 265;
            bool finalPage = false;

            try
            {
                WebClient wcTemp = new WebClient();
                wcTemp.Headers.Add("user-agent", "****");
                string userData = wcTemp.DownloadString($"https://api.github.com/users/{username}");
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(userData);
                repoCount = jsonData.public_repos;
            }
            catch (System.Net.WebException) { };

            while(!finalPage)
            {
                if (_gitLinks.Count + 100 > repoCount)
                {
                    Console.WriteLine(_gitLinks.Count);
                    finalPage = true;
                }

                try
                { 
                    data = wc.DownloadString($"https://api.github.com/users/{username}/repos?page={currentPage.ToString()}&per_page=100");
                }
                catch (System.Net.WebException)
                {
                    Console.WriteLine($"fatal: username '{username}' does not exist");
                    return new Dictionary<string, string>();
                }

                dynamic dataDeserialized = JsonConvert.DeserializeObject<dynamic>(data);

                foreach (var i in dataDeserialized)
                {
                    _gitLinks.Add(i.name.ToString(), i.clone_url.ToString());
                }

                currentPage++;
            }

            return _gitLinks;
        }

        public static void PromptForRepoLink(Dictionary<string, string> userSetting)
        {
            _userSettings = userSetting;

            Console.WriteLine("Please input the link of the repo you wish to clone: ");
            string cloneLink = Console.ReadLine();

            Clone(cloneLink);
        }

        private static void PromptForUserChoice()
        {
            Console.WriteLine("\n\n═════╣ Available Repositories ╠═════\n");

            for (int i = 0; i < _gitLinks.Count; i++)
            {
                Console.WriteLine($"{i+1}) {_gitLinks.ElementAt(i).Key}");
            }

            Console.WriteLine("\n[ '1' - Clones the first repo ]\n[ '1-5' - Clones the repos within that range (inclusively) ]\n[ 'X' - Clones every repo ]\n\nPlease input beneath:");

            string userInput = Console.ReadLine().Trim();

            if(userInput.ToUpper() == "X")
            {
                Console.WriteLine("\nCloning every repository\n");

                foreach(var i in _gitLinks.Values)
                {
                    Clone(i);
                }
            }
            else if(userInput.Contains("-"))
            {
                try
                {
                    int lowIndex = Convert.ToInt32(userInput.Substring(0, userInput.IndexOf('-')));
                    int highIndex = Convert.ToInt32(userInput.Substring(userInput.IndexOf('-') + 1, userInput.Length - userInput.IndexOf('-') - 1));

                    if(highIndex > _gitLinks.Count || lowIndex <= 0)
                    {
                        Console.WriteLine("Incorrect Bounds");
                        return;
                    }

                    for(int i = lowIndex; i <= highIndex; i++)
                    {
                        Clone(_gitLinks.ElementAt(i).Value);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Incorrect input");
                }
            }
            else
            {
                try
                {
                    Clone(_gitLinks.ElementAt(Convert.ToInt32(userInput)).Value);
                }
                catch(FormatException)
                {
                    Console.WriteLine("Incorrect input");
                }
            }
        }

        private static void Clone(string cloneLink)
        {
            Console.WriteLine($"\n{cloneLink}");
            Process process = Process.Start("CMD.EXE", $@"/C {_userSettings["defaultDirectory"].Substring(0, 1)}:&&cd {_userSettings["defaultDirectory"]}&&git clone {cloneLink}&&echo Repo Cloned to {_userSettings["defaultDirectory"]}");
            process.WaitForExit();
        }

        private static bool CheckForNullRepos()
        {
            if (_gitLinks.Count == 0)
            {
                Console.WriteLine("No Repos Available");
                return true;
            }
            return false;
        }
    }
}
