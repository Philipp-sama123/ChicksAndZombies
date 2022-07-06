using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Movement")] public float playerSpeed = 1.75f;
    public float playerSprintSpeed = 3f;

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


    // ToDo: Think of Required Fields
    private Animator _animator;
    private InputManager _inputManager;


    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int IdleAim = Animator.StringToHash("IdleAim");
    private static readonly int Jump = Animator.StringToHash("Jump");


    private void Awake()
    {
        if (Camera.main != null) playerCamera = Camera.main.transform;

        _animator = GetComponent<Animator>();
        _inputManager = GetComponent<InputManager>();
        characterController = GetComponent<CharacterController>();
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

        Moving();
    }

    private void Moving()
    {
        float horizontal = _inputManager.horizontalInput;
        float vertical = _inputManager.verticalInput;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            _animator.SetBool(Idle, false);
            _animator.SetBool(Walk, true);
            _animator.SetBool(IdleAim, false);

            if (!_inputManager.sprintInput)
                _animator.SetBool(Running, false);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            characterController.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);
        }
        else
        {
            _animator.SetBool(Idle, true);
            _animator.SetBool(Walk, false);
            if (!_inputManager.sprintInput)
                _animator.SetBool(Running, false);
        }
    }

    public void Jumping()
    {
        _animator.SetBool(Idle, false);
        _animator.SetTrigger(Jump);
        _velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
    }

    public void Sprinting()
    {
        float horizontal = _inputManager.horizontalInput;
        float vertical = _inputManager.verticalInput;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            _animator.SetBool(Walk, false);
            _animator.SetBool(Running, true);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDirection.normalized * playerSprintSpeed * Time.deltaTime);
        }
        else
        {
            _animator.SetBool(Walk, true);
            _animator.SetBool(Running, false);
        }
    }
}