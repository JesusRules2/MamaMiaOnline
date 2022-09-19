using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
public class HealthScriptPikachu : NetworkBehaviour
{
    [SyncVar]
    public int Health = 30;
    Text healthText;
    Text ammoText;
    public Image crosshair2;
    public Animator anim;

    [SyncVar]
    string reoccuringName;

    //[SerializeField]
    public NetworkAnimator networkAnimator = null;
    [SyncVar]
    public PickupStarPikachu pikachuStar = null;

    public GameObject SpawnedGameObject;
    private GameObject crosshair;
    [SerializeField] GameObject parentGO;

    [SyncVar]
    public bool IsDead = false;
    [SyncVar]
    public bool IsDeadAnimate = false;
    [SyncVar]
    public bool bKillCountDisable = false;

    public AudioSource pikachuDie;
    public AudioClip[] pikachuDieClips;

    void Start () {
        // StartCoroutine (WaitForGameManager ());

       // StartCoroutine(UpdateClient());

        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
        crosshair2 = GameObject.Find("Crosshair2").GetComponent<Image>();

        IsDeadAnimate = false;
        IsDead = false;
        bKillCountDisable = false;
        Health = 30;

        //GameOverManager.instance.CursorPrefab.SetActive(false); //Mario in HandleShooting

    }

    public override void OnStartAuthority()
    {
        //GameOverManager.instance.CursorPrefab.SetActive(false); //Mario in HandleShooting
        crosshair2 = GameObject.Find("Crosshair2").GetComponent<Image>();
    }

    public override void OnStartClient()
    {
       // StartCoroutine(UpdateClient());

        pikachuStar = GetComponent<PickupStarPikachu>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
        crosshair2 = GameObject.Find("Crosshair2").GetComponent<Image>();

        IsDeadAnimate = false;
        IsDead = false;
        bKillCountDisable = false;
        Health = 30;

        gameObject.tag = "Player";

        crosshair2.enabled = false;

        TurnManager.instance.CursorPrefab.SetActive(true); //Mario in HandleShooting

         if (!isLocalPlayer)
        {
            //Destroy(GetComponent<Rigidbody>());
            // GetComponent<Rigidbody>().isKinematic = false; NOTHING
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }

    }

    public override void OnStartLocalPlayer () 
    {
        //StartCoroutine(UpdateClient());

        pikachuStar = GetComponent<PickupStarPikachu>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
        //crosshair2 = GameObject.Find("Crosshair2").GetComponent<Image>();

        gameObject.tag = "Player";
        //GameOverManager.instance.CursorPrefab.SetActive(false); //Mario in HandleShooting

    }


    void Update()
    {
        //while (true)
        {
           // if (netIdentity.hasAuthority)
            {
                //crosshair2.enabled = false;
                if (!hasAuthority) { return; }

                //if (gameObject.transform.position.y < 128.0f)
                if (gameObject.transform.position.y < -22f || gameObject.transform.position.y > 74f) //For Peach Castle = 0 Y-height
                {
                    Health = 0;
                }

                if (Health <= 0)
                {
                    Health = 0;
                    Die();
                }


            }
            //yield return null;
        }
    }


    

    public void TakeDamage (int damage, GameObject shooter) 
    { //Called on server

        //RpcTakeDamage (damage, shooter);

        if (pikachuStar.bStarMode) { return; } //yup

        Health -= damage;

        if (Health <= 0 && !IsDead)
        {
            IsDead = true;

            //shooter.GetComponent<KillCount>().kills++;
            //Die();
        }
    }

    //NONE OF THESE USED
    [ClientRpc]
    void RpcTakeDamage(int damage, GameObject shooter)
    {
        //TargetTakeDamage(damage, shooter);

        if (pikachuStar.bStarMode) { return; } //yup

        Health -= damage;

        if (Health <= 0 && !IsDead)
        {
            IsDead = true;

            //shooter.GetComponent<KillCount>().KillPlus();
            //shooter.GetComponent<KillCount>().kills++;
           // Die();
        }
    }

    [TargetRpc]
    void TargetTakeDamage(int damage, GameObject shooter)
    {
        if (pikachuStar.bStarMode) { return; } //yup

        Health -= damage;

        if (Health <= 0 && !IsDead)
        {
            IsDead = true;

            //shooter.GetComponent<KillCount>().KillPlus();
            shooter.GetComponent<KillCount>().kills++;
            Die();
        }
    }

    public void Die()
    {
        //if (anim && !IsDead) 
        if (!IsDeadAnimate)
        {
            //GameOverManager.instance.PikachusLeft2--;
            //GetComponent<GamePlayers>().PikachusLeft2--;
            CmdDie();
            IsDead = true;
            IsDeadAnimate = true;

            PikachuDeathSound(); //needs command
            anim.SetTrigger("Death");

            StartCoroutine(RespawnDelay()); //REDO?

            GetComponent<CharacterController>().enabled = false;
            networkAnimator.SetTrigger("Death"); //Keep?
        }
    }

    [Command]
    void CmdDie()
    {
        IsDead = true;
        IsDeadAnimate = true;
        //GameOverManager.instance.PikachusLeft2--;
        RpcDie();
        anim.SetTrigger("Death");

    }
    [ClientRpc]
    void RpcDie()
    {
        IsDeadAnimate = true;
        IsDead = true;
        anim.SetTrigger("Death");

        TurnManager.instance.pikachusLeft--;
    }

    IEnumerator RespawnDelay() //Mario
    {
        yield return new WaitForSeconds(1.0f); //1.0!!!!!!!!!!!!!!!!!!!!!

       // StartCoroutine(PikachuMoveRespawn());

        yield return new WaitForSeconds(0.01f);

        CmdHealth();
        GetComponent<PlayerSwitch>().SwitchToMario();
    }

    [Command]
    void CmdHealth()
    {
        Health = 30;
        IsDead = false;
        IsDeadAnimate = false;

        RpcHealth();
    }

    [ClientRpc]
    void RpcHealth()
    {
        //yield return new WaitForSeconds(1.2f); //3.2
        Health = 30;
        IsDead = false;
        IsDeadAnimate = false;

        //Transform newPos = NetworkManager.singleton.GetStartPosition();
        //this.gameObject.transform.position = newPos.position;
        //this.gameObject.transform.rotation = newPos.rotation;

    }



    //IEnumerator PikachuMoveRespawn() //Moves when dies
    //{
    //    Transform StartPosition = NetworkManager.singleton.GetStartPosition();

    //    GetComponent<PlayerController_Platform>().bPikachuMove = false;
    //    yield return new WaitForSeconds(0.01f);

    //    //pikachuSpawn.GetComponent<PlayerController_Platform>().controller.Move(GetRandomStartPosition().position); GETS STUCK WHEN MOVES
    //    transform.position = StartPosition.position;
    //    //transform.rotation = StartPosition.rotation;

    //    yield return new WaitForSeconds(0.01f);
    //    GetComponent<PlayerController_Platform>().bPikachuMove = true;

    //}

    IEnumerator NameSpawn (GameObject playerSpawn) {
        yield return new WaitForSeconds (0.33f);
        playerSpawn.GetComponent<PlayerName> ().playerNameTag = GetComponent<PlayerName> ().playerNameTag; //move down?

        //NetworkServer.ReplacePlayerForConnection (connectionToClient, playerSpawn, true); //works on server, not client
        //NetworkServer.Destroy (gameObject);
    }


    public void PikachuDeathSound () //NEEDS COMMAND
    {
        CmdPikachuDeathSound();

        int ran = Random.Range (0, pikachuDieClips.Length);
        pikachuDie.clip = pikachuDieClips[ran];

        pikachuDie.Play ();
    }
    [Command]
    void CmdPikachuDeathSound()
    {
        int ran = Random.Range(0, pikachuDieClips.Length);
        pikachuDie.clip = pikachuDieClips[ran];

        pikachuDie.Play();

        RpcPikachuDeathSound();
    }
    [ClientRpc]
     void RpcPikachuDeathSound() //NEEDS COMMAND
    {
        int ran = Random.Range(0, pikachuDieClips.Length);
        pikachuDie.clip = pikachuDieClips[ran];

        pikachuDie.Play();
    }
}