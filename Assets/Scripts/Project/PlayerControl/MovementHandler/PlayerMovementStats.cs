using UnityEngine;

namespace Project.PlayerControl.Movement{
    [CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "SO/PlayerMovementStats", order = 0)]
    public sealed class PlayerMovementStats : ScriptableObject{

        [Header("Run")]
        [Range(1f, 100f)] public float MaxRunSpeed = 5f;
        [Range(0.1f, 20f)] public float MaxFallMoveSpeed = 2f;
        [Range(0.2f, 20f)] public float GroundAcceleration = 5f;
        [Range(0.2f, 20f)] public float GroundDeceleration = 5f;
        [Range(0.2f, 20f)] public float AirAcceleration = 5f;
        [Range(0.2f, 20f)] public float AirDeceleration = 5f;

        [Header("Collider Ground Check")]
        public LayerMask GroundLayer;
        public float GroundCheckRadius = 0.5f;
        public float GroundCheckDistance = 0.5f;
        
        [Header("Jump")]
        public int NumberOfJumps = 1;
        public float JumpBufferTimer = 0.125f;
        public float JumpHeight = 12f;
        public float JumpApexThreshold = 0.97f;
        public float JumpApexHangTime = 0.075f;
        public float TimeTillJumpApex = 0.35f;
        public float FallSpeed = 26f;
        public float GravityOnReleaseMultiplier = 2f;
        [Range(0f, 1f)]public float CoyoteTime = 0.1f;

        [System.NonSerialized]public float Gravity;
        [System.NonSerialized]public float JumpVelocity;

        #if UNITY_EDITOR
        void OnValidate(){
            CalculatePhysicValues();
        }
        #endif

        void OnEnable(){
            CalculatePhysicValues();
        }
        void CalculatePhysicValues(){
            Gravity = -2f * JumpHeight / (TimeTillJumpApex * TimeTillJumpApex);
            JumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
        }
    }
}