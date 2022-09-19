//OLD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    // [SyncVar]
    InputHandler ih;
    //[SyncVar]
    StateManager states;
    public Rigidbody rb;

    //[SyncVar]
    Vector3 lookPosition;
    //[SyncVar]
    Vector3 storeDirection;

    HealthScript marioHealth = null;

    //[SyncVar]
    public float runSpeed = 3.2f; //Opposite? LOOK AT PREFAB
    public float sprintSpeed = 2.3f; //Opposite? LOOK AT PREFAB
    public float walkSpeed = 1.5f; //1.5 LOOK AT PREFAB

    [SerializeField] float staminaMaxDuration = 3.2f;
    [SerializeField] float staminaRechargeDuration = 3.2f;
    public float stamina;
    private float rechargeStaminaRate;
    private bool sprintBool = true;

    //[SyncVar]
    public float aimSpeed = 1;
    //[SyncVar]
    public float speedMultiplier = 10;
    //[SyncVar]
    public float rotateSpeed = 2;
    //[SyncVar]
    public float turnSpeed = 5;

    public float jumpForce = 10f;
    public float gravityForce = -0.5f;

    [SyncVar]
    public bool bStopMovement = false;

    //[SyncVar]
    Vector3 v;
    //[SyncVar]
    Vector3 h;
    //[SyncVar]
    Quaternion targetRotation;

    //[SyncVar]
    float horizontal;
    //[SyncVar]
    float vertical;


    //[SyncVar]
    Vector3 lookDirection;

    //[SyncVar]
    PhysicMaterial zFriction;
    //[SyncVar]
    PhysicMaterial mFriction;
    public Collider col;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        ih = GetComponent<InputHandler>();
        //rb = GetComponentInChildren<Rigidbody>();
        states = GetComponent<StateManager>();
        //col = GetComponent<Collider>();
        marioHealth = GetComponent<HealthScript>();
        anim = GetComponent<HandleAnimations>().anim;

        zFriction = new PhysicMaterial();
        zFriction.dynamicFriction = 0;
        zFriction.staticFriction = 0;

        mFriction = new PhysicMaterial();
        mFriction.dynamicFriction = 1;
        mFriction.staticFriction = 1;

        //StartCoroutine(Update1());
        //StartCoroutine(Update2());
    }

    public override void OnStartClient()
    {
        if (!isLocalPlayer)
        {
            //Destroy(GetComponent<Rigidbody>());
        }

    }

    // Update is called once per frame

    ///IEnumerator Update2()
    void FixedUpdate()
    {
        //while (true)
        {
               if (!hasAuthority) { return; }
//             if (!hasAuthority)
//             {
//                 yield return new WaitForFixedUpdate();
//                 continue;
//             }

              if (bStopMovement) { return; } //Meant for BEGINNING Lobby
//             if (bStopMovement)
//             {
//                 yield return new WaitForFixedUpdate();
//                 continue;
//             }
             if (marioHealth.IsDead) { return; }
//             if (marioHealth.IsDead)
//             {
//                 yield return new WaitForFixedUpdate();
//                 continue;
//             }

            lookPosition = states.lookPosition;
            lookDirection = lookPosition - transform.position;

            //Handle movement
            horizontal = states.horizontal;
            vertical = states.vertical;

            bool onGround = states.onGround;

            if (horizontal != 0 || vertical != 0 || !onGround)
            {
                col.material = zFriction;
            }
            else
            {
                col.material = mFriction;
            }

            Vector3 v = ih.camTrans.forward * vertical;
            Vector3 h = ih.camTrans.right * horizontal;


            v.y = 0;
            h.y = 0;

            // HandleMovement(h, v, onGround);
            //CmdHandleRotation(h, v, onGround);


            //if (onGround)
            //{
            //    rb.drag = 4;
            //}
            //else
            //{
            //    rb.drag = 0;
            //}

            rb.drag = 4;

            // CmdHandleMovement(h, v, onGround);
            // CmdHandleRotation(h, v, onGround);

            rb.AddForce((v + h).normalized * speed()); //MOVEENT

            //ROTATION
            if (states.aiming)
            {
                //lookDirection += trasnform.right;//add offset if needed
                lookDirection.y = 0;

                targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotateSpeed);
            }
            else
            {
                storeDirection = transform.position + h + v;

                Vector3 dir = storeDirection - transform.position;
                dir.y = 0;

                if (horizontal != 0 || vertical != 0)
                {
                    float angl = Vector3.Angle(transform.forward, dir);

                    if (angl != 0)
                    {
                        float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));
                        if (angle != 0)
                        {
                            rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
                        }
                    }
                }
            }

            //yield return new WaitForFixedUpdate();
        }
    }

    //IEnumerator Update1()
    private void Update()
    {
        //while (true)
        {
            if (!hasAuthority) { return; }
//             if (!hasAuthority)
//             {
//                 yield return null;
//                 continue;
//             }

              if (GetComponent<InputHandler>().bPlayerLobby) { return; }
//             if (GetComponent<InputHandler>().bPlayerLobby)
//             {
//                 yield return null;
//                 continue;
//             }
            if (marioHealth.IsDead) { return; }
//             if (marioHealth.IsDead)
//             {
//                 yield return null;
//                 continue;
//             }

            bool onGround = states.onGround;

            if (Input.GetButtonDown("Jump") && onGround)
            {
              //  anim.SetTrigger("Jump");
                GetComponent<HandleAnimations>().JumpAnim();

                states.audioManager.CmdPlayJumpSound();
                states.audioManager.landed = false;
                states.audioManager.jumpTimer = 0f;

                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                //    rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
                //  rb.velocity += Vector3.up * jumpForce;
            }
            if (!onGround && rb.velocity.y < 11) //GRAVITY
            {
                rb.velocity = new Vector3(rb.velocity.x, gravityForce, rb.velocity.z); //push down
            }


            //SPRINTING RATES
            if (states.sprint && rb.velocity.magnitude > 0.01f && stamina > 0 && states.reloading == false && states.shoot == false) //if sprinting
            {
                stamina -= Time.deltaTime;
                rechargeStaminaRate = 0.0f;

                //Debug.Log(stamina);
            }

            if (states.sprint == false)
            {
                rechargeStaminaRate += Time.deltaTime;
            }

            if (rechargeStaminaRate >= staminaRechargeDuration)
            {
                rechargeStaminaRate = staminaRechargeDuration;
                stamina += Time.deltaTime;

                //Debug.Log(stamina);
            }
            if (stamina > staminaMaxDuration)
            {
                stamina = staminaMaxDuration;
            }
            if (stamina <= 0)
            {
                stamina = 0;
                sprintBool = false;
            }
            else
            {
                sprintBool = true;
            }
            //yield return null;

        }
    }

    void LateUpdate()
    {
        if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Reload"))
        {
            states.reloading = true;
        }
    }

    [Command]
    void CmdHandleMovement(Vector3 h, Vector3 v, bool onGround)
    {
        RpcHandleMovement(h, v, onGround);
    }

    [ClientRpc] //delete?
    void RpcHandleMovement(Vector3 h, Vector3 v, bool onGround)
    {
        if (onGround)
        {
            rb.AddForce((v + h).normalized * speed());
        }
    }

    float speed()
    {
        float speed = 0;

        if (states.aiming)
        {
            speed = aimSpeed;
        }
        else
        {
            if (states.walk || states.reloading)
            {
                //speed = walkSpeed; //2.5
                speed = aimSpeed + 2; //3
            }
            else
            {
                speed = runSpeed;
            }

            if (states.sprint && sprintBool && states.reloading == false)
            {
                speed = sprintSpeed;
            }
        }

        speed *= speedMultiplier;


        return speed;
    }
}






////NEW
//using System.Collections;
//using System.Collections.Generic;
//using Mirror;
//using UnityEngine;

//public class PlayerMovement : NetworkBehaviour
//{
//    // [SyncVar]
//    InputHandler ih;
//    //[SyncVar]
//    StateManager states;
//    Rigidbody rb;

//    //[SyncVar]
//    Vector3 lookPosition;
//    //[SyncVar]
//    Vector3 storeDirection;

//    //[SyncVar]
//    public float runSpeed = 3.2f; //Opposite? LOOK AT PREFAB
//    public float sprintSpeed = 2.3f; //Opposite? LOOK AT PREFAB
//    public float walkSpeed = 1.5f; //1.5 LOOK AT PREFAB

//    [SerializeField] float staminaMaxDuration = 3.2f;
//    [SerializeField] float staminaRechargeDuration = 3.2f;
//    public float stamina;
//    private float rechargeStaminaRate;
//    private bool sprintBool = true;

//    //[SyncVar]
//    public float aimSpeed = 1;
//    //[SyncVar]
//    public float speedMultiplier = 10; //10
//    //[SyncVar]
//    public float rotateSpeed = 2;
//    //[SyncVar]
//    public float turnSpeed = 5;

//    public float jumpForce = 10f;
//    public float gravityForce = -0.5f;

//    public bool bStopMovement = false;

//    //[SyncVar]
//    Vector3 v;
//    //[SyncVar]
//    Vector3 h;
//    //[SyncVar]
//    Quaternion targetRotation;

//    //[SyncVar]
//    float horizontal;
//    //[SyncVar]
//    float vertical;

//    //[SyncVar]
//    Vector3 lookDirection;

//    //[SyncVar]
//    PhysicMaterial zFriction;
//    //[SyncVar]
//    PhysicMaterial mFriction;
//    Collider col;

//    // Start is called before the first frame update
//    // public override void OnStartClient () {
//    //     ih = GetComponent<InputHandler> ();
//    //     rb = GetComponent<Rigidbody> ();
//    //     states = GetComponent<StateManager> ();
//    //     col = GetComponent<Collider> ();

//    //     zFriction = new PhysicMaterial ();
//    //     zFriction.dynamicFriction = 0;
//    //     zFriction.staticFriction = 0;

//    //     mFriction = new PhysicMaterial ();
//    //     mFriction.dynamicFriction = 1;
//    //     mFriction.staticFriction = 1;

//    //     StartCoroutine (UpdateClient ());
//    //     StartCoroutine (UpdateClientFixed ());
//    // }

//    public override void OnStartClient()
//    {
//        ih = GetComponent<InputHandler>();
//        rb = GetComponent<Rigidbody>();
//        states = GetComponent<StateManager>();
//        col = GetComponent<Collider>();

//        stamina = staminaMaxDuration;

//        if (isLocalPlayer)
//        {
//            Debug.Log($"Respawn isLocalPlayer: {isLocalPlayer}");

//            zFriction = new PhysicMaterial();
//            zFriction.dynamicFriction = 0;
//            zFriction.staticFriction = 0;

//            mFriction = new PhysicMaterial();
//            mFriction.dynamicFriction = 1;
//            mFriction.staticFriction = 1;

//            StartCoroutine(UpdateClient());
//            StartCoroutine(UpdateClientFixed());
//        }
//        else
//        {
//            rb.isKinematic = true;
//        }
//    }

//    //public override void OnStartAuthority()
//    //{
//    //    base.OnStartAuthority();

//    //    ih = GetComponent<InputHandler>();
//    //    rb = GetComponent<Rigidbody>();
//    //    states = GetComponent<StateManager>();
//    //    col = GetComponent<Collider>();

//    //    zFriction = new PhysicMaterial();
//    //    zFriction.dynamicFriction = 0;
//    //    zFriction.staticFriction = 0;

//    //    mFriction = new PhysicMaterial();
//    //    mFriction.dynamicFriction = 1;
//    //    mFriction.staticFriction = 1;

//    //}

//    // Update is called once per frame

//    //public override void OnStartClient()
//    // public override void OnStartAuthority()
//    // {
//    //StartCoroutine(UpdateClient());
//    // StartCoroutine(UpdateClientFixed());
//    // }

//    //void FixedUpdate()
//    [Client]
//    IEnumerator UpdateClientFixed() //FIXED UPDATE
//    {
//        while (true)
//        {
//            //Debug.Log("YEAAAA");

//            //if (!hasAuthority) { return; }
//            if (!hasAuthority)
//            {
//                yield return new WaitForFixedUpdate();
//                continue;
//            }

//            if (bStopMovement)
//            {
//                yield return new WaitForFixedUpdate();
//                continue;
//            }

//            lookPosition = states.lookPosition;
//            lookDirection = lookPosition - transform.position;

//            //Handle movement
//            horizontal = states.horizontal;
//            vertical = states.vertical;

//            bool onGround = states.onGround;

//            if (horizontal != 0 || vertical != 0 || !onGround)
//            {
//                col.material = zFriction;
//            }
//            else
//            {
//                col.material = mFriction;
//            }

//            Vector3 v = ih.camTrans.forward * vertical;
//            Vector3 h = ih.camTrans.right * horizontal;

//            v.y = 0;
//            h.y = 0;

//            // HandleMovement(h, v, onGround);
//            //CmdHandleRotation(h, v, onGround);

//            //if (onGround)
//            //{
//            //    rb.drag = 4;
//            //}
//            //else
//            //{
//            //    rb.drag = 0;
//            //}

//            rb.drag = 4;

//            // CmdHandleMovement(h, v, onGround);
//            // CmdHandleRotation(h, v, onGround);

//            rb.AddForce((v + h).normalized * speed()); //MOVEENT

//            //ROTATION
//            if (states.aiming)
//            {
//                //lookDirection += trasnform.right;//add offset if needed
//                lookDirection.y = 0;

//                targetRotation = Quaternion.LookRotation(lookDirection);
//                transform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotateSpeed);
//            }
//            else
//            {
//                storeDirection = transform.position + h + v;

//                Vector3 dir = storeDirection - transform.position;
//                dir.y = 0;

//                if (horizontal != 0 || vertical != 0)
//                {
//                    float angl = Vector3.Angle(transform.forward, dir);

//                    if (angl != 0)
//                    {
//                        float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));
//                        if (angle != 0)
//                        {
//                            rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
//                        }
//                    }
//                }
//            }

//            CmdUpdatePosRot(transform.position, transform.rotation);

//            yield return new WaitForFixedUpdate();
//        }
//    }

//    [Command]
//    void CmdUpdatePosRot(Vector3 position, Quaternion rotation)
//    {
//        RpcUpdatePosRot(position, rotation);
//    }

//    [ClientRpc]
//    void RpcUpdatePosRot(Vector3 position, Quaternion rotation)
//    {
//        if (!isLocalPlayer)
//        {
//            transform.position = position;
//            transform.rotation = rotation;
//        }
//    }

//    //void Update()
//    [Client] //NO ClientCallback
//    IEnumerator UpdateClient() ///////////////////NORMAL UPDATE
//    {
//        while (true)
//        {
//            if (netIdentity.hasAuthority)
//            {
//                //if (!hasAuthority) { return; }
//                if (!hasAuthority)
//                {
//                    yield return null;
//                    continue;
//                }

//                if (bStopMovement)
//                {
//                    yield return null;
//                    continue;
//                }


//                bool onGround = states.onGround;

//                if (Input.GetButtonDown("Jump") && onGround)
//                {
//                    states.audioManager.CmdPlayJumpSound();
//                    states.audioManager.landed = false;
//                    states.audioManager.jumpTimer = 0f;

//                    rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
//                    //    rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
//                    //  rb.velocity += Vector3.up * jumpForce;
//                }
//                if (!onGround && rb.velocity.y < 11) //GRAVITY
//                {
//                    rb.velocity = new Vector3(rb.velocity.x, gravityForce, rb.velocity.z); //push down
//                }


//                //SPRINTING RATES
//                if (states.sprint && rb.velocity.magnitude > 0.01f && stamina > 0) //if sprinting
//                {
//                    stamina -= Time.deltaTime;
//                    rechargeStaminaRate = 0.0f;

//                    Debug.Log(stamina);
//                }

//                if (states.sprint == false)
//                {
//                    rechargeStaminaRate += Time.deltaTime;
//                }

//                if (rechargeStaminaRate >= staminaRechargeDuration)
//                {
//                    rechargeStaminaRate = staminaRechargeDuration;
//                    stamina += Time.deltaTime;

//                    //Debug.Log(stamina);
//                }
//                if (stamina > staminaMaxDuration)
//                {
//                    stamina = staminaMaxDuration;
//                }
//                if (stamina <= 0)
//                {
//                    stamina = 0;
//                    sprintBool = false;
//                }
//                else
//                {
//                    sprintBool = true;
//                }
//            }

//            yield return null;

//        }

//    }

//    // [Command]
//    // void CmdHandleMovement (Vector3 h, Vector3 v, bool onGround) {
//    //     RpcHandleMovement (h, v, onGround);
//    // }

//    // [ClientRpc] //delete?
//    // void RpcHandleMovement (Vector3 h, Vector3 v, bool onGround) {
//    //     if (onGround) {
//    //         rb.AddForce ((v + h).normalized * speed ());
//    //     }
//    // }

//    //[Command]
//    //void CmdHandleRotation(Vector3 h, Vector3 v, bool onGround)
//    //{
//    //    //if (!hasAuthority) { return; }

//    //    if (states.aiming)
//    //    {
//    //        //lookDirection += trasnform.right;//add offset if needed
//    //        lookDirection.y = 0;

//    //        targetRotation = Quaternion.LookRotation(lookDirection);
//    //        transform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotateSpeed);
//    //    }
//    //    else
//    //    {
//    //        storeDirection = transform.position + h + v;

//    //        Vector3 dir = storeDirection - transform.position;
//    //        dir.y = 0;

//    //        if (horizontal != 0 || vertical != 0)
//    //        {
//    //            float angl = Vector3.Angle(transform.forward, dir);

//    //            if (angl != 0)
//    //            {
//    //                float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));
//    //                if (angle != 0)
//    //                {
//    //                    rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
//    //                }
//    //            }
//    //        }
//    //    }
//    //}

//    float speed()
//    {
//        float speed = 0;

//        if (states.aiming)
//        {
//            speed = aimSpeed;
//        }
//        else
//        {
//            if (states.walk || states.reloading)
//            {
//                speed = walkSpeed;
//            }
//            else
//            {
//                speed = runSpeed;
//            }

//            if (states.sprint && sprintBool && states.reloading == false)
//            {
//                speed = sprintSpeed;
//            }
//        }

//        speed *= speedMultiplier;

//        return speed;
//    }
//}