using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int whoTurn; //0 = x; 1 = o
    public int turnCount; //count the number of turn played
    public GameObject[] turnIcons; //display whose turn is it
    public Sprite[] playIcons; // 0 = x icon and 1 = o icon
    public Button[] tictactoeSpaces; //playable space for our game
    public int[] markedSpaces; //id's which space was marked by which player
    public TextMeshProUGUI winnerText; //Hold the component of the winner text
    public GameObject[] winningLine; //hold all the different lines  
    public GameObject winnerPannel;
    public int xPlayerScore;
    public int oPlayerScore;
    public TextMeshProUGUI xPlayerScoreText;
    public TextMeshProUGUI oPlayerScoreText;
    public Button xPlayerButton;
    public Button oPlayerButton;
    public GameObject catImage;
    public GameObject StartPannel;
    public Button backButton;
    public AudioSource buttonClickAudio;

    private bool aiMode = false;

    void Start()
    {
        beforeStart();
        //GameSetup();
    }

    void GameSetup()
    {
        backButton.gameObject.SetActive(true);
        whoTurn = 0;
        turnCount = 0;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);
        for (int i = 0; i < tictactoeSpaces.Length; i++)
        {
            tictactoeSpaces[i].interactable = true;
            tictactoeSpaces[i].GetComponent<Image>().sprite = null;
        }
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            markedSpaces[i] = -100;
        }
        foreach (GameObject line in winningLine)
        {
            if (line != null)
                line.SetActive(false);
        }
        if (winnerPannel != null)
            winnerPannel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //in aimode and to ai turn
        if (aiMode && whoTurn == 1)
        {
            AITurn();
        }
    }

    public void TicTacToeButton(int WhichNumber)
    {
        if (tictactoeSpaces[WhichNumber].interactable)
        {
            PlaceMark(WhichNumber);

            if (turnCount > 4)
            {
                bool isWinner = WinnerCheck();
                if (turnCount == 9 && isWinner == false)
                {
                    Cat();
                }
            }

            whoTurn = 1 - whoTurn;
            turnIcons[0].SetActive(whoTurn == 0);
            turnIcons[1].SetActive(whoTurn == 1);
        }
    }

    void PlaceMark(int WhichNumber)
    {
        tictactoeSpaces[WhichNumber].image.sprite = playIcons[whoTurn];
        tictactoeSpaces[WhichNumber].interactable = false;
        markedSpaces[WhichNumber] = whoTurn + 1;
        turnCount++;
    }

    bool WinnerCheck()
    {
        int s1 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2];
        int s2 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5];
        int s3 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8];
        int s4 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6];
        int s5 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7];
        int s6 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8];
        int s7 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8];
        int s8 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6];
        var solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };
        for (int i = 0; i < solutions.Length; i++)
        {
            if (solutions[i] == 3 * (whoTurn + 1))
            {
                Debug.Log("Winner found: " + (whoTurn == 0 ? "Player X" : "Player O"));
                WinnerDisplay(i);
                return true;
            }
        }
        return false;
    }

    void WinnerDisplay(int indexIn)
    {
        if (winnerPannel != null)
            winnerPannel.SetActive(true);

        if (winnerText != null)
        {
            if (whoTurn == 0)
            {
                xPlayerScore++;
                xPlayerScoreText.text = xPlayerScore.ToString();
                winnerText.text = "Player X wins";
            }
            else if (whoTurn == 1)
            {
                oPlayerScore++;
                oPlayerScoreText.text = oPlayerScore.ToString();
                winnerText.text = "Player O wins";
            }
        }

        if (indexIn >= 0 && indexIn < winningLine.Length && winningLine[indexIn] != null)
        {
            Debug.Log("Activating winning line: " + indexIn);
            winningLine[indexIn].SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid winning line index: " + indexIn);
        }
    }

    public void Rematch()
    {
        GameSetup();
        for (int i = 0; i < winningLine.Length; i++)
        {
            winningLine[i].SetActive(false);
        }
        winnerPannel.SetActive(false);
        xPlayerButton.interactable = true;
        oPlayerButton.interactable = true;
        catImage.SetActive(false);
    }

    public void Restart()
    {
        Rematch();
        xPlayerScore = 0;
        oPlayerScore = 0;
        xPlayerScoreText.text = "0";
        oPlayerScoreText.text = "0";
    }

    public void SwitchPlayer(int whichPlayer)
    {
        if (whichPlayer == 0)
        {
            whoTurn = 0;
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
        }
        else if (whichPlayer == 1)
        {
            whoTurn = 1;
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
    }

    void Cat()
    {
        winnerPannel.SetActive(true);
        catImage.SetActive(true);
        winnerText.text = "CAT";
    }

    public void beforeStart()
    {
        StartPannel.SetActive(true);
        backButton.gameObject.SetActive(false);
    }

    public void playerVsPlayer()
    {
        Debug.Log("playerVsPlayer button clicked");
        StartPannel.SetActive(false);
        aiMode = false;
        GameSetup();
    }

    public void back()
    {
        Restart();
        StartPannel.SetActive(true);
    }

    public void playWithAi()
    {
        StartPannel.SetActive(false);
        aiMode = true;
        GameSetup();
    }

    void AITurn()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < markedSpaces.Length; i++)
        {
            if (markedSpaces[i] == -100)
            {
                markedSpaces[i] = 2;
                int score = MiniMax(markedSpaces, 0, false);
                markedSpaces[i] = -100;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        if (bestMove != -1)
        {
            PlaceMark(bestMove);
            if (turnCount > 4)
            {
                bool isWinner = WinnerCheck();
                if (turnCount == 9 && isWinner == false)
                {
                    Cat();
                }
            }

            whoTurn = 1 - whoTurn;
            turnIcons[0].SetActive(whoTurn == 0);
            turnIcons[1].SetActive(whoTurn == 1);
        }
    }

    int MiniMax(int[] board, int depth, bool isMaximizing)
    {
        int result = EvaluateBoard(board);
        if (result != 0)
        {
            return result;
        }

        if (IsBoardFull(board))
        {
            return 0;
        }

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == -100)
                {
                    board[i] = 2;
                    int score = MiniMax(board, depth + 1, false);
                    board[i] = -100;
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == -100)
                {
                    board[i] = 1;
                    int score = MiniMax(board, depth + 1, true);
                    board[i] = -100;
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    int EvaluateBoard(int[] board)
    {
        int[,] winningCombinations = new int[,]
        {
            {0, 1, 2},
            {3, 4, 5},
            {6, 7, 8},
            {0, 3, 6},
            {1, 4, 7},
            {2, 5, 8},
            {0, 4, 8},
            {2, 4, 6}
        };

        for (int i = 0; i < winningCombinations.GetLength(0); i++)
        {
            int a = winningCombinations[i, 0];
            int b = winningCombinations[i, 1];
            int c = winningCombinations[i, 2];

            if (board[a] == board[b] && board[b] == board[c])
            {
                if (board[a] == 1)
                {
                    return -10;
                }
                else if (board[a] == 2)
                {
                    return 10;
                }
            }
        }
        return 0;
    }

    bool IsBoardFull(int[] board)
    {
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == -100)
            {
                return false;
            }
        }
        return true;
    }

   public void PlayButtonClick()
    {
        buttonClickAudio.Play();
    }
}
