using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectCollide : MonoBehaviour
{
    HealthScript healthScript = null;

    private void Start()
    {
        healthScript = GetComponent<HealthScript>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "HitEffect")
        {
            healthScript.TakeDamage(10);
        }
    }
}
