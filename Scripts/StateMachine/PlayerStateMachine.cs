using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // 컴포넌트
    private CharacterController _cc;
    private Animator _anim;
    [SerializeField] public ParticleSystem s;
    
    // 상수
    [Header("Walk")]
    [SerializeField] private const float _MovSpeed = 6f;
    [SerializeField] private const float _RunMult = 1.5f;
    private const float RotSpeed = .5f;
    private const float _GroundedGravity = -.05f;
    
    // FSM State
    private PlayerBaseState _currentMoveState;    // MoveFsm
    private PlayerBaseState2 _currentActionState; // ActionFsm
    private PlayerStateFactory _factory;

    #region getter & setters
    
    public PlayerBaseState CurrentMoveState { get => _currentMoveState; set => _currentMoveState = value; }
    public PlayerBaseState2 CurrentActionState { get => _currentActionState; set => _currentActionState = value; }
    public Animator Animator => _anim;
    public CharacterController cc => _cc;
    
    // 점프
    public Coroutine CurrenJumpResetRoutine { get => _jumpResetRoutine; set => _jumpResetRoutine = value; }
    public Dictionary<int, float> InitialJumpVelocities { get => _initialJumpVelocities; }
    public Dictionary<int, float> JumpGravities { get => _jumpGravities; }
    public int JumpCount { get => _jumpCount; set => _jumpCount = value; }
    public bool RequireNewJumpPress { get => _requireNewJumpPress; set => _requireNewJumpPress = value; }
    public bool IsJumping { set => _isJumping = value; }
    public bool IsJumpPressed => _isJumpPressed;
    
    // 이동
    public bool IsRunPressed => _isRunPressed;
    public bool IsMovementPressed => _isMovementPressed;
    public float GroundedGravity => _GroundedGravity;
    public float MovementY { get => _movement.y; set => _movement.y = value; }
    public float AppliedMovementY { get => _appliedMovement.y; set => _appliedMovement.y = value; }
    public float AppliedMovementX { get => _appliedMovement.x; set => _appliedMovement.x = value; }
    public float AppliedMovementZ { get => _appliedMovement.z; set => _appliedMovement.z = value; }
    public float RunMult => _RunMult;
    public Vector2 Movement => _movementInput;
    public float MovSpeed => _MovSpeed;
    public float Gravity => _gravity;
    
    // 공격
    public Coroutine CurrenAttackResetRoutine { get => _attackResetRoutine; set => _attackResetRoutine = value; }
    public bool IsAttackPressed => _isAttackPressed;
    public int AttackCount { get => _attackCount; set => _attackCount = value; }
    public bool RequireNewAttackPress { get => _requireNewAttackPress; set => _requireNewAttackPress = value; }
    
    #endregion 
    
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();

        _factory = new PlayerStateFactory(this);
        _factory = new PlayerStateFactory(this);
        
        _currentMoveState = _factory.Grounded();
        _currentActionState = _factory.None();
        
        _currentMoveState.EnterState();
        _currentActionState.EnterState();
        
        SetupJumpVariables();
    }
    
    public void OnRun(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started || ctx.phase == InputActionPhase.Canceled)
        {
            _isRunPressed = ctx.ReadValueAsButton();
        }
    }
    
    #region Input
    
    // 사용자 입력 변수
    private Vector2 _movementInput;
    private Vector3 _movement;
    private Vector3 _appliedMovement;
    private bool _isMovementPressed;
    private bool _isRunPressed;
    private bool _isJumpPressed;
    private bool _isAttackPressed;

    public void OnMoveInput(InputAction.CallbackContext ctx)
    {
        _movementInput = ctx.ReadValue<Vector2>();
        _movement.x = _movementInput.x;
        _movement.z = _movementInput.y;
        _isMovementPressed = _movement.x != 0 || _movement.z != 0;
    }
    
    #endregion
    
    #region Jump

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started || ctx.phase == InputActionPhase.Canceled)
        {
            _isJumpPressed = ctx.ReadValueAsButton();
            _requireNewJumpPress = false;
        }
    }
    
    private float _gravity = -9.8f;
    private float _initialJumpVelocity;
    [Header("Jump")]
    [SerializeField] private float _maxJumpHeight;
    [SerializeField] private float _maxJumpTime;
    private bool _isJumping;
    private bool _requireNewJumpPress;
    private int _jumpCount = 0;
    private Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
    private Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();
    private Coroutine _jumpResetRoutine = null;
    
    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * (_maxJumpHeight) / timeToApex);
        float secondJumpGravity = (-2 * (_maxJumpHeight + 1f)) / Mathf.Pow(timeToApex * 1.25f, 2);
        float secondJumpInitialVelocity = (2 * (_maxJumpHeight + 1f)) / (timeToApex * 1.25f);
        float thirdJumpGravity = (-2 * (_maxJumpHeight + 2f)) / Mathf.Pow(timeToApex * 1.5f, 2);
        float thirdJumpInitialVelocity = (2 * (_maxJumpHeight + 2f)) / (timeToApex * 1.5f);
        
        _initialJumpVelocities.Add(1, _initialJumpVelocity);
        _initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        _initialJumpVelocities.Add(3, thirdJumpInitialVelocity);
        
        _jumpGravities.Add(0,_gravity);
        _jumpGravities.Add(1,_gravity);
        _jumpGravities.Add(2,secondJumpGravity);
        _jumpGravities.Add(3,thirdJumpGravity);
    }
    
    #endregion 
    
    #region Attack
    
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started || ctx.phase == InputActionPhase.Canceled)
        {
            _isAttackPressed = ctx.ReadValueAsButton();
            _requireNewAttackPress = false;
        }
    }

    private int _attackCount = 0;
    private Coroutine _attackResetRoutine = null;
    private bool _requireNewAttackPress;

    #endregion
    
    void Update()
    {
        HandleRotation();
        
        _currentMoveState.UpdateStates();
        _currentActionState.UpdateStates();
        
        Debug.Log(_currentActionState);
        
        _cc.Move((_appliedMovement) * Time.deltaTime);
    }
    
    void HandleRotation()
    {
        if (_isMovementPressed)
        {
            Vector3 dir = new Vector3(_movement.x, 0f, _movement.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), RotSpeed);
        }
    }
}