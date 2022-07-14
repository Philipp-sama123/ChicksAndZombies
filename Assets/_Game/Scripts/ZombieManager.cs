using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ZombieManager : MonoBehaviour
{
    [Header("Zombie things")] public LayerMask playerLayer;
    public Transform lookPoint;
    public Transform player;
    public Camera attackingRaycastArea;
    public NavMeshAgent zombieAgent;

    [Header("Zombie Health & Damage")] public float giveDamage = 5f;
    public float currentHealth;
    public float initHealth = 100f;

    [Header("Zombie Attacking Variables")] public float timeBetweenAttacks;
    public bool previousAttack;
    public GameObject bloodEffect;
    [Header("Zombie Walking")] public GameObject[] wayPoints;
    public float zombieSpeed = 1f;
    public float zombieRunningSpeed = 3f;
    private float _waypointRadius = 2f;
    private int _currentZombiePosition = 0;

    [Header("Zombie Mood/States")] public float visionRadius;
    public float attackingRadius;
    public bool isPlayerInVisionRadius;
    public bool isPlayerInAttackingRadius;

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentHealth = initHealth;
    }

    private void Update()
    {
        var position = transform.position;
        isPlayerInVisionRadius = Physics.CheckSphere(position, visionRadius, playerLayer);
        isPlayerInAttackingRadius = Physics.CheckSphere(position, attackingRadius, playerLayer);

        if (!isPlayerInVisionRadius && !isPlayerInAttackingRadius)
        {
            Guard();
        }

        if (isPlayerInVisionRadius && !isPlayerInAttackingRadius)
        {
            RunTowardsPlayer();
        }

        if (isPlayerInVisionRadius && isPlayerInAttackingRadius)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        zombieAgent.SetDestination(transform.position);
        transform.LookAt(lookPoint);
        if (!previousAttack)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(attackingRaycastArea.transform.position, attackingRaycastArea.transform.forward,
                    out hitInfo, attackingRadius))
            {
                Debug.Log("Attacking");

                Animator animator = GetComponent<Animator>();
                animator.CrossFade("Attack", 0.2f);

                PlayerManager playerManager = hitInfo.transform.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.OnPlayerDamage(giveDamage);
                    GameObject impactGo = Instantiate(bloodEffect, hitInfo.point,
                        Quaternion.LookRotation(hitInfo.normal));
                    Destroy(impactGo, 1f);
                }
            }

            previousAttack = true;
            Invoke(nameof(ActiveAttacking), timeBetweenAttacks); // Todo also for player like this 
        }
    }

    private void ActiveAttacking()
    {
        previousAttack = false;
    }

    private void RunTowardsPlayer()
    {
        var playerPosition = player.position;
        
        Animator animator = GetComponent<Animator>();
        animator.SetFloat("MoveAmount", zombieSpeed*2);
        
        zombieAgent.SetDestination(playerPosition);
        transform.LookAt(playerPosition);
    }

    private void Guard()
    {
        if (Vector3.Distance(wayPoints[_currentZombiePosition].transform.position, transform.position) <
            _waypointRadius)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetFloat("MoveAmount", zombieSpeed);
            
            _currentZombiePosition = Random.Range(0, wayPoints.Length);
            if (_currentZombiePosition >= wayPoints.Length)
            {
                _currentZombiePosition = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position,
            wayPoints[_currentZombiePosition].transform.position, Time.deltaTime * zombieSpeed);
        transform.LookAt(wayPoints[_currentZombiePosition].transform.position);
    }

    public void OnZombieDamage(float takeDamage)
    {
        currentHealth -= takeDamage;
        if (currentHealth <= 0)
        {
            ZombieDeath();
        }
    }

    private void ZombieDeath()
    {
        zombieAgent.SetDestination(transform.position);
        zombieSpeed = 0f;
        attackingRadius = 0f;
        visionRadius = 0f;
        isPlayerInAttackingRadius = false;
        isPlayerInVisionRadius = false;

        Animator animator = GetComponent<Animator>();
        animator.CrossFade("Death Fall Forward", .2f);
        Destroy(gameObject, 5f);
    }
}