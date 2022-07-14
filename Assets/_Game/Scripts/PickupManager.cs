using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
// TODO : ALARM!!

    [Header("Rifle's")] public GameObject PlayerRifle;
    public GameObject PickupRifle;
    [Header("Rifle Assign Things")] public PlayerManager player;
    public float radius = 2.5f;

    private void Awake()
    {
        PlayerRifle.SetActive(false);
    }

    
    // TODO: First when done -> sphere collider 
    public void CheckForPlayerAndPickUpRifle()
    {
        Debug.Log("PICKUP!!");
        Debug.Log(Vector3.Distance(transform.position, player.transform.position) );
        if (Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            PlayerRifle.SetActive(true);
            PickupRifle.SetActive(false);
            player.SetEquipWeapon(true); 
            // sound

            //complete
        }
    }
}