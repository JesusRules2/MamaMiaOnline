using UnityEngine;
using Mirror;

public class FreeCameraLook : Pivot {

	[SerializeField] private float moveSpeed = 5f;
    [SerializeField] public float turnSpeed = 1.5f; //1.5f
    [SerializeField] private float turnsmoothing = .1f;
    [SerializeField] private float tiltMax = 82f; //75
    [SerializeField] private float tiltMin = 32f; //45
    [SerializeField] private bool lockCursor = false;


    //[SyncVar]
    private float lookAngle;
   // [SyncVar]
    private float tiltAngle;

    private const float LookDistance = 100f;

    private float smoothX = 0;
    private float smoothY = 0;
    private float smoothXvelocity = 0;
    private float smoothYvelocity = 0;

    public float crosshairOffsetWiggle = 0.2f;

    CrosshairManager crosshairManager;

    //add the singleton
    public static FreeCameraLook instance;
    
    public static FreeCameraLook GetInstance()
    {
        return instance;
    }

	protected override void Awake()
	{
        instance = this;

		base.Awake();

        //if (!hasAuthority) { return; }
		cam = GetComponentInChildren<Camera>().transform;
		pivot = cam.parent.parent; //take the correct pivot

        //DontDestroyOnLoad(this.gameObject);
    }

    protected override void Start()
    {
        base.Start();

        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;

        crosshairManager = CrosshairManager.GetInstance();
    }

    // Update is called once per frame
    //[Client]
    protected override	void Update ()
	{
		base.Update();

      //  if (!hasAuthority) { return; }
        //if (!isLocalPlayer) { return; }

		HandleRotationMovement();

	}

    //[Client]
	protected override void Follow (float deltaTime)
	{
        //if (!isLocalPlayer) { return; } //delete maybs
		transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);

	}

    //[Client]
    void HandleRotationMovement()
    {
        HandleOffsets();

        float x = Input.GetAxis("Mouse X") + offsetX;
        float y = Input.GetAxis("Mouse Y") + offsetY;

        if (turnsmoothing > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothXvelocity, turnsmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothYvelocity, turnsmoothing);
        }
        else
        {
            smoothX = x;
            smoothY = y;
        }

        lookAngle += smoothX * turnSpeed;

        transform.rotation = Quaternion.Euler(0f, lookAngle, 0);

        tiltAngle -= smoothY * turnSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);

        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

        if (x > crosshairOffsetWiggle || x < -crosshairOffsetWiggle || y > crosshairOffsetWiggle || y < -crosshairOffsetWiggle)
        {
            WiggleCrosshairAndCamera(0);
        }
    }

    //[SyncVar]
    float offsetX;
    //[SyncVar]
    float offsetY;


    void HandleOffsets()
    {
        if (offsetX != 0)
        {
            offsetX = Mathf.MoveTowards(offsetX, 0, Time.deltaTime);
        }

        if (offsetY != 0)
        {
            offsetY = Mathf.MoveTowards(offsetY, 0, Time.deltaTime);
        }
    }

    public void WiggleCrosshairAndCamera(float kickback)
    { 
       // crosshairManager.activeCrosshair.WiggleCrosshair();

        //offsetY = kickback;
    }


}
