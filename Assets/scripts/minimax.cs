using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.UI;

public class minimax : MonoBehaviour
{
    private string[,] board;
    private int[] lastmove;
    public GameManager gameManager;
    string currentPlayer = "O";
    private int movesCounter = 0;

    public Dictionary<int, int[]> Xhistory ;
    public Dictionary<int, int[]> Ohistory ;

    public void setBoard(Button[,] Board)
    {
        board = new string[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = Board[i, j].GetComponentInChildren<Text>().text;
            }
        }

        //PrintBoard();
    }

    public void PrintBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                string cellValue = board[row, col];
                Debug.Log("Cell [" + row + ", " + col + "]: " + cellValue);
            }
        }
    }

    public void setHistory(Dictionary<int, int[]> xH, Dictionary<int, int[]> oH)
    {
        Xhistory = new Dictionary<int, int[]>();
        Ohistory = new Dictionary<int, int[]>();

        foreach (KeyValuePair<int, int[]> entry in xH)
        {
            Xhistory.Add(entry.Key, new int[8]);
            for(int i=0;i<8;i++)
            {
                Xhistory[entry.Key][i] = xH[entry.Key][i];
            }
        }

        foreach (KeyValuePair<int, int[]> entry in oH)
        {
            Ohistory.Add(entry.Key, new int[8]);
            for (int i = 0; i < 8; i++)
            {
                Ohistory[entry.Key][i] = oH[entry.Key][i];
            }
        }
    }
    public int[] FindBestMove()
    {

        int bestScore = int.MinValue;
        int[] bestMove = new int[2];
        lastmove = new int[2] { 0, 0 };
        movesCounter = gameManager.movesCount;
        //Debug.Log("AI move");

        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                if (board[col, row] == " " && checkIfAllowd(col,row))
                {
                    //Debug.Log("move:" + col + "," + row);
                    board[col, row] = gameManager.GetPlayerSide();
                    movesCounter++;

                    if (currentPlayer == "X")
                        updateHistory(col, row, Xhistory);
                    else
                        updateHistory(col, row, Ohistory);

                    int score = Minimax(board, 0, false, int.MinValue, int.MaxValue, col,row);
                    //int score= Random.Range(10, 0);
                    board[col, row] = " ";
                    movesCounter--;
                    try
                    {
                        if (currentPlayer == "X")
                            Xhistory.Remove(row);
                        else
                            Ohistory.Remove(row);
                    }catch
                    {
                        Debug.Log("error in minimaxing");
                    }
                  

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove[0] = row;
                        bestMove[1] = col;
                    }
                }
            }
        }
        board = null;
        Xhistory = null;
        Ohistory = null;
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        return bestMove;
    }

    public int Minimax(string[,] board, int depth, bool isMaximizingPlayer, int alpha, int beta,int i,int j)
    {
        currentPlayer = (currentPlayer == "X") ? "O" : "X";

        if (depth>3) //h()
        {
             try
             {
                 if (gameManager.GetPlayerSide() == "X")
                     return heuristic(i, j, Xhistory) - heuristic(lastmove[0], lastmove[1], Ohistory);
                 else
                     return heuristic(i, j, Ohistory) - heuristic(lastmove[0], lastmove[1], Xhistory);
             }catch
             {
                Debug.Log("error in heuristic");
                 if (currentPlayer == "X")
                     return 0;
                 else
                     return 1000;
             }

            //return Random.Range(0, 10);
        }

        if (IsBoardFull())
            return 0;

        if (isMaximizingPlayer)//max turn
        {
            int bestScore = int.MinValue;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col] == " " && checkIfAllowd(row, col))
                    {
                        board[row, col] = currentPlayer;
                        movesCounter++;

                        if (currentPlayer == "X")
                            updateHistory(row, col, Xhistory);
                        else
                            updateHistory(row, col, Ohistory);

                        int score = Minimax(board, depth + 1, false, alpha, beta,row,col);
                        board[row, col] = " ";
                        lastmove[0] = row;
                        lastmove[1] = col;
                        movesCounter--;

                        try
                        {
                            if (currentPlayer == "X")
                                Xhistory.Remove(col);
                            else
                                Ohistory.Remove(col);
                        }
                        catch
                        {
                            Debug.Log("error in max");
                        }

                        bestScore = Max(bestScore, score);
                        alpha = Max(alpha, bestScore);

                        if (beta <= alpha)
                            break;
                    }
                }
            }

            return bestScore;
        }
        else //min turn
        {
            int bestScore = int.MaxValue;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col] == " " && checkIfAllowd(row, col))
                    {
                        board[row, col] = currentPlayer;
                        movesCounter++;

                        if (currentPlayer == "X")
                            updateHistory(row, col, Xhistory);
                        else
                            updateHistory(row, col, Ohistory);

                        int score = Minimax(board, depth + 1, true, alpha, beta,row,col);
                        board[row, col] = " ";
                        lastmove[0] = row;
                        lastmove[1] = col;
                        movesCounter--;
                        try
                        {
                            if (currentPlayer == "X")
                                Xhistory.Remove(row);
                            else
                                Ohistory.Remove(row);
                        }catch
                        {
                            Debug.Log("error in min");
                        }
                       

                        bestScore = Min(bestScore, score);
                        beta = Min(beta, bestScore);

                        if (beta <= alpha)
                            break;
                    }
                }
            }

            return bestScore;
        }
    }

    public bool IsBoardFull()
    {
        if(movesCounter>=64)
        {
            return true;
        }

        return false;
    }

    public bool checkIfAllowd(int col, int row)
    {
        if (board[row, col] != " ")
            return false;

        if (row == 0 || row == 7)
            return true;
        else
        {
            if (board[row - 1, col] != " " || board[row + 1, col] != " ")
                return true;
        }

        return false;
    }

    public void updateHistory(int col, int row,Dictionary<int,int[]> temp)
    {
        if (!temp.ContainsKey(col))
        {                           //0, 1, 2, 3, 4, 5, 6, 7
            temp.Add(col, new int[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        }
        temp[col][row] = 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <param name="temp"></param>
    /// <returns></returns>
    public int heuristic(int col, int row, Dictionary<int, int[]> temp)
    {
        if (col == -1 || row == -1)
            return 0;

        int winningMovesCount = 0;
        int colmaxCounter = 0, rowMaxCounter = 0, diaMaxCounter1 = 0, diaMaxCounter2 = 0;

        for (int j = 0; j < 4; j++)
        {
            winningMovesCount = 0;
            try
            {
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
            }
            catch
            {
                winningMovesCount = 0; ;
            }
           
            colmaxCounter = Max(colmaxCounter, winningMovesCount);
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
        return rowMaxCounter + colmaxCounter + diaMaxCounter1 + diaMaxCounter2;
    }
}
