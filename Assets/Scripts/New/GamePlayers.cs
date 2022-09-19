using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GamePlayers : NetworkBehaviour
{
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    bool bShowupScore = false;


    public int PikachusLeft2 = 0;
    HealthScriptPikachu[] allGamePlayers;
    Text pikachusLeftText;


    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    public string transferName;

    public override void OnStartClient()
    {
        StartCoroutine(RespawnLevelDelay());
        //StartCoroutine(UpdateClient());

        PikachusLeft2 = 0;

        if (NetworkServer.active) { return; }

        //  ((NetworkManagerMario)NetworkManager.singleton).Players.Add(this);

        if (!isClientOnly) { return; }
    }

    public override void OnStartAuthority()
    {
    }

    //void Update()
    //{
    //   // while (true)
    //    {
    //        {
  

    //            TimerManager.instance.ShowTimer = true;


    //        }
    //       // yield return null;

    //    }

    //}

    IEnumerator RespawnLevelDelay()
    {
        yield return new WaitForSeconds(3.4f);

        PikachusLeft2 = 0;

        allGamePlayers = FindObjectsOfType<HealthScriptPikachu>();

        foreach (HealthScriptPikachu player in allGamePlayers)
        {
            if (player.isActiveAndEnabled)
                PikachusLeft2++;
        }

        //CmdRespawnLevel();
    }


    //[Command]
    //public void CmdRespawnLevel()
    //{
    //    PikachusLeft2 = 3;

    //    allGamePlayers = FindObjectsOfType<PlayerSwitch>();

    //    foreach (PlayerSwitch player in allGamePlayers)
    //    {
    //        if (player.isMario == false) //is pikachu
    //        {
    //            PikachusLeft2++;
    //        }
    //    }

    //    RpcRespawnLevel();
    //}

    //[ClientRpc]
    //public void RpcRespawnLevel()
    //{
    //    PikachusLeft2 = 4;

    //    allGamePlayers = FindObjectsOfType<PlayerSwitch>();

    //    foreach (PlayerSwitch player in allGamePlayers)
    //    {
    //        if (player.isMario == false) //is pikachu
    //        {
    //            PikachusLeft2++;
    //        }
    //    }
    //}


    public string GetDisplayName()
    {
        return displayName;
    }
    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) { return; }

        ((NetworkManagerMario2)NetworkManager.singleton).Players.Remove(this); //CHANGE BACK
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) { return; }

        ((NetworkManagerMario)NetworkManager.singleton).StartGame();
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

}
