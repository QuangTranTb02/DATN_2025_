using System;
using Project.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.PlayerControl{

    [System.Serializable]
    public struct PlayerInputEventIndices{
        public int MovementEventIndex;
        public int JumpEventIndex;
        public PlayerInputEventIndices(int movementEventIndex, int jumpEventIndex){
            MovementEventIndex = movementEventIndex;
            JumpEventIndex = jumpEventIndex;
        }
    }
    public class PlayerInput_InputSystem : IPlayerInput, IInputHandler
    {
        public event Action<Vector3> MovementInputEvent;
        public event Action<float> JumpInputEvent;

        private readonly PlayerInputEventIndices m_eventIndices;
        private readonly IPlayerInputConverter m_converter;

        private Vector3 m_cacheDirection;
        private float m_cacheJump;

        public PlayerInput_InputSystem(PlayerInputEventIndices eventIndices, IPlayerInputConverter converter){
            m_eventIndices = eventIndices;
            m_converter = converter;
            m_cacheDirection = Vector3.zero;
        }

        public void OnRegisteredTo(InputSystemEventStorage eventStorage)
        {
            eventStorage.RegisterCallbackAt(m_eventIndices.MovementEventIndex, OnMovementInput);
            eventStorage.RegisterCallbackAt(m_eventIndices.JumpEventIndex, OnJumpInput);
        }

        public void OnUnregisteredTo(InputSystemEventStorage eventStorage)
        {
            eventStorage.UnregisterCallbackAt(m_eventIndices.MovementEventIndex, OnMovementInput);
            eventStorage.UnregisterCallbackAt(m_eventIndices.JumpEventIndex, OnJumpInput);
        }

        private void OnJumpInput(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started) return;
            
            if(true == m_converter.TryGetPlayerJumpInput(context, ref m_cacheJump)){
                JumpInputEvent?.Invoke(m_cacheJump);
            }
            #if UNITY_EDITOR
            else{
                Debug.LogWarning("TryGetPlayerJumpInput failed");
            }
            #endif
        }

        private void OnMovementInput(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed){
                if(true == m_converter.TryGetPlayerMovement(context, ref m_cacheDirection)){                    
                    MovementInputEvent?.Invoke(m_cacheDirection);
                }
                #if UNITY_EDITOR
                else{
                    Debug.LogWarning("TryGetPlayerMovement failed");
                }
                #endif
            }
        }
    }
}