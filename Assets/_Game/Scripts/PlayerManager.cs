using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;


public class PlayerManager : MonoBehaviour
{
    private static readonly float InitPlayerSpeed = 1.75f;
    private static readonly float InitPlayerSprintSpeed = 3f;


    [Header("Player Movement")] public float playerSpeed = InitPlayerSpeed;
    public float playerSprintSpeed = InitPlayerSprintSpeed;

    [Header("Player Health")] public float initHealth = 100f;
    public float currentHealth;

    [Header("Player Script Camera")] public Transform playerCamera;

    [Header("Player Animator and Gravity")]
    public CharacterController characterController;

    public float gravity = -9.81f;


    [Header("Player Jumping and velocity")]
    public float turnTime = .1f;

    public float turnVelocity;
    public float jumpRange = 1f;
    private Vector3 _velocity;
    public Transform surfaceCheck;
    private bool _onSurface;
    public float surfaceDistance;
    public LayerMask surfaceMask;

    public bool isGrounded;

    // ToDo: Think of Required Fields
    private Animator _animator;
    private InputManager _inputManager;
    private PlayerCombatManager _playerCombatManager;
    private RifleManager _rifleManager;

    [Header("Player Combat Stuff")] public float playerPunchRadius;
    private float _nextTimeToPunch = 0f;
    public float punchCharge = 15f;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int IdleAim = Animator.StringToHash("IdleAim");
    private static readonly int Punch = Animator.StringToHash("Punch");
    private static readonly int Reloading = Animator.StringToHash("Reloading");
    private static readonly int FireWalk = Animator.StringToHash("FireWalk");
    private static readonly int Fire = Animator.StringToHash("Fire");
    private static readonly int IsEquipped = Animator.StringToHash("IsEquipped");
    private bool _previousAttack =false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentHealth = initHealth;
    }

    private void Awake()
    {
        if (Camera.main != null) playerCamera = Camera.main.transform;

        _animator = GetComponent<Animator>();
        _inputManager = GetComponent<InputManager>();
        _playerCombatManager = GetComponent<PlayerCombatManager>();
        characterController = GetComponent<CharacterController>();

        playerPunchRadius = PlayerCombatManager.GetPlayerCombatRadius();
        _rifleManager = GetComponentInChildren<RifleManager>();
    }

    private void Update()
    {
        _onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);

        if (_onSurface && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;
        characterController.Move(_velocity * Time.deltaTime);

        if (_inputManager.sprintInput)
        {
            Sprinting();
        }
        else
        {
            Moving();
        }
    }

    public void ResetPlayerSpeeds()
    {
        playerSpeed = InitPlayerSpeed;
        playerSprintSpeed = InitPlayerSprintSpeed;
    }

    private void Moving()
    {
        float horizontal = _inputManager.horizontalInput;
        float vertical = _inputManager.verticalInput;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            characterController.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);
        }
    }


    public void Jumping()
    {
        _animator.CrossFade("Jump", 0.2f);
        _velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
    }

    private void Sprinting()
    {
        float horizontal = _inputManager.horizontalInput;
        float vertical = _inputManager.verticalInput;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDirection.normalized * playerSprintSpeed * Time.deltaTime);
        }
    }


    public void HandleAttackInput()
    {
        _rifleManager = GetComponentInChildren<RifleManager>();
        if (_rifleManager == null)
        {
            if (!_previousAttack)
                Punching();
        }
        else
        {
            _rifleManager.Shooting();
            StartCoroutine(
                ResetAnimationsAfterSeconds());
        }
    }


    private void Punching()
    {
        _previousAttack = true;
        _animator.CrossFade("Punch", .2f);
        _playerCombatManager.Punching();
        StartCoroutine(
            ResetAnimationsAfterSeconds());
    }

    public void PlayReloadAnimation()
    {
        _animator.CrossFade("Reload", .2f);
    }

    public IEnumerator ResetAnimationsAfterSeconds()
    {
        yield return new WaitForSeconds(1f);
        _animator.SetBool(Punch, false);
        _animator.SetBool(Reloading, false);
        _animator.SetBool(Idle, true);
        _previousAttack = false;
    }

    public void SetEquipWeapon(bool isEquipped)
    {
        _animator.SetBool(IsEquipped, isEquipped);
    }

    //Todo: Extract to base class --> Damageable
    public void OnPlayerDamage(float takeDamage)
    {
        currentHealth -= takeDamage;
        if (currentHealth <= 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        Cursor.lockState = CursorLockMode.None;
        Destroy(gameObject, 1.0f);
    }
}