using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
public class HealthScript : NetworkBehaviour {

    [SyncVar]
    public int Health = 100;
    Text ammoText;

    bool bBulletShot = false;

    Text healthText;
    public Image crosshair2;
    public Animator anim;

    [SyncVar]
    public string reoccuringName;

    [SerializeField]
    private HandleShooting handleShooting = null;

    [SyncVar]
    public NetworkAnimator networkAnimator = null;

    public GameObject thisGameObject;

    [SyncVar]
    public bool IsDead = false;
    //public bool IsDeadAnimate = false;

    StateManager states;

    void Start()
    {
        //StartCoroutine(Update1());
        StartCoroutine(SetText());

        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        crosshair2 = GameObject.Find("Crosshair2").GetComponent<Image>();
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();

        Health = 100;
        //GameOverManager.instance.bHidePanelsBeginning = false; Update in gamePlayers
    }

    public override void OnStartClient () 
    {
        //StartCoroutine(Update1());

        //anim = GetComponent<Animator> ();
        states = GetComponent<StateManager> ();
        reoccuringName = GetComponent<PlayerName> ().playerNameTag;

        healthText = GameObject.Find ("HealthText").GetComponent<Text> ();
        crosshair2 = GameObject.Find ("Crosshair2").GetComponent<Image> ();
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();

        gameObject.tag = "Player";

        Health = 100;

        if (!isLocalPlayer)
        {
            //Destroy(GetComponent<Rigidbody>());
        }

        crosshair2.enabled = true;
        TurnManager.instance.CursorPrefab.SetActive(true); //Mario in HandleShooting


    }

    IEnumerator SetText()
    {
        yield return new WaitForSeconds(0.32f);
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        crosshair2 = GameObject.Find("Crosshair2").GetComponent<Image>();
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
    }

    //IEnumerator UpdateClient () //DUNNO
    void Update()
    {
            if (!hasAuthority) { return; }
            //if (!hasAuthority)
            {
                //yield return null;
                //continue;
            }

            if (gameObject.transform.position.y < -22f || gameObject.transform.position.y > 74f) // Original (Peach Castle At 0 Y-height)
            {
                Health = 0;
            }

            if (Health <= 0)
            {
                Health = 0;
                Die();
            }

            //if (healthText == null)
            //{
            //    healthText = GameObject.Find("HealthText").GetComponent<Text>();
            //}

            //if (crosshair2 == null)
            //{
            //    crosshair2 = GameObject.Find("Crosshair2").GetComponent<Image>();
            //    crosshair2.enabled = true;
            //}
            //if (ammoText == null)
            //{
            //    ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
            //}

            if (healthText)
            {
                healthText.text = "Health: " + Health.ToString();
            }

            //if (!isClient) { return; }

            if (gameObject.transform.position.y < -22f || gameObject.transform.position.y > 74f) // Original (Peach Castle At 0 Y-height)
            {
                Health = 0;
            }

            if (Health <= 0)
            {
                Health = 0;
                Die();
            }

            reoccuringName = GetComponent<PlayerName>().playerNameTag;

        
    }


    public void TakeDamage (int damage) 
    {
        Health -= damage;
        Debug.Log (Health);
    }

    private void Die () 
    {
        if (anim && !IsDead) 
        {
            CmdIsDeadOn();
            IsDead = true;

            //   gameObject.tag = "Dead";

            states.audioManager.PlayDeathSound ();


            //networkAnimator.SetTrigger ("Death");
            anim.SetBool("Death", true);
            
            //GetComponent<PlayerMovement> ().enabled = false;

           // GetComponent<PlayerMovement>().bStopMovement = true;
            //GetComponent<InputHandler>().bStopAiming = true;

            GetComponent<HandleAnimations> ().enabled = false;
            GetComponent<HandleShooting> ().enabled = false;

            StartCoroutine(Respawn(thisGameObject)); //switch aroo?
        }
    }

    [Command]
    void CmdIsDeadOn()
    {
        IsDead = true;
        RpcIsDeadOn();
        anim.SetBool("Death", true);
    }
    [ClientRpc]
    void RpcIsDeadOn()
    {
        IsDead = true;
        anim.SetBool("Death", true);

    }

    IEnumerator Respawn (GameObject go) {
        //NetworkServer.UnSpawn(this.gameObject);
        //Transform newPos = NetworkManager.singleton.GetStartPosition();
        //go.transform.position = newPos.position;
        //go.transform.rotation = newPos.rotation;

        yield return new WaitForSeconds (3.2f); //3.2

        Health = 100;
        //RpcHealth();
        CmdHealth();

        //networkAnimator.SetTrigger ("Revive");
        anim.SetBool("Revive", true);

        GetComponent<PlayerMovement> ().enabled = true;

       // GetComponent<PlayerMovement>().bStopMovement = false;
        //GetComponent<InputHandler>().bStopAiming = false;

        GetComponent<HandleAnimations> ().enabled = true;
        GetComponent<HandleShooting> ().enabled = true;

        Transform newPos = NetworkManager.singleton.GetStartPosition ();
        this.gameObject.transform.position = newPos.position;
        this.gameObject.transform.rotation = newPos.rotation;
        IsDead = false;
        handleShooting.curBullets = handleShooting.BulletsMax; //current bullet script

       // NetworkServer.Spawn (thisGameObject, connectionToClient);
        //GetComponent<PlayerName> ().playerNameTag = reoccuringName; //?????????
    }

    [Command]
    void CmdHealth()
    {
        Health = 100;
        IsDead = false;
        anim.SetBool("Revive", true);


        RpcHealth();
    }

    [ClientRpc]
    void RpcHealth()
    {
        //yield return new WaitForSeconds(1.2f); //3.2
        Health = 100;
        IsDead = false;

        //networkAnimator.SetTrigger("Revive");
        anim.SetBool("Revive", true);


        Transform newPos = NetworkManager.singleton.GetStartPosition();
        this.gameObject.transform.position = newPos.position;
        this.gameObject.transform.rotation = newPos.rotation;

    }
}