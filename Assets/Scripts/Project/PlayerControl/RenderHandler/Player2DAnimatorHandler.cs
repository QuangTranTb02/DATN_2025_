using UnityEngine;

namespace Project.PlayerControl
{
    public class Player2DAnimatorHandler : IPlayerRenderHandler
    {
        readonly PlayerAnimatorHash m_playerAnimatorHash;
        readonly Animator m_animator;
        readonly SpriteRenderer m_spriteRenderer;

        public Player2DAnimatorHandler(Animator animator, PlayerAnimatorHash playerAnimatorHash, SpriteRenderer spriteRenderer)
        {
            m_animator = animator;
            m_playerAnimatorHash = playerAnimatorHash;
            m_spriteRenderer = spriteRenderer;
        }

        public void OnDirectionChanged(Vector3 direction)
        {
            if(direction.x != 0f){ // only flip when move
                m_spriteRenderer.flipX = direction.x < 0f;
            }
            m_animator.SetFloat(m_playerAnimatorHash.HorizontalDirection, Mathf.Abs(direction.x));
            m_animator.SetFloat(m_playerAnimatorHash.VerticalDirection, direction.z);
        }

        public class PlayerAnimatorHash{
            public int HorizontalDirection;
            public int VerticalDirection;
            public int JumpTrigger;
        }
    }
}