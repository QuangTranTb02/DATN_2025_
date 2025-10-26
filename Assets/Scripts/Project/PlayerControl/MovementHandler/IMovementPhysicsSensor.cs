using UnityEngine;

namespace Project.PlayerControl.Movement
{
    public interface IMovementPhysicsSensor{
        Transform CenterTransform { get; set; }
        bool IsGrounded { get; }
        bool IsBumpHead { get; }
        bool GroundCheck();
        bool BumpHeadCheck();
    }
}