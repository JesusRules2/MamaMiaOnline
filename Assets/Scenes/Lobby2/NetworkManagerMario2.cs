using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManagerMario2 : NetworkManager {



    public List<GamePlayers> Players = new List<GamePlayers> (); //List of Mario AND pikachus
    // public List<Player> Players2 = new List<Player> (); //List of Mario AND pikachus

    // public GameObject MarioPrefab;
    // public GameObject PikachuPrefab;
    // private int numPlayers2 = 0;

    // public string PlayerName { get; set; }
    // public GameObject nameInputObject = null;


    ////public struct CreateName : NetworkMessage
    ////{
    ////    public string name;
    ////}

    ////public override void OnStartServer()
    ////{
    ////    base.OnStartServer();
    ////    //NetworkServer.RegisterHandler<CreateName>(OnCreatePlayer);
    ////}

    ////public override void OnClientConnect(NetworkConnection conn)
    ////{
    ////    base.OnClientConnect(conn);

    ////    // tell the server to create a player with this name
    ////    conn.Send(new CreateName { name = PlayerName });
    ////}

    // public override void Start () {
        // base.Start ();
        // Players2 = TurnManager.lobbyPlayers;
        //GameOverManager.instance.PikachusAlive = 0;
        //GameOverManager.instance.PikachusDead = 0;
        // numPlayers2 = 0;
    // }

    // public void SpawnPlayers () {
    //     List<Characters> charactersAvailable = new List<Characters> ();

    //     foreach (Characters character in characters) {
    //         charactersAvailable.Add (character);
    //     }

    //     foreach (Player player in Players2) {
    //         int index = Random.Range (0, charactersAvailable.Count);
    //         Characters selectedCharacter = charactersAvailable[index];
    //         charactersAvailable.RemoveAt (index);

    //         GameObject characterSpawn = Instantiate (selectedCharacter.characterPrefab, GetStartPosition ().position, Quaternion.identity);

    //         StartCoroutine (DisplayNameChange (player, characterSpawn));
    //         NetworkServer.Spawn (characterSpawn, player.connectionToClient);
    //     }

        //GameOverManager.instance.PikachusAlive = 0;
        //GameOverManager.instance.PikachusDead = 0;
        //numPlayers2 = 0;

        // int MarioPlayerStart = UnityEngine.Random.Range (1, Players.Count + 1); //gotta add 1 for max on Ints
        // Debug.Log ("Players: " + Players.Count);
        // Debug.Log ("Mario Player: " + MarioPlayerStart);

        // if (SceneManager.GetActiveScene ().name.StartsWith ("CastleScene")) {
        //     foreach (Player player in Players2) {
        //         numPlayers2++;

        //         if (numPlayers2 == MarioPlayerStart) {
        //             GameObject marioSpawn = Instantiate (MarioPrefab, //on server, put over network
        //                 GetStartPosition ().position, Quaternion.identity);

        //             StartCoroutine (DisplayNameChange (player, marioSpawn));
        //             NetworkServer.Spawn (marioSpawn, player.connectionToClient);
        //         } else {
        //             GameObject pikachuSpawn = Instantiate (PikachuPrefab, //on server, put over network
        //                 GetStartPosition ().position, Quaternion.identity);

        //             StartCoroutine (DisplayNameChange (player, pikachuSpawn));
        //             NetworkServer.Spawn (pikachuSpawn, player.connectionToClient);
        //         }
        //     }
        // }
    // }

    // IEnumerator DisplayNameChange (Player player, GameObject playerSpawn) {
    //     yield return new WaitForSeconds (0.32f);

    //     playerSpawn.GetComponent<PlayerName> ().playerNameTag = player.displayName;

    //     //  Player player2 = NetworkClient.connection.identity.GetComponent<Player>();
    //     //player.playerName = ((NetworkManagerMario2)NetworkManager.singleton).OnCreatePlayer(player.connectionToClient);
    //     //playerSpawn.GetComponent<PlayerName>().playerNameTag = player.playerName;

    // }

    // private void Update () {
    //     if (nameInputObject == null) {
    //         nameInputObject = GameObject.Find ("NameInput");
    //     }
    //     if (nameInputObject != null) {
    //         PlayerName = nameInputObject.GetComponent<InputField> ().text;
    //     }

    // }

    // public string OnCreatePlayer (NetworkConnection connection) {
    //     //connection.identity.GetComponent<Player>().playerName = PlayerName;
    //     return PlayerName;
    // }

    //public override void OnServerAddPlayer(NetworkConnection conn)
    //{
    //    base.OnServerAddPlayer(conn);

    //    Player player = conn.identity.GetComponent<Player>();

    //    //player.SetDisplayName(playerNameSteam);
    //    player.playerName = PlayerName;
    //    //player.transferName = $"Player {Players.Count}";
    //}
}