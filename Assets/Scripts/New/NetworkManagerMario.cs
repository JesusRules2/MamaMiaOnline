using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
// using Steamworks;

public class NetworkManagerMario : NetworkManager
{
    public GameObject MarioPrefab;
    public GameObject PikachuPrefab;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private bool isGameInProgress = false;

  //  public List<GamePlayers> Players { get; } = new List<GamePlayers>(); //List of Mario AND pikachus
    public List<GamePlayers> Players = new List<GamePlayers>(); //List of Mario AND pikachus

    private int numPlayers2 = 0;

    #region Server



    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) { return; }

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        GamePlayers player = conn.identity.GetComponent<GamePlayers>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        //if (Players.Count < 2) { return; }

        isGameInProgress = true;

        Debug.Log("GAME START?");

        ServerChangeScene("CastleScene");
        //SceneManager.LoadScene("CastleScene");

        //StartCoroutine(StartGameName());
    }

    //IEnumerator StartGameName()
    //{
    //    yield return new WaitForSeconds(0.32f);

    //    PlayerName[] players = FindObjectsOfType<PlayerName>();

    //    foreach (PlayerName player in players)
    //    {
    //        PlayerInfoDisplay playerInfoDisplay = player.GetComponent<PlayerInfoDisplay>();

    //        if (playerInfoDisplay)
    //        {
    //            var cSteamId = new CSteamID(steamId.m_SteamID);
    //            string playerNameSteam = SteamFriends.GetFriendPersonaName(cSteamId);
    //        }
    //    }
    //}

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        //UNDO ALL!!!
        //CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(
        //    MainMenu.LobbyId,
        //    numPlayers - 1);

        //PlayerInfoDisplay playerInfoDisplay = conn.identity.GetComponent<PlayerInfoDisplay>();

        //if (playerInfoDisplay)
        //{
        //    playerInfoDisplay.SetSteamId(steamId.m_SteamID);
        //}



        GamePlayers player = conn.identity.GetComponent<GamePlayers>();

        Players.Add(player);

        //var cSteamId = new CSteamID(steamId.m_SteamID);
        //string playerNameSteam = SteamFriends.GetFriendPersonaName(cSteamId);

        //1 (STEAM)
        //player.SetDisplayName(playerNameSteam);
        //player.transferName = playerNameSteam;

        //2 (LOCAL)
       player.SetDisplayName($"Player {Players.Count}");
       player.transferName = $"Player {Players.Count}";

        player.SetPartyOwner(Players.Count == 1);



        //////////////////////////////////////////////////////CRAP
        //NetworkServer.AddPlayerForConnection(conn, playerSpawn);

        //PlayerMovement player = conn.identity.GetComponent<PlayerMovement>(); //this script is on Player (prefab inside NetworkManager)

        // player.tag = "Player" + numPlayers;
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        //NO GameOverHandler instance

        //GameOverManager.instance.PikachusAlive = 0;
       // GameOverManager.instance.PikachusDead = 0;
        numPlayers2 = 0;

        int MarioPlayerStart = UnityEngine.Random.Range(1, Players.Count + 1); //gotta add 1 for max on Ints
        Debug.Log("Players: " + Players.Count);
        Debug.Log("Mario Player: " + MarioPlayerStart);

        if (SceneManager.GetActiveScene().name.StartsWith("CastleScene"))
        {
            foreach (GamePlayers player in Players)
            {
                numPlayers2++;

                if (numPlayers2 == MarioPlayerStart)
                {
                    GameObject marioSpawn = Instantiate(MarioPrefab, //on server, put over network
                    GetStartPosition().position, Quaternion.identity);

                    StartCoroutine(DisplayNameChange(player, marioSpawn));
                    NetworkServer.Spawn(marioSpawn, player.connectionToClient);
                }
                else
                {
                    GameObject pikachuSpawn = Instantiate(PikachuPrefab, //on server, put over network
                    GetStartPosition().position, Quaternion.identity);

                    StartCoroutine(DisplayNameChange(player, pikachuSpawn));
                    NetworkServer.Spawn(pikachuSpawn, player.connectionToClient);
                }
            }
        }
    }

    IEnumerator DisplayNameChange(GamePlayers player, GameObject playerSpawn)
    {
        yield return new WaitForSeconds(0.32f);

        playerSpawn.GetComponent<PlayerName>().playerNameTag = player.transferName;
    }

    #endregion

    #region Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        //if (conn.identity.transform.GetComponent<HealthScriptPikachu>())
        //{
        //    GameOverManager.instance.PikachusDead++;
        //}

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion



    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    base.OnClientConnect(conn);

    //    //GameObject playerSpawn = Instantiate(PikachuPrefab, //on server, put over network
    //    //    conn.identity.transform.position,
    //    //    conn.identity.transform.rotation);

    //    //NetworkServer.Spawn(playerSpawn, conn);

    //    //PlayerMovement player = conn.identity.GetComponent<PlayerMovement>(); //this script is on Player (prefab inside NetworkManager)

    //    //  player.tag = "Player" + numPlayers;

    //    //Debug.Log("Player" + numPlayers + " has joined.");
    //    //Debug.Log(conn.identity.tag);
    //}
}
