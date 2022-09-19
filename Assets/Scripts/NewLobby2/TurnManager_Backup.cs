using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager2 : NetworkBehaviour 
{
    public static TurnManager2 instance;

    [SyncVar]
    public float timeStart = 200;

    [SyncVar]
    public int pikachusLeft; //Jared
    [SyncVar]
    public int MariosLeft;
    Text pikachusLeftText;
    Text mariosLeftText;
    [SyncVar]
    public bool bMarioWins = false;
    [SyncVar]
    public bool bPikachuWins = false;

    // public GameObject MarioPrefab;
    // public GameObject PikachuPrefab;
    public GameObject CursorPrefab;

    public GameObject MarioWinPanel = null;
    public GameObject PikachuWinPanel = null;
    // [SyncVar]
    public GameObject LoadingScreen = null;

    [SyncVar] 
    public bool bGameOverRespawn;
    [SyncVar]
    bool bGameOverTimer;
    [SyncVar]
    bool bMarioCheck;
    [SyncVar]
    bool bMarioTurnOffTest;
    [SyncVar]
    float scopeTimer = 0;

    //////////////////////////////MARIO SPAWN BOOLS ////////////
    [SyncVar]
   bool bPikachusSpawn = false;
   [SyncVar]
    bool bMarioSpawn = false;
    [SyncVar]
    bool bCheckMario = false;
    [SyncVar]
     bool bMarioEquals1 = false;
     [SyncVar]
     bool bMarioEquals1_others = false;

    public Player[] PlayersLeft;
    GameObject[] PlayersLeftGO;

    // [SyncVar]
    HealthScriptPikachu[] allPikachuPlayers;
    // [SyncVar]
    HealthScript[] allMarioPlayers;
    Player[] allPlayers;

    [SyncVar]
    float EndTimer = 0f;
    [SyncVar]
    float LoadTimer = 0f;
    NetworkManager nm;
    [SyncVar]
    float BeginnerTimer = 0f;

    public static List<Player> lobbyPlayers = new List<Player> ();

    //STAR EFFECT VARIABLES
    public GameObject starPrefab;
    //public bool bStopSpawningOLD = false;
    //public float spawnTime = 8.0f;
    //public float spawnDelay = 23.0f;

    [SyncVar]
    public float starSpawnTime = 12.5f;
    NetworkMatchChecker networkMatchChecker;
    NetworkSpawnPositions[] spawnPositions;


    void Awake () {
        networkMatchChecker = GetComponent<NetworkMatchChecker> ();
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            //Destroy(gameObject);
        }
    }

    public override void OnStartClient () {
        ServerSpawnConnection ();
        //RespawnConnection ();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        MarioWinPanel.SetActive (false);
        PikachuWinPanel.SetActive (false);

        bPikachuWins = false;
        pikachusLeftText = GameObject.Find ("PikachusLeftText").GetComponent<Text> ();
        nm = FindObjectOfType<NetworkManager>();
    }

    public override void OnStartServer () {
       // Debug.Log ($"Start Spawning Stars on Server");
        StartCoroutine (SpawnStars ());
    }

    void Start()
    {
        LoadTimer = 0;
        bGameOverTimer = false;
        bGameOverRespawn = false;
        bMarioCheck = false;
        spawnPositions = FindObjectsOfType<NetworkSpawnPositions>();
        bMarioTurnOffTest = false;
    }

    public void AddPlayer (Player _player) {
        lobbyPlayers.Add (_player);
        // NetworkServer.Spawn (_player.gameObject, _player.connectionToClient);
    }

    public void ServerSpawnConnection () {
        StartCoroutine (ServerSpawn ());
    }

    [Client]
    IEnumerator ServerSpawn () {
        PlayersLeft = FindObjectsOfType<Player> ();

        foreach (Player player in PlayersLeft) {
            yield return new WaitForSeconds (0.001f);
            player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();

            yield return new WaitForSeconds (0.010f);
            player.GetComponent<PlayerSwitch> ().SwitchToMario ();

            yield return new WaitForSeconds (0.001f);
            player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();

            yield return new WaitForSeconds (0.001f);
            player.GetComponent<PlayerSwitch> ().SwitchToMario ();

            yield return new WaitForSeconds (0.001f);
            player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();

            yield return new WaitForSeconds (0.001f);
            player.GetComponent<PlayerSwitch> ().SwitchToMario ();

            yield return new WaitForSeconds (0.001f);
            player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();

            pikachusLeftText = GameObject.Find ("PikachusLeftText").GetComponent<Text> ();

        }
    }

    void Update () {


        //PlayersLeft = FindObjectsOfType<Player> ();
         //allPikachuPlayers = FindObjectsOfType<HealthScriptPikachu> ();
        // allMarioPlayers = FindObjectsOfType<HealthScript> ();

          PlayersLeftGO = GameObject.FindGameObjectsWithTag("PlayerAll");



        if (bGameOverRespawn) {
            
            LoadingScreen.SetActive(true);
            LessMarios();   
            TimerManager.instance.timeValue = 200;
            bPikachuWins = false;
            bMarioEquals1_others = false;
        }
        else
        {
            RespawnLevelUpdateAttempt();

            bMarioEquals1 = false;

             scopeTimer = 0;
             bPikachusSpawn = false;
             bMarioSpawn = false;
             bCheckMario = false;

            LoadingScreen.SetActive(false);

            foreach (GameObject player in PlayersLeftGO) 
             {                          
                player.GetComponent<PlayerSwitch> ().bGameStarted = true;
            }

            if (MariosLeft == 0 && bMarioEquals1_others == false)
            {
                GameObject randPlayer1 = PlayersLeftGO[Random.Range(0,PlayersLeftGO.Length)];
                randPlayer1.GetComponent<PlayerSwitch> ().SwitchToMario ();
                bMarioEquals1_others = true;
            }
                
        }


        if (pikachusLeftText == null) {
            pikachusLeftText = GameObject.Find ("PikachusLeftText").GetComponent<Text> ();
        }
        if (pikachusLeftText) {
            pikachusLeftText.text = "Pikachus Left: " + pikachusLeft.ToString ();
        }
         if (mariosLeftText == null) {
            mariosLeftText = GameObject.Find ("MariosLeftText").GetComponent<Text> ();
        }
        if (mariosLeftText) {
            mariosLeftText.text = "Marios Left: " + MariosLeft.ToString ();
        }

        // if (bPikaMarioCountBeginning)
        // {
        //     pikachusLeftText.text = "";
        //     mariosLeftText.text = "";
        // }
        // else
        // {
        //     pikachusLeftText.text = "Pikachus Left: " + pikachusLeft.ToString ();
        //     mariosLeftText.text = "Marios Left: " + MariosLeft.ToString ();
        // }

        if (pikachusLeft <= 0) //PikachusLeft
        {
            bMarioWins = true;
        } else {
            bMarioWins = false;
        }

        if (bMarioWins && bPikachuWins == false) {
            MarioWinPanel.SetActive (true);
        } else {
            MarioWinPanel.SetActive (false);
        }

        if (bPikachuWins) //ONLY happens when timer goes to 0!
        {
            PikachuWinPanel.SetActive (true);
            if (PlayersLeft != null) {
                foreach (Player player in PlayersLeft) {
                    //player.GetComponent<HandleShooting>().bGameOver = true;
                    if (player != null) player.GetComponent<HandleShooting> ().bGameOver = true;
                }
            }
        } else {
            PikachuWinPanel.SetActive (false);
            if (PlayersLeft != null) {
                foreach (Player player in PlayersLeft) {
                    if (player != null) player.GetComponent<HandleShooting> ().bGameOver = false;
                    // player.GetComponent<HandleShooting>().bGameOver = false;
                }
            }
        }

        if (bMarioWins || bPikachuWins) {
            EndTimer += Time.deltaTime;
            //Debug.Log(EndTimer.ToString()); //test
        } else {
            EndTimer = 0;
        }

        if (EndTimer > 6.2f) //change back to 6.2f
        {
            bPikachuWins = false;
            bGameOverRespawn = true; //1!!


            EndTimer = 0f;
        }
    }

    IEnumerator GameOverTimer() 
    {
        yield return new WaitForSeconds(3.2f);
        bGameOverTimer = false;
        bGameOverRespawn = false;

        LoadingScreen.SetActive(false);
        GetComponent<PauseMenuManager>().GameStartVolume();
    }

    public void RespawnConnection () 
    {
        LoadingScreen.SetActive(true);

        StartCoroutine (RespawnLevel ());
        StartCoroutine(Respawn());

    }
    
    IEnumerator Respawn () {

        yield return new WaitForSeconds (1.2f); //1.32

        TimerManager.instance.timeValue = 200;

        bPikachuWins = false;

        //  PlayersLeft = FindObjectsOfType<Player> ();

        // foreach (Player player in PlayersLeft) 
        //     {                
        //         player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
        //         player.GetComponent<CameraController> ().bHideCamera = false;
        //         player.GetComponent<InputHandler> ().bPlayerLobby = false;
        //         player.GetComponent<PlayerSwitch> ().bGameStarted = true;

        //         player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
                
        //     }

        


    }

    // public void RespawnLevelConnector () {
    //     //RespawnLevel();
    //     StartCoroutine (RespawnLevel ());
    // }

    //void RespawnLevel()
    IEnumerator RespawnLevel () {

       yield return new WaitForSeconds (4.2f); //6.2f

        // ChangeMarioToPikachu();

        // RespawnLevelUpdateAttempt();


       yield return new WaitForSeconds (2.2f); //6.2f


        // bMarioSpawn = true;
        // StartCoroutine(MarioSpawnConnector());
        // LoadingScreen.SetActive(false);
        // GetComponent<PauseMenuManager>().GameStartVolume();
        // bMarioCheck = true;
        // MoreMarios();


        yield return new WaitForSeconds(4f);
        // bMarioCheck = false;
        // MoreMarios();

    }
    // [Server]
    // void ChangeMarioToPikachu() {
    //      PlayersLeft = FindObjectsOfType<Player> ();

    //      foreach (Player player in PlayersLeft) 
    //         {                
    //             player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
    //             player.GetComponent<CameraController> ().bHideCamera = false;
    //             player.GetComponent<InputHandler> ().bPlayerLobby = false;
    //             player.GetComponent<PlayerSwitch> ().bGameStarted = true;

    //             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
                
    //         }

    //     Player randPlayer = PlayersLeft[Random.Range(0,PlayersLeft.Length)];

    //     randPlayer.GetComponent<PlayerSwitch> ().SwitchToMario ();

    //     RespawnLevelUpdateAttempt();

    // }

    void RespawnLevelUpdateAttempt() {

        // if (bMarioEquals1 == false)
        {
            pikachusLeft = 0;
            MariosLeft = 0;

            PlayersLeft = FindObjectsOfType<Player> ();
            allPikachuPlayers = FindObjectsOfType<HealthScriptPikachu> ();
            allMarioPlayers = FindObjectsOfType<HealthScript> ();
            // PlayerSwitch[] allPlayers = FindObjectsOfType<PlayerSwitch>();

            // pikachusLeft = allPikachuPlayers.Length; //NOT WORKING -DETECTS ALL
            // MariosLeft = allMarioPlayers.Length;

            foreach (HealthScriptPikachu player in allPikachuPlayers) {
                if (player.isActiveAndEnabled)
                   pikachusLeft++;
            }
            foreach (HealthScript player in allMarioPlayers) {
                if (player.isActiveAndEnabled)
                    MariosLeft++;
            }
            // MariosLeft = lobbyPlayers.Count;
            // pikachusLeft = lobbyPlayers.Count;

        }

    }
    
    void LessMarios() {

        if (bMarioEquals1 == false)
         {
            scopeTimer += Time.deltaTime;

            if (scopeTimer > .2f)
            {
                if (bPikachusSpawn == false)
                {
                    
                    foreach (GameObject player in PlayersLeftGO) 
                    {                
                            player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
                    }

                    if (scopeTimer > .6f)
                    {
                        bPikachusSpawn = true;
                    }

                }

                 if (scopeTimer > 3.2f && bMarioSpawn == false)
                {
                    GameObject randPlayer1 = PlayersLeftGO[Random.Range(0,PlayersLeftGO.Length)];
                    randPlayer1.GetComponent<PlayerSwitch> ().SwitchToMario ();
                    bMarioSpawn = true;
                }

                 if (scopeTimer > 3.7f && bCheckMario == false)
                {
                    //CONSTANT UPDATING
                    pikachusLeft = 0;
                    MariosLeft = 0;


                    foreach (HealthScriptPikachu player in allPikachuPlayers) {
                        if (player.isActiveAndEnabled)
                        pikachusLeft++;
                    }
                    foreach (HealthScript player in allMarioPlayers) {
                        if (player.isActiveAndEnabled)
                            MariosLeft++;
                    }
                     bCheckMario = true;

                }


                 if (scopeTimer > 3.8f) 
                 {
                    if (MariosLeft == 1)
                    {
                        bMarioEquals1 = true;
                        bGameOverRespawn = false; //

                        bPikachusSpawn = false;
                        bMarioSpawn = false;
                        bCheckMario = false;
                        scopeTimer = 0;
                        foreach (GameObject player in PlayersLeftGO) 
                        {                          
                            player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
                            player.GetComponent<CameraController> ().bHideCamera = false;
                            player.GetComponent<InputHandler> ().bPlayerLobby = false;
                        }
                    }

                    if (scopeTimer > 4.2) { //dunno if i need
                        scopeTimer = 0;
                        bPikachusSpawn = false;
                        bMarioSpawn = false;
                        bCheckMario = false;

                     foreach (GameObject player in PlayersLeftGO) 
                    {                          
                        player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
                        player.GetComponent<CameraController> ().bHideCamera = false;
                        player.GetComponent<InputHandler> ().bPlayerLobby = false;
                    }
                }

                }
             }
        }
        else{
            // scopeTimer = 0;
        }
    }

    

    // IEnumerator MarioSpawnConnector() {
    //     yield return new WaitForSeconds (1f); //6.2f
    //     // bMarioSpawn = true;
    //     LoadingScreen.SetActive(false);

    //     yield return new WaitForSeconds (4f); //6.2f
    //     // bMarioSpawn = false;
    // }

    // void MarioSpawn() {
    //     PlayersLeft = FindObjectsOfType<Player> ();
    //     // int randomIndex = UnityEngine.Random.Range (0, PlayersLeft.Length); //0?
    //     // int playerIndex = 0;
    //     // foreach (Player player in Pl ayersLeft) 

    //     foreach (HealthScript player in allMarioPlayers) 
    //         {            
    //             //  if (bMarioSpawn == true)
    //               {    
    //             //_player.StartGame(shufflePlayerIndex[randomIndex]);
    //             player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
    //             player.GetComponent<CameraController> ().bHideCamera = false;
    //             player.GetComponent<InputHandler> ().bPlayerLobby = false;
    //             player.GetComponent<PlayerSwitch> ().bGameStarted = true;

    //             player.GetComponent<PlayerSwitch> ().SwitchToMario (); //SWITCHED?
    //                              RespawnLevelUpdateAttempt();
    //              }
                
    //         }
    //         foreach (HealthScriptPikachu player in allPikachuPlayers) 
    //         {               
    //             //  if (bMarioSpawn == true)
    //               { 
    //             //_player.StartGame(shufflePlayerIndex[randomIndex]);
    //             player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
    //             player.GetComponent<CameraController> ().bHideCamera = false;
    //             player.GetComponent<InputHandler> ().bPlayerLobby = false;
    //             player.GetComponent<PlayerSwitch> ().bGameStarted = true;

    //             player.GetComponent<PlayerSwitch> ().SwitchToMario (); //SWITCHED?
    //                              RespawnLevelUpdateAttempt();
    //              }
                
    //         }
    // }



    //JARED CODE////////////////////////////
    [Server] //Called from MatchMaker
    public void SetPlayerCount (int playerCount) {
        //pikachusLeft = playerCount - 1;
    }

    [Server]
    public void PlayerDied (Player player) {
        //Was pikachu, become mario
        pikachusLeft--;
        player.SetPlayerType (PlayerType.Mario);
        if (pikachusLeft <= 0) {
            //gameOver
        }
    }

    IEnumerator SpawnStars () 
    {
        float time = starSpawnTime;
        while (true) 
        {
            if (time > 0) 
            {
                time -= Time.deltaTime;
              //  Debug.Log ($"Reducing time: {time}");
            } 
            else 
            {
                time = starSpawnTime;
                SpawnStar ();
            }
           // Debug.Log($"Time Scale: {Time.timeScale}");
            yield return null;
        }
    }

    void SpawnStar () {
        //Vector3 spawnPos = NetworkManager.singleton.GetStartPosition ().position + Vector3.up;
        Vector3 spawnPos = GetRandomStarPosition() + Vector3.up;

        GameObject starObject = Instantiate (starPrefab, spawnPos, Quaternion.identity);
        starObject.GetComponent<NetworkMatchChecker> ().matchId = networkMatchChecker.matchId;
        //Debug.Log ($"Spawning Star: {networkMatchChecker.matchId}");

        NetworkServer.Spawn (starObject);
    }

    Vector3 GetRandomStarPosition()
    {
        return spawnPositions[Random.Range(0, spawnPositions.Length)].transform.position;
    }
}












// using System.Collections;
// using System.Collections.Generic;
// using Mirror;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class TurnManager : NetworkBehaviour 
// {
//     public static TurnManager instance;

//     public float timeStart = 200;

//     [SyncVar]
//     public int pikachusLeft; //Jared
//     [SyncVar]
//     public int MariosLeft;
//     Text pikachusLeftText;
//     Text mariosLeftText;
//     [SyncVar]
//     public bool bMarioWins = false;
//     [SyncVar]
//     public bool bPikachuWins = false;

//     // public GameObject MarioPrefab;
//     // public GameObject PikachuPrefab;
//     public GameObject CursorPrefab;

//     public GameObject MarioWinPanel = null;
//     public GameObject PikachuWinPanel = null;
//     // [SyncVar]
//     public GameObject LoadingScreen = null;

//     [SyncVar] 
//     public bool bGameOverRespawn;
//     [SyncVar]
//     bool bGameOverTimer;
//     [SyncVar]
//     bool bMarioCheck;
//     [SyncVar]
//     bool bMarioTurnOffTest;
//     [SyncVar]
//     float scopeTimer = 0;

//     //////////////////////////////MARIO SPAWN BOOLS ////////////
//     [SyncVar]
//    bool bPikachusSpawn = false;
//    [SyncVar]
//     bool bMarioSpawn = false;
//     [SyncVar]
//     bool bCheckMario = false;
//     [SyncVar]
//      bool bMarioEquals1 = false;

//     public Player[] PlayersLeft;
//     GameObject[] PlayersLeftGO;

//     // [SyncVar]
//     HealthScriptPikachu[] allPikachuPlayers;
//     // [SyncVar]
//     HealthScript[] allMarioPlayers;
//     Player[] allPlayers;

//     [SyncVar]
//     float EndTimer = 0f;
//     [SyncVar]
//     float LoadTimer = 0f;
//     NetworkManager nm;
//     [SyncVar]
//     float BeginnerTimer = 0f;

//     public static List<Player> lobbyPlayers = new List<Player> ();

//     //STAR EFFECT VARIABLES
//     public GameObject starPrefab;
//     //public bool bStopSpawningOLD = false;
//     //public float spawnTime = 8.0f;
//     //public float spawnDelay = 23.0f;

//     [SyncVar]
//     public float starSpawnTime = 12.5f;
//     NetworkMatchChecker networkMatchChecker;
//     NetworkSpawnPositions[] spawnPositions;


//     void Awake () {
//         networkMatchChecker = GetComponent<NetworkMatchChecker> ();
//         if (instance == null) {
//             instance = this;
//         } else if (instance != this) {
//             //Destroy(gameObject);
//         }
//     }

//     public override void OnStartClient () {
//         ServerSpawnConnection ();
//         //RespawnConnection ();

//         Cursor.visible = true;
//         Cursor.lockState = CursorLockMode.None;

//         MarioWinPanel.SetActive (false);
//         PikachuWinPanel.SetActive (false);

//         bPikachuWins = false;
//         pikachusLeftText = GameObject.Find ("PikachusLeftText").GetComponent<Text> ();
//         nm = FindObjectOfType<NetworkManager>();
//     }

//     public override void OnStartServer () {
//        // Debug.Log ($"Start Spawning Stars on Server");
//         StartCoroutine (SpawnStars ());
//     }

//     void Start()
//     {
//         LoadTimer = 0;
//         bGameOverTimer = false;
//         bGameOverRespawn = false;
//         bMarioCheck = false;
//         spawnPositions = FindObjectsOfType<NetworkSpawnPositions>();
//         bMarioTurnOffTest = false;
//     }

//     public void AddPlayer (Player _player) {
//         lobbyPlayers.Add (_player);
//         // NetworkServer.Spawn (_player.gameObject, _player.connectionToClient);
//     }

//     public void ServerSpawnConnection () {
//         StartCoroutine (ServerSpawn ());
//     }

//     [Client]
//     IEnumerator ServerSpawn () {
//         PlayersLeft = FindObjectsOfType<Player> ();

//         foreach (Player player in PlayersLeft) {
//             yield return new WaitForSeconds (0.001f);
//             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();

//             yield return new WaitForSeconds (0.010f);
//             player.GetComponent<PlayerSwitch> ().SwitchToMario ();

//             yield return new WaitForSeconds (0.001f);
//             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();

//             yield return new WaitForSeconds (0.001f);
//             player.GetComponent<PlayerSwitch> ().SwitchToMario ();

//             yield return new WaitForSeconds (0.001f);
//             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();

//             yield return new WaitForSeconds (0.001f);
//             player.GetComponent<PlayerSwitch> ().SwitchToMario ();

//             yield return new WaitForSeconds (0.001f);
//             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();

//             pikachusLeftText = GameObject.Find ("PikachusLeftText").GetComponent<Text> ();

//         }
//     }

//     void Update () {
//         PlayersLeft = FindObjectsOfType<Player> ();
//         allPikachuPlayers = FindObjectsOfType<HealthScriptPikachu> ();
//          allMarioPlayers = FindObjectsOfType<HealthScript> ();
//          PlayersLeftGO = GameObject.FindGameObjectsWithTag("PlayerAll");

//         RespawnLevelUpdateAttempt();
         
//         if (pikachusLeft <= 0) //PikachusLeft
//         {
//             bMarioWins = true;
//         } else {
//             bMarioWins = false;
//         }
        
//         if (EndTimer > 6.2f) //change back to 6.2f
//         {
//             bGameOverRespawn = true; //1!!
//             bPikachuWins = false;
//             bMarioWins = false;

//             EndTimer = 0f;
//         }
        
//         if (bMarioWins || bPikachuWins) {
//             EndTimer += Time.deltaTime;

//         } else {
//             // EndTimer = 0;
//         }


//         if (bGameOverRespawn) {
            
//             LoadingScreen.SetActive(true);
//             LessMarios();   
//             TimerManager.instance.timeValue = 200;
//             bPikachuWins = false;
//         }
//         else
//         {
//             RespawnLevelUpdateAttempt();
//             bMarioEquals1 = false;
//             scopeTimer = 0;
//              bPikachusSpawn = false;
//              bMarioSpawn = false;
//              bCheckMario = false;

//             LoadingScreen.SetActive(false);

//             foreach (GameObject player in PlayersLeftGO) 
//              {                          
//                 player.GetComponent<PlayerSwitch> ().bGameStarted = true;
//             }
//         }



//         if (pikachusLeftText == null) {
//             pikachusLeftText = GameObject.Find ("PikachusLeftText").GetComponent<Text> ();
//         }
//         if (pikachusLeftText) {
//             pikachusLeftText.text = "Pikachus Left: " + pikachusLeft.ToString ();
//         }
//          if (mariosLeftText == null) {
//             mariosLeftText = GameObject.Find ("MariosLeftText").GetComponent<Text> ();
//         }
//         if (mariosLeftText) {
//             mariosLeftText.text = "Marios Left: " + MariosLeft.ToString ();
//         }

//         // if (bPikaMarioCountBeginning)
//         // {
//         //     pikachusLeftText.text = "";
//         //     mariosLeftText.text = "";
//         // }
//         // else
//         // {
//         //     pikachusLeftText.text = "Pikachus Left: " + pikachusLeft.ToString ();
//         //     mariosLeftText.text = "Marios Left: " + MariosLeft.ToString ();
//         // }


//         if (bMarioWins && bPikachuWins == false) {
//             MarioWinPanel.SetActive (true);
//         } else {
//             MarioWinPanel.SetActive (false);
//         }

//         if (bPikachuWins) //ONLY happens when timer goes to 0!
//         {
//             PikachuWinPanel.SetActive (true);
//             if (PlayersLeft != null) {
//                 foreach (Player player in PlayersLeft) {
//                     //player.GetComponent<HandleShooting>().bGameOver = true;
//                     if (player != null) player.GetComponent<HandleShooting> ().bGameOver = true;
//                 }
//             }
//         } else {
//             PikachuWinPanel.SetActive (false);
//             if (PlayersLeft != null) {
//                 foreach (Player player in PlayersLeft) {
//                     if (player != null) player.GetComponent<HandleShooting> ().bGameOver = false;
//                     // player.GetComponent<HandleShooting>().bGameOver = false;
//                 }
//             }
//         }
//     }

//     IEnumerator GameOverTimer() 
//     {
//         yield return new WaitForSeconds(3.2f);
//         bGameOverTimer = false;
//         bGameOverRespawn = false;

//         LoadingScreen.SetActive(false);
//         GetComponent<PauseMenuManager>().GameStartVolume();
//     }

//     public void RespawnConnection () 
//     {
//         LoadingScreen.SetActive(true);

//         StartCoroutine (RespawnLevel ());
//         StartCoroutine(Respawn());

//     }
    
//     IEnumerator Respawn () {

//         yield return new WaitForSeconds (1.2f); //1.32

//         TimerManager.instance.timeValue = 200;

//         bPikachuWins = false;

//         //  PlayersLeft = FindObjectsOfType<Player> ();

//         // foreach (Player player in PlayersLeft) 
//         //     {                
//         //         player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
//         //         player.GetComponent<CameraController> ().bHideCamera = false;
//         //         player.GetComponent<InputHandler> ().bPlayerLobby = false;
//         //         player.GetComponent<PlayerSwitch> ().bGameStarted = true;

//         //         player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
                
//         //     }

        


//     }

//     // public void RespawnLevelConnector () {
//     //     //RespawnLevel();
//     //     StartCoroutine (RespawnLevel ());
//     // }

//     //void RespawnLevel()
//     IEnumerator RespawnLevel () {

//        yield return new WaitForSeconds (4.2f); //6.2f

//         // ChangeMarioToPikachu();

//         // RespawnLevelUpdateAttempt();


//        yield return new WaitForSeconds (2.2f); //6.2f


//         // bMarioSpawn = true;
//         // StartCoroutine(MarioSpawnConnector());
//         // LoadingScreen.SetActive(false);
//         // GetComponent<PauseMenuManager>().GameStartVolume();
//         // bMarioCheck = true;
//         // MoreMarios();


//         yield return new WaitForSeconds(4f);
//         // bMarioCheck = false;
//         // MoreMarios();

//     }
//     // [Server]
//     // void ChangeMarioToPikachu() {
//     //      PlayersLeft = FindObjectsOfType<Player> ();

//     //      foreach (Player player in PlayersLeft) 
//     //         {                
//     //             player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
//     //             player.GetComponent<CameraController> ().bHideCamera = false;
//     //             player.GetComponent<InputHandler> ().bPlayerLobby = false;
//     //             player.GetComponent<PlayerSwitch> ().bGameStarted = true;

//     //             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
                
//     //         }

//     //     Player randPlayer = PlayersLeft[Random.Range(0,PlayersLeft.Length)];

//     //     randPlayer.GetComponent<PlayerSwitch> ().SwitchToMario ();

//     //     RespawnLevelUpdateAttempt();

//     // }

//     void RespawnLevelUpdateAttempt() {

//         // if (bMarioEquals1 == false)
//         {
//             pikachusLeft = 0;
//             MariosLeft = 0;

//             PlayersLeft = FindObjectsOfType<Player> ();
//             allPikachuPlayers = FindObjectsOfType<HealthScriptPikachu> ();
//             allMarioPlayers = FindObjectsOfType<HealthScript> ();
//             // PlayerSwitch[] allPlayers = FindObjectsOfType<PlayerSwitch>();

//             // pikachusLeft = allPikachuPlayers.Length; //NOT WORKING -DETECTS ALL
//             // MariosLeft = allMarioPlayers.Length;

//             foreach (HealthScriptPikachu player in allPikachuPlayers) {
//                 if (player.isActiveAndEnabled)
//                    pikachusLeft++;
//             }
//             foreach (HealthScript player in allMarioPlayers) {
//                 if (player.isActiveAndEnabled)
//                     MariosLeft++;
//             }
//             // MariosLeft = lobbyPlayers.Count;
//             // pikachusLeft = lobbyPlayers.Count;

//         }

//     }
    
//     void LessMarios() {

//         if (bMarioEquals1 == false)
//          {
//             scopeTimer += Time.deltaTime;

//             if (scopeTimer > .2f)
//             {
//                 if (bPikachusSpawn == false)
//                 {
                    
//                     foreach (GameObject player in PlayersLeftGO) 
//                     {                
//                             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
//                     }
//                     foreach (Player player in PlayersLeft) 
//                     {                
//                             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
//                     }
//                     foreach (HealthScript player in allMarioPlayers) 
//                     {                
//                             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
//                     }
//                     foreach (HealthScriptPikachu player in allPikachuPlayers) 
//                     {                
//                             player.GetComponent<PlayerSwitch> ().SwitchToPikachu ();
//                     }

         

//                     if (scopeTimer > .6f)
//                     {
//                         bPikachusSpawn = true;
//                     }

//                 }

//                  if (scopeTimer > 3.2f && bMarioSpawn == false)
//                 {
//                     GameObject randPlayer1 = PlayersLeftGO[Random.Range(0,PlayersLeftGO.Length)];
//                     randPlayer1.GetComponent<PlayerSwitch> ().SwitchToMario ();
//                     bMarioSpawn = true;
//                 }

//                  if (scopeTimer > 3.7f && bCheckMario == false)
//                 {
//                     //CONSTANT UPDATING
//                     pikachusLeft = 0;
//                     MariosLeft = 0;


//                     foreach (HealthScriptPikachu player in allPikachuPlayers) {
//                         if (player.isActiveAndEnabled)
//                         pikachusLeft++;
//                     }
//                     foreach (HealthScript player in allMarioPlayers) {
//                         if (player.isActiveAndEnabled)
//                             MariosLeft++;
//                     }
//                      bCheckMario = true;

//                 }


//                  if (scopeTimer > 3.8f) 
//                  {
//                     if (MariosLeft == 1)
//                     {
//                         bMarioEquals1 = true;
//                         bGameOverRespawn = false; //

//                         bPikachusSpawn = false;
//                         bMarioSpawn = false;
//                         bCheckMario = false;
//                         scopeTimer = 0;
//                         foreach (GameObject player in PlayersLeftGO) 
//                         {                          
//                             player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
//                             player.GetComponent<CameraController> ().bHideCamera = false;
//                             player.GetComponent<InputHandler> ().bPlayerLobby = false;
//                         }
//                     }

//                     if (scopeTimer > 4.2) { //dunno if i need
//                         scopeTimer = 0;
//                         bPikachusSpawn = false;
//                         bMarioSpawn = false;
//                         bCheckMario = false;

//                      foreach (GameObject player in PlayersLeftGO) 
//                     {                          
//                         player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
//                         player.GetComponent<CameraController> ().bHideCamera = false;
//                         player.GetComponent<InputHandler> ().bPlayerLobby = false;
//                     }
//                 }

//                 }
//              }
//         }
//         else{
//             // scopeTimer = 0;
//         }
//     }

    

//     // IEnumerator MarioSpawnConnector() {
//     //     yield return new WaitForSeconds (1f); //6.2f
//     //     // bMarioSpawn = true;
//     //     LoadingScreen.SetActive(false);

//     //     yield return new WaitForSeconds (4f); //6.2f
//     //     // bMarioSpawn = false;
//     // }

//     // void MarioSpawn() {
//     //     PlayersLeft = FindObjectsOfType<Player> ();
//     //     // int randomIndex = UnityEngine.Random.Range (0, PlayersLeft.Length); //0?
//     //     // int playerIndex = 0;
//     //     // foreach (Player player in Pl ayersLeft) 

//     //     foreach (HealthScript player in allMarioPlayers) 
//     //         {            
//     //             //  if (bMarioSpawn == true)
//     //               {    
//     //             //_player.StartGame(shufflePlayerIndex[randomIndex]);
//     //             player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
//     //             player.GetComponent<CameraController> ().bHideCamera = false;
//     //             player.GetComponent<InputHandler> ().bPlayerLobby = false;
//     //             player.GetComponent<PlayerSwitch> ().bGameStarted = true;

//     //             player.GetComponent<PlayerSwitch> ().SwitchToMario (); //SWITCHED?
//     //                              RespawnLevelUpdateAttempt();
//     //              }
                
//     //         }
//     //         foreach (HealthScriptPikachu player in allPikachuPlayers) 
//     //         {               
//     //             //  if (bMarioSpawn == true)
//     //               { 
//     //             //_player.StartGame(shufflePlayerIndex[randomIndex]);
//     //             player.GetComponent<CameraController> ().lobbyCam.SetActive (false);
//     //             player.GetComponent<CameraController> ().bHideCamera = false;
//     //             player.GetComponent<InputHandler> ().bPlayerLobby = false;
//     //             player.GetComponent<PlayerSwitch> ().bGameStarted = true;

//     //             player.GetComponent<PlayerSwitch> ().SwitchToMario (); //SWITCHED?
//     //                              RespawnLevelUpdateAttempt();
//     //              }
                
//     //         }
//     // }



//     //JARED CODE////////////////////////////
//     [Server] //Called from MatchMaker
//     public void SetPlayerCount (int playerCount) {
//         //pikachusLeft = playerCount - 1;
//     }

//     [Server]
//     public void PlayerDied (Player player) {
//         //Was pikachu, become mario
//         pikachusLeft--;
//         player.SetPlayerType (PlayerType.Mario);
//         if (pikachusLeft <= 0) {
//             //gameOver
//         }
//     }

//     IEnumerator SpawnStars () 
//     {
//         float time = starSpawnTime;
//         while (true) 
//         {
//             if (time > 0) 
//             {
//                 time -= Time.deltaTime;
//               //  Debug.Log ($"Reducing time: {time}");
//             } 
//             else 
//             {
//                 time = starSpawnTime;
//                 SpawnStar ();
//             }
//            // Debug.Log($"Time Scale: {Time.timeScale}");
//             yield return null;
//         }
//     }

//     void SpawnStar () {
//         //Vector3 spawnPos = NetworkManager.singleton.GetStartPosition ().position + Vector3.up;
//         Vector3 spawnPos = GetRandomStarPosition() + Vector3.up;

//         GameObject starObject = Instantiate (starPrefab, spawnPos, Quaternion.identity);
//         starObject.GetComponent<NetworkMatchChecker> ().matchId = networkMatchChecker.matchId;
//         //Debug.Log ($"Spawning Star: {networkMatchChecker.matchId}");

//         NetworkServer.Spawn (starObject);
//     }

//     Vector3 GetRandomStarPosition()
//     {
//         return spawnPositions[Random.Range(0, spawnPositions.Length)].transform.position;
//     }
// }
