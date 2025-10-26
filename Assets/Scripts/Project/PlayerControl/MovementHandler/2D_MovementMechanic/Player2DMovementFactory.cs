using System;
using BlackboardSystem;
using BlackboardSystem.FSM;
using UnityEngine;

namespace Project.PlayerControl.Movement
{
    [CreateAssetMenu(fileName = "Player2DMovementFactory", menuName = "SO/Player2DMovementFactory")]
    public class Player2DMovementFactory : ScriptableMovementHandlerFactory
    {
        [SerializeField] float Offset;
        public override IPlayerMovementHandler CreateMovementHandler(PlayerMovementStats stats, Transform target)
        {
            Rigidbody2DMovementController movementController = new(target);
            Physics2DMovementSensor sensor = new(target, new Physics2DMovementSensorData(Offset, stats.GroundLayer));
            Player2DMovementHandler handler = new Player2DMovementHandler(new BlackboardMovementMapper(), movementController, sensor);

            InitStates(handler, stats);

            return handler;
        }

        #region State Intialization
        private IState m_runState, m_jumpState, m_fallState, m_fastFallState;
        private void InitStates(Player2DMovementHandler handler, PlayerMovementStats stats)
        {
            m_runState = new PlayerRunState(new PlayerRunStateContext(stats.MaxRunSpeed, stats.GroundAcceleration, stats.GroundDeceleration));
            m_jumpState = new PlayerJumpState(new ReadonlyJumpContext(stats.JumpBufferTimer,stats.JumpVelocity, stats.JumpApexThreshold, stats.JumpApexHangTime, stats.Gravity, stats.GroundAcceleration, stats.GroundDeceleration, stats.MaxRunSpeed));
            m_fastFallState = new PlayerFastFallState(new ReadonlyFastFallContext(0.1f, stats.Gravity, stats.GravityOnReleaseMultiplier, stats.AirAcceleration, stats.AirDeceleration, stats.MaxFallMoveSpeed));
            m_fallState = new PlayerFallState(new ReadonlyNormalFallContext(stats.Gravity, stats.GravityOnReleaseMultiplier, stats.AirAcceleration, stats.AirDeceleration, stats.MaxFallMoveSpeed));

            handler.AddState(m_runState, links: BuildLinkForRunState(stats));
            handler.AddState(m_jumpState, links: BuildLinkForJumpState(stats));

            IStateLink fallToGround = new FuncWrapStateLink((context)=>{
                bool grounded = context.GetValue<bool>("grounded");
                return grounded ? m_runState : null;
            });
            handler.AddState(m_fallState, fallToGround);
            handler.AddState(m_fastFallState, fallToGround);
            handler.InitialState(m_runState);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private IStateLink BuildLinkForRunState(PlayerMovementStats stats){
            IStateLink toJump = new FuncWrapStateLink((context)=>{
                bool jumpPressed = context.GetValue<bool>("jump");
                return jumpPressed ? m_jumpState : null;
            });

            return toJump;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private IStateLink BuildLinkForJumpState(PlayerMovementStats stats){
            IStateLink toFastFall = new FuncWrapStateLink((context)=>{
                bool jumpPressed = context.GetValue<bool>("jump");
                bool bumpHead = context.GetValue<bool>("bumpHead");
                if(bumpHead) return m_fastFallState;
                if(jumpPressed == true){
                    
                    return null;
                }
                
                return m_fallState;
            });

            return toFastFall;
        }
        #endregion

        private class BlackboardMovementMapper : IBlackboardMapper
        {
            public void MapDataTo(Blackboard blackboard)
            {
                blackboard.RegisterKey<bool>("grounded");
                blackboard.RegisterKey<bool>("bumpHead");
                blackboard.RegisterKey<bool>("pastApex");
                blackboard.RegisterKey<Vector3>("velocity");
                blackboard.RegisterKey<float>("jumpBufferTimer");
                blackboard.RegisterKey<float>("fastFallTime");
            }
        }
    }
}