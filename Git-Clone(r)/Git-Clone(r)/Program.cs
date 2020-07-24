using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Git_Clone_r_.API_Classes;
using Newtonsoft.Json;

namespace Git_Clone_r_
{
    class Program
    {
        private static Dictionary<string, string> userSettings;

        static void Main(string[] args)
        {
            string userSettingsJson = File.ReadAllText("UserSettings.json");
            userSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(userSettingsJson);

            bool menuActive = true;

            while(menuActive)
            {
                DisplayMenu();

                int selection = Convert.ToInt32(Console.ReadLine());

                switch(selection)
                {
                    case 1:
                        //GetRepos.CloneRepos("public");
                        break;
                    case 2:
                        GetRepos.CloneRepos("public", userSettings);
                        break;
                    case 3:
                        GetRepos.PromptForRepoLink(userSettings);
                        break;
                    case 4:
                        GetRepos.CloneRepos("self", userSettings);
                        break;
                    case 5:
                        UserInputHandling.SetDefaultDir(userSettings);
                        break;
                    case 8:
                        UserInputHandling.SetDefaultUser(userSettings);
                        break;
                    case 11:
                        menuActive = false;
                        break;
                }
            }
        }

        private static void DisplayMenu()
        {
            ShowAsciiArt();

            Console.ForegroundColor = ConsoleColor.Red;

            try
            {
                if (string.IsNullOrWhiteSpace(userSettings["defaultDirectory"]))
                {
                    Console.WriteLine("(!) DEFAULT CLONE DIRECTORY NOT SET");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"(0) DEFAULT CLONE DIRECTORY: {userSettings["defaultDirectory"]}");
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                if (string.IsNullOrWhiteSpace(userSettings["defaultUsername"]))
                {
                    Console.WriteLine("(!) DEFAULT USER TO CLONE FROM NOT SET");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"(0) DEFAULT USER: {userSettings["defaultUsername"]}");
                }
            }
            catch(System.Collections.Generic.KeyNotFoundException)
            {
                Console.WriteLine("(!) UserSettings.json NOT FOUND");
            }
            
            Console.ForegroundColor = ConsoleColor.Gray;  

            Console.WriteLine("\n\nGit Clone(r) Menu:\n\n    " +
                "1: Clone your repositories (Includes the private repos)\n    " +
                "2: Clone a user's repositories\n    " + //^Have an inside one saying which repo to clone and then have an X for all
                "3: Clone a selected repository\n    " +
                "4: Clone from the default user\n    " + 
                "5: Set default clone directory\n    " +
                "6: Check default clone directory\n    " + //Does an 'ls' on the dir
                "7: Open default clone directory\n    " +
                "8: Set default user to clone from\n    " +
                "11: Exit"
            );
        }

        private static void ShowAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine();
            Console.WriteLine(@"   _____   _   _               _____   _                           __        __  
  / ____| (_) | |             / ____| | |                         / /        \ \ 
 | |  __   _  | |_   ______  | |      | |   ___    _ __     ___  | |   _ __   | |
 | | |_ | | | | __| |______| | |      | |  / _ \  | '_ \   / _ \ | |  | '__|  | |
 | |__| | | | | |_           | |____  | | | (_) | | | | | |  __/ | |  | |     | |
  \_____| |_|  \__|           \_____| |_|  \___/  |_| |_|  \___| | |  |_|     | |
                                                                  \_\        /_/ ");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(" Made by Nathan Jukes: https://github.com/nathanjukes\n\n");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
