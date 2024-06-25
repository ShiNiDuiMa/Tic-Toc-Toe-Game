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



    // Start is called before the first frame update
    void Start()
    {
        GameSetup();
    }

    void GameSetup()
    {
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

    }

    public void TicTacToeButton(int WhichNumber)
    {
        xPlayerButton.interactable = false;
        oPlayerButton.interactable = false;
        if (WhichNumber < 0 || WhichNumber >= tictactoeSpaces.Length)
        {
            Debug.LogError("Invalid button index: " + WhichNumber);
            return;
        }

        if (tictactoeSpaces[WhichNumber] != null)
        {
            tictactoeSpaces[WhichNumber].image.sprite = playIcons[whoTurn];
            tictactoeSpaces[WhichNumber].interactable = false;
        }

        markedSpaces[WhichNumber] = whoTurn + 1;
        turnCount++;
        if (turnCount > 4)
        {
            bool isWinner = WinnerCheck();
            if(turnCount == 9 && isWinner == false)
            {
                Cat();
            }
            
        }

        whoTurn = 1 - whoTurn;
        turnIcons[0].SetActive(whoTurn == 0);
        turnIcons[1].SetActive(whoTurn == 1);
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
        for(int i = 0; i < winningLine.Length; i++)
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
        if(whichPlayer == 0)
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
}
