using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Match {
    public string matchID;

    public bool publicMatch;
    public bool inMatch;
    public bool matchFull;

    public SyncListGameObject players = new SyncListGameObject ();
    public Match (string matchID, GameObject player) {
        this.matchID = matchID;
        players.Add (player);
    }

    public Match () { }

}

[System.Serializable]
public struct Characters {
    public GameObject characterPrefab;
}

[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> { }

[System.Serializable]
public class SyncListMatch : SyncList<Match> { }

public class MatchMaker : NetworkBehaviour {

    public static MatchMaker instance;

    public SyncListMatch matches = new SyncListMatch ();
    //public SyncListString matchIDs = new SyncListString();
    public SyncList<string> matchIDs = new SyncList<string> ();

    [SerializeField] int maxMatchPlayers = 8;

    [SerializeField] GameObject turnManagerPrefab;
    [SerializeField] GameObject gameOverManagerPrefab;

    [SerializeField] GameObject PlayerCharacterPrefab;

    GameObject gameOverManager;
    public TurnManager turnManager;

    int MarioPlayerStart = 0;
    int numPlayers2 = 0;

    [SerializeField] List<Characters> characterPrefabs = new List<Characters> ();

    public List<Player> matchPlayers; //NEW

    public string PlayerName { get; set; }
    public InputField nameInputObject;

    private void Start () {
        instance = this;
    }

    public bool HostGame (string _matchID, GameObject _player, bool publicMatch, out int playerIndex) {
        playerIndex = -1;

        if (!matchIDs.Contains (_matchID)) {
            matchIDs.Add (_matchID);
            Match match = new Match (_matchID, _player);
            match.publicMatch = publicMatch;
            matches.Add (match);
            Debug.Log ($"Match ID generated");
            playerIndex = 1;
            return true;
        } else {
            Debug.Log ($"Match ID already exists");
            return false;
        }
    }

    public bool JoinGame (string _matchID, GameObject _player, out int playerIndex) {
        playerIndex = -1;

        if (matchIDs.Contains (_matchID)) {

            for (int i = 0; i < matches.Count; i++) {
                if (matches[i].matchID == _matchID) {
                    if (!matches[i].inMatch && !matches[i].matchFull) {

                    matches[i].players.Add (_player);
                //    _player.GetComponent<Player>().currentMatch = matches[i]; //DUNNO
                    playerIndex = matches[i].players.Count;

                    if (matches[i].players.Count == maxMatchPlayers) {
                                matches[i].matchFull = true;
                    }
                    break;
                    } else {
                        return false;
                    }
                }
            }

            Debug.Log ($"Match joined");
            return true;
        } else {
            Debug.Log ($"Match ID does not exist");
            return false;
        }
    }

    public bool SearchGame (GameObject _player, out int playerIndex, out string matchID) {
        playerIndex = -1;
        matchID = string.Empty;

        for (int i = 0; i < matches.Count; i++) {
            if (matches[i].publicMatch && !matches[i].matchFull && !matches[i].inMatch) {
                 // matchID = matches[i].matchID;
                //if (JoinGame (matchID, _player, out playerIndex)) {
                 if (JoinGame (matches[i].matchID, _player, out playerIndex)) {
                     matchID = matches[i].matchID;
                    return true;
                }
            }
        }

        return false;
    }

    public void BeginGame(string _matchID)
    {
        //if (gameOverManager != null)
        //{
        //    gameOverManager.GetComponent<GameOverManager>().ChaseLevelServer();
        //}
        //else
        //{
        //    //Do spawning stuff
        //    gameOverManager = Instantiate(gameOverManagerPrefab);
        //    NetworkServer.Spawn(gameOverManager);
        //}

        GameObject newTurnManager = Instantiate(turnManagerPrefab);

        NetworkServer.Spawn (newTurnManager);

        //TimerManager.instance.timeValue = 200; //important

        newTurnManager.GetComponent<NetworkMatchChecker> ().matchId = _matchID.ToGuid (); //need to have in TurnManager
        turnManager = newTurnManager.GetComponent<TurnManager> ();

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchID)
            {

                matches[i].inMatch = true;

                //Randomize list of player for starting
                //  List<int> shufflePlayerIndex = new List<int>();
                // for (var h = 0; h < matches[i].players.Count; h++)
                // {
                //      shufflePlayerIndex.Add(h);
                // }

                matchPlayers = new List<Player>();

                Debug.Log ($"BeginGame {_matchID} | {matches[i].players.Count} Players.");

                foreach (var player in matches[i].players) 
                {
                     Player _player = player.GetComponent<Player> ();
                     matchPlayers.Add (_player);
                     turnManager.AddPlayer (_player); /////// Spawning after
                    //_player.StartGame ();
                    //_player.StartGame(shufflePlayerIndex[]UnityEngine.Random.Range(0, shufflePlayerIndex.Count)]);
                    _player.StartGame(0, turnManager);

                }
                Debug.Log ($"Game Started. Spawning {matchPlayers.Count} Players.");
                //StartCoroutine(SpawnPlayers(matchPlayers, _matchID));

                turnManager.SetPlayerCount(matches[i].players.Count); //Jared asked for this

                break;
            }
        }
    }


    IEnumerator SpawnPlayers (List<Player> matchPlayers2222, string _matchID) 
    {
        yield return new WaitForSeconds(3.2f); //3.2

        int MarioPlayerStart = UnityEngine.Random.Range(1, matchPlayers.Count + 1); //gotta add 1 for max on Ints
        numPlayers2 = 0;

        foreach (Player player in matchPlayers)
        {
            numPlayers2++;

            if (numPlayers2 == MarioPlayerStart) //STARTS as Mario!
            {
                Transform StartPosition = NetworkManager.singleton.GetStartPosition();

                GameObject marioSpawn = Instantiate(PlayerCharacterPrefab, //PlayerCharacter
                new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

                NetworkServer.Spawn(marioSpawn, player.connectionToClient);
                //NetworkServer.ReplacePlayerForConnection(player.connectionToClient, marioSpawn, true); //works on server, not client

                if (marioSpawn.TryGetComponent<NetworkMatchChecker>(out NetworkMatchChecker networkMatchChecker))
                {
                    networkMatchChecker.matchId = _matchID.ToGuid();
                }


                turnManager.AddPlayer(marioSpawn.GetComponentInParent<Player>()); /////// Spawning after
               // marioSpawn.GetComponentInParent<Player>().StartGame();
                //marioSpawn.GetComponent<PlayerSwitch>().SwitchToMario(); //STARTS as Mario! NO Command needed (FOR Mario!)


                //matchPlayers.Remove(player);
                
                //marioSpawn.GetComponent<PlayerSwitch>().playerNameTag2 = player.displayName;
            }
            else
            {
                Transform StartPosition = NetworkManager.singleton.GetStartPosition();

                GameObject pikachuSpawn = Instantiate(PlayerCharacterPrefab, ////PlayerCharacter
                new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

                NetworkServer.Spawn(pikachuSpawn, player.connectionToClient);
                //NetworkServer.ReplacePlayerForConnection(player.connectionToClient, pikachuSpawn, true); //works on server, not client

                if (pikachuSpawn.TryGetComponent<NetworkMatchChecker>(out NetworkMatchChecker networkMatchChecker))
                {
                    networkMatchChecker.matchId = _matchID.ToGuid();
                }


                turnManager.AddPlayer(pikachuSpawn.GetComponentInParent<Player>());
                //pikachuSpawn.GetComponentInParent<Player>().StartGame();

                //CmdSwitchToPikachu3(pikachuSpawn);
                // pikachuSpawn.GetComponent<PlayerSwitch>().SwitchToPikachu();


                //matchPlayers.Remove(player);
                
                //pikachuSpawn.GetComponent<PlayerSwitch>().playerNameTag2 = player.displayName;
            }
        }

        yield return new WaitForSeconds(0.32f);

        //turnManager.RespawnConnection();

    }




    IEnumerator DisplayNameChange(Player player, GameObject playerSpawn)
    {
        yield return new WaitForSeconds(0.32f);

        playerSpawn.GetComponent<PlayerName>().playerNameTag = player.displayName;
    }


    public static string GetRandomMatchID () {
        string _id = string.Empty;
        for (int i = 0; i < 5; i++) {
            int random = UnityEngine.Random.Range (0, 36);
            if (random < 26) {
                _id += (char) (random + 65);
            } else {
                _id += (random - 26).ToString ();
            }
        }
        Debug.Log ($"Random Match ID: {_id}");
        return _id;
    }

    public void PlayerDisconnected (Player player, string _matchID) {
        for (int i = 0; i < matches.Count; i++) {
            if (matches[i].matchID == _matchID) {
                int playerIndex = matches[i].players.IndexOf (player.gameObject);
                matches[i].players.RemoveAt (playerIndex);
                Debug.Log ($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");

                if (matches[i].players.Count == 0) {
                    Debug.Log ($"No more players in Match. Terminating {_matchID}");
                    matches.RemoveAt (i);
                    matchIDs.Remove (_matchID);
                }
                break;
            }
        }
    }

}

public static class MatchExtensions {
    public static Guid ToGuid (this string id) {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider ();
        byte[] inputBytes = Encoding.Default.GetBytes (id);
        byte[] hashBytes = provider.ComputeHash (inputBytes);

        return new Guid (hashBytes);
    }
}











//public void TurnChange()
//{
//    TimerManager.instance.timeValue = 200;

//    Player[] PlayersLeft = FindObjectsOfType<Player>();
//    // allPlayers = GameObject.FindGameObjectsWithTag("Player");

//    List<int> shufflePlayerIndex = new List<int>();


//    //if (SceneManager.GetActiveScene().name.StartsWith("CastleScene"))
//    {
//        //foreach (GameObject player in allPlayers)
//        //foreach (Player player in lobbyPlayers) //IDEAL
//        //for (var h = 0; h < PlayersLeft.Length; h++)
//        for (var h = 0; h < 2; h++)
//        {
//            shufflePlayerIndex.Add(h);
//        }

//        foreach (Player player in PlayersLeft)
//        {
//            int randomIndex = UnityEngine.Random.Range(0, shufflePlayerIndex.Count);

//            //_player.StartGame(shufflePlayerIndex[randomIndex]);

//            if (shufflePlayerIndex[randomIndex] == 1)
//            {
//                player.GetComponent<PlayerSwitch>().SwitchToPikachu();
//            }
//            else
//            {
//                player.GetComponent<PlayerSwitch>().SwitchToMario();
//            }

//            shufflePlayerIndex.RemoveAt(randomIndex);
//        }

//    }

//    //GameOverManager is MonoBehaviour?
//    GameOverManager.instance.RespawnLevelConnector();
//}