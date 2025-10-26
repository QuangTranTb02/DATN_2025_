using System;
using UnityEngine;

namespace Project.PlayerControl
{
    public interface IPlayerInput{
        public event Action<Vector3> MovementInputEvent;
        public event Action<float> JumpInputEvent;
    }
}