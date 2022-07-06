using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public float health = 100f;

    public void OnDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
