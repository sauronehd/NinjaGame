using UnityEngine;
using System.Collections.Generic;
using Alteruna;

using AlterunaAvatar = Alteruna.Avatar;
using UnityAvatar = UnityEngine.Avatar;

// Base class for all player states
public abstract class PlayerBaseState
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Tick(float deltaTime);
    public abstract void Exit();
}

// The main state machine component
public class PlayerStateMachine : MonoBehaviour
{
    public bool wasGroundedLastFrame = true;
    public Transform PlayerTransform ;  // Reference to the player's transform

    // --- Coyote time (grounded grace period) ---
    private float jumpGroundedGraceTimer = 0f;
    private const float jumpGroundedGraceDuration = 0.10f; // 0.1 seconds of grace after jumping
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5f; // Example speed
    [field: SerializeField] public float WallJumpForce { get; private set; } = 7.5f;
    [field: SerializeField] public int MaxJumps { get; private set; } = 2; // 1 = no double jump, 2 = double jump
    public int JumpsRemaining { get; set; }

    [Header("Collider Settings")]
    [SerializeField] private CapsuleCollider2D playerCollider; // Assign in Inspector
    [SerializeField] private Vector2 standingColliderSize = new Vector2(1f, 2f); // Example
    [SerializeField] private Vector2 standingColliderOffset = new Vector2(0f, 0f); // Example
    [SerializeField] private Vector2 crouchingColliderSize = new Vector2(1f, 1f); // Example
    [SerializeField] private Vector2 crouchingColliderOffset = new Vector2(0f, -0.5f); // Example
    [SerializeField] private float standUpCheckDistance = 0.1f; // Distance above collider to check
    [SerializeField] private LayerMask groundLayer; // Assign layers considered ground/obstacles

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint; // Assign an empty GameObject childed to the player at their feet
    [SerializeField] private float groundCheckRadius = 0.2f; // Adjust radius as needed

    [Header("Crouch Settings")]
    [SerializeField] public float CrouchSpeedMultiplier { get; private set; } = 0.25f; // Half of WalkState's 0.5 multiplier


    private PlayerBaseState currentState;

    // State registry for extensibility
    private Dictionary<string, PlayerBaseState> stateRegistry = new Dictionary<string, PlayerBaseState>();

    // Concrete states
    public PlayerIdleState IdleState { get; private set; } // Add IdleState property back

    public WalkState WalkState { get; private set; }
    public RunState RunState { get; private set; }
    public JumpState JumpState { get; private set; }
    public CrouchState CrouchState { get; private set; }
    public SlideState SlideState { get; private set; }
    public WallClingState WallClingState { get; private set; }
    public AttackState AttackState { get; private set; } // Add AttackState property back
    public ShootState ShootState { get; private set; } // Add ShootState declaration
    public FallState FallState { get; private set; } // Add FallState declaration
    public AerialAttackState AerialAttackState { get; private set; } // Add AerialAttackState declaration
    public UpTiltState UpTiltState { get; private set; } // Add UpTiltState declaration
    public SideTiltState SideTiltState { get; private set; } // Add SideTiltState declaration
    public stunLockState StunLockStateInstance { get; private set; } // Rename to avoid conflict with type name
    // Component References (Example)
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }
    // Add InputReader reference if using one

    // State transition event
    public delegate void StateChangedEvent(PlayerBaseState fromState, PlayerBaseState toState);
    public event StateChangedEvent OnStateChanged;

    // State duration tracking
    private float stateEnterTime;
    public GameObject jabObject;
    public GameObject aerialAttackObject;
    public GameObject upTiltObject;
    public GameObject sideTiltObject; 
    private AlterunaAvatar avatar;
    public CamerWillWorkPls camera;
    Alteruna.User User = Multiplayer.Instance.Me; // Declare and initialize the User variable
    public float GetStateDuration()
    {
        return Time.time - stateEnterTime;
    }

    // InputReader abstraction (now a separate class)
    public InputReader InputReader { get; private set; } // Public property for states to access

    private void Awake()
    {
        // Get Components
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>(); // Or GetComponent<Animator>()
        if (playerCollider == null)
        {
            playerCollider = GetComponent<CapsuleCollider2D>();
            if (playerCollider != null)
            {
                // Store initial size/offset if not set via Inspector
                if (standingColliderSize == Vector2.zero) standingColliderSize = playerCollider.size;
                if (standingColliderOffset == Vector2.zero && playerCollider.offset != Vector2.zero) standingColliderOffset = playerCollider.offset;
            }
            else
            {
                Debug.LogError("Player Collider not found or assigned!", this);
            }
        }

    
        // Initialize input reader
        InputReader = new InputReader(); // Instantiate the new InputReader class

        // Initialize concrete states
        IdleState = new PlayerIdleState(this); // Instantiate the new PlayerIdleState class
        // MoveState removed
        WalkState = new WalkState(this);
        RunState = new RunState(this);
    
        // Register states
        stateRegistry[nameof(PlayerIdleState)] = IdleState; // Register the new PlayerIdleState
        // MoveState registration removed
        stateRegistry[nameof(WalkState)] = WalkState;
        stateRegistry[nameof(RunState)] = RunState;
        JumpState = new JumpState(this);
        stateRegistry[nameof(JumpState)] = JumpState;
        CrouchState = new CrouchState(this);
        stateRegistry[nameof(CrouchState)] = CrouchState;
        SlideState = new SlideState(this);
        // ... register other states
        WallClingState = new WallClingState(this);
        stateRegistry[nameof(WallClingState)] = WallClingState;
        stateRegistry[nameof(SlideState)] = SlideState;
        ShootState = new ShootState(this); // Initialize ShootState
        stateRegistry[nameof(ShootState)] = ShootState; // Register ShootState
        FallState = new FallState(this); // Initialize FallState
        stateRegistry[nameof(FallState)] = FallState; // Register FallState
        AttackState = new AttackState(this); // Initialize AttackState
        stateRegistry[nameof(AttackState)] = AttackState; // Register AttackState
        AerialAttackState = new AerialAttackState(this); // Initialize AerialAttackState
        stateRegistry[nameof(AerialAttackState)] = AerialAttackState; // Register AerialAttackState
        UpTiltState = new UpTiltState(this); // Initialize UpTiltState
        stateRegistry[nameof(UpTiltState)] = UpTiltState; // Register UpTiltState
        SideTiltState = new SideTiltState(this); // Initialize SideTiltState
        stateRegistry[nameof(SideTiltState)] = SideTiltState; // Register SideTiltState
        StunLockStateInstance = new stunLockState(this); // Initialize StunLockState
        stateRegistry[nameof(StunLockStateInstance)] = StunLockStateInstance; // Register StunLockState


        // Initialize jumps
        JumpsRemaining = MaxJumps;

        //Avatar
        

    }

    private void Start()
    {
        
        if (User != Multiplayer.Instance.Me)
        {
            return;
        }
        // Set the initial state
        SwitchState(IdleState); // Start in Idle state
        JumpsRemaining = MaxJumps;
        jabObject.SetActive(false); // Ensure jab object is inactive at start
        aerialAttackObject.SetActive(false); // Ensure aerial attack object is inactive at start
        upTiltObject.SetActive(false); // Ensure up tilt object is inactive at 
        sideTiltObject.SetActive(false); // Ensure side tilt object is inactive at start


        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamerWillWorkPls>(); // Find the camera object
        if (camera == null)
        {
            Debug.LogError("Camera not found! Ensure the camera has the 'MainCamera' tag and the CamerWillWorkPls script attached.", this);
            return;
        }
        camera.playerTransform = PlayerTransform; // Assign the player transform to the camera
        
    }

    private void Update()
    {
        if(User!= Multiplayer.Instance.Me)
        {
            return;
        }
        // Update coyote time timer
        if (jumpGroundedGraceTimer > 0f)
            jumpGroundedGraceTimer -= Time.deltaTime;

        // Track grounded state for jump reset logic
        bool isGroundedNow = IsGrounded();
        if (!wasGroundedLastFrame && isGroundedNow)
        {
            // Landed this frame, reset jumps
            JumpsRemaining = Mathf.Max(0, MaxJumps - 1);
        }
        wasGroundedLastFrame = isGroundedNow;

        currentState?.Tick(Time.deltaTime);

        Vector2 movementInput = GetMovementInput();
        if (movementInput.x != 0)
        {
            if(currentState == WalkState || currentState == RunState)
            {
                PlayerTransform.localScale = new Vector3(Mathf.Sign(movementInput.x), 1, 1); // Flip player based on input direction
            }
        }

        


    }

    public void SwitchState(PlayerBaseState newState)
    {
        if (currentState == newState) return; // Re-entrancy guard
        var prevState = currentState;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
        OnStateChanged?.Invoke(prevState, newState);
        stateEnterTime = Time.time;
    }

    public Vector2 GetMovementInput()
    {
        // Delegate to the InputReader instance
        return InputReader.GetMovementInput();
    }

    public bool IsRunPressed()
    {
        // Delegate to the InputReader instance
        return InputReader.IsRunPressed();
    }

    // For extensibility: get state by name
    public PlayerBaseState GetState(string stateName)
    {
        if (stateRegistry.TryGetValue(stateName, out var state))
            return state;
        return null;
    }
    // Robust ground check using OverlapCircle
    public bool IsGrounded()
    {
        // During coyote time after jump, always return false
        if (jumpGroundedGraceTimer > 0f)
            return false;

        if (groundCheckPoint == null)
        {
             Debug.LogError("Ground Check Point not assigned in the Inspector!", this);
             return false; // Cannot check without the point
        }
        // Check if the circle overlaps with anything on the ground layer
        bool grounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        // Warn if grounded while moving up (likely ground check is inside collider)
        if (grounded && RB != null && RB.linearVelocity.y > 0.1f)
        {
            Debug.LogWarning("[PlayerStateMachine] IsGrounded() is true while moving upward. Adjust groundCheckPoint position or groundCheckRadius in the Inspector so it is just below the feet and not inside the collider.");
        }
        return grounded;
    }

    // Simple wall check (replace with your own logic)
    public bool IsTouchingWall()
    {
        // Wall detection using 2D raycast
        float wallCheckDistance = 0.1f; // How far to check
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left; // Check based on facing direction
        RaycastHit2D hit = Physics2D.Raycast(playerCollider.bounds.center, direction, playerCollider.bounds.extents.x + wallCheckDistance, groundLayer);
        Debug.DrawRay(playerCollider.bounds.center, direction * (playerCollider.bounds.extents.x + wallCheckDistance), hit.collider != null ? Color.green : Color.red);
        return hit.collider != null;
    }

    public void SetColliderCrouching()
    {
        if (playerCollider == null) return;
        playerCollider.size = crouchingColliderSize;
        playerCollider.offset = crouchingColliderOffset;
    }

    public void SetColliderStanding()
    {
        if (playerCollider == null) return;
        playerCollider.size = standingColliderSize;
        playerCollider.offset = standingColliderOffset;
    }

    public bool CanStandUp()
    {
        if (playerCollider == null) return true; // Cannot check, assume okay

        // Calculate the top position of the standing collider
        Vector2 standingTopPoint = (Vector2)transform.position + standingColliderOffset + Vector2.up * (standingColliderSize.y / 2f);
        // Calculate the size of the check area (slightly larger than the top part of the standing collider)
        Vector2 checkSize = new Vector2(standingColliderSize.x * 0.9f, standUpCheckDistance); // Check slightly narrower
        // Calculate the center of the check area
        Vector2 checkCenter = standingTopPoint + Vector2.up * (standUpCheckDistance / 2f);

        // Perform an overlap check
        Collider2D hit = Physics2D.OverlapBox(checkCenter, checkSize, 0f, groundLayer);

        return hit == null; // Can stand up if nothing is hit
    }

}