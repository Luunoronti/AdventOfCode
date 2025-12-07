
namespace Year2016;

class Day04
{
    class RoomData
    {
        public string Name;
        public string DecryptedName;
        public int SectorId;
        public string Checksum;
    }
    private static RoomData GetRoomFromInputLine(string line)
    {
        var checksumId = line.IndexOf('[');
        if (checksumId == -1) return null;
        var lineNC = line[..checksumId];
        var secIdindex = lineNC.LastIndexOf('-');
        if (secIdindex == -1) return null;

        if (int.TryParse(lineNC[(secIdindex + 1)..], out int sectorId) == false) return null;

        RoomData d = new()
        {
            Name = lineNC[..secIdindex],
            SectorId = sectorId,
            Checksum = line[(checksumId + 1)..^1]
        };


        return d;
    }
    private static bool IsRealRoom(RoomData room)
    {
        var name = room.Name.Replace("-", "");
        var letterCounts = new Dictionary<char, int>();
        foreach (var c in name)
        {
            if (letterCounts.ContainsKey(c)) letterCounts[c]++;
            else letterCounts[c] = 1;
        }
        var orderedLetters = letterCounts
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key)
            .Take(5)
            .Select(kv => kv.Key);
        var calculatedChecksum = new string([.. orderedLetters]);
        return calculatedChecksum == room.Checksum;
    }
    private static void DecryptRoomName(RoomData room)
    {
        int shift = room.SectorId % 26;
        char ShiftChar(char c)
        {
            if (c == '-') return ' ';
            int baseChar = 'a';
            int offset = c - baseChar;
            offset = (offset + shift) % 26;
            return (char)(baseChar + offset);
        }
        room.DecryptedName = new string(room.Name.Select(ShiftChar).ToArray());
    }

    public string Part1(PartInput Input)
    {
        long result = 0;
        foreach (var line in Input.Lines)
        {
            var room = GetRoomFromInputLine(line);
            if (room != null && IsRealRoom(room))
                result += room.SectorId;
        }

        return result.ToString();
    }
    public string Part2(PartInput Input)
    {
        long result = 0;
        foreach (var line in Input.Lines)
        {
            var room = GetRoomFromInputLine(line);
            if (room != null && IsRealRoom(room))
            {
                DecryptRoomName(room);
                if (room.DecryptedName.ToLower().Contains("northpole"))
                {
                    result = room.SectorId;
                    break;
                }
            }
        }

        return result.ToString();
    }
}
