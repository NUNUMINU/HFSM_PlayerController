using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController1 : MonoBehaviour
{
    // 컴포넌트
    private CharacterController _cc;
    private Animator _anim;

    // 사용자 입력 변수
    private Vector2 _movementInput;
    private Vector3 _movement;
    private Vector3 _appliedMovement;
    private bool _isMovementPressed;
    private bool _isRunPressed;
    private bool _isJumpPressed;
    
    // 상수
    private const float MovSpeed = 5.0f;
    private const float RotSpeed = .8f;
    private const float GroundedGravity = -.05f;
    
    // 점프 변수
    private float _gravity = -9.8f;
    private float _initialJumpVelocity;
    [SerializeField] private float _maxJumpHeight = 2f;
    [SerializeField] private float _maxJumpTime = 0.75f;
    private bool _isJumping;
    private bool _isJumpingAnimating;
    private int _jumpCount = 0;
    private Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
    private Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();
    private Coroutine _jumpResetRoutine = null;
    
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();

        SetupJumpVariables();
    }

    public void OnMoveInput(InputAction.CallbackContext ctx)
    {
        _movementInput = ctx.ReadValue<Vector2>();
        _movement.x = _movementInput.x;
        _movement.z = _movementInput.y;
        _isMovementPressed = _movement.x != 0 || _movement.z != 0;
    }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started || ctx.phase == InputActionPhase.Canceled)
        {
            _isRunPressed = ctx.ReadValueAsButton();
        }
    }
    
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started || ctx.phase == InputActionPhase.Canceled)
        {
            _isJumpPressed = ctx.ReadValueAsButton();
        }
    }
    
    
    
    void Update()
    {
        HandleAnim();
        HandleRotation();
        
        if (_isRunPressed)
        {
            _appliedMovement.x = _movement.x * 2 * MovSpeed;
            _appliedMovement.z = _movement.z * 2 * MovSpeed;
        }
        else 
        {
            _appliedMovement.x = _movement.x * MovSpeed;
            _appliedMovement.z = _movement.z * MovSpeed;
        }
        
        _cc.Move((_appliedMovement) * Time.deltaTime);
        
        Debug.Log(_appliedMovement.y);
        HandleGravity();
        HandleJump();
    }

    
    
    void HandleAnim()
    {
        bool isWalking = _anim.GetBool("isWalking");
        bool isRunning = _anim.GetBool("isRunning");
        
        if(!isWalking && _isMovementPressed) _anim.SetBool("isWalking", true);
        if(isWalking && !_isMovementPressed) _anim.SetBool("isWalking", false);
        
        if(!isRunning && _isMovementPressed && _isRunPressed) _anim.SetBool("isRunning", true);
        if(isRunning && (!_isMovementPressed || !_isRunPressed)) _anim.SetBool("isRunning", false);
    }
    
    void HandleRotation()
    {
        if (_isMovementPressed)
        {
            Vector3 dir = new Vector3(_movement.x, 0f, _movement.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), RotSpeed);
        }
    }

    void HandleGravity()
    {
        bool isFalling = _movement.y <= .0f|| !_isJumpPressed;
        float falMult = 2.0f;
        
        if (_cc.isGrounded)
        {
            if (_isJumpingAnimating) // 점프 후 착지
            {
                _isJumpingAnimating = false;
                _anim.SetBool("isJumping", _isJumpingAnimating);
                _jumpResetRoutine = StartCoroutine(JumpResetRoutine()); // 연속 점프를 위한 코루틴
                if (_jumpCount == 3)
                {
                    _jumpCount = 0;
                    _anim.SetInteger("JumpCount", _jumpCount);
                }
            }

            _movement.y = GroundedGravity;
            _appliedMovement.y = GroundedGravity;
        }
        else if (isFalling) // 떨어질 때 더 빠르게
        {
            float previousYVelocity = _movement.y;
            _movement.y = previousYVelocity + (_jumpGravities[_jumpCount] * Time.deltaTime * falMult);
            _appliedMovement.y = (previousYVelocity + _movement.y) * 0.5f;
        }
        else
        {
            // Movement.y += gravity * Time.deltaTime;
            float previousYVelocity = _movement.y; 
            _movement.y = previousYVelocity + (_jumpGravities[_jumpCount] * Time.deltaTime);
            _appliedMovement.y = (previousYVelocity + _movement.y) * 0.5f;
        }
    }

    void HandleJump()
    {
        //애니메이션 로직과 물리 로직을 분리 -> _isJumping & _isJumpingAnimating
        if (!_isJumping && _isJumpPressed && _cc.isGrounded) // 바닥에 있을때만 점프 가능
        {
            if (_jumpCount < 3 && _jumpResetRoutine != null)
            {
                StopCoroutine(_jumpResetRoutine);
            }
            
            _isJumping = true;
            _isJumpingAnimating = true;
            _anim.SetBool("isJumping", _isJumpingAnimating);
            
            _jumpCount += 1;
            _anim.SetInteger("JumpCount", _jumpCount);
            
            _movement.y = _initialJumpVelocities[_jumpCount];
            _appliedMovement.y = _initialJumpVelocities[_jumpCount];
            // 자세히는 아래와 같이 구함
            // float previousYVelocity = Movement.y;
            // float newYVelocity = (Movement.y + initialJumpVelocity);
            // float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            // Movement.y = nextYVelocity;
        }
        else if (_isJumping && !_isJumpPressed && _cc.isGrounded)
        {
            _isJumping = false;
        }
    }
    
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

    IEnumerator JumpResetRoutine()
    {
        yield return new WaitForSeconds(.5f);
        _jumpCount = 0;
    }
}
