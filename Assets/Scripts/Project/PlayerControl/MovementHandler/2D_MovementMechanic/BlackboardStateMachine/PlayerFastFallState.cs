using BlackboardSystem;
using BlackboardSystem.FSM;
using UnityEngine;

namespace Project.PlayerControl.Movement
{
    internal readonly struct ReadonlyFastFallContext{
        public readonly float TimeForUpwardsCancel;
        public readonly float Gravity;
        public readonly float GravityOnReleaseMultiplier;
        public readonly float AirAcceleration;
        public readonly float AirDeceleration;
        public readonly float MaxAirMoveSpeed;

        public ReadonlyFastFallContext(float timeForUpwardsCancel, float gravity, float gravityOnReleaseMultiplier, float airAcceleration, float airDeceleration, float maxAirMoveSpeed)
        {
            TimeForUpwardsCancel = timeForUpwardsCancel;
            Gravity = gravity;
            GravityOnReleaseMultiplier = gravityOnReleaseMultiplier;
            AirAcceleration = airAcceleration;
            AirDeceleration = airDeceleration;
            MaxAirMoveSpeed = maxAirMoveSpeed;
        }
    }
    internal class PlayerFastFallState : IState
    {
        readonly ReadonlyFastFallContext m_FastFallContext;
        [BlackboardInjectKey] readonly Blackboard m_context;

        [BBKey("fastFallTime")] readonly BlackboardKey m_fastFallTimeKey;
        [BBKey("velocity")] readonly BlackboardKey m_velocityKey;
        [BBKey("direction")] readonly BlackboardKey m_directionKey;
        float m_fastFallTime;
        Vector3 m_currentVelocity;
        float m_velocityY;

        public PlayerFastFallState(in ReadonlyFastFallContext fastFallContext)
        {
            m_FastFallContext = fastFallContext;
        }

        public void OnEnter()
        {
            m_fastFallTime = m_context.GetValue<float>(m_fastFallTimeKey);
            m_currentVelocity = m_context.GetValue<Vector3>(m_velocityKey);
            m_velocityY = 0f;
        }

        public void OnExit(){}

        public void FixedUpdate()
        {
            if(m_fastFallTime >= m_FastFallContext.TimeForUpwardsCancel){
                m_velocityY += m_FastFallContext.Gravity * m_FastFallContext.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else{
                m_velocityY = Mathf.Lerp(m_fastFallTime, 0f, m_fastFallTime / m_FastFallContext.TimeForUpwardsCancel);
            }

            m_fastFallTime += Time.fixedDeltaTime;
            m_context.SetValue(m_fastFallTimeKey, m_fastFallTime);

            m_currentVelocity.y = m_velocityY;
            HandleFallMovement();

            m_context.SetValue(m_velocityKey, m_currentVelocity);
        }

        public void Update(){}

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void HandleFallMovement(){
            Vector3 inputDirection = m_context.GetValue<Vector3>(m_directionKey);
            bool isStopMoving = inputDirection.x == 0f;
            if(isStopMoving && m_currentVelocity.x == 0f) return;

            float acceleration = isStopMoving ? m_FastFallContext.AirDeceleration : m_FastFallContext.AirAcceleration;
            float targetVelocity = inputDirection.x * m_FastFallContext.MaxAirMoveSpeed;
                         
            m_currentVelocity.x = Mathf.Lerp(m_currentVelocity.x, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
    }
}