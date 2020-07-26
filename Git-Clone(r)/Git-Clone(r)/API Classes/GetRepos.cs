using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Git_Clone_r_.API_Classes
{
    static class GetRepos
    {
        private static Dictionary<string, string> _gitLinks;
        private static Dictionary<string, string> _userSettings;

        public static void CloneRepos(string type, Dictionary<string, string> userSettings)
        {
            _userSettings = userSettings;

            if (type == "public")
            {
                _gitLinks = GetPublicRepos();

                if (CheckForNullRepos())
                {
                    return;
                }

                PromptForUserChoice();
            }
            else if (type == "private")
            {
                _gitLinks = GetPrivateRepos(_userSettings["defaultUsername"]);

                if (CheckForNullRepos())
                {
                    return;
                }

                PromptForUserChoice();
            }
            else if (type == "self")
            {
                if (!string.IsNullOrWhiteSpace(_userSettings["defaultUsername"]))
                {
                    _gitLinks = GetRepoLinks(_userSettings["defaultUsername"]);
                }
                else
                {
                    Console.WriteLine("Error: No default user set");
                }


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
            Console.WriteLine("\nPlease input the user you wish to clone from: ");
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

            while (!finalPage)
            {
                if (_gitLinks.Count + 100 > repoCount)
                {
                    finalPage = true;
                }

                try
                {
                    data = wc.DownloadString($"https://api.github.com/users/{username}/repos?page={currentPage}&per_page=100");
                }
                catch (System.Net.WebException)
                {
                    Console.WriteLine($"fatal: could not get page {currentPage} for '{username}'");
                    return _gitLinks;
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

        //Uses Github Authorization Code Flow for Getting an OAuth token to access Private Repos
        private static Dictionary<string, string> GetPrivateRepos(string username)
        {
            WebClient wc = new WebClient();
            _gitLinks = new Dictionary<string, string>();

            string OAuthToken = AuthorizationCodeFlow.ReturnOAuthToken(_userSettings);
            wc.Headers.Add("Authorization", $"token {OAuthToken}");
            wc.Headers.Add("User-Agent", "*");

            string data;

            try
            {
                data = wc.DownloadString("https://api.github.com/user/repos");
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine($"fatal: could not get private repos from '{username}'");
                return _gitLinks;
            }

            dynamic dataDeserialized = JsonConvert.DeserializeObject<dynamic>(data);

            foreach (var i in dataDeserialized)
            {
                _gitLinks.Add(i.name.ToString(), i.clone_url.ToString());
            }

            return _gitLinks;
        }

        public static void PromptForRepoLink(Dictionary<string, string> userSetting)
        {
            _userSettings = userSetting;

            Console.WriteLine("Please input the link of the repo you wish to clone: ");
            string cloneLink = Console.ReadLine();

            if(string.IsNullOrWhiteSpace(cloneLink))
            {
                Console.WriteLine($"fatal: repository '{cloneLink}' does not exist");
            }
            else
            {
                Clone(cloneLink);
            }
        }

        private static void PromptForUserChoice()
        {
            Console.WriteLine("\n\n═════╣ Available Repositories ╠═════\n");

            for (int i = 0; i < _gitLinks.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {_gitLinks.ElementAt(i).Key}");
            }

            Console.WriteLine("\n[ '1' - Clones the first repo ]\n[ '1-5' - Clones the repos within that range (inclusively) ]\n[ 'A' - Clones every repo ]\n[ 'X' - Return ]\n\nPlease input beneath:");

            string userInput = Console.ReadLine().Trim();

            if (userInput.ToUpper() == "A")
            {
                Console.WriteLine("\nCloning every repository: \n");

                foreach (var i in _gitLinks.Values)
                {
                    Clone(i);
                }
            }
            else if (userInput.Contains("-"))
            {
                try
                {
                    int lowIndex = Convert.ToInt32(userInput.Substring(0, userInput.IndexOf('-')));
                    int highIndex = Convert.ToInt32(userInput.Substring(userInput.IndexOf('-') + 1, userInput.Length - userInput.IndexOf('-') - 1));

                    if (highIndex > _gitLinks.Count || lowIndex <= 0)
                    {
                        Console.WriteLine("Incorrect Bounds");
                        return;
                    }

                    for (int i = lowIndex; i <= highIndex; i++)
                    {
                        Clone(_gitLinks.ElementAt(i - 1).Value);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Incorrect input");
                }
            }
            else
            {
                if(userInput.ToUpper() == "X")
                {
                    Console.Clear();
                    return;
                }

                try
                {
                    Clone(_gitLinks.ElementAt(Convert.ToInt32(userInput)).Value);
                }
                catch (FormatException)
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
