using System.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static System.Math;
//using Math;

public class GameManager : MonoBehaviour
{
    private int mode = 0;

    public Button buttonPrefab;
    public Canvas canvas;
    Vector3 position;
    public Button[,] Board;
    int vertical, horizontal, colums, rows;

    public Dictionary<int, int[]> Xhistory = new Dictionary<int, int[]>();
    public Dictionary<int, int[]> Ohistory = new Dictionary<int, int[]>();
    public bool newCol = false;

    private string playerSide = "X";
    public int movesCount = 0;

    public GameObject gameOverPanel;
    public Text gameOverText;

    public GameObject restartButton;

    public GameObject[] modeButtons = new GameObject[3];
    private int[] lastMove = new int[2] { -1, -1 };

    public minimax aiscript;
    void Awake()
    {
        gameOverPanel.SetActive(false);
        movesCount = 0;
        restartButton.SetActive(false);
        setModeButtons(true);
       // aiscript.trythisfun();
    }
    // Start is called before the first frame update
    void Start()
    {
        vertical = (int)Camera.main.orthographicSize-1;
        horizontal = vertical; //* (Screen.width / Screen.height);
        colums = horizontal * 2;
        rows = vertical * 2;
        Board = new Button[colums, rows];
        
        for(int i=colums-1; i>=0;i--)
        {
            for(int j=rows-1;j>=0;j--)
            {
                position = new Vector3(i*128 - (horizontal - 500f), j*128 - (vertical - 100f));
                Board[i, j] = CreateButton(buttonPrefab, canvas, position,i,j);
                Board[i,j].GetComponentInParent<ButtonScript>().SetGameControllerRefrence(this);
            }
        }
        SetBoardInteractable(false);
    }

    public static Button CreateButton(Button buttonPrefab, Canvas canvas, Vector3 vector,int i,int j)
    {
        var button = Object.Instantiate(buttonPrefab, vector, Quaternion.identity) as Button;
        var rectTransform = button.GetComponent<RectTransform>();
        rectTransform.SetParent(canvas.transform);
        button.name = i+" "+j;
        button.GetComponentInChildren<Text>().text = " ";

        return button;
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }
    void ChangeSide()
    {
        playerSide = (playerSide == "X") ? "O" : "X";
        if(mode == 3 || (mode == 2 && playerSide == "O"))
        {
            AImove();
        }
    }

    public void EndTurn(int col, int row)
    {
        movesCount++;
        updateHistory(col, row);
        
        if (checkWinningConditions(col, row,Board))
        {
            GameOver(playerSide);
        }
        newCol= false;

        if(movesCount>=(64))
        {
            GameOver("draw");
        }
        lastMove[0] = col;
        lastMove[1] = row;
        ChangeSide();
    }
    public bool checkIfAllowd(int col, int row)
    {
        if (col == 0 || col == 7)
            return true;
        else
        {
            if (Board[col - 1, row].interactable == false || Board[col + 1, row].interactable == false)
                return true;
        }

        return false;
    }

    public void updateHistory(int col,int row)
    {
        Dictionary<int, int[]> temp;
        if(playerSide == "X")
        {
            temp = Xhistory;
        }else
        {
            temp = Ohistory;
        }

        if(!temp.ContainsKey(col))
        {                           //0, 1, 2, 3, 4, 5, 6, 7
            temp.Add(col, new int[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            newCol = true;
        }
        temp[col][row] = 1;
    }
  
    public int heuristic(int col,int row, Dictionary<int, int[]> temp)
    {
        if (col == -1 || row == -1)
            return 0;

        int winningMovesCount = 0;
        int colmaxCounter = 0, rowMaxCounter = 0,diaMaxCounter1=0,diaMaxCounter2=0;
        
          for(int j=0;j<4;j++)
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
        foreach(KeyValuePair<int, int[]> ele in temp)
        {
            for(int j=0;j<4;j++)
            {
                winningMovesCount = 0;
                try
                {
                    if (temp[ele.Key][j] == 1)
                    {
                        winningMovesCount++;
                        if(temp[ele.Key + 1][j + 1] == 1)
                        {
                            winningMovesCount++;
                            if(temp[ele.Key + 2][j + 2] == 1)
                            {
                                winningMovesCount++;
                                if(temp[ele.Key + 3][j + 3] == 1)
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
            for(int j=4;j<8;j++)
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
            return rowMaxCounter+colmaxCounter+diaMaxCounter1+diaMaxCounter2;
    }

    public void GameOver(string winner)
    {
        restartButton.SetActive(true);
        SetBoardInteractable(false);
        
        if (winner == "draw")
        {
            SetGameOverText("It's a draw!");
        }
        else
        {
            SetGameOverText(playerSide + " wins");
        }
    }

    void SetGameOverText(string winner)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = winner;
    }
    void SetBoardInteractable(bool toggle)
    {
        for (int i = 0; i < colums; i++)
        {
           for(int j = 0; j < rows; j++)
            {
                Board[i,j].interactable = toggle;
                if(toggle==true)
                    Board[i, j].GetComponentInChildren<Text>().text = " ";
            }
        }
    }
    public void restartGame()
    {
        playerSide = "X";
        movesCount = 0;
        gameOverPanel.SetActive(false);

        Xhistory.Clear();
        Ohistory.Clear();

        restartButton.SetActive(false);
        setModeButtons(true);
    }
    
    public void setMode(int m)
    {
        mode = m;

        setModeButtons(false);
        startGame();
    }
    public void startGame()
    {
        SetBoardInteractable(true);
        if(mode == 3)
        {
            AImove();
        }
    }
    public void setModeButtons(bool toggle)
    {
        for(int i=0;i<3;i++)
        {
            modeButtons[i].SetActive(toggle);
        }
    }
    public int GetMode()
    {
        return mode;
    }

    public void AImove()
    {
        aiscript.setBoard(Board);
        aiscript.setHistory(Xhistory, Ohistory);
        int[] move = aiscript.FindBestMove();
        //Debug.Log("AI move:"+move[0]+","+move[1]);
        if (move[0] < 0 || move[0] > 7 || move[1] < 0 || move[1] > 7)
            Debug.Log("AI made a wrong move");
        else
        {
            //Debug.Log(move[0] + "," + move[1]);
            Board[move[0], move[1]].GetComponentInChildren<Text>().text = playerSide;
            Board[move[0], move[1]].interactable = false;

            EndTurn(move[0], move[1]);

            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }

   public bool checkWinningConditions(int col, int row,Button[,] currBoard)
    {
        Dictionary<int, int[]> temp;
        if (playerSide == "X")
        {
            temp = Xhistory;
        }
        else
        {
            temp = Ohistory;
        }

        //check the same row
        if (!newCol)
        {
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
        }

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
}

