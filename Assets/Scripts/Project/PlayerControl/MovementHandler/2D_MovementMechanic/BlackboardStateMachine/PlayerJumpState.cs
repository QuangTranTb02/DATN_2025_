using BlackboardSystem;
using BlackboardSystem.FSM;
using UnityEngine;

namespace Project.PlayerControl.Movement
{
    internal readonly struct ReadonlyJumpContext
    {
        public readonly float JumpBufferTimer;
        public readonly float InitialJumpVelocity;
        public readonly float ApexThreshold;
        public readonly float ApexHangTime;
        public readonly float Gravity;
        public readonly float AirAcceleration;
        public readonly float AirDeceleration;
        public readonly float MaxAirMoveSpeed;

        public ReadonlyJumpContext(
            float jumpBufferTimer,
            float initialJumpVelocity,
            float apexThreshold,
            float apexHangTime,
            float gravity,
            float airAcceleration,
            float airDeceleration,
            float maxAirMoveSpeed)
        {
            JumpBufferTimer = jumpBufferTimer;
            InitialJumpVelocity = initialJumpVelocity;
            ApexThreshold = apexThreshold;
            ApexHangTime = apexHangTime;
            Gravity = gravity;
            AirAcceleration = airAcceleration;
            AirDeceleration = airDeceleration;
            MaxAirMoveSpeed = maxAirMoveSpeed;
        }
    }
    internal sealed class PlayerJumpState : IState
    {
        readonly ReadonlyJumpContext m_jumpContext;
        [BBKey("jumpBufferTimer")] private readonly BlackboardKey m_jumpBufferTimerKey;
        [BBKey("velocity")] private readonly BlackboardKey m_velocityKey;
        [BBKey("pastApex")]private readonly BlackboardKey m_pastApexKey;
        [BBKey("direction")] private readonly BlackboardKey m_directionKey;

        Vector3 m_currentVelocity;
        float m_jumpBufferTimer, m_verticalVelocity;

        // apex variables
        float m_timePastApex, m_apexPoint;
        bool m_isPastApex;

        [BlackboardInjectKey] private readonly Blackboard m_context;

        public PlayerJumpState(in ReadonlyJumpContext jumpContext)
        {
            m_jumpContext = jumpContext;
        }

        public void FixedUpdate()
        {
            m_apexPoint = Mathf.InverseLerp(m_jumpContext.InitialJumpVelocity, 0f, m_verticalVelocity);
            if (m_apexPoint > m_jumpContext.ApexThreshold)
            {
                if (!m_isPastApex)
                {
                    m_isPastApex = true;
                    m_timePastApex = 0f;
                }
                
                if(m_isPastApex)
                {
                    m_timePastApex += Time.fixedDeltaTime;
                    if (m_timePastApex < m_jumpContext.ApexHangTime)
                    {
                        m_verticalVelocity = 0f;
                    }
                    else
                    {
                        m_verticalVelocity = -0.01f;
                    }
                }

            }
            else
            {
                m_verticalVelocity += m_jumpContext.Gravity * Time.fixedDeltaTime;
                m_isPastApex = false;
            }
            Debug.Log(m_verticalVelocity);
            m_currentVelocity.y = m_verticalVelocity;

            HandleMoveWhileJumping();

            m_context.SetValue(m_pastApexKey, m_isPastApex);
            m_context.SetValue(m_velocityKey, m_currentVelocity);
        }

        public void OnEnter()
        {
            m_currentVelocity = m_context.GetValue<Vector3>(m_velocityKey);
            m_verticalVelocity = m_jumpContext.InitialJumpVelocity;
            m_jumpBufferTimer = m_jumpContext.JumpBufferTimer;
        }

        public void OnExit(){}

        public void Update()
        {
            m_jumpBufferTimer -= Time.deltaTime;
            
            m_context.SetValue(m_jumpBufferTimerKey, m_jumpBufferTimer);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void HandleMoveWhileJumping(){
            Vector3 inputDirection = m_context.GetValue<Vector3>(m_directionKey);
            bool isStopMoving = inputDirection.x == 0f;
            if(isStopMoving && m_currentVelocity.x == 0f) return;

            float acceleration = isStopMoving ? m_jumpContext.AirDeceleration : m_jumpContext.AirAcceleration;
            float targetVelocity = inputDirection.x * m_jumpContext.MaxAirMoveSpeed;
                         
            m_currentVelocity.x = Mathf.Lerp(m_currentVelocity.x, targetVelocity, acceleration * Time.fixedDeltaTime);
            m_context.SetValue<Vector3>(m_velocityKey, m_currentVelocity);
        }
    }
}