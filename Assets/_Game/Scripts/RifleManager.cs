using System;
using System.Collections;
using UnityEngine;

public class RifleManager : MonoBehaviour
{
    [Header(("Rifle Things"))] public Camera mainCamera;
    public float damage = 10f;
    public float shootingRange = 100f;
    public Animator rifleAnimator;

    public float fireCharge = 15f;
    public float nextTimeToShoot = 0f;

    [Header(("Rifle Effects"))] public ParticleSystem muzzleFlashEffect;
    public GameObject impactEffect;
    public GameObject bloodEffect;

    [Header(("Rifle Ammunation and Shooting"))]
    public int maximumAmmunition = 32;

    public int magazines = 10;
    public int presentAmmunition;
    public float reloadingTime = 1.3f;
    public bool isReloading = false;
    public PlayerManager player;

    public Transform hand; // TODO: rmv with Inverse Kinematics

    private void Awake()
    {
        rifleAnimator = GetComponent<Animator>(); 
        transform.SetParent(hand); // TODO: rmv with Inverse Kinematics
        mainCamera = Camera.main;
        presentAmmunition = maximumAmmunition;
    }



    public void Shooting()
    {
        if (isReloading)
        {
            return;
        }

        if (presentAmmunition <= 0)
        {
            StartCoroutine(Reloading());
            return;
        }

        // check for magazines 
        if (magazines == 0)
        {
            // show msg
            return;
        }

        presentAmmunition--;

        if (presentAmmunition == 0)
        {
            magazines--;
        }

        if (Time.time >= nextTimeToShoot)
        {
            nextTimeToShoot = Time.deltaTime + 1f / fireCharge;
            
            RaycastHit hitInfo;
            muzzleFlashEffect.Play();
            var mainCameraTransform = mainCamera.transform;
            if (Physics.Raycast(mainCameraTransform.position, mainCameraTransform.forward, out hitInfo, shootingRange))
            {
                DamageManager damageManager = hitInfo.transform.GetComponent<DamageManager>();
                ZombieManager zombieManager = hitInfo.transform.GetComponent<ZombieManager>();
                if (zombieManager != null)
                {
                    zombieManager.OnZombieDamage(damage);
                    GameObject impactGo = Instantiate(bloodEffect, hitInfo.point,
                        Quaternion.LookRotation(hitInfo.normal));
                    Destroy(impactGo, 1f);
                }
                if (damageManager != null)
                {
                    damageManager.OnDamage(damage);
                    GameObject impactGo = Instantiate(impactEffect, hitInfo.point,
                        Quaternion.LookRotation(hitInfo.normal));
                    Destroy(impactGo, 1f);
                }
            }
        }
    }

    IEnumerator Reloading()
    {
        player.playerSpeed = 0f;
        player.playerSprintSpeed = 0;
        player.PlayReloadAnimation();
        isReloading = true;
        Debug.Log("Reloading .... ");
        // play anim 
        // play reloadSound 
        // Todo: Wait for animation 
        yield return new WaitForSeconds(reloadingTime);
        presentAmmunition = maximumAmmunition;
        player.ResetPlayerSpeeds();
        isReloading = false;
    }
}