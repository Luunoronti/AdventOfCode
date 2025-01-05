using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace aocnetagent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) { return; }
            switch (args[0])
            {
                case "download-input":
                    if (args.Length != 4) { return; }
                    if (int.TryParse(args[2], out int day) && int.TryParse(args[1], out int year))
                    {
                        try
                        {
                            DownloadInputFile(year, day, args[3]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine($"Arguments: day: {day}, year: {year}, file: {args[3]}");
                        }
                    }
                    break;
            }
        }
        private static void DownloadInputFile(int year, int day, string outputFile)
        {
            if (File.Exists("./Database/session.txt") == false)
            {
                Console.WriteLine("Create session.txt file in your <root>/Database and copy session cookie from AoC page.");
                return;
            }
            // this session stuff is a test only.
            // get it from file
            

            var session = File.ReadAllText("./Database/session.txt");
            var url = $"https://adventofcode.com/{year}/day/{day}/input";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var wc = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            wc.Headers.Add(HttpRequestHeader.UserAgent, "https://github.com/Luunoronti/AdventOfCode");
            wc.Headers.Add(HttpRequestHeader.Cookie, $"{nameof(session)}={session}");

            var contents = wc.DownloadString(url);

            Console.WriteLine($"\n{Encoding.UTF8.GetByteCount(contents)} bytes of live data downloaded.");

            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            File.WriteAllText(outputFile, contents);
        }
    }


}
