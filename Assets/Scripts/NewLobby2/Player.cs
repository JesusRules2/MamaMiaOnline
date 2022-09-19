using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerType
{
    Pikachu,
    Mario
}

public class Player : NetworkBehaviour {
    private GameObject lobbyUI = null;
    public static Player localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;
    [SyncVar] public string displayName;
    public string THIS;

    public NetworkMatchChecker networkMatchChecker;

    [SyncVar] public Match currentMatch;

    GameObject playerLobbyUI;

    public bool bStartLevel = false;

    public override void OnStartAuthority () {
        // CmdSetDisplayName(UILobby.DisplayName);

        // lobbyUI_NameInput.SetActive(true);
    }

    private void Awake () {
        networkMatchChecker = GetComponent<NetworkMatchChecker> ();

    }

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
    }

    public override void OnStartClient () {
        if (isLocalPlayer) 
        {
            localPlayer = this;
        } else 
        {
            Debug.Log ($"Spawning other player UI");
            playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab (this);
        }

        if (!isLocalPlayer)
        {
            Destroy(GetComponent<Rigidbody>());
        }
    }

    public override void OnStopClient () {
        Debug.Log ($"Client stopped");
        ClientDisconnect ();
    }

    public override void OnStopServer () {
        Debug.Log ($"Client stopped on server");
        ServerDisconnect ();
    }

    //TEACHER CUT THIS OUT? ///////////////////////////
    private void Update()
    {
        //if (bStartLevel)
        //{
        //    GameOverManager.instance.ChaseLevelServer();
        //}

        if (!hasAuthority) { return; }

        CmdSetDisplayName(UILobby.DisplayName);
    }

    [Command]
    private void CmdSetDisplayName (string displayName) 
    {
        //GetComponent<PlayerName>().playerNameTag = UILobby.DisplayName;
        GetComponent<PlayerName>().playerNameTag = displayName;
        this.displayName = displayName;

        //RpcSetDisplayName(UILobby.DisplayName);
    }

    //[ClientRpc]
    //private void RpcSetDisplayName(string displayName)
    //{
    //    GetComponent<PlayerName>().playerNameTag = UILobby.DisplayName;
    //    this.displayName = displayName;

    //    //TargetSetDisplayName(UILobby.DisplayName);
    //}
    //[TargetRpc]
    //private void TargetSetDisplayName(string displayName)
    //{
    //    GetComponent<PlayerName>().playerNameTag = UILobby.DisplayName;
    //    this.displayName = displayName;
    //}
    /*
        HOST MATCH
    */

    public void HostGame (bool publicMatch) {
        string matchID = MatchMaker.GetRandomMatchID ();
        CmdHostGame (matchID, publicMatch);
    }

    [Command]
    void CmdHostGame (string _matchID, bool publicMatch) {
        matchID = _matchID;
        if (MatchMaker.instance.HostGame (_matchID, gameObject, publicMatch, out playerIndex)) {
            Debug.Log ($"<color = green>Game hosted successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid ();
            TargetHostGame (true, _matchID, playerIndex);
        } else {
            Debug.Log ($"<color = red>Game hosted failed</color>");
            TargetHostGame (false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetHostGame (bool success, string _matchID, int _playerIndex) {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log ($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.HostSuccess (success, _matchID);
    }

    /*
        JOIN MATCH
    */

    public void JoinGame (string _inputID) {
        //string matchID = MatchMaker.GetRandomMatchID();
        CmdJoinGame (_inputID);
    }

    [Command]
    void CmdJoinGame (string _matchID) {
        matchID = _matchID;
        if (MatchMaker.instance.JoinGame (_matchID, gameObject, out playerIndex)) {
            Debug.Log ($"<color = green>Game joined successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid ();
            TargetJoinGame (true, _matchID, playerIndex);
        } else {
            Debug.Log ($"<color = red>Game joined failed</color>");
            TargetJoinGame (false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetJoinGame (bool success, string _matchID, int _playerIndex) {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log ($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.JoinSuccess (success, _matchID);
    }

    /*
       SEARCH MATCH
    */

    public void SearchGame () {
        CmdSearchGame ();
    }

    [Command]
    void CmdSearchGame () {
        if (MatchMaker.instance.SearchGame (gameObject, out playerIndex, out matchID)) {
            Debug.Log ($"<color=green>Game Found</color>");
            networkMatchChecker.matchId = matchID.ToGuid ();
            TargetSearchGame (true, matchID, playerIndex);
        } else {
            Debug.Log ($"<color=red>Game Not Found</color>");
            TargetSearchGame (false, matchID, playerIndex);
        }
    }

    [TargetRpc]
    public void TargetSearchGame (bool success, string _matchID, int _playerIndex) {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log ($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.SearchSuccess (success, _matchID);
    }

    /*
        BEGIN MATCH
    */

    public void BeginGame () {
        CmdBeginGame ();
        //string matchID = MatchMaker.GetRandomMatchID();
        UILobby.instance.connectObject.SetActive (false);
        UILobby.instance.lobbyCanvas.enabled = false;
        UILobby.instance.gameObject.SetActive (false); //DUNNO
        UILobby.instance.pictures.SetActive(false);

        StartCoroutine(DelayBeginGame());
        //GameOverManager.instance.bBeginGame = false;
        //GameOverManager.instance.ChaseLevelServer();


    }

    IEnumerator DelayBeginGame() //chase level server
    {
        //bStartLevel = true;

        yield return new WaitForSeconds(0.32f); //has to be lower then 0.16f

        //GameOverManager.instance.bHidePanelsBeginning = true;

        //bStartLevel = false;

        //GameOverManager.instance.bBeginGame = false;
        //GameOverManager.instance.ChaseLevelServer();

    }

    [Command]
    void CmdBeginGame () {
        // UILobby.instance.connectObject.SetActive(false);
        Debug.Log ($"<color = red>Game Beginning</color>");
        MatchMaker.instance.BeginGame (matchID);
    }

    //public void StartGame (int playerIndex) 
    public void StartGame(int playerIndex, TurnManager turnManager)
    {
        
            // UILobby.instance.connectObject.SetActive(false);
            // UILobby.instance.lobbyCanvas.enabled = false;
            // UILobby.instance.gameObject.SetActive(false); //DUNNO
            TargetBeginGame (playerIndex, turnManager);

        //IF CHARACTER IS SELECTED ON [SERVER] THEN CALL HERE
    }

    //IEnumerator SwitchCharacter(int playerIndex)
    //{
    //    yield return new WaitForSeconds(0.032f);
    //    if (playerIndex == 1)
    //    {
    //        GetComponent<PlayerSwitch>().SwitchToPikachu();
    //    }
    //    else
    //    {
    //        GetComponent<PlayerSwitch>().SwitchToMario();
    //    }

    //    yield return new WaitForSeconds(0.032f);

    //    GameOverManager.instance.RespawnLevelConnector();
    //}

    [TargetRpc]
    void TargetBeginGame (int playerIndex, TurnManager turnManager) 
    {
        //IF CHARACTER IS SELECTED ON [CLIENT] THEN CALL HERE
        //eg. TurnManager.instance.RespawnPikachu();
        //StartCoroutine(SwitchCharacter(playerIndex));

        UILobby.instance.connectObject.SetActive (false);
        UILobby.instance.lobbyCanvas.enabled = false;
        UILobby.instance.gameObject.SetActive (false); //DUNNO
        UILobby.instance.pictures.SetActive(false);


        Debug.Log ($"MatchID: {matchID} | Beginning");
        //Additively load game scene
        SceneManager.LoadSceneAsync (2, LoadSceneMode.Additive);

        // Set Scene2 as the active Scene
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("CastleScene")); DONT WORK FOR MIRROR

       StartCoroutine(StartGamePikachu(turnManager));

    }

    IEnumerator StartGamePikachu(TurnManager turnManager)
    {
        yield return new WaitForSeconds(0.7f); //0.7
        turnManager.bGameOverRespawn = true;
    }

    /*
        DISCONNECT MATCH
    */

    public void DisconnectGame () {
        CmdDisconnectGame ();
    }

    [Command]
    void CmdDisconnectGame () {
        ServerDisconnect ();
    }

    void ServerDisconnect () {
        MatchMaker.instance.PlayerDisconnected (this, matchID);
        networkMatchChecker.matchId = string.Empty.ToGuid ();
        RpcDisconnectGame ();
    }

    [ClientRpc]
    void RpcDisconnectGame () {
        ClientDisconnect ();
    }

    void ClientDisconnect () {
        if (playerLobbyUI != null) {
            Destroy (playerLobbyUI);
        }
    }

    [Server]
    public void SetDisplayName (string displayName) {
        this.displayName = displayName;
    }


    public PlayerType currentPlayerType;

    [Server]
    public void SetPlayerType(PlayerType _playerType)
    {
        //Called by StartGame at start of round
        //Called by TurnManager when changing PlayerType (Mario <-> Pikachu)
        currentPlayerType = _playerType;
        RpcSetPlayerType(currentPlayerType);
    }

    [ClientRpc]
    void RpcSetPlayerType(PlayerType _playerType)
    {
        currentPlayerType = _playerType;
        //UpdateCharacter();
    }

    void UpdateCharacter()
    {
        if (currentPlayerType == PlayerType.Pikachu)
        {
            //Turn on pikachu stuff
            // GetComponent<PlayerSwitch>().SwitchToPikachu();
        }
        else
        {
            //Turn on mario stuff
            // GetComponent<PlayerSwitch>().SwitchToMario();
        }
    }

    [Client]
    public void Die()
    { //Called from health script
        CmdDie(TurnManager.instance); //Must pass a direct reference because of the potential to have more than one TurnManager on the server at a time with multiple matches.
    }

    [Command]
    void CmdDie(TurnManager turnManager)
    {
        turnManager.PlayerDied(this); //'this' is used for TurnManager to select and set the new PlayerType with SetPlayerType
    }
}