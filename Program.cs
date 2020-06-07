using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Diagnostics;

namespace Koelkasttry
{
    class Program
    {
        private int playerX, playerY, BoxX, BoxY, goalX, goalY, nCols;
        uint destBoard, currBoard;
        private string Playboard;

        Program(string[] board)
        {
            nCols = board[0].Length;
            StringBuilder playboard = new StringBuilder();

            for (int r = 0; r < board.Length; r++)
            {
                for (int c = 0; c < nCols; c++)
                {
                    char ch = board[r][c];

                    playboard.Append(ch != '?' && ch != '!' && ch != '+' ? ch : '.');

                    if (ch == '!')
                    {
                        this.BoxX = c;
                        this.BoxY = r;
                    }

                    if (ch == '?')
                    {
                        this.goalX = c;
                        this.goalY = r;
                    }

                    if (ch == '+')
                    {
                        this.playerX = c;
                        this.playerY = r;
                    }
                }
            }

            destBoard = (uint)goalY + ((uint)goalX << 8);
            currBoard = (uint)BoxY + ((uint)BoxX << 8) + ((uint)playerY << 16) + ((uint)playerX << 24);

            Playboard = playboard.ToString();
        }

        private class Board
        {
            public uint Cur { get; internal set; }
            public string Sol { get; internal set; }
            public int X { get; internal set; }
            public int Y { get; internal set; }

            public Board(uint cur, string sol, int x, int y)
            {
                Cur = cur;
                Sol = sol;
                X = x;
                Y = y;
            }
        }

        //------------------------
        static void Main()
        {
            string readLine = Console.ReadLine();
            string[] splitLine = readLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            int x = 0;
            string temp = "";
            string[] level = new string[int.Parse(splitLine[1])];

            while (x < int.Parse(splitLine[1]))
            {
                temp += Console.ReadLine();
                x++;
            }

            int o = 0;

            for (int i = 0; i < int.Parse(splitLine[1]); i++)
            {
                string m = "";

                for (int c = 0; c < int.Parse(splitLine[0]); c++)
                {
                    m += temp[o];
                    o++;
                }

                level[i] = m;
            }

            string solved = new Program(level).Solve();

            if ("L".CompareTo(splitLine[2]) == 0 && solved != "No solution")
            {
                Console.WriteLine(solved.Length);
            }
            else if ("P".CompareTo(splitLine[2]) == 0 && solved != "No solution")
            {
                Console.WriteLine(solved.Length);
                Console.WriteLine(solved);
            }
            else
            {
                Console.WriteLine(solved);
            }
        }

        //-------
        private string Solve()
        {
            char[,] dirLabels = { { 'N', 'N' }, { 'E', 'E' }, { 'S', 'S' }, { 'W', 'W' } };
            int[,] dirs = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

            ISet<uint> history = new HashSet<uint>();
            LinkedList<Board> open = new LinkedList<Board>();
            List<string> solution = new List<string>();

            open.AddLast(new Board(currBoard, string.Empty, playerX, playerY));

            history.Add(currBoard);


            while (!open.Count.Equals(0))
            {
                Board item = open.First();
                open.RemoveFirst();
                uint cur = item.Cur;
                string sol = item.Sol;
                int x = item.X;
                int y = item.Y;

                for (int i = 0; i < dirs.GetLength(0); i++)
                {
                    uint trial = cur;
                    int dx = dirs[i, 0];
                    int dy = dirs[i, 1];

                    // are we standing next to a box ?
                    if (comparing(trial & 65535, x + dx, y + dy))
                    {
                        // can we push it ?
                        if ((trial = Push(x, y, dx, dy)) != 0)
                        {
                            // or did we already try this one ?
                            if (!history.Contains(trial))
                            {

                                string newSol = sol + dirLabels[i, 1];

                                if (IsSolved(trial & 65535))
                                    return newSol;

                                open.AddLast(new Board(trial, newSol, x + dx, y + dy));
                                history.Add(trial);
                            }
                        }
                        // otherwise try changing position
                    }
                    else if ((trial = Move(x, y, dx, dy, trial)) != 0)
                    {
                        if (!history.Contains(trial))
                        {
                            string newSol = sol + dirLabels[i, 0];
                            open.AddLast(new Board(trial, newSol, x + dx, y + dy));
                            history.Add(trial);
                        }
                    }
                }
            }
            return "No solution";
        }

        private bool comparing(uint trail, int x, int y)
        {
            uint a = trail, b = 0, c = 0;

            b = (a >> 8);

            for (int i = 8; i < 16; i++)
            {
                trail &= (uint)~(1 << i);
            }

            c = trail;
            //   Console.Write(x + " " + y + " " + b + " " + c);
            //  Console.WriteLine();

            if (b == x && c == y)
            {
             
                    

             //   Console.WriteLine(true);
                return (true);
            }


            return (false);
        }

        private uint Move(int x, int y, int dx, int dy, uint trialBoard)
        {
            int newPlayerPos = (y + dy) * nCols + x + dx;

            if (Playboard[newPlayerPos] != '.')
                return 0;

            for (int i = 16; i < 32; i++)
            {
                trialBoard &= (uint)~(1 << i);
            }

            trialBoard += ((uint)(x + dx) << 24) + ((uint)(y + dy) << 18);

            return trialBoard;
        }

        private uint Push(int x, int y, int dx, int dy)
        {
            int newBoxPos = (y + 2 * dy) * nCols + x + 2 * dx;

            if (Playboard[newBoxPos] != '.')
                return 0;

            uint a = ((uint)(x + dx) << 24) + ((uint)(y + dy) << 16) + ((uint)(x + 2 * dx) << 8) + ((uint)(y + 2 * dy));

            return a;
        }

        private bool IsSolved(uint trialBoard)
        {
            if (trialBoard == destBoard)
                return true;

            return false;
        }
    }
}

