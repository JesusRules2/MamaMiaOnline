using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Platform : NetworkBehaviour
{
    [SyncVar]
    public float moveSpeed;
    public float moveSpeedMax = 17;
    //  public Rigidbody theRB;
    public float jumpForce;
   
    public CharacterController controller;

    [SyncVar]
    private Vector3 moveDirection;
    public float gravityScale;

    public Animator anim;

    [SerializeField]
    private NetworkAnimator networkAnimator = null;
    public Animator animator = null;
    public HealthScriptPikachu pikachuHealth = null;
    public PickupStarPikachu starPikachu;

   // [SyncVar]
    public Transform pivot;
    public float rotateSpeed;
    public GameObject playerModel; //REMEMBER TO USE FOR ANIMATOR CHARACTERS!

    bool bJumpAgain = true; //landed
    bool bJumpAgainTimer = true; //timer
    bool bResetJump = true;
    float JumpAgainTimer = 0f;

    //Attempt camera fix for Pikachu Client
    public Transform initialPikachuTransform;

    float fallTimer;

    [SerializeField] Camera pikachuCamera;

    [SyncVar]
    public bool bPikachuMove = true; //meant for respawning Pikachus, KEEEEP ON

    //public override void OnStartClient()
    public override void OnStartClient()
    {
        //StartCoroutine(UpdateClient());
        
        controller = GetComponent<CharacterController>();
        pikachuHealth = GetComponent<HealthScriptPikachu>();
        starPikachu = GetComponent<PickupStarPikachu>();
        moveSpeed = moveSpeedMax;

        // if (!isLocalPlayer)
        // {
        //     //Destroy(GetComponent<Rigidbody>());
        //     // GetComponent<Rigidbody>().isKinematic = false;
        //     GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        // }

    }


    void Start()
    {
        //StartCoroutine(UpdateClient());

        initialPikachuTransform = gameObject.transform; //2

         if (!isLocalPlayer)
        {
            //Destroy(GetComponent<Rigidbody>());
            // GetComponent<Rigidbody>().isKinematic = false; NOTHING
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        }

    }

    public void ResetPlayer()
    {
        //gameObject.transform.position = initialPikachuTransform.position;
        //gameObject.transform.rotation = initialPikachuTransform.rotation;

        //transform.rotation = Quaternion.LookRotation(-pikachuCamera.transform.forward, pikachuCamera.transform.up);
        // transform.Rotate(0.0f, -90.0f, 0.0f);

        pivot.position = new Vector3(0.0f, 0.0f, 0.0f);
        pivot.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    //IEnumerator UpdateClient()
    //[Client]
    void Update()
    {
        //while(true)
        {
            if (!isLocalPlayer)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }

            if (!isLocalPlayer) { return; }

            //if (!hasAuthority || pikachuHealth.IsDead)
            //{
            //    yield return null;
            //    continue;
            //}


            float yStore = moveDirection.y;

            //if (pikachuHealth.IsDead == false)
            {
                moveDirection = (-transform.forward * Input.GetAxisRaw("Vertical")) +
                    (-transform.right * Input.GetAxisRaw("Horizontal"));
            }

            moveDirection = moveDirection.normalized * moveSpeed;
            moveDirection.y = yStore;


            if (controller.isGrounded)
            {
                moveDirection.y = 0.0f;
                JumpAgainTimer += Time.deltaTime;
            }

            // JUMP CODE
            //if (Input.GetButtonDown("Jump") && bJumpAgain && pikachuHealth.IsDead == false)
            if (Input.GetButtonDown("Jump") && bJumpAgain)
            {
                moveDirection.y = jumpForce;

                bJumpAgain = false;
                JumpAgainTimer = 0f;
            }

            if (JumpAgainTimer > 0.12f && bJumpAgain == false)
            {
                bJumpAgain = true;
                JumpAgainTimer = 0f;
            }

            if (bPikachuMove) //meant for respawning - turns on to move and OFF to respawn
            {
                moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime); //HERE
                controller.Move(moveDirection * Time.deltaTime);
            }

            if (pivot == null) //not this
            {
                //pivot = transform.Find("Main Camera (1)/PivotPikachu");
                pivot = GameObject.FindGameObjectWithTag("PivotPikachu").transform;
            }

            //Move the player in different directions based on camera look direction
            //if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && pivot != null && pikachuHealth.IsDead == false) //IF DEAD, DONT MOVE
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && pivot != null) //IF DEAD, DONT MOVE
            {
                transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
            }


            if (!controller.isGrounded && fallTimer > 0.09f)
            {
                animator.SetBool("Jump", true);
                networkAnimator.SetTrigger("Jump");
                animator.SetTrigger("Jump");

            }

            ////if in air longer then a second, then do anim change
            if (!controller.isGrounded)
            {
                fallTimer += Time.deltaTime;
            }
            else
            {
                fallTimer = 0;
            }

            anim.SetBool("isGrounded", controller.isGrounded);
            anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal"))));

            if (starPikachu.bStarMode)
            {
                anim.SetBool("StarPower", true);
                //anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal"))) * 3.2f); //*2 speed
            }
            else
            {
                anim.SetBool("StarPower", false);
            }

            //yield return null;
        }
    }

    [Command]
    void CmdMove()
    {
        RpcMove();
        return;
        //  if (!hasAuthority) { return; }

        //theRB.velocity = new Vector3(Input.GetAxis("Horizontal") * moveSpeed,
        //    theRB.velocity.y, Input.GetAxis("Vertical") * moveSpeed); 

        //if (Input.GetButtonDown("Jump"))
        //{
        //    theRB.velocity = new Vector3(theRB.velocity.x, jumpForce, theRB.velocity.z);
        //}

        //  moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, moveDirection.y,
        //Input.GetAxis("Vertical") * moveSpeed);

        float yStore = moveDirection.y;
        moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) +
            (transform.right * Input.GetAxisRaw("Horizontal"));
        moveDirection = moveDirection.normalized * moveSpeed;
        moveDirection.y = yStore;

        if (controller.isGrounded)
        {
            moveDirection.y = 0.0f;
            bJumpAgain = true;
            //if (Input.GetButtonDown("Jump") && bJumpAgain)
            //{
            //    moveDirection.y = jumpForce;
            //    bJumpAgain = false;
            //}
        }
        if (Input.GetButtonDown("Jump") && bJumpAgain)
        {
            moveDirection.y = jumpForce;
            bJumpAgain = false;
        }

        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);

        //Move the player in different directions based on camera look direction
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && pivot != null)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        //if (!controller.isGrounded && controller.velocity.y < 32)
        //{
        //    anim.SetBool("isFalling", true); //change to network animator
        //}
        //if (!controller.isGrounded && controller.velocity.y > 1)
        //{
        //    anim.SetBool("isFalling", false);
        //}

        if (!controller.isGrounded && fallTimer > 0.09f)
        {
            animator.SetBool("Jump", true);
            networkAnimator.SetTrigger("Jump");
            animator.SetTrigger("Jump");

        }

        ////if in air longer then a second, then do anim change
        if (!controller.isGrounded)
        {
            fallTimer += Time.deltaTime;
        }
        else
        {
            fallTimer = 0;
        }

        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal"))));
    }

    [ClientRpc]
    void RpcMove()
    {
        float yStore = moveDirection.y;
        moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) +
            (transform.right * Input.GetAxisRaw("Horizontal"));
        moveDirection = moveDirection.normalized * moveSpeed;
        moveDirection.y = yStore;

        if (controller.isGrounded)
        {
            moveDirection.y = 0.0f;
            bJumpAgain = true;
            //if (Input.GetButtonDown("Jump") && bJumpAgain)
            //{
            //    moveDirection.y = jumpForce;
            //    bJumpAgain = false;
            //}
        }
        if (Input.GetButtonDown("Jump") && bJumpAgain)
        {
            moveDirection.y = jumpForce;
            bJumpAgain = false;
        }

        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);

        //Move the player in different directions based on camera look direction
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && pivot != null)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        //if (!controller.isGrounded && controller.velocity.y < 32)
        //{
        //    anim.SetBool("isFalling", true); //change to network animator
        //}
        //if (!controller.isGrounded && controller.velocity.y > 1)
        //{
        //    anim.SetBool("isFalling", false);
        //}

        if (!controller.isGrounded && fallTimer > 0.09f)
        {
            animator.SetBool("Jump", true);
            networkAnimator.SetTrigger("Jump");
            animator.SetTrigger("Jump");
        }

        ////if in air longer then a second, then do anim change
        if (!controller.isGrounded)
        {
            fallTimer += Time.deltaTime;
        }
        else
        {
            fallTimer = 0;
        }

        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal"))));
    }
}
