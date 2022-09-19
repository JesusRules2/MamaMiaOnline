// using Mirror;
// using Mirror.FizzySteam;
// using Steamworks;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// public class MainMenu : MonoBehaviour
// {
//     [SerializeField] private GameObject landingPagePanel = null;
//     //[SerializeField] private GameObject lobbyUI = null;

//     [SerializeField] private bool useSteam = false;

//     protected Callback<LobbyCreated_t> lobbyCreated;
//     protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
//     protected Callback<LobbyEnter_t> lobbyEntered;

//     [SerializeField] private GameObject marioPic;
//     [SerializeField] private GameObject pikachuPic;

//     Transport telepathyTransport;
//     Transport fizzySteamworksTransport;

//     [SerializeField] private Button modeButton;
//     [SerializeField] private Button joinButton;
//     [SerializeField] private GameObject steamText;
//     [SerializeField] private GameObject steamText2;
//     private bool bSwitchMode = false;

//     public static CSteamID LobbyId { get; private set; }

//     private void Start()
//     {
//         Cursor.visible = true;
//         Cursor.lockState = CursorLockMode.None;

//         telepathyTransport = GameObject.Find("NetworkManager").GetComponent<TelepathyTransport>();
//         fizzySteamworksTransport = GameObject.Find("NetworkManager").GetComponent<FizzySteamworks>();
//         //SteamModeOn();
//         LocalModeOn();


//         if (!useSteam) { return; }

//         lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
//         gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
//         lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
//     }

//     public void EndGame()
//     {
//         Application.Quit();
//     }

//     public void HostLobby()
//     {
//         landingPagePanel.SetActive(false);
//         //     lobbyUI.SetActive(true);
//         steamText.SetActive(false);
//         steamText2.SetActive(false);

//         if (useSteam)
//         {
//             SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 12); //set players
//             return;
//         }

//         NetworkManager.singleton.StartHost();


//     }

//     public void SwitchMode()
//     {
//         bSwitchMode = !bSwitchMode;

//         if (bSwitchMode == true)
//         {
//             SteamModeOn();
//         }
//         if (bSwitchMode == false)
//         {
//             LocalModeOn();
//         }
//     }

//     private void Update()
//     {
//        // telepathyTransport = GameObject.Find("NetworkManager").GetComponent<TelepathyTransport>();
//        // fizzySteamworksTransport = GameObject.Find("NetworkManager").GetComponent<FizzySteamworks>();
//     }

//     void SteamModeOn()
//     {
//          modeButton.GetComponentInChildren<TMP_Text>().text = "STEAM VERSION";
//         GameObject.Find("NetworkManager").GetComponent<NetworkManagerMario>().transport = fizzySteamworksTransport;
//         GameObject.Find("NetworkManager").GetComponent<FizzySteamworks>().enabled = true;
//         GameObject.Find("NetworkManager").GetComponent<SteamManager>().enabled = true;
//         GameObject.Find("NetworkManager").GetComponent<TelepathyTransport>().enabled = false;
//         useSteam = true;

//         steamText.SetActive(true);
//         steamText2.SetActive(true);
//         joinButton.gameObject.SetActive(false);
//     }
//     void LocalModeOn()
//     {
//         modeButton.GetComponentInChildren<TMP_Text>().text = "LOCAL VERSION";
//         GameObject.Find("NetworkManager").GetComponent<NetworkManagerMario>().transport = telepathyTransport;
//         GameObject.Find("NetworkManager").GetComponent<FizzySteamworks>().enabled = false;
//        GameObject.Find("NetworkManager").GetComponent<SteamManager>().enabled = false;
//        GameObject.Find("NetworkManager").GetComponent<TelepathyTransport>().enabled = true;
//         useSteam = false;

//         steamText.SetActive(false);
//         steamText2.SetActive(false);
//         joinButton.gameObject.SetActive(true);

//     }

//     private void OnLobbyCreated(LobbyCreated_t callback)
//     {
//         if (callback.m_eResult != EResult.k_EResultOK)
//         {
//             landingPagePanel.SetActive(true);
//             return;
//         }

//         LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

//         NetworkManager.singleton.StartHost();

//         SteamMatchmaking.SetLobbyData(
//             LobbyId, 
//             "HostAddress",
//             SteamUser.GetSteamID().ToString());
//     }

//     private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
//     {
//         SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
//     }

//     private void OnLobbyEntered(LobbyEnter_t callback)
//     {
//         if (NetworkServer.active) { return; }

//         string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),
//             "HostAddress");

//         NetworkManager.singleton.networkAddress = hostAddress;
//         NetworkManager.singleton.StartClient();

//         landingPagePanel.SetActive(false);
//         pikachuPic.SetActive(false);
//         marioPic.SetActive(false);
//         steamText.SetActive(false);
//         steamText2.SetActive(false);


//     }
// }
