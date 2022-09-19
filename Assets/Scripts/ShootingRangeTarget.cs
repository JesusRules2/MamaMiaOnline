using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeTarget : MonoBehaviour
{
    public Animator anim;

    public void HitTarget()
    {
        Debug.Log("MARIO SHOT THIS!");
       //anim.SetBool("Down", true);
    }
}
