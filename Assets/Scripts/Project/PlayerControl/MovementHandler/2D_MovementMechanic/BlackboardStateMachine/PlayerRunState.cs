using BlackboardSystem;
using UnityEngine;

namespace Project.PlayerControl.Movement
{
    internal readonly struct PlayerRunStateContext{
        public readonly float MaxRunSpeed;
        public readonly float GroundAcceleration;
        public readonly float GroundDeceleration;

        public PlayerRunStateContext(float maxRunSpeed, float groundAcceleration, float groundDeceleration)
        {
            MaxRunSpeed = maxRunSpeed;
            GroundAcceleration = groundAcceleration;
            GroundDeceleration = groundDeceleration;
        }
    }
    internal sealed class PlayerRunState : BlackboardSystem.FSM.IState
    {
        [BBKey("direction")] readonly BlackboardKey m_directionKey;
        [BBKey("velocity")] readonly BlackboardKey m_velocityKey;
        private BlackboardEntry<Vector3> m_velocityEntry;
        private readonly PlayerRunStateContext m_runContext;
        [BlackboardInjectKey] readonly Blackboard m_context;
        Vector3 m_currentVelocity;

        public PlayerRunState(in PlayerRunStateContext context){
            m_runContext = context;
        }

        public void OnEnter()
        {
            m_velocityEntry = m_context.GetRefUnsafe<Vector3>(m_velocityKey);
            m_currentVelocity = m_velocityEntry.Value;
        }

        public void FixedUpdate()
        {
            Vector3 inputDirection = m_context.GetValue<Vector3>(m_directionKey);
            bool isStopMoving = inputDirection.x == 0f;
            if(isStopMoving && m_currentVelocity.x == 0f) return;

            float acceleration = isStopMoving ? m_runContext.GroundDeceleration : m_runContext.GroundAcceleration;
            float targetVelocity = inputDirection.x * m_runContext.MaxRunSpeed;
                         
            m_currentVelocity.x = Mathf.Lerp(m_currentVelocity.x, targetVelocity, acceleration * Time.fixedDeltaTime);
            m_velocityEntry.SetValue(m_currentVelocity);
        }

        public void OnExit(){}
        public void Update(){}
    }
}