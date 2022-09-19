using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UILobby : MonoBehaviour
{
    public static UILobby instance;

    [Header("Host Join")]
    [SerializeField] InputField joinMatchInput;
    [SerializeField] List<Selectable> lobbySelectables = new List<Selectable>();
    [SerializeField] public InputField nameInput;
    [SerializeField] public Canvas lobbyCanvas;
    [SerializeField] Canvas searchCanvas;

    [Header("Lobby")]
    [SerializeField] public GameObject connectObject;
    [SerializeField] Transform UIPlayerParent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] Text matchIDText;
    [SerializeField] GameObject beginGameButton;
    [SerializeField] GameObject sceneObject;
    [SerializeField] GameObject hostPrivateFirstBtn;
    [SerializeField] GameObject nameUI;
    [SerializeField] GameObject confirmButton;

    GameObject playerLobbyUI;

    public GameObject pictures;

    bool searching = false;

    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNameKey = "PlayerName";

    public bool bHideCanvas = true;
    Text ammoText;

private string[] randomNames = new string[] { "Alex", "Andreas", "Justin", "Emilio", "Jasmine", "Cindy", "Garth", "Jesus", "Donte", "Domenic", "Jeremy",
    "Amanda", "John", "Suzy", "Gabriel", "Elayna", "Sal", "Karl", "Shelley", "Amber", "Michelle", "Linda", "Matt", "Trish", "Michael", "Samantha" };

    private void Start()
    {
        instance = this;
        bHideCanvas = true;

         #if UNITY_EDITOR
             Debug.unityLogger.logEnabled = true;
        #else
             Debug.unityLogger.logEnabled=false;
        #endif

      //  ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
       // ammoText.text = "";

        StartCoroutine(UpdateLobby());
        StartCoroutine(LateUpdateLobby());

        // if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        // string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        // nameInput.text = defaultName;
        nameInput.text = GetRandomName();   
     }
    public string GetRandomName()
     {
         return randomNames[Random.Range(0, randomNames.Length)];
     }

    public void SetPlayerName(string name) //makes it interactable button
    {
        //joinButton.interactable = !string.IsNullOrEmpty(name);
        //hostButton.interactable = !string.IsNullOrEmpty(name);

        DisplayName = name;
    }

    public void SavePlayerName()
    {
        DisplayName = nameInput.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }




    public void HostPrivate()
    {
        joinMatchInput.interactable = false;

        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.HostGame(false);
    }

    public void HostPublic()
    {
        joinMatchInput.interactable = false;

        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.HostGame(true);
    }

    public void HostSuccess(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;
            //clear selected object
            EventSystem.current.SetSelectedGameObject(null);
            //select a new selected object
            EventSystem.current.SetSelectedGameObject(beginGameButton);

            if (playerLobbyUI != null)
            {
                Destroy(playerLobbyUI);
            }
            playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = matchID;
            beginGameButton.SetActive(true);
        }
        else
        {
            joinMatchInput.interactable = true;
            lobbySelectables.ForEach(x => x.interactable = true);

        }
    }

    public void Join()
    {
        joinMatchInput.interactable = false;
        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;
            beginGameButton.SetActive(false);

            if (playerLobbyUI != null)
            {
                Destroy(playerLobbyUI);
            }
            playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = matchID;
        }
        else
        {
            joinMatchInput.interactable = true;
            lobbySelectables.ForEach(x => x.interactable = true);

        }
    }

    public GameObject SpawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player, DisplayName);
        newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
        return newUIPlayer;
    }

    public void BeginGame()
    {
        // CmdTurnOffLobby();
        sceneObject.SetActive(false);
        lobbyCanvas.enabled = false;
        gameObject.SetActive(false);
        connectObject.SetActive(false);
        pictures.SetActive(false);
        //GameOverManager.instance.bBeginGame = false;

        Player.localPlayer.BeginGame();
    }

    public void SearchGame()
    {
        Debug.Log($"Searching for game");
        searchCanvas.enabled = true;
        StartCoroutine(SearchingForGame());
    }

    IEnumerator SearchingForGame()
    {
        searching = true;

        float currentTime = 1;
        while (searching)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            } 
            else
            {
                currentTime = 1;
                Player.localPlayer.SearchGame();
            }
            yield return null;
        }
    }

    public void SearchSuccess(bool success, string matchID)
    {
        if (success)
        {
            searchCanvas.enabled = false;
            JoinSuccess(success, matchID);
            searching = false;
        }
    }

    public void SearchCancel()
    {
        searchCanvas.enabled = false;
        searching = false;
        lobbySelectables.ForEach(x => x.interactable = true);
    }

    //public void GoBackLobby()
    //{
    //    joinMatchInput.interactable = true;
    //    lobbySelectables.ForEach(x => x.interactable = true);
    //    // Application.LoadLevel(Application.loadedLevel);
    //}

    public void DisconnectLobby()
    {
        if (playerLobbyUI != null)
        {
            Destroy(playerLobbyUI);
        }
        Player.localPlayer.DisconnectGame();
        
        lobbyCanvas.enabled = false;
        lobbySelectables.ForEach(x => x.interactable = true);
        beginGameButton.SetActive(false);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //select a new selected object
        EventSystem.current.SetSelectedGameObject(hostPrivateFirstBtn);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    //void Update()
    IEnumerator UpdateLobby() //GamePLayers AND UILobby //gameplayers = in game, UiLobby = in lobby
    {
        while(true)
        {
            //Text pikachusLeftText = GameObject.Find("PikachusLeftText").GetComponent<Text>();
            //pikachusLeftText.text = "";
            //GameOverManager.instance.bShowPikaText = false;

            //GameOverManager.instance.bHidePanelsBeginning = true; //Update in GamePlayers
            //TimerManager.instance.ShowTimer = false;

            //HandleShooting[] shooterMarios = FindObjectsOfType<HandleShooting>(); //UI LOBBY!!!!!!!!!!!!!!!!!! and gameplayers
            //foreach (HandleShooting mario in shooterMarios)
            //{
            //    mario.bShowAmmoText = false;
            //}

            //ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
            //ammoText.text = "";

            //PickupSpawningManager.instance.bStopSpawning = true;

            if (bHideCanvas)
            {
                //GameObject.Find("GameOverCanvas").SetActive(false);
            }

            //TurnOffGameOverPanels.instance.bTurnOnGameOverManager = false; //turn off HUD on lobby

            yield return null;

        }
    }

    //void LateUpdate()
    IEnumerator LateUpdateLobby()
    {
        while (true)
        {
            //ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
            //ammoText.text = "";

            yield return new WaitForFixedUpdate();
        }
    }


    public void ConfirmSelect() { //Beginning Confirm Button
        connectObject.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //select a new selected object
        EventSystem.current.SetSelectedGameObject(hostPrivateFirstBtn);
    }
    public void BackButtonLobby1() { //Beginning Confirm Button
        nameUI.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //select a new selected object
        EventSystem.current.SetSelectedGameObject(confirmButton);
    }



    //[Server]
    public void SetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
}
