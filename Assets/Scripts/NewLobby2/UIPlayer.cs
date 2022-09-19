using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    [SerializeField] Text text;
    Player player;
    InputField nameInput2;

    public void SetPlayer(Player player, string name)
    {
        //nameInput2 = GameObject.Find("NameInput").GetComponent<InputField>();

        this.player = player;
        this.player.displayName = name;

        //if (!hasAuthority) { return; }

        text.text = "Player " + player.playerIndex.ToString(); //Player 1/2/3 Names        
        //text.text = this.player.displayName;

    }
}
