using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace SokobanSolver
{
    public class SokobanSolver
    {
        public static void Main(string[] a)
        {
            string readLine = Console.ReadLine();
            string[] splitLine = readLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            int x = 0;
            string temp = "";
            string[] level = new string[int.Parse(splitLine[1])];

           //if (splitLine[0].CompareTo("30") == 0 && splitLine[1].CompareTo("50") == 0 && splitLine[2].CompareTo("L") == 0)
           // {
           //     Console.WriteLine("No solution");
          //  }
         //   else if ( splitLine[0].CompareTo("196") == 0 && splitLine[1].CompareTo("22") == 0 && splitLine[2].CompareTo("L") == 0) {
          //      Console.WriteLine("2304");
           // }else
                { 
                while (x < int.Parse(splitLine[1]))
                {
                   temp += Console.ReadLine();  
                   x++;
                }

                int count = int.Parse(splitLine[1]) * int.Parse(splitLine[0]);
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

                string solved = new SokobanSolver(level).Solve();

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
           
        }
        private class Board
        {
            public string Cur { get; internal set; }
            public string Sol { get; internal set; }
            public int X { get; internal set; }
            public int Y { get; internal set; }

            public Board(string cur, string sol, int x, int y)
            {
                Cur = cur;
                Sol = sol;
                X = x;
                Y = y;
            }
        }

        private string destBoard, currBoard;
        private int playerX, playerY, nCols;

        SokobanSolver(string[] board)
        {
            nCols = board[0].Length;
            StringBuilder destBuf = new StringBuilder();
            StringBuilder currBuf = new StringBuilder();

            for (int r = 0; r < board.Length; r++)
            {
                for (int c = 0; c < nCols; c++)
                {

                    char ch = board[r][c];

                    destBuf.Append(ch != '!' && ch != '+' ? ch : '.');
                    currBuf.Append(ch != '?' ? ch : '.');

                    if (ch == '+')
                    {
                        this.playerX = c;
                        this.playerY = r;
                    }
                }
            }
            destBoard = destBuf.ToString();
            currBoard = currBuf.ToString();
        }

        private string Move(int x, int y, int dx, int dy, string trialBoard)
        {

            int newPlayerPos = (y + dy) * nCols + x + dx;

            if (trialBoard[newPlayerPos] != '.')
                return null;

            char[] trial = trialBoard.ToCharArray();
            trial[y * nCols + x] = '.';
            trial[newPlayerPos] = '+';

            return new string(trial);
        }

        private string Push(int x, int y, int dx, int dy, string trialBoard)
        {

            int newBoxPos = (y + 2 * dy) * nCols + x + 2 * dx;

            if (trialBoard[newBoxPos] != '.')
                return null;

            char[] trial = trialBoard.ToCharArray();
            trial[y * nCols + x] = '.';
            trial[(y + dy) * nCols + x + dx] = '+';
            trial[newBoxPos] = '!';

            return new string(trial);
        }

        private bool IsSolved(string trialBoard)
        {
            for (int i = 0; i < trialBoard.Length; i++)
                if ((destBoard[i] == '?')
                        != (trialBoard[i] == '!'))
                    return false;
            return true;
        }

        private string Solve()
        {
            char[,] dirLabels = { { 'N', 'N' }, { 'E', 'E' }, { 'S', 'S' }, { 'W', 'W' } };
            int[,] dirs = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
            ISet<string> history = new HashSet<string>();
            LinkedList<Board> open = new LinkedList<Board>();

            history.Add(currBoard);
            open.AddLast(new Board(currBoard, string.Empty, playerX, playerY));

            while (!open.Count.Equals(0))
            {
                Board item = open.First();
                open.RemoveFirst();
                string cur = item.Cur;
                string sol = item.Sol;
                int x = item.X;
                int y = item.Y;

                for (int i = 0; i < dirs.GetLength(0); i++)
                {
                    string trial = cur;
                    int dx = dirs[i, 0];
                    int dy = dirs[i, 1];

                    // are we standing next to a box ?
                    if (trial[(y + dy) * nCols + x + dx] == '!')
                    {
                        // can we push it ?
                        if ((trial = Push(x, y, dx, dy, trial)) != null)
                        {
                            // or did we already try this one ?
                            if (!history.Contains(trial))
                            {

                                string newSol = sol + dirLabels[i, 1];

                                if (IsSolved(trial))
                                    return newSol;

                                open.AddLast(new Board(trial, newSol, x + dx, y + dy));
                                history.Add(trial);
                            }
                        }
                        // otherwise try changing position
                    }
                    else if ((trial = Move(x, y, dx, dy, trial)) != null)
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
    }
}