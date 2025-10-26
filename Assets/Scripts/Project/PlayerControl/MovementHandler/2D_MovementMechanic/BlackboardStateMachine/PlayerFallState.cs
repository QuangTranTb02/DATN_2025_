using BlackboardSystem;
using BlackboardSystem.FSM;
using UnityEngine;

namespace Project.PlayerControl.Movement
{
    internal readonly struct ReadonlyNormalFallContext{
        public readonly float Gravity;
        public readonly float GravityOnReleaseMultiplier;
        public readonly float AirAcceleration;
        public readonly float AirDeceleration;
        public readonly float MaxAirMoveSpeed;

        public ReadonlyNormalFallContext(float gravity, float gravityOnReleaseMultiplier, float airAcceleration, float airDeceleration, float maxAirMoveSpeed)
        {
            Gravity = gravity;
            GravityOnReleaseMultiplier = gravityOnReleaseMultiplier;
            AirAcceleration = airAcceleration;
            AirDeceleration = airDeceleration;
            MaxAirMoveSpeed = maxAirMoveSpeed;
        }
    }
    internal class PlayerFallState : IState
    {
        [BBKey("velocity")] readonly BlackboardKey m_velocityKey;
        [BBKey("direction")] readonly BlackboardKey m_directionKey;

        private BlackboardEntry<Vector3> m_currentVelocityRef;
        private Vector3 m_velocity;
        private float m_verticalVelocity;
        private readonly ReadonlyNormalFallContext m_FallContext;


        [BlackboardInjectKey] private readonly Blackboard m_context;

        public PlayerFallState(in ReadonlyNormalFallContext context)
        {
            m_FallContext = context;
        }

        public void OnEnter()
        {
            m_currentVelocityRef = m_context .GetRefUnsafe<Vector3>(m_velocityKey);
            m_velocity = m_currentVelocityRef.Value;
            m_verticalVelocity = m_currentVelocityRef.Value.y;
        }

        public void OnExit(){}

        public void FixedUpdate()
        {
            m_verticalVelocity += m_FallContext.Gravity * m_FallContext.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            m_velocity.y = m_verticalVelocity;
            HandleFallMovement();
            m_currentVelocityRef.SetValue(m_velocity);
        }

        public void Update(){}

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void HandleFallMovement(){
            Vector3 inputDirection = m_context.GetValue<Vector3>(m_directionKey);
            bool isStopMoving = inputDirection.x == 0f;
            if(isStopMoving && m_velocity.x == 0f) return;

            float acceleration = isStopMoving ? m_FallContext.AirDeceleration : m_FallContext.AirAcceleration;
            float targetVelocity = inputDirection.x * m_FallContext.MaxAirMoveSpeed;

            m_velocity.x = Mathf.Lerp(m_velocity.x, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
    }
}