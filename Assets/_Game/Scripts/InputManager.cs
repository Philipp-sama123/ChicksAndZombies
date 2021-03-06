using System;
using _Game.Scripts;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    // private LocomotionManager _locomotionManager; // ToDo: Think of Required Field 
    // CombatManager _combatManager; // ToDo: Think of Required Field 
    private PlayerManager _playerManager;
    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float horizontalInput;
    public float verticalInput;


    public bool actionInput;
    public bool dodgeInput;
    public bool sprintInput;
    public bool jumpInput;

    public bool isAiming;
    public bool primaryAttackInput;

    public bool right_trigger_input;
    public bool left_trigger_input;

    public bool right_button_hold_input; // todo think of sth better
    public bool left_button_hold_input; // todo think of sth better
    private CameraHandler _switchCanvas;

    //Todo: this can cause you Performance Problems (future us Problem) 
    private PickupManager[] _pickupManager;

    // private CameraManager _cameraManager;
    public float moveAmountAnimator = 0f;
    private AnimatorManager _animatorManager;
    private static readonly int MoveAmount = Animator.StringToHash("MoveAmount");
    private static readonly int IsAiming = Animator.StringToHash("IsAiming");

    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        _switchCanvas = FindObjectOfType<CameraHandler>(); // todo (!) better way
        _pickupManager = FindObjectsOfType<PickupManager>();
        _animatorManager = GetComponent<AnimatorManager>();
    }

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();

            _playerInput.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            _playerInput.PlayerMovement.Camera.performed +=
                i => cameraInput =
                    i.ReadValue<Vector2>(); // if you move the mouse or the right joystick it will then send it to the camera input // more explain needed

            _playerInput.PlayerMovement.Dodge.performed += i => dodgeInput = true;
            _playerInput.PlayerMovement.Dodge.canceled += i => dodgeInput = false;

            _playerInput.PlayerMovement.Sprint.performed += i => sprintInput = true;
            _playerInput.PlayerMovement.Sprint.canceled += i => sprintInput = false;

            _playerInput.PlayerActions.PrimaryAttack.performed +=
                i => primaryAttackInput = true; // set true when pressed 
            _playerInput.PlayerActions.PrimaryAttack.canceled += i => primaryAttackInput = false;

            _playerInput.PlayerActions.SecondaryAttack.performed += i => right_trigger_input = true;
            _playerInput.PlayerActions.SecondaryAttack.canceled += i => right_trigger_input = false;

            _playerInput.PlayerMovement.Jump.performed += i => jumpInput = true; // set true when pressed 
            _playerInput.PlayerMovement.Jump.canceled += i => jumpInput = false;

            _playerInput.PlayerActions.RB_Hold.performed += i => right_button_hold_input = true;
            _playerInput.PlayerActions.RB_Hold.canceled += i => right_button_hold_input = false;

            _playerInput.PlayerActions.LB_Hold.performed += i => left_button_hold_input = true;
            _playerInput.PlayerActions.LB_Hold.canceled += i => left_button_hold_input = false;

            _playerInput.PlayerActions.Action.performed += i => actionInput = true;
            _playerInput.PlayerActions.Action.canceled += i => actionInput = false;

            _playerInput.PlayerActions.Aiming.performed += i => isAiming = true;
            _playerInput.PlayerActions.Aiming.canceled += i => isAiming = false;
        }

        _playerInput.Enable();
    }

    private void Update()
    {
        HandleMovementInput(sprintInput);
        HandleJumpingInput();

        HandleAttackInput();
        HandleAimingInput();
        HandleActionInput();
    }

    private void HandleActionInput()
    {
        if (actionInput)
        {
            if (_pickupManager.Length > 0)
            {
                foreach (var pickupManager in _pickupManager)
                {
                    pickupManager.CheckForPlayerAndPickUpRifle();
                }
            }
        }
    }

    private void OnDisable()
    {
        //    playerControls.PlayerActions.LT.performed -= _ => HandleAimingInput(true);
        //    playerControls.PlayerActions.LT.canceled -= _ => HandleAimingInput(false);

        _playerInput.Disable();
    }


    private void HandleMovementInput(bool isSprinting)
    {
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
        moveAmountAnimator = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        _animatorManager.UpdateAnimatorMovementValues(moveAmountAnimator, isSprinting);

        cameraInputX = cameraInput.x; // take input from joystick and then pass it to move the camera 
        cameraInputY = cameraInput.y;
    }

    private void HandleJumpingInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            _playerManager.Jumping();
        }
    }

    private void HandleAttackInput()
    {
        if (primaryAttackInput)
        {
            _playerManager.HandleAttackInput();
        }
    }

    private void HandleAimingInput()
    {
        if (isAiming)
        {
            _animatorManager.animator.SetBool(IsAiming, true);
        }
        else
        {
            _animatorManager.animator.SetBool(IsAiming, false);
        }

        _switchCanvas.HandleAiming(isAiming);
    }
}