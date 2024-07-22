using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using static System.Math;
using UnityEngine.UI;

public class AIscript : MonoBehaviour
{
    int BoardSize = 8;
    string[,] currBoard;
    private Dictionary<int, int[]> Xhistory; //= new Dictionary<int, int[]>();
    private Dictionary<int, int[]> Ohistory; //= new Dictionary<int, int[]>();
    public GameManager gameController;
    private int[] lastMove = new int[2] { -1, -1 };
    private int[] lastMoveX = new int[2] { -1, -1 };
    private int[] lastMoveO = new int[2] { -1, -1 };

    public int[] Minimax(string[,] board)
    {
       // UnityEngine.Debug.Log("hello");
        currBoard = board;
        Xhistory = gameController.Xhistory;
        Ohistory = gameController.Ohistory;

        int[] bestMove = new int[2] { 0, 0 };
        int bestScore = int.MinValue;
        int alpha = int.MinValue;
        int beta = int.MaxValue;


        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                if(currBoard[i, j] == " ")
                {
                    if (checkIfAllowd(i, j, currBoard))
                    {
                        currBoard[i, j] = "X";
                        //currBoard[i, j].GetComponentInChildren<Text>().text = "X";
                        //currBoard[i, j].interactable = false;
                        updateHistory(i, j, Xhistory);
                        int score = Minimax(currBoard, 1, false, alpha, beta, i, j);
                        currBoard[i, j] = " ";
                        //currBoard[i, j].GetComponentInChildren<Text>().text = "";
                        //currBoard[i, j].interactable = true;
                        Xhistory[i][j] = 0;

                        lastMoveX[0] = i;
                        lastMoveX[1] = j;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            //bestMove = Tuple.Create(i, j);
                            bestMove[0] = i;
                            bestMove[1] = j;
                        }

                        alpha = Max(alpha, bestScore);

                        // Prune the search if beta <= alpha
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
               
            }
        }

        return bestMove;
    }

    private int Minimax(string[,] currBoard, int depth, bool isMaximizingPlayer, int alpha, int beta,int col,int row)
    {
        if ( depth >= 3 )//|| stopwatch.Elapsed.TotalSeconds >= TimeLimitInSeconds)
        {
            return GetScore(col,row,lastMove);
    }

        if (isMaximizingPlayer)
        {
            int bestScore = int.MinValue;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(currBoard[i, j] == " ")
                    {
                        if (checkIfAllowd(i, j, currBoard))
                        {
                            currBoard[i, j] = "X";
                            //currBoard[i, j].GetComponentInChildren<Text>().text = "X";
                            //currBoard[i, j].interactable = false;
                            updateHistory(i, j, Xhistory);
                            int score = Minimax(currBoard, depth + 1, false, alpha, beta, i, j);
                            currBoard[i, j] = " ";
                            //currBoard[i, j].GetComponentInChildren<Text>().text = "";
                            //currBoard[i, j].interactable = true;
                            Xhistory[i][j] = 0;

                            lastMove[0] = i;
                            lastMove[1] = j;

                            bestScore = Max(bestScore, score);

                            alpha= Min(alpha, bestScore);

                            // Prune the search if beta <= alpha
                            if (beta <= alpha)
                            {
                                return bestScore;
                            }
                        }
                    }
                   
                }
            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if(currBoard[i, j] == " ")
                    {
                        if (checkIfAllowd(i, j, currBoard))
                        {
                            currBoard[i, j] = "O";
                            //currBoard[i, j].GetComponentInChildren<Text>().text = "O";
                            //currBoard[i, j].interactable = false;
                            updateHistory(i, j, Ohistory);
                            int score = Minimax(currBoard, depth + 1, true, alpha, beta, i, j);
                            currBoard[i, j] = " ";
                            //currBoard[i, j].GetComponentInChildren<Text>().text = "";
                            //currBoard[i, j].interactable = true;
                            Ohistory[i][j] = 0;

                            lastMove[0] = i;
                            lastMove[1] = j;

                            bestScore = Min(bestScore, score);

                            beta = Min(beta, bestScore);

                            // Prune the search if beta <= alpha
                            if (beta <= alpha)
                            {
                                return bestScore;
                            }
                        }
                    }
                        
                }
            }

            return bestScore;
        }
    }

    public bool checkIfAllowd(int col, int row,string[,] Board)
    {
        if (col == 0 || col == 7)
            return true;
        else
        {
            if (Board[col - 1, row] != " " || Board[col + 1, row] != " ")
                return true;
        }

        return false;
    }
    public bool checkWinningConditions(int col, int row, Dictionary<int, int[]> temp)
    {
        
        //Dictionary<int, int[]> temp=currBoard;
        /*if (gameController.GetPlayerSide() == "X")
        {
            temp = Xhistory;
        }
        else
        {
            temp = Ohistory;
        }*/

        //check the same row
        //if (!gameController.newCol)
        //{
            for (int j = 0; j < 4; j++)
            {
                if (temp[col][j] == 1 &&
                    temp[col][j + 1] == 1 &&
                    temp[col][j + 2] == 1 &&
                    temp[col][j + 3] == 1 &&
                    temp[col][j + 4] == 1)
                {
                    return true;
                }
            }
        //}

        //check the same column
        if (temp.Count >= 5)
        {
            foreach (KeyValuePair<int, int[]> ele in temp)
            {
                try
                {
                    if (temp[ele.Key][row] == 1 &&
                        temp[ele.Key + 1][row] == 1 &&
                        temp[ele.Key + 2][row] == 1 &&
                        temp[ele.Key + 3][row] == 1 &&
                        temp[ele.Key + 4][row] == 1)
                    {
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        //check diagonals
        foreach (KeyValuePair<int, int[]> ele in temp)
        {
            for (int j = 0; j < 4; j++)
            {
                try
                {
                    if (temp[ele.Key][j] == 1 &&
                        temp[ele.Key + 1][j + 1] == 1 &&
                        temp[ele.Key + 2][j + 2] == 1 &&
                        temp[ele.Key + 3][j + 3] == 1 &&
                        temp[ele.Key + 4][j + 4] == 1)
                    {
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        foreach (KeyValuePair<int, int[]> ele in temp)
        {
            for (int j = 4; j < 8; j++)
            {
                try
                {
                    if (temp[ele.Key][j] == 1 &&
                     temp[ele.Key + 1][j - 1] == 1 &&
                     temp[ele.Key + 2][j - 2] == 1 &&
                     temp[ele.Key + 3][j - 3] == 1 &&
                     temp[ele.Key + 4][j - 4] == 1)
                    {
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        return false;
    }
    public int GetScore(int col,int row,int[] lastMove)
    {
        int score = 0;
        score= heuristic(col, row, Xhistory) - heuristic(col, row, Ohistory);
        UnityEngine.Debug.Log(score);
        return score;
    }
    public void updateHistory(int col, int row, Dictionary<int, int[]> temp)
    {
        if (!temp.ContainsKey(col))
        {                           //0, 1, 2, 3, 4, 5, 6, 7
            temp.Add(col, new int[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            //newCol = true;
        }
        temp[col][row] = 1;
    }

    public void trythisfun()
    {
        UnityEngine.Debug.Log("hello from AI script");
    }
    public int heuristic(int col, int row, Dictionary<int, int[]> temp)
    {
        if (col == -1 || row == -1)
            return 0;

        int winningMovesCount = 0;
        int colmaxCounter = 0, rowMaxCounter = 0, diaMaxCounter1 = 0, diaMaxCounter2 = 0;

        try
        {
            for (int j = 0; j < 4; j++)
            {
                winningMovesCount = 0;
                if (temp[col][j] == 1)
                {
                    winningMovesCount++;
                    if (temp[col][j + 1] == 1)
                    {
                        winningMovesCount++;
                        if (temp[col][j + 2] == 1)
                        {
                            winningMovesCount++;
                            if (temp[col][j + 3] == 1)
                            {
                                winningMovesCount++;
                                if (temp[col][j + 4] == 1)
                                    winningMovesCount++;
                            }

                        }

                    }

                }
                colmaxCounter = Max(colmaxCounter, winningMovesCount);
            }
        }catch
        {
        }
        


        winningMovesCount = 0;
        foreach (KeyValuePair<int, int[]> ele in temp)
        {
            try
            {
                if (temp[ele.Key][row] == 1)
                {
                    winningMovesCount++;
                    if (temp[ele.Key + 1][row] == 1)
                    {
                        winningMovesCount++;
                        if (temp[ele.Key + 2][row] == 1)
                        {
                            winningMovesCount++;
                            if (temp[ele.Key + 3][row] == 1)
                            {
                                winningMovesCount++;
                                if (temp[ele.Key + 4][row] == 1)
                                {
                                    winningMovesCount++;
                                }
                            }

                        }

                    }
                    rowMaxCounter = Max(rowMaxCounter, winningMovesCount);
                }

            }
            catch
            {
                rowMaxCounter = Max(rowMaxCounter, winningMovesCount);
                continue;
            }
        }

        //check diagonals
        foreach (KeyValuePair<int, int[]> ele in temp)
        {
            for (int j = 0; j < 4; j++)
            {
                winningMovesCount = 0;
                try
                {
                    if (temp[ele.Key][j] == 1)
                    {
                        winningMovesCount++;
                        if (temp[ele.Key + 1][j + 1] == 1)
                        {
                            winningMovesCount++;
                            if (temp[ele.Key + 2][j + 2] == 1)
                            {
                                winningMovesCount++;
                                if (temp[ele.Key + 3][j + 3] == 1)
                                {
                                    winningMovesCount++;
                                    if (temp[ele.Key + 4][j + 4] == 1)
                                        winningMovesCount++;
                                }

                            }

                        }

                    }
                    diaMaxCounter1 = Max(diaMaxCounter1, winningMovesCount);
                }
                catch
                {
                    diaMaxCounter1 = Max(diaMaxCounter1, winningMovesCount);
                    continue;
                }
            }
        }

        foreach (KeyValuePair<int, int[]> ele in temp)
        {
            for (int j = 4; j < 8; j++)
            {
                winningMovesCount = 0;
                try
                {
                    if (temp[ele.Key][j] == 1)
                    {
                        winningMovesCount++;
                        if (temp[ele.Key + 1][j - 1] == 1)
                        {
                            winningMovesCount++;
                            if (temp[ele.Key + 2][j - 2] == 1)
                            {
                                winningMovesCount++;
                                if (temp[ele.Key + 3][j - 3] == 1)
                                {
                                    winningMovesCount++;
                                    if (temp[ele.Key + 4][j - 4] == 1)
                                        winningMovesCount++;
                                }

                            }

                        }

                    }
                    diaMaxCounter2 = Max(diaMaxCounter2, winningMovesCount);
                }
                catch
                {
                    diaMaxCounter2 = Max(diaMaxCounter2, winningMovesCount);
                    continue;
                }
            }
        }
        UnityEngine.Debug.Log(rowMaxCounter + colmaxCounter + diaMaxCounter1 + diaMaxCounter2);
        return rowMaxCounter + colmaxCounter + diaMaxCounter1 + diaMaxCounter2;
    }

   /* private int GetScore(string[,] currBoard)
    {
        // Check for winning configurations
        if (IsWinningConfiguration(currBoard, "X"))
        {
            return int.MaxValue; // Max score for the maximizing player (X)
        }
        else if (IsWinningConfiguration(currBoard, "O"))
        {
            return int.MinValue; // Min score for the minimizing player (O)
        }
        else if (IsBoardFull(currBoard))
        {
            return 0; // Tie game, score of 0
        }
        else
        {
            // Calculate the score based on the number of aligned bricks
            int score = CalculateAlignedBricks(currBoard, "X") - CalculateAlignedBricks(currBoard, "O");
            return score;
        }
    }

    private bool IsWinningConfiguration(string[,] currBoard, string player)
    {
        // Check for winning configurations in rows, columns, and diagonals
        if (CheckRows(currBoard, player) || CheckColumns(currBoard, player) || CheckDiagonals(currBoard, player))
        {
            return true;
        }
        return false;
    }

    private bool CheckRows(string[,] currBoard, string player)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col <= BoardSize - 5; col++)
            {
                bool isWinning = true;
                for (int k = 0; k < 5; k++)
                {
                    if (currBoard[row,col + k] != player)
                    {
                        isWinning = false;
                        break;
                    }
                }
                if (isWinning)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckColumns(string[,] currBoard, string player)
    {
        for (int col = 0; col < BoardSize; col++)
        {
            for (int row = 0; row <= BoardSize - 5; row++)
            {
                bool isWinning = true;
                for (int k = 0; k < 5; k++)
                {
                    if (currBoard[row + k,col] != player)
                    {
                        isWinning = false;
                        break;
                    }
                }
                if (isWinning)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckDiagonals(string[,] currBoard, string player)
    {
        // Check diagonals starting from top-left to bottom-right
        for (int row = 0; row <= BoardSize - 5; row++)
        {
            for (int col = 0; col <= BoardSize - 5; col++)
            {
                bool isWinning = true;
                for (int k = 0; k < 5; k++)
                {
                    if (currBoard[row + k,col + k] != player)
                    {
                        isWinning = false;
                        break;
                    }
                }
                if (isWinning)
                {
                    return true;
                }
            }
        }

        // Check diagonals starting from top-right to bottom-left
        for (int row = 0; row <= BoardSize - 5; row++)
        {
            for (int col = BoardSize - 1; col >= 4; col--)
            {
                bool isWinning = true;
                for (int k = 0; k < 5; k++)
                {
                    if (currBoard[row + k,col - k] != player)
                    {
                        isWinning = false;
                        break;
                    }
                }
                if (isWinning)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsBoardFull(string[,] currBoard)
    {
        // Check if the board is full (no empty cells)
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (currBoard[row,col] == " ")
                {
                    return false;
                }
            }
        }
        return true;
    }

    private int CalculateAlignedBricks(string[,] currBoard, string player)
    {
        int alignedBricks = 0;

        // Calculate the number of aligned bricks in rows
        alignedBricks += CalculateAlignedBricksInRows(currBoard, player);

        // Calculate the number of aligned bricks in columns
        alignedBricks += CalculateAlignedBricksInColumns(currBoard, player);

        // Calculate the number of aligned bricks in diagonals
        alignedBricks += CalculateAlignedBricksInDiagonals(currBoard, player);

        return alignedBricks;
    }

    private int CalculateAlignedBricksInRows(string[,] currBoard, string player)
    {
        int alignedBricks = 0;

        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col <= BoardSize - 5; col++)
            {
                bool isAligned = true;
                for (int k = 0; k < 5; k++)
                {
                    if (currBoard[row,col + k] != player)
                    {
                        isAligned = false;
                        break;
                    }
                }
                if (isAligned)
                {
                    alignedBricks++;
                }
            }
        }

        return alignedBricks;
    }

    private int CalculateAlignedBricksInColumns(string[,] currBoard,string player)
    {
        int alignedBricks = 0;

        for (int col = 0; col < BoardSize; col++)
        {
            for (int row = 0; row <= BoardSize - 5; row++)
            {
                bool isAligned = true;
                for (int k = 0; k < 5; k++)
                {
                    if (currBoard[row + k,col] != player)
                    {
                        isAligned = false;
                        break;
                    }
                }
                if (isAligned)
                {
                    alignedBricks++;
                }
            }
        }

        return alignedBricks;
    }

    private int CalculateAlignedBricksInDiagonals(string[,] currBoard, string player)
    {
        int alignedBricks = 0;

        // Calculate aligned bricks in diagonals starting from top-left to bottom-right
        for (int row = 0; row <= BoardSize - 5; row++)
        {
            for (int col = 0; col <= BoardSize - 5; col++)
            {
                bool isAligned = true;
                for (int k = 0; k < 5; k++)
                {
                    if (currBoard[row + k,col + k] != player)
                    {
                        isAligned = false;
                        break;
                    }
                }
                if (isAligned)
                {
                    alignedBricks++;
                }
            }
        }

        // Calculate aligned bricks in diagonals starting from top-right to bottom-left
        for (int row = 0; row <= BoardSize - 5; row++)
        {
            for (int col = BoardSize - 1; col >= 4; col--)
            {
                bool isAligned = true;
                for (int k = 0; k < 5; k++)
                {
                    if (currBoard[row + k,col - k] != player)
                    {
                        isAligned = false;
                        break;
                    }
                }
                if (isAligned)
                {
                    alignedBricks++;
                }
            }
        }

        return alignedBricks;
    }
   */
}
