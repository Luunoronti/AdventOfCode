
using System;

namespace Year2016;

class Day05
{
    private void Show(char[] password)
    {
        const string hexChars = "0123456789abcdef";
        var display = new char[8];
        for (int j = 0; j < 8; j++)
        {
            display[j] = password[j] != 0 ? password[j] : hexChars[new Random().Next(hexChars.Length)];
        }
        Console.Write("\rDecrypting: {0}", new string(display));
    }
    public string Part1(PartInput Input)
    {
        var doorId = Input.FullString;

        char[] password = new char[8];
        int nextPwdPos = 0;
        for (int i = 0; password.Contains((char)0); i++)
        {
            if (i % 5000 == 0)
            {
                Show(password);
            }

            var hexRepresentation = Convert.ToHexString(System.Security.Cryptography.MD5.HashData(System.Text.Encoding.ASCII.GetBytes(doorId + i.ToString())));
            if (hexRepresentation.StartsWith("00000"))
                password[nextPwdPos++] = hexRepresentation[5];
        }

        Console.WriteLine();
        return new string(password).ToLower();
    }
    public string Part2(PartInput Input)
    {
        var doorId = Input.FullString;

        char[] password = new char[8];
        
        for (int i = 0; password.Contains((char)0); i++)
        {
            if (i % 5000 == 0)
            {
                Show(password);
            }

            var hexRepresentation = Convert.ToHexString(System.Security.Cryptography.MD5.HashData(System.Text.Encoding.ASCII.GetBytes(doorId + i.ToString())));
            if (hexRepresentation.StartsWith("00000"))
            {
                var locationChar = hexRepresentation[5] - '0';
                if (locationChar < 0 || locationChar > 7) continue;
                if(password[locationChar] != 0) continue;
                password[locationChar] = hexRepresentation[6];
            }
        }

        Console.WriteLine();
        return new string(password).ToLower();
    }
}
