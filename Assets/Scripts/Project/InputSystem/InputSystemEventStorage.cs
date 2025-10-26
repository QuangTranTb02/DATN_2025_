using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Project.InputSystem
{
    [CreateAssetMenu(fileName = "InputSystemEventStorage", menuName = "SO/InputSystem/InputSystemEventStorage")]
    public sealed class InputSystemEventStorage : ScriptableObject{
        private readonly Dictionary<int, InputActionCallbackWrapper> m_callbacks = new();
        private PlayerInput playerInput;
        public PlayerInput PlayerInput => playerInput;

        /// <summary>
        /// simple initialization check
        /// </summary>
        private bool isInitialized = false;

        void OnEnable() => isInitialized = false;
        void OnDisable() => isInitialized = false;

        internal void Initialize(ref PlayerInput playerInput)
        {
            if(isInitialized == true) return;
            this.playerInput = playerInput;
            isInitialized = true;
        }

        public void RegisterCallbackAt(int index, Action<InputAction.CallbackContext> callback){
            if (!m_callbacks.TryGetValue(index, out InputActionCallbackWrapper wrapper)){
                wrapper = new InputActionCallbackWrapper();
                m_callbacks.Add(index, wrapper);
                playerInput.actionEvents[index].AddListener(wrapper.InvokeWrappedCallback);
            }
            wrapper.AddWrappedListener(callback);
        }

        public void UnregisterCallbackAt(int index, Action<InputAction.CallbackContext> callback){
            if (m_callbacks.TryGetValue(index, out InputActionCallbackWrapper wrapper)){
                wrapper.RemoveWrappedListener(callback);
                if(wrapper.IsEmpty){
                    playerInput.actionEvents[index].RemoveListener(wrapper.InvokeWrappedCallback);
                    m_callbacks.Remove(index);
                }
            }
        }

        private class InputActionCallbackWrapper{
            public bool IsEmpty => WrappedCallback == null || WrappedCallback.GetInvocationList().Length == 0;
            private event Action<InputAction.CallbackContext> WrappedCallback;

            public void AddWrappedListener(Action<InputAction.CallbackContext> callback){
                WrappedCallback += callback;
            }

            public void RemoveWrappedListener(Action<InputAction.CallbackContext> callback){
                WrappedCallback -= callback;
            }

            public void InvokeWrappedCallback(InputAction.CallbackContext context){
                WrappedCallback?.Invoke(context);
            }
        }
    } 
}