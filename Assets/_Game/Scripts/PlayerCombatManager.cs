using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    public const float InitPlayerPunchingRadius = 10f;

    [Header("Player Combat Variables")] public Camera mainCameraTransform;
    public float damage = 10f;
    public GameObject impactEffect;
    public Animator animator; 
    public void Awake()
    {
        mainCameraTransform = Camera.main;
        animator = GetComponent<Animator>();
    }

    public void Punching()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(mainCameraTransform.transform.position, mainCameraTransform.transform.forward, out hitInfo,
                InitPlayerPunchingRadius))
        {
            Debug.LogWarning("Hit Punching");
            Debug.LogWarning(hitInfo.transform.name);
            DamageManager damageManager = hitInfo.transform.GetComponent<DamageManager>();
            if (damageManager != null)
            {
                damageManager.OnDamage(damage);
                GameObject impactGo = Instantiate(impactEffect, hitInfo.point,
                    Quaternion.LookRotation(hitInfo.normal));
                Destroy(impactGo, 1f);
            }
        }
    }

    public static float GetPlayerCombatRadius()
    {
        return InitPlayerPunchingRadius;
    }
}