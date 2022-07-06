using System.Collections;
using UnityEngine;

public class RifleManager : MonoBehaviour
{
    [Header(("Rifle Things"))] public Camera mainCamera;
    public float damage = 10f;
    public float shootingRange = 100f;

    [Header(("Rifle Effects"))] public ParticleSystem muzzleFlashEffect;


    private void Awake()
    {
        mainCamera = Camera.main;
    }


    public void Shooting()
    {
        RaycastHit hitInfo;
        muzzleFlashEffect.Play(); 
        var mainCameraTransform = mainCamera.transform;
        if (Physics.Raycast(mainCameraTransform.position, mainCameraTransform.forward, out hitInfo, shootingRange))
        {
            Debug.Log("Hit");
            Debug.Log(hitInfo.transform.name);

            DamageManager damageManager = hitInfo.transform.GetComponent<DamageManager>();
            if (damageManager != null)
            {
                damageManager.OnDamage(damage);
            }
        }
    }

}