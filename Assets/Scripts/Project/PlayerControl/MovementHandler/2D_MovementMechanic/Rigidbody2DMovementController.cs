using System;
using UnityEngine;

namespace Project.PlayerControl.Movement
{
    public class Rigidbody2DMovementController : IMovementController
    {
        private Transform m_target;
        public Transform target {
            get => m_target;
            set => OnTargetChanged(value);
        }
        private Rigidbody2D m_rigidbody;

        private Vector3 m_velocity; 
        public Vector3 velocity{
            get => m_velocity;
            set {
                m_velocity = value;
                m_rigidbody.velocity = m_velocity;
            }
        }

        public Rigidbody2DMovementController(Transform target){
            this.target = target;
        }

        
        private void OnTargetChanged(Transform target)
        {
            if(target == null || m_target == target) return;

            m_target = target;
            m_rigidbody = ConfigRigidbody();
        }

        private Rigidbody2D ConfigRigidbody()
        {
            Rigidbody2D result = m_target.TryGetComponent(out result) ? result : m_target.gameObject.AddComponent<Rigidbody2D>();
            result.gravityScale = 0f;
            result.mass = 0f;
            result.freezeRotation = true;
            return result;
        }

        public void SetVelocityX(float value) {
            m_velocity.x = value;
            m_rigidbody.velocity = m_velocity;
        }

        public void SetVelocityY(float value){
            m_velocity.y = value;
            m_rigidbody.velocity = m_velocity;
        }

        public void SetVelocityZ(float value){
            m_velocity.z = value;
            m_rigidbody.velocity = m_velocity;
        }
    }
}