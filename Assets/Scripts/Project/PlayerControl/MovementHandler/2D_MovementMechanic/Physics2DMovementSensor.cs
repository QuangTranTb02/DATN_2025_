using System;
using UnityEngine;

namespace Project.PlayerControl.Movement{
    public readonly struct Physics2DMovementSensorData{
        public readonly float Offset; // the offset from the center
        public readonly int GroundLayerMask;

        public Physics2DMovementSensorData(float offset, int groundLayerMask){
            Offset = offset;
            GroundLayerMask = groundLayerMask;
        }
    }
    public class Physics2DMovementSensor : IMovementPhysicsSensor
    {
        private bool m_isGrounded, m_isBumpHead;
        private Transform m_centerTransform;
        public Transform CenterTransform{
             get => m_centerTransform;
             set {
                if(m_centerTransform != value){
                    m_centerTransform = value;
                    m_isGrounded = false;
                    m_isBumpHead = false;
                }
             }
        }
        public bool IsGrounded => m_isGrounded;
        public bool IsBumpHead => m_isBumpHead;

        public Physics2DMovementSensorData SensorData;

        internal Physics2DMovementSensor(UnityEngine.Transform centerTransform, Physics2DMovementSensorData data){
            CenterTransform = centerTransform;
            SensorData = data;
        }

        public bool BumpHeadCheck()
        {
            // implement later
            Vector2 center = m_centerTransform.position;
            center.y += SensorData.Offset;
            m_isBumpHead = Physics2D.OverlapCircle(center, 0.1f, SensorData.GroundLayerMask);
            return m_isBumpHead;
        }

        public bool GroundCheck()
        {
            // implement later
            Vector2 center = m_centerTransform.position;
            center.y -= SensorData.Offset;
            m_isGrounded = Physics2D.OverlapCircle(center, 0.1f, SensorData.GroundLayerMask);
            return m_isGrounded;
        }
    }
}