using System.Text.Json;
using System.Text.RegularExpressions;

namespace shxHistoryRebuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "";
            string dirToWrite = "D";

            // Continue asking until the directory isn't null/invalid
            while (input == "")
            {
                Console.Write("Directory to create History.json file from: ");
                input = Console.ReadLine();
                if (Directory.Exists(input))
                {                    
                    Console.WriteLine("Valid directory input.");
                }
                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", input);
                    input = "";
                }
            }
            Console.Write("Currently writing History.json to {0}:, type another valid drive letter or leave blank to keep current: ", dirToWrite);
            dirToWrite = ChooseDrive();
            if (dirToWrite.Contains(":"))
                dirToWrite = dirToWrite.Replace(":", "");

            List<ScreenGrab> toWrite = DisplayFileInfo(input);
            WriteJson(toWrite, dirToWrite);
            Console.WriteLine("Write complete.");

        }
        static string ChooseDrive()
        {            
            string? input = Console.ReadLine();
            string dir = "";

            if (input == null || input == "")
            {
                return "D";
            }
            dir = input;
            if (!input.Contains(":"))
                dir = input + ":";

            if (Directory.Exists(dir))
                return input;
            else if (dir == "")
                return "D";
            else if (!Directory.Exists(dir))
            {
                Console.WriteLine("Directory not found.");
                Console.Write("Please enter a valid drive or leave blank for D: ");
                ChooseDrive();
            }
            return input;
        }

        static List<ScreenGrab> DisplayFileInfo(string input)
        {
            Console.WriteLine("Scanning " + input + "...");

            // Create a list for the final values
            List<ScreenGrab> toWrite = new List<ScreenGrab>();
            // Create a list with every file
            List<string> filePaths = Directory.GetFiles(input, "*.*", SearchOption.AllDirectories).ToList();
            string fileType = "";

            foreach (var file in filePaths)
            {
                var fileInfo = new FileInfo(file);

                var fileName = fileInfo.Name;
                Console.WriteLine("File name: {0}", fileName);
                var filePath = fileInfo.FullName;
                Console.WriteLine("File path: {0}", filePath);
                var created = fileInfo.CreationTime.ToString("O");
                Console.WriteLine("File created on: {0}", created);
                var fileExt = fileInfo.Extension;
                if (fileExt.Contains("png") || fileExt.Contains("jpg"))
                {
                    fileType = "Image";
                    Console.WriteLine("File type: {0}", fileType);
                }
                else if (fileExt.Contains("mp4") || fileExt.Contains("webm") || fileExt.Contains("gif"))
                {
                    fileType = "File";
                    Console.WriteLine("File type: {0}", fileType);
                }
                else
                {
                    Console.WriteLine("Unrecognized extension: {0}", fileExt.ToString());
                    Console.Write("Paused for error - Press any key to continue.");
                    Console.ReadLine();
                }
                // Default to unknown process name for ease of fixing manually
                string processName = "Unknown";
                // See if there is an _ since that's part of the default naming convention
                if (fileName.Contains("_"))
                {
                    // Figure out if the file is using the new or old default naming scheme by counting
                    // how many "_", as the new method only has a single underscore
                    string source = fileName;
                    int count = source.Length - source.Replace("_", "").Length;

                    // Old naming scheme, has _ after process name and also date
                    if (count > 1)
                    {
                        // Second to last index of...
                        processName = source.Substring(0, source.Substring(0, fileName.LastIndexOf("_")).LastIndexOf("_"));
                        processName = processName.Replace("_", " ");
                    }
                    // New naming scheme, has one _ and 10 random letters after it
                    else if (count == 1)
                    {
                        processName = source.Substring(0, fileName.LastIndexOf("_"));
                        processName = processName.Replace("_", " ");
                    }

                }
                if (!isValid(processName))
                    processName = "Unknown";
                Console.WriteLine("Process name: {0}", processName);
                ScreenGrab f = new ScreenGrab();
                f.FileName = fileName;
                f.FilePath = filePath;
                f.DateTime = created;
                f.Type = fileType;
                f.Host = "";
                Tags t = new Tags();
                t.processName = processName;
                t.windowTitle = processName;
                f.Tags = t;
                toWrite.Add(f);                
            }
            return toWrite;
        }
        static void WriteJson(List<ScreenGrab> toWrite, string dir)
        {
            Console.WriteLine("Writing data to JSON file...");
            string locToWrite = dir + ":\\History.json";
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(toWrite, options);
            File.WriteAllText(locToWrite, json);
        }
        private static bool isValid(String str)
        {
            return Regex.IsMatch(str, @"^[a-zA-Z]+$");
        }
    }
    public class ScreenGrab
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string DateTime { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public Tags Tags { get; set; }
    }

    public class Tags
    {
        public string windowTitle { get; set; }
        public string processName { get; set; }
    }
}