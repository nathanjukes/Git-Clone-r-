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
                PromptForUserChoice();
            }
            else if(type == "private")
            {
                //Get private repos
            }
        }

        public static Dictionary<string, string> GetPublicRepos() //From a user
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
            wc.Headers.Add("user-agent", "*"); //Github was throwing 403 forbidden due to no user-agent header so I had to add this - it means nothing
            string data = wc.DownloadString($"https://api.github.com/users/{username}/repos");

            dynamic dataDeserialized = JsonConvert.DeserializeObject<dynamic>(data);

            foreach(var i in dataDeserialized)
            {
                _gitLinks.Add(i.name.ToString(), i.clone_url.ToString());
            }

            return _gitLinks;
        }

        private static void PromptForUserChoice()
        {
            Console.WriteLine("\n\n═════╣ Available Repositories ╠═════\n");

            for (int i = 0; i < _gitLinks.Count; i++)
            {
                Console.WriteLine($"{i+1}) {_gitLinks.ElementAt(i).Key}");
            }

            Console.WriteLine("\n[ '1' - Clones the first repo || '1-5' - Clones the repos within that range (inclusively) || 'X' - Clones every repo ]\n\nPlease input beneath:");

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
            Process process = Process.Start("CMD.EXE", $@"/C {_userSettings["defaultDirectory"].Substring(0, 1)}:&&cd {_userSettings["defaultDirectory"]}&&git clone https://github.com/nathanjukes/nathanjukes.git {_gitLinks.Where(x => x.Value == cloneLink).FirstOrDefault().Key}&&echo Repo Cloned to {_userSettings["defaultDirectory"]}");
            process.WaitForExit();
        }
    }
}
