﻿using System.Text;

namespace AdventOfCode2023
{
    //[Force] // uncomment to force processing of this type
    [AlwaysEnableLog]
    //[DisableLogInDebug]
    [UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day10
    {
        public static string TestFile => "2023\\10\\test.txt";
        public static string LiveFile => "2023\\10\\live.txt";

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


            // make initial move
            GetVelocityAtStartPoint(out var velocityX, out var velocityY);


            var sum = 1L;
            Set(x, y, Get(x, y));

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

            GetVelocityAtStartPoint(out velocityX, out velocityY);

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



            if (Log.Enabled)
            {
                var sb = new StringBuilder();

                var clr = CC.Sys;
                for (int i = 0; i < lines.Length; i++)
                {
                    for (int j = 0; j < lines[0].Length; j++)
                    {
                        var c = GetFE(j, i);

                        var cc = c switch
                        {
                            '?' => CC.Val,
                            '*' => CC.Val,
                            'S' => CC.Att,
                            _ => CC.Frm
                        };

                        bool clrC = cc != clr;
                        clr = cc;

                        if (clrC) sb.Append($"{clr}{c}");
                        else sb.Append($"{c}");
                    }
                    sb.AppendLine($"");
                }
                Log.WriteLine(sb.ToString());
            }


            sum2 = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[0].Length; j++)
                {
                    var c = GetFE(j, i);
                    if (c == '?' || c == '*') sum2++;
                }
            }

            char Translate(char ic)
                => ic switch
                {
                    'L' => '└',
                    'J' => '┘',
                    'F' => '┌',
                    '7' => '┐',
                    '|' => '│',
                    '-' => '─',
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

            void GetVelocityAtStartPoint(out int velocityX, out int velocityY)
            {
                velocityX = 0;
                velocityY = 0;
                if (startX - 1 >= 0)
                {
                    var c = Get(startX - 1, startY);
                    if (c == '-' || c == 'F' || c == 'L') velocityX = -1;
                }
                if (startX + 1 < lines[0].Length)
                {
                    var c = Get(startX + 1, startY);
                    if (c == '-' || c == '7' || c == 'J') velocityX = 1;
                }
                if (startY + 1 < lines.Length)
                {
                    var c = Get(startX, startY + 1);
                    if (c == '|' || c == 'J' || c == 'L') velocityY = 1;
                }
                if (startY - 1 >= 0)
                {
                    var c = Get(startX, startY - 1);
                    if (c == '|' || c == 'F' || c == '7') velocityY = -1;
                }

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
        private static int sum2;
        public static long Part2(string[] lines) => sum2;
    }
}