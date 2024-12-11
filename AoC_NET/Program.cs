
using System.Net;
using System.Text;

Console.WriteLine("Hello");

string t = GetLiveCode(10);

static string GetLiveCode(int day)
{
    var session =
        "53616c7465645f5fdcb29680e75e8d905cf922803ad9fe580c8d383897ef974b44d9a305f1cb50f3a244e0fc5dcf5d6fd3745503f7900cafcdcc1dbc4a081330";// File.ReadAllText($"{RootPath}session.txt");
    var url = $"https://adventofcode.com/2024/day/{day}/input";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
    var wc = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
    wc.Headers.Add(HttpRequestHeader.UserAgent, "https://github.com/Luunoronti/AdventOfCode");
    wc.Headers.Add(HttpRequestHeader.Cookie, $"{nameof(session)}={session}");

    var aaa = wc.Headers.ToString();
    var contents = wc.DownloadString(url);

    Console.WriteLine($"\n{Encoding.UTF8.GetByteCount(contents)} bytes of live data downloaded.");
    return contents;
}

