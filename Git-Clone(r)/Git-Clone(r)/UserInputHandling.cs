using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Git_Clone_r_
{
    static class UserInputHandling
    {
        public static void SetDefaultDir(Dictionary<string, string> userSettings)
        {
            Console.WriteLine("Please input the directory you wish to set as the default for cloning: ");
            string dir = Console.ReadLine();

            Directory.CreateDirectory(dir); //Creates if it doesn't already exist

            try
            {
                userSettings["defaultDirectory"] = dir;
            }
            catch(System.Collections.Generic.KeyNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("(!) UserSettings.json NOT FOUND");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }
           
            string serializedUserSettings = JsonConvert.SerializeObject(userSettings);
            File.WriteAllText("UserSettings.json", serializedUserSettings);
        }

        public static void SetDefaultUser(Dictionary<string, string> userSettings)
        {
            Console.WriteLine("Please input the user you wish to set as the default for cloning from: ");
            string username = Console.ReadLine();

            try
            {
                userSettings["defaultUsername"] = username;
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("(!) UserSettings.json NOT FOUND");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }

            string serializedUserSettings = JsonConvert.SerializeObject(userSettings);
            File.WriteAllText("UserSettings.json", serializedUserSettings);
        }

        public static void OpenDefDir(Dictionary<string, string> userSettings)
        {
            if(!string.IsNullOrWhiteSpace(userSettings["defaultDirectory"]))
            {
                Process process = Process.Start("explorer.exe", $@"/open, {userSettings["defaultDirectory"]}");
                Console.WriteLine("Default Directory opened in file explorer.\n");
                process.WaitForExit();
            }
            else
            {
                Console.WriteLine("Error: No default directory set");
            }
        }
    }
}
