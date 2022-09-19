
//using System.Collections;
//using System.Collections.Generic;
//using Mirror;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class GameOverManager : MonoBehaviour
//{
//    public static GameOverManager instance = null;

//    public float timeStart = 200; //200 = 3:20aasd

//    //[SerializeField] public GameObject MarioWinPanel = null;
//    //[SerializeField] public GameObject PikachuWinPanel = null;

//    //Text pikachusLeftText;
//    Text mariosLeftText;

//    bool isMario = false;

//    private GameObject[] allPlayers;
//    int MarioPlayerStart;

//    //float StartTimer = 0f;
//    //bool StartEnabled = false;

//    float EndTimer = 0f;
//    //public bool bGameOver = false;

//    //public bool bMarioWins = false;
//    //public bool bPikachuWins = false;

//    // public GameObject MarioPrefab;
//    //public GameObject PikachuPrefab;
//    //public GameObject CursorPrefab;

//    // public bool bMario;


//    //int numPlayers2 = 0;

//    public int PikachusLeft2 = 0;
//    public int MariosLeft = 0;

//    HealthScriptPikachu[] allPikachuPlayers;
//    HealthScript[] allMarioPlayers;


//    NetworkSpawnPositions[] spawnPositions;

//    //Dictionary<NetworkConnection, int> killsForConnection = new Dictionary<NetworkConnection, int>();

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else if (instance != this)
//        {
//            Destroy(gameObject);
//        }
//    }

//    //public override void OnStartAuthority()
//    void Start()
//    {
//        TurnManager.instance.ServerSpawnConnection();

//        bBeginGame = false;

//        // pikachusLeftText = GameObject.Find("PikachusLeftText").GetComponent<Text>();

//        Cursor.visible = true;
//        Cursor.lockState = CursorLockMode.None;

//        MarioWinPanel.SetActive(false);
//        PikachuWinPanel.SetActive(false);

//        spawnPositions = FindObjectsOfType<NetworkSpawnPositions>();

//        bPikachuWins = false; //only true when timer is 0
//    }

//    IEnumerator SpawnStartUp()
//    {
//        yield return new WaitForSeconds(1.2f);

//        TurnManager.instance.RespawnConnection();

//    }

//    //IEnumerator UpdateManager()
//    void Update()
//    {
//        {

//            //if (pikachusLeftText == null)
//            //{
//            //    pikachusLeftText = GameObject.Find("PikachusLeftText").GetComponent<Text>();
//            //}
//            //if (pikachusLeftText)
//            //{
//            //    pikachusLeftText.text = "Pikachus Left: " + PikachusLeft2.ToString();
//            //}

//            if (PikachusLeft2 <= 0) //PikachusLeft
//            {
//                bMarioWins = true;
//            }
//            else
//            {
//                bMarioWins = false;
//            }


//            if (bMarioWins && bPikachuWins == false)
//            {
//                MarioWinPanel.SetActive(true);
//            }
//            else
//            {
//                MarioWinPanel.SetActive(false);
//            }

//            if (bPikachuWins) //ONLY happens when timer goes to 0!
//            {
//                PikachuWinPanel.SetActive(true);
//            }
//            else
//            {
//                PikachuWinPanel.SetActive(false);
//            }

//            if (bMarioWins || bPikachuWins)
//            {
//                EndTimer += Time.deltaTime;
//                //Debug.Log(EndTimer.ToString()); //test
//            }
//            else
//            {
//                EndTimer = 0;
//            }

//            if (EndTimer > 6.2f) //change back to 6.2f
//            {

//                TurnManager.instance.RespawnConnection();
//                //MatchMaker.instance.TurnChange();
//                //StartCoroutine(ResetLevelServer());

//                bPikachuWins = false;
//                EndTimer = 0f;

//            }
//        }

//    }

//    //public void RespawnLevelConnector()
//    //{
//    //    //StartCoroutine(RespawnLevel());
//    //    RespawnLevel();
//    //}

//    ////IEnumerator RespawnLevel()
//    //void RespawnLevel()
//    //{
//    //    //yield return new WaitForSeconds(0.032f);//HAS to be ABOVE 1.32

//    //    PikachusLeft2 = 0;
//    //    MariosLeft = 0;

//    //    allPikachuPlayers = FindObjectsOfType<HealthScriptPikachu>();
//    //    allMarioPlayers = FindObjectsOfType<HealthScript>();

//    //    foreach (HealthScriptPikachu player in allPikachuPlayers)
//    //    {
//    //        if (player.isActiveAndEnabled)
//    //            PikachusLeft2++;
//    //    }
//    //    foreach (HealthScript player in allMarioPlayers)
//    //    {
//    //        if (player.isActiveAndEnabled)
//    //            MariosLeft++;
//    //    }

//    //    if (MariosLeft <= 0)
//    //    {
//    //        TurnManager.instance.RespawnConnection();
//    //    }

//    //    //CmdRespawnLevel();
//    //}








//    public void ChaseLevelServer()
//    {

//        StartCoroutine(ResetLevelServer());
//    }

//    //CODE DOWN AND BELOW DIFFERENT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//    // [Server]
//    IEnumerator ResetLevelServer() //Marios Win ////// THIS IS MEANT FOR Local/Steam! Need MatchMaking CODE HERE!!
//    {
//        yield return new WaitForSeconds(3.22f); //0.016f

//        MarioPlayerStart = 0;
//        numPlayers2 = 0;

//        //allPlayers = GameObject.FindGameObjectsWithTag("Player"); //Dont need Pikachus (for this game mode) - maybe for timer event
//        Player[] allPlayers = FindObjectsOfType<Player>();
//        MarioPlayerStart = allPlayers.Length;

//        List<Player> allPlayersDirect = MatchMaker.instance.matchPlayers;

//        //foreach (Player player in allPlayersDirect)
//        foreach (Player player in allPlayers)
//        {
//            Transform StartPosition2 = NetworkManager.singleton.GetStartPosition();

//            NetworkIdentity id = player.GetComponent<NetworkIdentity>();

//            if (id != null)
//            {

//                StartCoroutine(RespawnNew(player));

//            }
//        }




//    }

//    //  [Client]
//    IEnumerator RespawnNew(Player player) //Mario
//    {
//        yield return new WaitForSeconds(0.32f); //0.032

//        //int MarioPlayerStart = UnityEngine.Random.Range(1, allPlayers.Length + 1);

//        //TimerManager.instance.timeValue = timeStart; //important

//        //PickupSpawningManager.instance.ResetGamePickups();

//        Transform StartPosition2 = NetworkManager.singleton.GetStartPosition();

//        numPlayers2++;
//        /////////////////////ENDS


//        if (numPlayers2 == 2)
//        {
//            if (player)
//            {
//                player.GetComponent<PlayerSwitch>().SwitchToMario();
//            }

//        }
//        else
//        {
//            if (player)
//            {
//                player.GetComponent<PlayerSwitch>().SwitchToPikachu();
//            }
//        }
//    }

//    //[Server]
//    GameObject SpawnGameObject(GameObject go, Transform transform)
//    {
//        GameObject newGameObject = Instantiate(go,
//            transform.position, Quaternion.identity);

//        return newGameObject;
//    }
//    // [Server]
//    IEnumerator NameChangeSame(GameObject go, string nameTag)
//    {
//        yield return new WaitForSeconds(0.01f);

//        CmdNameChangeSame(go, nameTag);
//        go.GetComponent<PlayerName>().playerNameTag = nameTag;
//    }

//    //[Command]
//    void CmdNameChangeSame(GameObject go, string nameTag)
//    {
//        RpcNameChangeSame(go, nameTag);
//        go.GetComponent<PlayerName>().playerNameTag = nameTag;

//    }
//    //[ClientRpc]
//    void RpcNameChangeSame(GameObject go, string nameTag)
//    {
//        go.GetComponent<PlayerName>().playerNameTag = nameTag;

//    }

//}
