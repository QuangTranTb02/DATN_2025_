using System;
using BlackboardSystem;
using BlackboardSystem.FSM;
using UnityEngine;

namespace Project.PlayerControl.Movement{
    public class Player2DMovementHandler : IPlayerMovementHandler
    {
        private BlackboardKey directionKey, jumpKey, velocityKey, groundedKey, bumpHeadKey;
        private readonly BlackboardFSM m_movementStateMachine;
        private readonly Blackboard m_blackboard;
        private readonly IMovementController m_movementController;
        private readonly IMovementPhysicsSensor m_sensor;

        public Player2DMovementHandler(IBlackboardMapper dataContext, IMovementController movementController, IMovementPhysicsSensor sensor){
            if(dataContext == null || movementController == null || sensor == null) throw new ArgumentNullException(nameof(dataContext));

            m_movementController = movementController;
            m_sensor = sensor;
            m_blackboard = InitializeBlackboard(dataContext);
            
            m_movementStateMachine = new BlackboardFSM(m_blackboard);
        }

        public void ChangeTarget(Transform target){
            m_movementController.target = target;
            m_sensor.CenterTransform = target;
        }

        public void ChangeDirection(Vector3 direction){
            m_blackboard.SetValue<Vector3>(directionKey, direction);
        }

        public void OnJump(bool value){
            m_blackboard.SetValue<bool>(jumpKey, value);
        }

        public void Update(){
            m_movementStateMachine.Update();
        }

        public void FixedUpdate()
        {
            m_movementStateMachine.FixedUpdate();
            bool isGrounded = m_sensor.GroundCheck();
            bool isBumpHead = m_sensor.BumpHeadCheck();

            m_blackboard.SetValue<bool>(groundedKey, isGrounded);
            m_blackboard.SetValue<bool>(bumpHeadKey, isBumpHead);
            
            Vector3 velocity = m_blackboard.GetValue<Vector3>(velocityKey);
            // Debug.Log($"velocity: {velocity}");
            m_movementController.velocity = velocity;
        }

        #region State Machine
        public void InitialState(IState initialState){
            m_movementStateMachine.Start(initialState);
        }
        public void AddState(IState state, params IStateLink[] links){
            m_movementStateMachine.AddState(state, links);
        }
        #endregion

        private Blackboard InitializeBlackboard(IBlackboardMapper dataContext){
            Blackboard blackboard = new Blackboard();
            dataContext.MapDataTo(blackboard);

            directionKey = blackboard.RegisterOrGetKey<Vector3>("direction");
            jumpKey = blackboard.RegisterOrGetKey<bool>("jump");
            velocityKey = blackboard.RegisterOrGetKey<Vector3>("velocity");
            groundedKey = blackboard.RegisterOrGetKey<bool>("grounded");
            bumpHeadKey = blackboard.RegisterOrGetKey<bool>("bumpHead");

            return blackboard;
        }
    }
}