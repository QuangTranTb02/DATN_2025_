using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor.Animations;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D _coll;
    [SerializeField] private Animator _animator;

    private Rigidbody2D _rb;

    //movement vars
    public bool IsFacingRight { get; private set; }
    public MovementController Controller { get; private set; }

    [HideInInspector] public Vector2 Velocity;

    //input
    private Vector2 _moveInput;
    private bool _runHeld;
    private bool _jumpPressed;
    private bool _jumpReleased;
    private bool _dashPressed;

    //jump vars
    private bool _isJumping; // Có đang trong trạng thái nhảy không
    private bool _isFastFalling; // Có đang rơi nhanh (fast fall) không
    private bool _isFalling; // Có đang rơi xuống không
    private float _fastFallTime; // Thời gian rơi nhanh
    private float _fastFallReleaseSpeed; // Tốc độ nhả nút khi rơi nhanh
    private int _numberOfAirJumpUsed; // Số lần nhảy đã sử dụng (cho nhảy kép)

    //apex vars
    private float _apexPoint; // Điểm đỉnh nhảy
    private float _timePastApexThreshold; // Thời gian đã trôi qua kể từ khi chạm ngưỡng đỉnh nhảy
    private bool _isPastApexThreshold; // Đã qua ngưỡng đỉnh nhảy chưa

    //jump buffer vars
    private float _jumpBufferTimer; // Bộ đếm thời gian Jump Buffer
    private bool _jumpReleasedDuringBuffer; // Thời gian nhả nút nhảy trong khi buffer còn hoạt động

    //coyote time vars
    private float _coyoteTimer; // Bộ đếm thời gian Coyote Time

    //Wall slide vars
    private bool _isWallSliding;
    private bool _isWallSlideFalling;

    //Wall jump vars
    private bool _useWallJumpMoveStats;
    private bool _isWallJumping;
    private float _wallJumpTime;
    private bool _isWallJumpFastFalling;
    private bool _isWallJumpFalling;
    private float _wallJumpFastFallTime;
    private float _wallJumpFastFallReleaseSpeed;
    private float _wallJumpPostBufferTimer;
    private float _wallJumpApexPoint;
    private float _timePastWallJumpApexThreshold;
    private bool _isPastWallJumpApexThreshold;
    private int _lastWallDir;

    //Dash vars
    private bool _isDashing;
    private bool _isAirDashing;
    private float _dashTimer;
    private float _dashOnGroundTimer;
    private int _numberOfDashesUsed;
    private Vector2 _dashDirection;
    private bool _isDashFastFalling;
    private float _dashFastFallTime;
    private float _dashFastFallReleaseSpeed;
    private float _dashBufferTimer;

    //head bump slide vars
    private float _jumpStartY;
    private bool _isHeadBumpSliding;
    private int _headBumpSlideDirection;
    private bool _justFinishedSlide;
    private float _dashStartY;
    private bool _slideFromDash;
    private void Awake()
    {
        IsFacingRight = true;
        _rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<MovementController>();
    }
    private void Update()
    {
        _moveInput = InputManager.Movement;
        _runHeld = InputManager.RunIsHeld;

        if (InputManager.JumpWasPressed) _jumpPressed = true;
        if (InputManager.JumpWasReleased) _jumpReleased = true;
        if (InputManager.DashWasPressed) _dashPressed = true;
    }


    private void FixedUpdate()
    {
        _justFinishedSlide = false;

        CountTimer(Time.fixedDeltaTime);

        WallJumpCheck();
        WallSlideCheck();

        JumpChecks();
        LandCheck();
        
        DashCheck();


        HandleHorizontalMovement(Time.fixedDeltaTime);
        HandleHeadBumpSlide();
        
        Jump(Time.fixedDeltaTime);
        WallJump(Time.fixedDeltaTime);
        WallSlide(Time.fixedDeltaTime);
        Dash(Time.fixedDeltaTime);
        Fall(Time.fixedDeltaTime);
        
        ClampVelocity();
        Controller.Move(Velocity * Time.fixedDeltaTime);

        _jumpPressed = false;
        _jumpReleased = false;
        _dashPressed = false;
    }
    private void ClampVelocity()
    {
        //CLAMP FALL SPEED
        if (!_isDashing)
        {
            Velocity.y = Mathf.Clamp(Velocity.y, -MoveStats.MaxFallSpeed, 50f); // Giới hạn tốc độ rơi
        }
        else
        {
            Velocity.y = Mathf.Clamp(Velocity.y, -50f, 50f);
        }
    }
    #region Movement

    private void HandleHorizontalMovement(float timeStep)
    {
        if (_isHeadBumpSliding) { return; }
        if (!_isDashing)
        {
            TurnCheck(_moveInput);

            float targetVelocityX = 0f;

            if (Mathf.Abs(_moveInput.x) > MoveStats.MoveThreshold)
            {
                float moveDirection = Mathf.Sign(_moveInput.x);
                targetVelocityX = _runHeld ? moveDirection * MoveStats.MaxRunSpeed : moveDirection * MoveStats.MaxWalkSpeed;
            }

            float acceleration = Controller.IsGrounded() ? MoveStats.GroundAcceleration : MoveStats.AirAcceleration;
            float deceleration = Controller.IsGrounded() ? MoveStats.GroundDeceleration : MoveStats.AirDeceleration;

            if (_useWallJumpMoveStats)
            {
                acceleration = MoveStats.WallJumpMoveAcceleration;
                deceleration = MoveStats.WallJumpMoveDeceleration;
            }

            if (Mathf.Abs(_moveInput.x) > MoveStats.MoveThreshold)
            {
                Velocity.x = Mathf.Lerp(Velocity.x, targetVelocityX, acceleration * timeStep);
            }
            else
            {
                Velocity.x = Mathf.Lerp(Velocity.x, 0, deceleration * timeStep);
            }
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (IsFacingRight && moveInput.x < 0)
        {
            Turn(false); // Turn Left
        }
        else if (!IsFacingRight && moveInput.x > 0)
        {
            Turn(true); // Turn Right
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            IsFacingRight = true;
            transform.Rotate(0f, 180f, 0f); // Xoay 180 độ (Thường dùng cho mô hình 3D)
        }
        else
        {
            IsFacingRight = false;
            transform.Rotate(0f, -180f, 0f); // Hoặc Rotate(0f, 180f, 0f) nếu muốn xoay theo cùng hướng
        }
    }
    private void HandleHeadBumpSlide()
    {
        if (!_isHeadBumpSliding && (_isJumping || _isDashing || _isWallJumping) && Controller.BumpedHead() && !Controller.IsHittingBothCorners && !Controller.IsHittingCeilingCenter)
        {
            _isHeadBumpSliding = true;
            _headBumpSlideDirection = Controller.HeadBumpSlideDirection;
        }

        if (_isHeadBumpSliding)
        {
            Velocity.y = 0;

            if (Controller.HeadBumpSlideDirection == 0 || !Controller.BumpedHead() || Controller.IsHittingCeilingCenter || Controller.IsHittingBothCorners)
            {
                _isHeadBumpSliding = false;
                Velocity.x = 0;

                if (!_slideFromDash)
                {
                    float compenstionFactor = (1 - MoveStats.JumpHeightCompensationFactor) + 1;
                    float jumpPeakY = _jumpStartY + (MoveStats.JumpHeight + compenstionFactor);
                    float remainingHeight = jumpPeakY - _rb.position.y;

                    if (remainingHeight > 0)
                    {
                        float requiredVelocity = Mathf.Sqrt(2 * Mathf.Abs(MoveStats.Gravity) * remainingHeight);
                        Velocity.y = requiredVelocity;
                    }
                }
                else if (_slideFromDash)
                {
                    float targetApexY = _dashStartY + MoveStats.DashTargetApexHeight;
                    float remainingHeight = targetApexY - _rb.position.y;

                    if(remainingHeight > 0)
                    {
                        float requiredVelocity = Mathf.Sqrt(2 * Mathf.Abs(MoveStats.Gravity) * remainingHeight);
                        Velocity.y = requiredVelocity;
                    }
                }
                _slideFromDash = false;
                _justFinishedSlide = true;
            }
            else
            {
                Velocity.x = _headBumpSlideDirection * MoveStats.HeadBumpSlideSpeed;
            }
        }

    }


    #endregion
    #region Land/Fall

    private void LandCheck()
    {
        
        if (Controller.IsGrounded()) 
        {
            //LANDED
            if ((_isJumping || _isFalling || _isWallJumpFalling || _isWallJumping || _isWallSlideFalling || _isWallSliding || _isDashFastFalling
            || _isHeadBumpSliding) && Velocity.y <= 0f)
            {
                _isHeadBumpSliding = false;


                ResetJumpValues();
                StopWallSlide();
                ResetWallJumpValues();
                ResetDashes();
                ResetDashValues();

                _numberOfAirJumpUsed = 0;


            }
            if(Velocity.y <= 0f)
            {
                Velocity.y = -2f;
            }
        }
    }
    private void Fall(float TimeStep)
    {
        if ( !Controller.IsGrounded() && !_isJumping && !_isDashing && !_isWallSliding && !_isWallJumping && !_isDashFastFalling)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }

            Velocity.y += MoveStats.Gravity * TimeStep;
        }
    }
    #endregion
    #region Jump
    private void ResetJumpValues()
    {
        _isJumping = false;
        _isFalling = false;
        _isFastFalling = false;
        _fastFallTime = 0f;
        _isPastApexThreshold = false;
    }
    private void JumpChecks()
    {
        //WHEN WE PRESS THE JUMP BUTTON
        if (_jumpPressed)
        {
            /*if (_isWallSlideFalling && _wallJumpPostBufferTimer >= 0f)
            {
                return;
            }
            else if(_isWallSliding || (Controller.IsTouchingWall(IsFacingRight) && !Controller.IsGrounded()))
            {
                return;
            }*/

            _jumpBufferTimer = MoveStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        //WHEN WE RELEASE THE JUMP BUTTON
        if (_jumpReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            if (_isJumping && Velocity.y > 0f) // If moving upwards
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.TimeForUpwardsCancel;
                    Velocity.y = 0f;
                }   
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = Velocity.y;
                }
            }
        }
        //INITIATE JUMP WITH JUMP BUFFERING AND COYOTE TIME
        if (_jumpBufferTimer > 0f && !_isJumping && (Controller.IsGrounded() || _coyoteTimer > 0f))
        {
            InitiateJump(0);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = Velocity.y;
            }
        }

        //DOUBLE JUMP
        else if ((_jumpBufferTimer > 0f) && (_isJumping || _isWallJumping || _isWallSlideFalling|| _isAirDashing
            || _isDashFastFalling) && !Controller.IsTouchingWall(IsFacingRight) && (_numberOfAirJumpUsed < MoveStats.NumberOfAirAllowed))
        {
            _isFastFalling = false;
            InitiateJump(1);
            if (_isDashFastFalling) { _isDashFastFalling = false; }
        }

        //AIR JUMP AFTER COYOTE TIME LAPSED
        else if (_jumpBufferTimer > 0f && !_isFalling && !_isWallSlideFalling && _numberOfAirJumpUsed < MoveStats.NumberOfAirAllowed)
        {
            InitiateJump(1); // initiate jump
            _isFastFalling = false;
        }
    }

    private void InitiateJump(int numberOfAirJumpUsed)
    {
        if (Controller.IsTouchingWall(IsFacingRight) && !Controller.IsGrounded())
        {
            return;  // ← Block normal jump khi ở tường
        }
    
        if (!_isJumping)
        {
            _isJumping = true;
        }
        ResetWallJumpValues();

        _jumpBufferTimer = 0f;
        _numberOfAirJumpUsed += numberOfAirJumpUsed;
        Velocity.y = MoveStats.InitialJumpVelocity;

        _jumpStartY = _rb.position.y;
    }
    private void Jump(float TimeStep)
    {
        //APPLY GRAVITY WHILE JUMPING
        if (_isJumping)
        {
            //CHECK FOR HEAD BUMP
            if (Controller.BumpedHead() && !_isHeadBumpSliding)
            {
                if (Controller.HeadBumpSlideDirection != 0 && !Controller.IsHittingCeilingCenter && !Controller.IsHittingBothCorners)
                {
                    _slideFromDash = false;
                }
                else
                {
                    Velocity.y = 0f;
                    _isFastFalling = true;
                }
            }

            if (_isHeadBumpSliding)
            {
                Velocity.y = 0f;
                return;
            }
            if (!_justFinishedSlide)
            {
                //GRAVITY ON ASCENDING
                if (Velocity.y >= 0f)
                {
                    //APEX CONTROLS
                    _apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, Velocity.y);

                    if (_apexPoint > MoveStats.ApexThreshold)
                    {
                        if (!_isPastApexThreshold)
                        {
                            _isPastApexThreshold = true;
                            _timePastApexThreshold = 0f;
                        }

                        if (_isPastApexThreshold)
                        {
                            _timePastApexThreshold += TimeStep;

                            if (_timePastApexThreshold < MoveStats.ApexHangTime)
                            {
                                Velocity.y = 0f;
                            }
                            else
                            {
                                Velocity.y = -0.01f;
                            }
                        }
                    }
                    //GRAVITY ON ASCENDING BUT NOT PAST APEX THRESHOLD
                    else if (!_isFastFalling)
                    {
                        Velocity.y += MoveStats.Gravity * Time.fixedDeltaTime;

                        if (_isPastApexThreshold)
                        {
                            _isPastApexThreshold = false;
                        }
                    }
                }
                //GRAVITY ON DESCENDING
                else if (!_isFastFalling)
                {
                    Velocity.y += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * TimeStep;
                }
                else if (Velocity.y < 0f)
                {
                    if (!_isFalling)
                    {
                        _isFalling = true;
                    }
                }
            }
        }
        // JUMP CUT
        if (_isFastFalling)
        {
            // Áp dụng trọng lực mạnh hơn theo thời gian
            if (_fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                Velocity.y += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * TimeStep;
                // ← ĐÂY LÀ LẦN THỨ 2 áp dụng GravityOnReleaseMultiplier (lần 1 ở trên rồi!)
            }
            // Đoạn Lerp này là tội đồ chính khiến nhảy siêu xa
            else if (_fastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                Velocity.y = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MoveStats.TimeForUpwardsCancel));
                // ← Đang set vận tốc về gần 0 → nhân vật lơ lửng trên không!!!
            }
            _fastFallTime += Time.fixedDeltaTime;
        }       
    }
    #endregion
    #region Wall Slide
    private void WallSlideCheck()
    {
        if (Controller.IsTouchingWall(IsFacingRight) && !Controller.IsGrounded() && !_isDashing)
        {
            if (Velocity.y < 0f && !_isWallSliding)
            {
                ResetJumpValues();
                ResetDashValues();
                ResetWallJumpValues();

                if (MoveStats.ResetDashOnWallSlide)
                {
                    ResetDashes();
                }

                _isWallSlideFalling = false;
                _isWallSliding = true;

                if (MoveStats.ResetJumpsOnWallSlide)
                {
                    _numberOfAirJumpUsed = 0;
                }
            }
        }
        else if (_isWallSliding && Controller.IsTouchingWall(IsFacingRight) && !Controller.IsGrounded() && !_isWallSlideFalling)
        {
            _isWallSlideFalling = true;
            StopWallSlide();
        }
        else
        {
            StopWallSlide();
        }
    }

    private void StopWallSlide()
    {
        if (_isWallSliding)
        {
            _isWallSliding = false;
        }
    }
    private void WallSlide(float timeStep)
    {
        if (_isWallSliding)
        {
                Velocity.y = Mathf.Lerp(Velocity.y, -MoveStats.WallSlideSpeed, MoveStats.WallSlideDecelerationSpeed * timeStep);
        }
    }
    #endregion
    #region Wall Jump
    private void WallJumpCheck()
    {
        if (ShouldApplyPostWallJumpBuffer())
        {
            _wallJumpPostBufferTimer = MoveStats.WallJumpPostBufferTime;
        }

        //wall jump fast falling
        if (_jumpReleased && !_isWallSliding && !Controller.IsTouchingWall(IsFacingRight) && _isWallJumping)
        {
            if (Velocity.y > 0f)
            {
                if (_isPastWallJumpApexThreshold)
                {
                    _isPastWallJumpApexThreshold = false;
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallTime = MoveStats.TimeForUpwardsCancel;

                    Velocity.y = 0f;
                }
                else
                {
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallReleaseSpeed = Velocity.y;
                }
            }
        }

        //actual jump with post wall jump buffer time
        if (_jumpPressed && // ← Dùng _jumpPressed thay vì InputManager.JumpWasPressed
        ((_wallJumpPostBufferTimer > 0f) ||
         _isWallSliding ||
         _isWallSlideFalling ||
         (Controller.IsTouchingWall(IsFacingRight) && !Controller.IsGrounded()))) // ← THÊM điều kiện này
        {
            InitiateWallJump();
            _jumpBufferTimer = 0f;  // Reset để không trigger normal jump
            _jumpPressed = false;    // Reset input flag ngay lập tức
        }
    }
    private void InitiateWallJump()
    {
        if (!_isWallJumping)
        {
            _isWallJumping = true;
            _useWallJumpMoveStats = true;
        }

        StopWallSlide();
        ResetJumpValues();
        _wallJumpTime = 0f;

        Velocity.y = MoveStats.InitialWallJumpVelocity;

        Velocity.x = Mathf.Abs(MoveStats.WallJumpDirection.x) * -_lastWallDir;

        _jumpStartY = _rb.position.y;
    }
    private void WallJump(float timeStep)
    {
        //APPLY WALL JUMP GRAVITY
        if (_isWallJumping)
        {
            //TIME TO TAKE OVER MOVEMENT CONTROLS WHILE WALL JUMPING
            _wallJumpTime += timeStep;
            if (_wallJumpTime >= MoveStats.TimeTillJumpApex)
            {
                _useWallJumpMoveStats = false;
            }

            //HIT HEAD
            if (Controller.BumpedHead() && !_isHeadBumpSliding)
            {
                if (Controller.HeadBumpSlideDirection != 0 && !Controller.IsHittingCeilingCenter && !Controller.IsHittingBothCorners)
                {
                    _slideFromDash = false;
                }
                else
                {
                    Velocity.y = 0f;
                    _isWallJumpFastFalling = true;
                    _useWallJumpMoveStats = false;
                }
            }

            if (_isHeadBumpSliding)
            {
                Velocity.y = 0f;
                return;
            }
            if (!_justFinishedSlide)
            {
                //GRAVITY IN ASCENDING
                if (Velocity.y >= 0f)
                {
                    //APEX CONTROLS
                    _wallJumpApexPoint = Mathf.InverseLerp(MoveStats.WallJumpDirection.y, 0f, Velocity.y);

                    if (_wallJumpApexPoint > MoveStats.ApexThreshold)
                    {
                        if (!_isPastWallJumpApexThreshold)
                        {
                            _isPastWallJumpApexThreshold = true;
                            _timePastWallJumpApexThreshold = 0f;
                        }

                        if (_isPastWallJumpApexThreshold)
                        {
                            _timePastWallJumpApexThreshold += timeStep;
                            if (_timePastWallJumpApexThreshold < MoveStats.ApexHangTime)
                            {
                                Velocity.y = 0f;
                            }
                            else
                            {
                                Velocity.y = -0.01f;
                            }
                        }
                    }
                    //GRAVITY IN ASCENDING BUT NOT PAST APEX THRESHOLD
                    else if (!_isWallJumpFastFalling)
                    {
                        Velocity.y += MoveStats.WallJumpGravity * timeStep;

                        if (_isPastWallJumpApexThreshold)
                        {
                            _isPastWallJumpApexThreshold = false;
                        }
                    }
                }
                //GRAVITY ON DESCENDING
                else if (!_isWallJumpFastFalling)
                {
                    Velocity.y += MoveStats.WallJumpGravity * timeStep ;
                }

                else if (Velocity.y < 0f)
                {
                    if (!_isWallJumpFalling)
                    {
                        _isWallJumpFalling = true;
                    }
                }
            }
        }
        //HANDLE WALL JUMP CUT TIME
        if (_isWallJumpFastFalling)
        {
            if (_wallJumpFastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                Velocity.y += MoveStats.WallJumpGravity * MoveStats.WallJumpGravityOnReleaseMultiplier * timeStep;
            }
            else if (_wallJumpFastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                Velocity.y = Mathf.Lerp(_wallJumpFastFallReleaseSpeed, 0f, (_wallJumpFastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            _wallJumpFastFallTime += timeStep;
        }
    }

    private bool ShouldApplyPostWallJumpBuffer()
    {
        if (Controller.IsTouchingWall(IsFacingRight) || _isWallSliding)
        {
            _lastWallDir = Controller.GetWallDirection();
            return true;
        }
        else { return false; }
    }
    private void ResetWallJumpValues()
    {
        
        _isWallSlideFalling = false;
        _useWallJumpMoveStats = false;
        _isWallJumping= false;
        _isWallJumpFastFalling = false;
        _isWallJumpFalling = false;
        _isPastWallJumpApexThreshold = false;

        _wallJumpFastFallTime = 0f;
        _wallJumpTime = 0f;
    }

    #endregion
    #region Dash
    private void DashCheck()
    {
        if (_dashPressed)
        {
            _dashBufferTimer = MoveStats.DashBufferTime;
        }
        if (_dashBufferTimer > 0f)
        {
            //ground dash
            if (Controller.IsGrounded() && _dashOnGroundTimer < 0 && !_isDashing)
            {
                InitiateDash();
                _dashBufferTimer = 0f;
            }

            //air dash
            else if (!Controller.IsGrounded() && !_isDashing && _numberOfDashesUsed < MoveStats.NumberOfDashes)
            {
                _isAirDashing = true;
                InitiateDash();
                _dashBufferTimer = 0f;
            }
        }
    }
    private void InitiateDash()
    {
        _dashStartY = _rb.position.y;
        _dashDirection = InputManager.Movement;
        TurnCheck(_dashDirection);

        Vector2 closestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[0]);

        for (int i = 0; i < MoveStats.DashDirections.Length; i++)
        {
            //skip if we hit it bang on
            if (_dashDirection == MoveStats.DashDirections[i])
            {
                closestDirection = _dashDirection;
                break;
            }

            float distance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[i]);

            //check if this is a diagonal direction and apply bias
            bool isDiagonal = (Mathf.Abs(MoveStats.DashDirections[i].x) == 1 && Mathf.Abs(MoveStats.DashDirections[i].y) == 1);
            if (isDiagonal)
            {
                distance -= MoveStats.DashDiagonalYBias;
            }

            else if (distance < minDistance)
            {
                minDistance = distance;
                closestDirection = MoveStats.DashDirections[i];
            }
        }

        //handle direction with NO input
        if (closestDirection == Vector2.zero)
        {
            if (IsFacingRight)
            {
                closestDirection = Vector2.right;
            }
            else { closestDirection = Vector2.left; }
        }
        if(Controller.IsGrounded() && closestDirection.y < 0 && closestDirection.x != 0)
        {
            closestDirection = new Vector2(Mathf.Sign(closestDirection.x), 0f);
        }   
        _dashDirection = closestDirection;
        _numberOfDashesUsed++;
        _isDashing = true;
        _dashTimer = 0f;
        _dashOnGroundTimer = MoveStats.TimeBtWDashesOnGround;

        ResetJumpValues();
        ResetWallJumpValues();
        StopWallSlide();
    }
    private void Dash(float timeStep)
    {
        if(_justFinishedSlide)
        {
            return;
        }
        if (_isDashing)
        {
            if (Controller.BumpedHead() && !_isHeadBumpSliding)
            {
                if (Controller.HeadBumpSlideDirection != 0 && !Controller.IsHittingCeilingCenter && !Controller.IsHittingBothCorners)
                {
                    _slideFromDash = true;
                    _dashTimer = 0f;
                }
                else
                {
                    Velocity.y = 0;
                    _isDashing = false;
                    _isAirDashing = false;
                    _dashTimer = 0f;
                }
            }

            if (_isHeadBumpSliding)
            {
                Velocity.y = 0f;
                return;
            }
            //stop the dash after the timer
            _dashTimer += timeStep;
            if (_dashTimer >= MoveStats.DashTime)
            {
                if (Controller.IsGrounded())
                {
                    ResetDashes();
                }

                _isAirDashing = false;
                _isDashing = false;

                if (!_isJumping && !_isWallJumping)
                {
                    _dashFastFallTime = 0f;
                    _dashFastFallReleaseSpeed = Velocity.y;

                    if (!Controller.IsGrounded())
                    {
                        _isDashFastFalling = true;
                    }
                }

                return;
            }

            Velocity.x = MoveStats.DashSpeed * _dashDirection.x;

            if (_dashDirection.y != 0f || _isAirDashing)
            {
                Velocity.y = MoveStats.DashSpeed * _dashDirection.y;
            }
            else if(!_isJumping && _dashDirection.y == 0)
            {
                Velocity.y = -0.0010f;
            }
        }
        //HANDLE DASH CUT TIME
        else if (_isDashFastFalling)
        {
            if (Velocity.y > 0f)
            {
                if (_dashFastFallTime < MoveStats.DashTimeForUpwardsCancel)
                {
                    Velocity.y = Mathf.Lerp(_dashFastFallReleaseSpeed, 0f, (_dashFastFallTime / MoveStats.DashTimeForUpwardsCancel));
                }
                else if (_dashFastFallTime >= MoveStats.DashTimeForUpwardsCancel)
                {
                    Velocity.y += MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }

                _dashFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                Velocity.y += MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
        }
    }
    private void ResetDashValues()
    {
        _isDashFastFalling = false;
        _dashFastFallTime = -0.01f;

        _dashFastFallReleaseSpeed = 0f;
        _dashFastFallTime = 0f;
        _dashDirection = Vector2.zero;  

    }
    private void ResetDashes()
    {
        _numberOfDashesUsed = 0;
    }
    #endregion
    #region Timer
    private void CountTimer(float timeStep)
    {
        //jump buffer
        _jumpBufferTimer -= timeStep;

        //coyate time
        if( !Controller.IsGrounded())
        {
            _coyoteTimer -= timeStep;
        }
        else
        {
            _coyoteTimer = MoveStats.JumpCoyoteTime;
        }
        //wall jump buffer time
       
         _wallJumpPostBufferTimer -= timeStep;
        //dash on ground timer
        if (Controller.IsGrounded())
        {
            _dashOnGroundTimer -= timeStep;
        }
        // dash buffer timer
        _dashBufferTimer -= timeStep;
    }
    #endregion
    private void OnDrawGizmos()
    {
        if (MoveStats == null) return;

        // Vẽ quỹ đạo nhảy khi đang đi bộ (walk speed)
        if (MoveStats.ShowWalkJumpArc)
        {
            DrawJumpArc(MoveStats.MaxWalkSpeed, Color.white);
        }

        // Vẽ quỹ đạo nhảy khi đang chạy (run speed)
        if (MoveStats.ShowRunJumpArc)
        {
            DrawJumpArc(MoveStats.MaxRunSpeed, Color.red);
        }
    }
    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(_coll.bounds.center.x, _coll.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if (MoveStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else { speed = -moveSpeed; }

        Vector2 velocity = new Vector2(speed, MoveStats.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * MoveStats.TimeTillJumpApex / MoveStats.ArcResolution; // time step for the simulation
                                                                                   //float totalTime = (2 * MoveStats.TimeTillJumpApex) + MoveStats.ApexHangTime; // total time of the arc including hang time

        for (int i = 0; i < MoveStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < MoveStats.TimeTillJumpApex) // Ascending
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, MoveStats.Gravity) * simulationTime * simulationTime;
            }
            else if (simulationTime < MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime) // Apex hang time
            {
                float apexTime = simulationTime - MoveStats.TimeTillJumpApex;
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime; // No vertical movement during hang time
            }
            else // Descending
            {
                float descendTime = simulationTime - (MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime);
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * MoveStats.ApexHangTime; // Horizontal movement during hang time
                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, MoveStats.Gravity) * descendTime * descendTime;
            }

            drawPoint = startPosition + displacement;

            if (MoveStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), MoveStats.GroundLayer);
                if (hit.collider != null)
                {
                    // If a hit is detected, stop drawing the arc at the hit point
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }
}