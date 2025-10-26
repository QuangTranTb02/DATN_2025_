using Project.PlayerControl.Movement;
using UnityEngine;
namespace Project.PlayerControl
{
    public sealed class PlayerMovementController : MonoBehaviour{
        private IPlayerInput m_playerInput;
        private IPlayerMovementHandler m_playerMovementHandler;
        private IPlayerRenderHandler m_playerRenderHandler;

        Vector3 m_cacheDirection;

        public void Initialize(IPlayerInput playerInput, IPlayerMovementHandler playerMovementHandler, IPlayerRenderHandler playerRenderHandler){
            m_playerMovementHandler = playerMovementHandler ?? throw new System.ArgumentNullException(nameof(playerMovementHandler));            
            m_playerRenderHandler = playerRenderHandler;
            if(m_playerInput != null){
                m_playerInput.MovementInputEvent -= OnMove;
                m_playerInput.JumpInputEvent -= OnJump;
            }
            m_playerInput = playerInput;
            if(m_playerInput != null){
                m_playerInput.MovementInputEvent += OnMove;
                m_playerInput.JumpInputEvent += OnJump;
            }
        }

        private void OnMove(Vector3 direction)
        {
            m_cacheDirection.Set(direction.x, m_cacheDirection.y, direction.z);
            m_playerMovementHandler.ChangeDirection(m_cacheDirection);
            m_playerRenderHandler?.OnDirectionChanged(direction);
        }

        private void OnJump(float value)
        {
            m_playerMovementHandler.OnJump(value > 0f);
        }

        void Update(){
            m_playerMovementHandler.Update();
        }
        void FixedUpdate(){
            m_playerMovementHandler.FixedUpdate();
        }
    }
}