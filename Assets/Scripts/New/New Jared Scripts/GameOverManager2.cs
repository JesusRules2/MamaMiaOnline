//using System.Collections;
//using System.Collections.Generic;
//using Mirror;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class GameOverManager2 : NetworkBehaviour {
//    public static GameOverManager instance = null;

//    public int PikachusAlive = 0;
//    public int PikachusDead = 0;

//    [SyncVar]
//    public float timeStart = 200;

//    int numPikasAttempt = 0;

//    [SerializeField] public GameObject MarioWinPanel = null;
//    [SerializeField] public GameObject PikachuWinPanel = null;

//    public bool bHidePikachuCount = false;

//    Text pikachusLeftText;
//    Text mariosLeftText;

//    bool isMario = false;

//    private GameObject[] allPlayers;
//    int MarioPlayerStart;

//    float StartTimer = 0f;
//    bool StartEnabled = false;
//    float EndTimer = 0f;
//    public bool bGameOver = false;

//    public bool bMarioWins = false;
//    public bool bPikachuWins = false;

//    public GameObject MarioPrefab;
//    public GameObject PikachuPrefab;

//    bool bBeginGame = false;

//    public List<GameObject> Players2 = new List<GameObject> (); //List of Mario AND pikachus
//    GamePlayers[] allGamePlayers;
//    int numPlayers2 = 0;

//    int PikachusLeft = 0;
//    int PikachusLeft2 = 0;

//    Dictionary<NetworkConnection, int> killsForConnection = new Dictionary<NetworkConnection, int> ();

//    //  [SerializeField] private NetworkManagerMario networkManagerMario = null;

//    private void Awake () {
//        if (instance == null) {
//            instance = this;
//        } else if (instance != this) {
//            Destroy (gameObject);
//        }
//    }

//    // Start is called before the first frame update
//    void Start () {

//        pikachusLeftText = GameObject.Find ("PikachusLeftText").GetComponent<Text> ();
//        pikachusLeftText.text = "Pikachus Left: " + PikachusLeft.ToString ();

//        Cursor.visible = false;
//        Cursor.lockState = CursorLockMode.Confined;

//        MarioWinPanel.SetActive (false);
//        PikachuWinPanel.SetActive (false);
//    }

//    // Update is called once per frame

//    void Update () {

//        if (bBeginGame == false) {
//            PikachusAlive = 0;
//            if (isServer) StartCoroutine (ResetLevelServer ());
//            //Debug.Log ("Update gets called???????????");
//            bBeginGame = true;
//        }

//        //PikachusLeft = PikachusAlive - PikachusDead;
//        PikachusLeft2 = FindObjectsOfType<HealthScriptPikachu> ().Length; //INSTEAD OF HealthScriptPikachu
//        pikachusLeftText.text = "Pikachus Left: " + PikachusLeft2.ToString ();
//        //Debug.Log("PIKAS!!!!: " + PikachusLeft2.ToString());

//        if (StartTimer < 0.12f && !StartEnabled) {
//            StartTimer += Time.deltaTime;
//        } else {
//            StartEnabled = true;
//        }

//        if (!StartEnabled) { return; }

//        //if (PikachusDead == PikachusAlive)
//        if (PikachusLeft2 <= 0) //PikachusLeft
//        {
//            bMarioWins = true;
//        } else {
//            bMarioWins = false;
//        }

//        //if (bMarioWins && bHidePikachuCount == true) 
//        if (bMarioWins) {
//            MarioWinPanel.SetActive (true);
//        } else {
//            MarioWinPanel.SetActive (false);
//        }

//        if (bPikachuWins) {
//            PikachuWinPanel.SetActive (true);
//        } else {
//            PikachuWinPanel.SetActive (false);
//        }

//        if (bMarioWins || bPikachuWins) {
//            EndTimer += Time.deltaTime;
//            //Debug.Log(EndTimer.ToString()); //test
//        } else {
//            EndTimer = 0;
//        }

//        if (EndTimer > 6.2f) //change back to 6.2f
//        {

//            //bMarioWins = false; ??
//            //bPikachuWins = false;
//            //PikachuWinPanel.SetActive(false);
//            //MarioWinPanel.SetActive(false);

//            EndTimer = 0f;

//            //Debug.Log("PIKAS: " + PikachusLeft.ToString());
//            PikachusAlive = 0;
//            PikachusDead = 0;

//            //PikachusLeft = PikachusAlive - PikachusDead;
//            //pikachusLeftText.text = "Pikachus Left: " + PikachusLeft.ToString();

//            //if (PikachusLeft <= 0)
//            // {
//            StartCoroutine (ResetLevelServer ());
//            //ResetLevelServer();
//            // }

//            //allPlayers = GameObject.FindGameObjectsWithTag("Player"); //Dont need Pikachus (for this game mode) - maybe for timer event
//        }
//    }

//    // [Server]
//    //void ResetLevelServer() //Marios Win
//    IEnumerator ResetLevelServer () {
//        yield return new WaitForSeconds (0.16f);

//        // TimerManager.instance.timeValue = TimerManager.instance.timeStart; //important
//        // TimerManager.instance.timeValue = timeStart; //important

//        MarioPlayerStart = 0;
//        numPlayers2 = 0;

//        //Debug.Log ("RESET_LEVEL_SERVER CALLED!!!");

//        allPlayers = GameObject.FindGameObjectsWithTag ("Player"); //Dont need Pikachus (for this game mode) - maybe for timer event
//        MarioPlayerStart = allPlayers.Length;

//        //Debug.Log ("Mario = " + MarioPlayerStart);

//        foreach (GameObject player in allPlayers) {
//            Transform StartPosition2 = NetworkManager.singleton.GetStartPosition ();

//            PlayerName namePlayer = player.GetComponent<PlayerName> ();
//            NetworkIdentity id = player.GetComponent<NetworkIdentity> ();
//            //KillCount playerKillCount = player.GetComponent<KillCount> ();

//            if (id != null) //return;
//            {

//                StartCoroutine (RespawnNew (id, namePlayer, player));

//            }
//        }
//    }

//    //NUMBER 2 (Bottom)

//    //  [Client]
//    IEnumerator RespawnNew (NetworkIdentity id, PlayerName namePlayer, GameObject player) {
//        yield return new WaitForSeconds (0.32f);

//        TimerManager.instance.timeValue = timeStart; //important

//        bMarioWins = false;
//        bPikachuWins = false;
//        PikachuWinPanel.SetActive (false);
//        MarioWinPanel.SetActive (false);

//        PickupSpawningManager.instance.ResetGamePickups ();

//        string nameTag = namePlayer.playerNameTag;
//        Transform startPosition = NetworkManager.singleton.GetStartPosition ();

//        numPlayers2++;

//        GameObject playerSpawn = SpawnGameObject (numPlayers2 == 2 ? TurnManager.instance.MarioPrefab : TurnManager.instance.PikachuPrefab, startPosition, nameTag, player);

//        Debug.Log ($"{(numPlayers2 == 2 ? "Mario Spawned" : "Pikachu Spawned")}");

//        if (NetworkServer.ReplacePlayerForConnection (id.connectionToClient, playerSpawn, true)) {
//            Debug.Log ($"Replaced player for connection successfully");
//        } else {
//            Debug.Log ($"Failed to replace player for connection");
//        }

//        // TargetKillCount (playerSpawn, id, player);
//        //int kills = player.GetComponent<KillCount>().savedKills;
//        NetworkServer.Destroy(id.gameObject);
//        playerSpawn.GetComponent<KillCount>().kills = killsForConnection[id.connectionToClient];

//        playerSpawn.GetComponent<PlayerName> ().playerNameTag = nameTag;

//        // playerSpawn.GetComponent<KillCount>().kills = player.GetComponent<KillCount>().kills; //DONT WORK ON SERVER BUILT
//        //playerSpawn.GetComponent<KillCount>().kills = 32; //Test

//        StartCoroutine (NameChangeSame (playerSpawn, nameTag, player, id));
//    }

//    [Server]
//    GameObject SpawnGameObject (GameObject go, Transform transform, string nameTag, GameObject player) {

//        GameObject newGameObject = Instantiate (go,
//            transform.position, Quaternion.identity);

//        return newGameObject;
//    }

//    // [Server]
//    IEnumerator NameChangeSame (GameObject go, string nameTag, GameObject player, NetworkIdentity id) { //name AND kills

//        yield return new WaitForSeconds (0.32f);

//        go.GetComponent<PlayerName> ().playerNameTag = nameTag;

//        go.GetComponent<KillCount> ().kills = player.GetComponent<KillCount> ().kills; //DONT WORK

//    }

//    public void AddKillsForConnection(NetworkConnection conn){
//        killsForConnection[conn]++;
//    }

//    // [TargetRpc]
//    // void TargetKillCount (GameObject playerSpawn, NetworkIdentity id, GameObject player) {
//    //     int kills = player.GetComponent<KillCount> ().savedKills;

//    //     //NetworkServer.Destroy(id.gameObject);

//    //     playerSpawn.GetComponent<KillCount> ().kills = kills;
//    // }

//    //[Command]
//    //void CmdNameChangeSame(GameObject go, string nameTag, GameObject player, NetworkIdentity id)
//    //{
//    //    go.GetComponent<PlayerName>().playerNameTag = nameTag;
//    //    go.GetComponent<KillCount>().kills = player.GetComponent<KillCount>().kills;

//    //    RpcNameChangeSame(go, nameTag, player, id);
//    //}

//    //[ClientRpc]
//    //void RpcNameChangeSame (GameObject go, string nameTag, GameObject player, NetworkIdentity id) 
//    //{
//    //    //go.GetComponent<KillCount>().kills = 32;
//    //    go.GetComponent<PlayerName> ().playerNameTag = nameTag;
//    //    go.GetComponent<KillCount>().kills = player.GetComponent<KillCount>().kills;                                                                                                 // go.GetComponent<KillCount>().kills = 32;

//    //}

//}