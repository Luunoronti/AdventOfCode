namespace AdventOfCode2023
{
    //[Force] // uncomment to force processing this type
    [AlwaysEnableLog]
    //[DisableLogInDebug]
    [UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day10
    {
        public static string TestFile => "2023\\10\\test.txt";
        public static string LiveFile => "2023\\10\\live.txt";

        [Flags]
        enum StartDirections
        {
            None = 0,
            North = 0x01,
            South = 0x02,
            West = 0x04,
            East = 0x08,
        }



        public static long Part1(string[] lines)
        {
            // look for 'S' character in any of the lines
            int startX = 0;
            int startY = 0;
            for (int sy = 0; sy < lines.Length; sy++)
            {
                for (int sx = 0; sx < lines[0].Length; sx++)
                {
                    if (Get(sx, sy) == 'S')
                    {
                        startX = sx;
                        startY = sy;
                        Log.WriteLine($"Starting position found at {CC.Val}{startX},{startY}{CC.Clr}");
                        break;
                    }
                }
            }
            var x = startX;
            var y = startY;

            var empty = new char[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                empty[i] = new char[lines[0].Length];
                for (var j = 0; j < lines[0].Length; j++)
                    empty[i][j] = ' ';
            }


            StartDirections initialDirection = StartDirections.None;

            // we must find any position that is one step away from S and is valid
            // and we know, we can go +/- x or +/- y, not both
            if (x - 1 >= 0)
            {
                var c = Get(x - 1, y);
                if (c == '-' || c == 'F' || c == 'L')
                    initialDirection |= StartDirections.West;
            }
            if (x + 1 < lines[0].Length)
            {
                var c = Get(x + 1, y);
                if (c == '-' || c == '7' || c == 'J')
                    initialDirection |= StartDirections.East;
            }
            if (y + 1 < lines.Length)
            {
                var c = Get(x, y + 1);
                if (c == '|' || c == 'J' || c == 'L')
                    initialDirection |= StartDirections.South;
            }
            if (y - 1 >= 0)
            {
                var c = Get(x, y - 1);
                if (c == '|' || c == 'F' || c == '7')
                    initialDirection |= StartDirections.North;
            }
            Log.WriteLine($"Possible starting directions are {CC.Val}{initialDirection}{CC.Clr}");

            var sum = 1L;
            Set(x, y, Get(x, y));

            // make initial move
            var velocityX = 0;
            var velocityY = 0;

            if (initialDirection.HasFlag(StartDirections.North)) velocityY = -1;
            else if (initialDirection.HasFlag(StartDirections.South)) velocityY = 1;
            else if (initialDirection.HasFlag(StartDirections.West)) velocityX = -1;
            else if (initialDirection.HasFlag(StartDirections.East)) velocityX = 1;

            x += velocityX;
            y += velocityY;
            do
            {
                var c = Get(x, y);
                Set(x, y, Translate(c));
                sum++;
                var newVel = GetVelocityForNextStep(c, velocityX, velocityY);
                velocityX = newVel.x;
                velocityY = newVel.y;

                x += velocityX;
                y += velocityY;
            } while (Get(x, y) != 'S');


            x = startX;
            y = startY;

            velocityX = 0;
            velocityY = 0;

            if (initialDirection.HasFlag(StartDirections.North)) velocityY = -1;
            else if (initialDirection.HasFlag(StartDirections.South)) velocityY = 1;
            else if (initialDirection.HasFlag(StartDirections.West)) velocityX = -1;
            else if (initialDirection.HasFlag(StartDirections.East)) velocityX = 1;

            do
            {
                Fill(x, y, velocityX, velocityY);

                var c = Get(x, y);
                var newVel = GetVelocityForNextStep(c, velocityX, velocityY);
                velocityX = newVel.x;
                velocityY = newVel.y;

                Fill(x, y, velocityX, velocityY);

                x += velocityX;
                y += velocityY;
            } while (Get(x, y) != 'S');

            // check if any of our characters is on the border. if so, remove them
            for (int i = 0; i < lines.Length; i++)
            {
                var c = GetCharAtBorder();
                if (c == '?')
                {
                    RemoveCharacterFE('?');
                    break;
                }
                if (c == '*')
                {
                    RemoveCharacterFE('*');
                    break;
                }
            }


            var newLines = new List<string>();
            for (int ye = 0; ye < lines.Length; ye++)
            {
                newLines.Add(new string(empty[ye]));
            }
            File.WriteAllLines("..\\..\\..\\2023\\10\\output.txt", newLines);

            char Translate(char ic)
                => ic switch
                {
                    'L' => '└',
                    'J' => '┘',
                    'F' => '┌',
                    '7' => '┐',
                    _ => ic
                };
            char Get(int x, int y) => lines[y][x];
            char GetFE(int x, int y) => empty[y][x];
            void Set(int x, int y, char c) => empty[y][x] = c;
            (int x, int y, bool dirChange) GetVelocityForNextStep(char c, int currVelX, int currVelY)
            {
                switch (c)
                {
                    case '|': return (0, currVelY, false);
                    case '-': return (currVelX, 0, false);

                    case 'J': // ┘
                        {
                            if (currVelY == 1) return (-1, 0, true);
                            if (currVelX == 1) return (0, -1, true);
                        }
                        break;

                    case 'L': //└
                        {
                            if (currVelY == 1) return (1, 0, true);
                            if (currVelX == -1) return (0, -1, true);
                        }
                        break;

                    case 'F': // ┌
                        {
                            if (currVelY == -1) return (1, 0, true);
                            if (currVelX == -1) return (0, 1, true);
                        }
                        break;
                    case '7': //┐
                        {
                            if (currVelY == -1) return (-1, 0, true);
                            if (currVelX == 1) return (0, 1, true);
                        }
                        break;
                        //case 'S': return (0, 0);
                }
                return (currVelX, currVelY, false);
            }

            void RemoveCharacterFE(char ce)
            {
                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[0].Length; x++)
                    {
                        if (GetFE(x, y) == ce)
                            Set(x, y, ' ');
                    }
                }
            }

            char GetCharAtBorder()
            {
                for (int y = 0; y < lines.Length; y++)
                {
                    var c1 = GetFE(0, y);
                    var c2 = GetFE(lines[0].Length - 1, y);
                    if (c1 == '?' || c2 == '?') return '?';
                    if (c1 == '*' || c2 == '*') return '*';
                }
                for (int x = 0; x < lines[0].Length; x++)
                {
                    var c1 = GetFE(x, 0);
                    var c2 = GetFE(x, lines.Length - 1);
                    if (c1 == '?' || c2 == '?') return '?';
                    if (c1 == '*' || c2 == '*') return '*';
                }
                return ' ';
            }

            void Fill(int x, int y, int velocityX, int velocityY)
            {
                if (velocityY == 1) FillRight(x, y, '?');
                if (velocityY == -1) FillLeft(x, y, '?');
                if (velocityX == 1) FillUp(x, y, '?');
                if (velocityX == -1) FillDown(x, y, '?');

                if (velocityY == -1) FillRight(x, y, '*');
                if (velocityY == 1) FillLeft(x, y, '*');
                if (velocityX == -1) FillUp(x, y, '*');
                if (velocityX == 1) FillDown(x, y, '*');
            }
            void FillRight(int x, int y, char c)
            {
                x++;
                for (; x < lines[0].Length; x++)
                {
                    var lc = GetFE(x, y);
                    if (lc != ' ' && lc != c) return;
                    Set(x, y, c);
                }
            }
            void FillLeft(int x, int y, char c)
            {
                x--;
                for (; x >= 0; x--)
                {
                    var lc = GetFE(x, y);
                    if (lc != ' ' && lc != c) return;
                    Set(x, y, c);
                }
            }
            void FillDown(int x, int y, char c)
            {
                y++;
                for (; y < lines.Length; y++)
                {
                    var lc = GetFE(x, y);
                    if (lc != ' ' && lc != c) return;
                    Set(x, y, c);
                }
            }
            void FillUp(int x, int y, char c)
            {
                y--;
                for (; y >= 0; y--)
                {
                    var lc = GetFE(x, y);
                    if (lc != ' ' && lc != c) return;
                    Set(x, y, c);
                }
            }

            return sum / 2;
        }
        public static long Part2(string[] lines)
        {
            // read our output file instead of standard lines, so we have our polygon ready
            lines = File.ReadAllLines("..\\..\\..\\2023\\10\\output.txt");

            int sum = 0;
            foreach(var l in lines)
            {
                foreach(var c in l)
                {
                    if (c == '?' || c == '*') sum++;
                }
            }

            return sum;
        }
    }
}