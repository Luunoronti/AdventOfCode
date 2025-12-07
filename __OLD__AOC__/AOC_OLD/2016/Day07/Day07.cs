
using System.Text;

namespace Year2016;

class Day07
{
    static bool HasAbba(string s)
    {
        for (int i = 0; i <= s.Length - 4; i++)
        {
            char a = s[i];
            char b = s[i + 1];
            char c = s[i + 2];
            char d = s[i + 3];

            if (a == d && b == c && a != b)
                return true;
        }
        return false;
    }
    static void ParseSegments(string line, List<string> supernets, List<string> hypernets)
    {
        bool inBrackets = false;
        var current = new StringBuilder();

        foreach (char c in line)
        {
            if (c == '[')
            {
                if (current.Length > 0)
                {
                    if (inBrackets)
                        hypernets.Add(current.ToString());
                    else
                        supernets.Add(current.ToString());
                    current.Clear();
                }
                inBrackets = true;
            }
            else if (c == ']')
            {
                if (current.Length > 0)
                {
                    if (inBrackets)
                        hypernets.Add(current.ToString());
                    else
                        supernets.Add(current.ToString());
                    current.Clear();
                }
                inBrackets = false;
            }
            else
            {
                current.Append(c);
            }
        }
        if (current.Length > 0)
        {
            if (inBrackets)
                hypernets.Add(current.ToString());
            else
                supernets.Add(current.ToString());
        }
    }

    static bool SupportsTls(string line)
    {
        var supernets = new List<string>();
        var hypernets = new List<string>();

        ParseSegments(line, supernets, hypernets);

        bool hasAbbaOutside = supernets.Any(HasAbba);
        bool hasAbbaInside = hypernets.Any(HasAbba);

        return hasAbbaOutside && !hasAbbaInside;
    }

    static IEnumerable<string> FindAbas(string s)
    {
        for (int i = 0; i <= s.Length - 3; i++)
        {
            if (s[i] == s[i + 2] && s[i] != s[i + 1])
                yield return s.Substring(i, 3);
        }
    }
    static bool SupportsSsl(string line)
    {
        var supernets = new List<string>();
        var hypernets = new List<string>();

        ParseSegments(line, supernets, hypernets);

        var abas = new List<string>();
        foreach (var s in supernets)
            abas.AddRange(FindAbas(s));

        if (abas.Count == 0)
            return false;
        
        var babs = new HashSet<string>();
        foreach (var aba in abas)
        {
            char a = aba[0];
            char b = aba[1];
            babs.Add($"{b}{a}{b}");
        }

        foreach (var h in hypernets)
        {
            foreach (var bab in babs)
            {
                if (h.Contains(bab))
                    return true;
            }
        }
        return false;
    }
    public string Part1(PartInput Input)
    {
        int countTls = Input.Lines.Count(SupportsTls);
        return countTls.ToString();
    }
    public string Part2(PartInput Input)
    {
        int countSsl = Input.Lines.Count(SupportsSsl);
        return countSsl.ToString();
    }
}
