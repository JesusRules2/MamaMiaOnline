using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HandleAnimations : NetworkBehaviour {
    [SerializeField]
    private NetworkAnimator networkAnimator = null;
    public Animator anim;

    public PlayerMovement playerMovement;
    public Rigidbody rb;

  //  [SyncVar]
    public StateManager states;
    Vector3 lookDirection;

   // [SyncVar]
    float movement;
  //  [SyncVar]
    float vertical;
  //  [SyncVar]
    float horizontal;
  //  [SyncVar]
    bool onGround;
    string[] joysticks;

    //public override void OnStartLocalPlayer () {
     void Start() {
        SetupAnimator ();
        //states = GetComponentInParent<StateManager> ();
        //playerMovement = GetComponentInParent<PlayerMovement>();
        //rb = GetComponent<Rigidbody> ();

    }


    //IEnumerator UpdateClient () //DUNNO
   // [Client]
    void FixedUpdate()
    {
        //while (true) 
        {
            //if (netIdentity.hasAuthority) 
            {
                if (!hasAuthority) { return; }


                if (states == null)
                {
                    states = GetComponent<StateManager>();
                }

                states.reloading = anim.GetBool ("Reloading");
                anim.SetBool ("Aim", states.aiming);


                // joysticks = Input.GetJoystickNames();
                // if (joysticks.Length != 0) { //joystick!

                //   horizontal = Input.GetAxis ("Horizontal");
                //   vertical = Input.GetAxis("Vertical");
                // }
                // else {
                //     horizontal = Input.GetAxisRaw ("Horizontal");
                //     vertical = Input.GetAxisRaw("Vertical");
                // }
                horizontal = Input.GetAxisRaw ("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");


                if (!states.canRun) {
                    anim.SetFloat ("Forward", vertical, 0.1f, Time.deltaTime);
                    anim.SetFloat ("Sideways", horizontal, 0.1f, Time.deltaTime);

                } 
                else 
                {
                     movement = Mathf.Abs (vertical) + Mathf.Abs (states.horizontal);

                    bool walk = states.walk;

                    //movement = Mathf.Clamp (movement, 0, (walk || states.reloading) ? 0.5f : 1); //regular
                    //movement = Mathf.Clamp(movement, 0, (walk || states.reloading) ? 0.333f : 0.666f);
                    movement = Mathf.Clamp(movement, 0, (walk || states.reloading) ? 0.385f : 0.666f);

                    if (states.reloading == true)
                    {
                        if (movement >= 0.385f)
                        {
                            movement = 0.385f;
                        }
                    }

                    anim.SetFloat ("Forward", movement, 0.1f, Time.deltaTime);

                    if (states.sprint && movement > 0.1f && playerMovement.stamina > 0 && states.reloading == false)
                    {
                        anim.SetFloat("Forward", 1.0f, 0.1f, Time.deltaTime);
                    }

                }

            }
           // yield return new WaitForFixedUpdate ();

        }
    }

    void LateUpdate()
    {
        if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Reload"))
        {
            states.reloading = true;
        }
    }

    //void UpdateClient2()
    //IEnumerator UpdateClient2 () //DUNNO
    void Update()
    {
        //while (true) 
        {
            if (!hasAuthority) { return; }

            if (states == null)
            {
                states = GetComponent<StateManager>();
            }

            onGround = states.onGround;


            if (!onGround && rb.velocity.y < 11) {
                anim.SetBool ("isJumping", false); //change to network animator
            }
            if (!onGround && rb.velocity.y > 1) {
                anim.SetBool ("isJumping", true);
            }

            anim.SetBool ("isGrounded", onGround);

            // yield return null;

        }

    }


    void SetupAnimator () 
    {
        Animator[] anims = GetComponentsInChildren<Animator>();

        return;

        for (int i = 0; i < anims.Length; i++) {
            if (anims[i] != anim) {
                anim.avatar = anims[i].avatar;
                Destroy (anims[i]);
                break;
            }
        }
    }

  //  [Client]
    public void StartReload () 
    {
        if (!states.reloading)
        {
            anim.SetTrigger("Reload");
            networkAnimator.SetTrigger("Reload");
        }
        RpcStartReload();
    }
    [Command]
    void CmdStartReload()
    {
        if (!states.reloading)
        {
            anim.SetTrigger("Reload");
            networkAnimator.SetTrigger("Reload");
        }
        RpcStartReload();
    }
    [ClientRpc]
    void RpcStartReload()
    {
        if (!states.reloading)
        {
            anim.SetTrigger("Reload");
            networkAnimator.SetTrigger("Reload");
        }
    }


  //  [Client]
    public void JumpAnim()
    {
       // anim.SetTrigger("Jump");
       // networkAnimator.SetTrigger("Jump");

        //CmdJump();
        RpcJump();
    }
    //JUMPING
    [Command]
    public void CmdJump()
    {
       // anim.SetTrigger("Jump");
       // networkAnimator.SetTrigger("Jump");

        RpcJump();
    }
    [ClientRpc]
    void RpcJump()
    {
        anim.SetTrigger("Jump");
        networkAnimator.SetTrigger("Jump");

    }
}