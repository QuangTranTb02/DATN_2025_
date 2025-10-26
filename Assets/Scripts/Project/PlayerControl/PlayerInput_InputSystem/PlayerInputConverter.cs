using UnityEngine.InputSystem;
using UnityEngine;
namespace Project.PlayerControl{
    /// <summary>
    /// try to get a value from input context (input system)
    /// </summary>
    public interface IPlayerInputConverter{
        bool TryGetPlayerMovement(InputAction.CallbackContext context, ref Vector3 value);
        bool TryGetPlayerJumpInput(InputAction.CallbackContext context, ref float value);
    }

    public class PlayerSafeInputConverter : IPlayerInputConverter
    {
        readonly IPlayerInputConverter m_wrapper;
        public PlayerSafeInputConverter(IPlayerInputConverter wrapper){
            m_wrapper = wrapper ?? throw new System.ArgumentNullException(nameof(wrapper));
        }

        public bool TryGetPlayerJumpInput(InputAction.CallbackContext context, ref float value)
        {
            try{
                return m_wrapper.TryGetPlayerJumpInput(context, ref value);
            }
            catch{
                Debug.LogWarning("Can not get jump input");
                return false;
            }
        }

        public bool TryGetPlayerMovement(InputAction.CallbackContext context, ref Vector3 value)
        {
            try{
                return m_wrapper.TryGetPlayerMovement(context, ref value);
            }
            catch{
                Debug.LogWarning("Can not get movement input");
                return false;
            }
        }
    }
    public class PlayerInputAxisConverter : IPlayerInputConverter
    {
        public bool TryGetPlayerJumpInput(InputAction.CallbackContext context, ref float value)
        {
            float val = context.ReadValue<float>();
            value = val;
            return true;
        }

        public bool TryGetPlayerMovement(InputAction.CallbackContext context, ref Vector3 value)
        {
            float val = context.ReadValue<float>();
            value.Set(val, 0f, 0f);
            return true;
        }
    }
    public class PlayerInput2DConverter : IPlayerInputConverter
    {
        public bool TryGetPlayerJumpInput(InputAction.CallbackContext context, ref float value)
        {
            float val = context.ReadValue<float>();
            value = val;
            return true;
        }

        public bool TryGetPlayerMovement(InputAction.CallbackContext context, ref Vector3 value)
        {
            Vector2 v2 = context.ReadValue<Vector2>();
            value.Set(v2.x, v2.y, 0f);
            return true;
        }
    }
}