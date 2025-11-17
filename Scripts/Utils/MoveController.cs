using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
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
    private const float MovSpeed = 3.0f;
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

        // SetupJumpVariables();
    }

    private void Start()
    {
        
    }
}

