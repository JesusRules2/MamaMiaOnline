using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    PlayerName[] players;
    int numPlayer = 0;

    [SerializeField] private TMP_Text[] playerNameTextsBoard = new TMP_Text[12];//change??? more players?
    [SerializeField] private TMP_Text[] playerKillTextsBoard = new TMP_Text[12];//change??? more players?

    //bool bShowupScore = false;
    //public GameObject killBoard = null;


    // [Server]
    void Start()
    {
        players = FindObjectsOfType<PlayerName>();
       // killBoard = GameObject.Find("KillScoreBoard");

        foreach (PlayerName player in players)
        {
            //numPlayer++;
            // player.playerNameTag = "Player " + numPlayer.ToString();

            //PlayerInfoDisplay playerInfoDisplay = player.GetComponent<PlayerInfoDisplay>();

            //if (playerInfoDisplay)
            //{
            //    playerInfoDisplay.SetSteamId(steamId.m_SteamID);
            //}
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        players = FindObjectsOfType<PlayerName>();

        foreach (PlayerName player in players)
        {
           // numPlayer++;
           // player.playerNameTag = "Player " + numPlayer.ToString();

            // PlayerInfoDisplay playerInfoDisplay = player.GetComponent<PlayerInfoDisplay>();

            // if (playerInfoDisplay)
            // {
            //     playerInfoDisplay.displayNameText.text = player.playerNameTag;
            // }
        }

        //KillCountBoard();
    }


    //void KillCountBoard()
    //{
    //    for (int i = 0; i < players.Length; i++)
    //    {
    //        playerNameTextsBoard[i].text = players[i].playerNameTag;
    //        playerKillTextsBoard[i].text = players[i].GetComponent<KillCount>().kills.ToString();
    //    }

    //    if (Input.GetKeyDown(KeyCode.H))
    //    {
    //        bShowupScore = !bShowupScore;
    //    }

    //    if (bShowupScore)
    //    {
    //        killBoard.SetActive(true);
    //    }
    //    if (!bShowupScore)
    //    {
    //        killBoard.SetActive(false);
    //    }

    //}

    public void MouseStuck()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
