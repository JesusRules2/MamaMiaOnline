using UnityEngine;
using Mirror;

public abstract class FollowTarget : MonoBehaviour {
	
	//[SyncVar]
	[SerializeField] public Transform target;
	//[SyncVar]
	[SerializeField] private bool autoTargetPlayer = true;

    //virtual protected void Start()
    virtual protected void Start()
    {
        if (autoTargetPlayer)
		{
			FindTargetPlayer();
		}
	}

	//[Client]
	void FixedUpdate () 
	{
		//if (!hasAuthority) { return; }
	  if (autoTargetPlayer && (target == null || !target.gameObject.activeSelf))
		{
			FindTargetPlayer();
		}
		if (target != null && (target.GetComponent<Rigidbody>() != null && !target.GetComponent<Rigidbody>().isKinematic)) 
		{
			Follow(Time.deltaTime);
		}
	}

	protected abstract void Follow(float deltaTime);
	

	public void FindTargetPlayer()
	{
		//if (target == null)
		//{
		//	GameObject targetObj = GameObject.FindGameObjectWithTag("Player1");
		//	if(targetObj)
		//	{
		//		SetTarget(targetObj.transform);
		//	}
		//}
    }
	//[Client]
	public virtual void SetTarget(Transform newTransform)
	{
		target = newTransform;
	}
	public Transform Target{get {return this.target;}}
}
